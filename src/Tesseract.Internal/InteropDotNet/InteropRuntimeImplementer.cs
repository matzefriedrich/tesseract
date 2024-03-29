//  Copyright (c) 2014 Andrey Akinshin
//  Project URL: https://github.com/AndreyAkinshin/InteropDotNet
//  Distributed under the MIT License: http://opensource.org/licenses/MIT

namespace InteropDotNet
{
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.InteropServices;

    internal static class InteropRuntimeImplementer
    {
        public static T? CreateInstance<T>() where T : class
        {
            Type interfaceType = typeof(T);
            if (!typeof(T).IsInterface)
                throw new Exception($"The type {interfaceType.Name} should be an interface");
            if (!interfaceType.IsPublic)
                throw new Exception($"The interface {interfaceType.Name} should be public");

            string assemblyName = GetAssemblyName(interfaceType);
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName);

            string typeName = GetImplementationTypeName(assemblyName, interfaceType);
            TypeBuilder typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Public,
                typeof(object), new[] { interfaceType });
            MethodItem[] methods = BuildMethods(interfaceType);

            ImplementDelegates(assemblyName, moduleBuilder, methods);
            ImplementFields(typeBuilder, methods);
            ImplementMethods(typeBuilder, methods);
            ImplementConstructor(typeBuilder, methods);

            TypeInfo implementationType = typeBuilder.CreateTypeInfo();
            return (T?)Activator.CreateInstance(implementationType, LibraryLoader.Instance);
        }

        #region Main steps

        private static MethodItem[] BuildMethods(Type interfaceType)
        {
            MethodInfo[] methodInfoArray = interfaceType.GetMethods();
            var methods = new MethodItem[methodInfoArray.Length];
            for (var i = 0; i < methodInfoArray.Length; i++)
            {
                MethodInfo methodInfo = methodInfoArray[i];
                RuntimeDllImportAttribute? attribute = GetRuntimeDllImportAttribute(methodInfo);
                if (attribute == null) throw new Exception($"Method '{methodInfo.Name}' of interface '{interfaceType.Name}' should be marked with the RuntimeDllImport attribute");
                methods[i] = new MethodItem(methodInfo, attribute);
            }

            return methods;
        }

        private static void ImplementDelegates(string assemblyName, ModuleBuilder moduleBuilder, IEnumerable<MethodItem> methods)
        {
            foreach (MethodItem method in methods)
                method.DelegateType = ImplementMethodDelegate(assemblyName, moduleBuilder, method);
        }

        private static Type ImplementMethodDelegate(string assemblyName, ModuleBuilder moduleBuilder, MethodItem method)
        {
            // Consts
            const MethodAttributes methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig |
                                                      MethodAttributes.NewSlot | MethodAttributes.Virtual;

            // Initial
            string delegateName = GetDelegateName(assemblyName, method.Info);
            TypeBuilder delegateBuilder = moduleBuilder.DefineType(delegateName,
                TypeAttributes.Public | TypeAttributes.AutoClass | TypeAttributes.Sealed, typeof(MulticastDelegate));

            // UnmanagedFunctionPointer
            RuntimeDllImportAttribute importAttribute = method.DllImportAttribute;
            ConstructorInfo? attributeCtor = typeof(UnmanagedFunctionPointerAttribute).GetConstructor(new[] { typeof(CallingConvention) });
            if (attributeCtor == null)
                throw new Exception("There is no the target constructor of the UnmanagedFunctionPointerAttribute");
            var attributeBuilder = new CustomAttributeBuilder(attributeCtor, new object[] { importAttribute.CallingConvention },
                new[]
                {
                    typeof(UnmanagedFunctionPointerAttribute).GetField("CharSet")!,
                    typeof(UnmanagedFunctionPointerAttribute).GetField("BestFitMapping")!,
                    typeof(UnmanagedFunctionPointerAttribute).GetField("ThrowOnUnmappableChar")!,
                    typeof(UnmanagedFunctionPointerAttribute).GetField("SetLastError")!
                },
                new object[]
                {
                    importAttribute.CharSet,
                    importAttribute.BestFitMapping,
                    importAttribute.ThrowOnUnmappableChar,
                    importAttribute.SetLastError
                });
            delegateBuilder.SetCustomAttribute(attributeBuilder);


            // ctor
            ConstructorBuilder ctorBuilder = delegateBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig |
                                                                               MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard,
                new[] { typeof(object), typeof(IntPtr) });
            ctorBuilder.SetImplementationFlags(MethodImplAttributes.CodeTypeMask);
            ctorBuilder.DefineParameter(1, ParameterAttributes.HasDefault, "object");
            ctorBuilder.DefineParameter(2, ParameterAttributes.HasDefault, "method");

            // Invoke
            LightParameterInfo[] parameters = GetParameterInfoArray(method.Info);
            MethodBuilder methodBuilder = DefineMethod(delegateBuilder, "Invoke", methodAttributes, method.ReturnType, parameters);
            methodBuilder.SetImplementationFlags(MethodImplAttributes.CodeTypeMask);

            // BeginInvoke
            parameters = GetParameterInfoArray(method.Info, InfoArrayMode.BeginInvoke);
            methodBuilder = DefineMethod(delegateBuilder, "BeginInvoke", methodAttributes, typeof(IAsyncResult), parameters);
            methodBuilder.SetImplementationFlags(MethodImplAttributes.CodeTypeMask);

            // EndInvoke
            parameters = GetParameterInfoArray(method.Info, InfoArrayMode.EndInvoke);
            methodBuilder = DefineMethod(delegateBuilder, "EndInvoke", methodAttributes, method.ReturnType, parameters);
            methodBuilder.SetImplementationFlags(MethodImplAttributes.CodeTypeMask);

            // Create type
            return delegateBuilder.CreateTypeInfo();
        }

        private static void ImplementFields(TypeBuilder typeBuilder, IEnumerable<MethodItem> methods)
        {
            foreach (MethodItem method in methods)
            {
                string fieldName = method.Info.Name + "Field";
                FieldBuilder fieldBuilder = typeBuilder.DefineField(fieldName, method.DelegateType!, FieldAttributes.Private);
                method.FieldInfo = fieldBuilder;
            }
        }

        private static void ImplementMethods(TypeBuilder typeBuilder, IEnumerable<MethodItem> methods)
        {
            foreach (MethodItem method in methods)
            {
                LightParameterInfo[] infoArray = GetParameterInfoArray(method.Info);
                MethodBuilder methodBuilder = DefineMethod(typeBuilder, method.Name,
                    MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot |
                    MethodAttributes.Final | MethodAttributes.Virtual,
                    method.ReturnType, infoArray);

                ILGenerator ilGen = methodBuilder.GetILGenerator();

                // Load this
                ilGen.Emit(OpCodes.Ldarg_0);
                // Load field
                ilGen.Emit(OpCodes.Ldfld, method.FieldInfo!);
                // Load arguments
                for (var i = 0; i < infoArray.Length; i++)
                    LdArg(ilGen, i + 1);
                // Invoke delegate
                ilGen.Emit(OpCodes.Callvirt, method.DelegateType!.GetMethod("Invoke")!);
                // Return value
                ilGen.Emit(OpCodes.Ret);

                // Associate the method body with the interface method
                typeBuilder.DefineMethodOverride(methodBuilder, method.Info);
            }
        }

        private static void ImplementConstructor(TypeBuilder typeBuilder, MethodItem[] methods)
        {
            // Preparing
            ConstructorBuilder ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public,
                CallingConventions.Standard, new[] { typeof(LibraryLoader) });
            ctorBuilder.DefineParameter(1, ParameterAttributes.HasDefault, "loader");
            if (typeBuilder.BaseType == null)
                throw new Exception("There is no a BaseType of typeBuilder");
            ConstructorInfo? baseCtor = typeBuilder.BaseType.GetConstructor(new Type[0]);
            if (baseCtor == null)
                throw new Exception("There is no a default constructor of BaseType of typeBuilder");

            // Build list of library names
            var libraries = new List<string>();
            foreach (MethodItem method in methods)
            {
                string libraryName = method.DllImportAttribute.LibraryFileName;
                if (!libraries.Contains(libraryName))
                    libraries.Add(libraryName);
            }

            // Create ILGenerator
            ILGenerator ilGen = ctorBuilder.GetILGenerator();
            // Declare locals for library handles
            for (var i = 0; i < libraries.Count; i++)
                ilGen.DeclareLocal(typeof(IntPtr));
            // Declare locals for a method handle
            ilGen.DeclareLocal(typeof(IntPtr));
            // Load this
            ilGen.Emit(OpCodes.Ldarg_0);
            // Run objector..ctor()
            ilGen.Emit(OpCodes.Call, baseCtor);
            for (var i = 0; i < libraries.Count; i++)
            {
                // Preparing
                string library = libraries[i];
                // Load LibraryLoader
                ilGen.Emit(OpCodes.Ldarg_1);
                // Load libraryName
                ilGen.Emit(OpCodes.Ldstr, library);
                // Load null
                ilGen.Emit(OpCodes.Ldnull);
                // Call LibraryLoader.LoadLibrary(libraryName, null)
                ilGen.Emit(OpCodes.Callvirt, typeof(LibraryLoader).GetMethod("LoadLibrary")!);
                // Store libraryHandle in locals[i]
                ilGen.Emit(OpCodes.Stloc, i);
            }

            foreach (MethodItem method in methods)
            {
                // Preparing
                int libraryIndex = libraries.IndexOf(method.DllImportAttribute.LibraryFileName);
                string methodName = method.DllImportAttribute.EntryPoint ?? method.Info.Name;
                // Load Library Loader
                ilGen.Emit(OpCodes.Ldarg_1);
                // Load libraryHandle (locals[libraryIndex])
                ilGen.Emit(OpCodes.Ldloc, libraryIndex);
                // Load methodName
                ilGen.Emit(OpCodes.Ldstr, methodName);
                // Call LibraryLoader.GetProcAddress(libraryHandle, methodName)
                ilGen.Emit(OpCodes.Callvirt, typeof(LibraryLoader).GetMethod("GetProcAddress")!);
                // Store methodHandle in locals
                ilGen.Emit(OpCodes.Stloc, libraries.Count);
                // Load this
                ilGen.Emit(OpCodes.Ldarg_0);
                // Load methodHandle from locals
                ilGen.Emit(OpCodes.Ldloc_1);
                // Load methodDelegate token
                ilGen.Emit(OpCodes.Ldtoken, method.DelegateType!);
                // Call typeof(methodDelegate)                                
                ilGen.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle")!);
                // Call Marshal.GetDelegateForFunctionPointer(methodHandle, typeof(methodDelegate))
                ilGen.Emit(OpCodes.Call, typeof(Marshal).GetMethod("GetDelegateForFunctionPointer", new[] { typeof(IntPtr), typeof(Type) })!);
                // Cast result to typeof(methodDelegate)
                ilGen.Emit(OpCodes.Castclass, method.DelegateType!);
                // Store result in methodField
                ilGen.Emit(OpCodes.Stfld, method.FieldInfo!);
            }

            // Return
            ilGen.Emit(OpCodes.Ret);
        }

        #endregion

        #region Reflection and emit helpers

        private static RuntimeDllImportAttribute GetRuntimeDllImportAttribute(MethodInfo methodInfo)
        {
            object[] attributes = methodInfo.GetCustomAttributes(typeof(RuntimeDllImportAttribute), true);
            if (attributes.Length == 0)
                throw new Exception($"RuntimeDllImportAttribute for method '{methodInfo.Name}' not found");
            return (RuntimeDllImportAttribute)attributes[0];
        }

        private static void LdArg(ILGenerator ilGen, int index)
        {
            switch (index)
            {
                case 0:
                    ilGen.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    ilGen.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    ilGen.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    ilGen.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    ilGen.Emit(OpCodes.Ldarg, index);
                    break;
            }
        }

        private static MethodBuilder DefineMethod(TypeBuilder typeBuilder, string name,
            MethodAttributes attributes, Type returnType, LightParameterInfo[] infoArray)
        {
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(name, attributes, returnType, GetParameterTypeArray(infoArray));
            for (var parameterIndex = 0; parameterIndex < infoArray.Length; parameterIndex++)
            {
                methodBuilder.DefineParameter(parameterIndex + 1,
                    infoArray[parameterIndex].Attributes, infoArray[parameterIndex].Name);
            }

            return methodBuilder;
        }

        #endregion

        #region Method helpers

        private class MethodItem
        {
            public MethodItem(MethodInfo info, RuntimeDllImportAttribute dllImportAttribute, Type? delegateType = null, FieldInfo? fieldInfo = null)
            {
                this.Info = info ?? throw new ArgumentNullException(nameof(info));
                this.DllImportAttribute = dllImportAttribute ?? throw new ArgumentNullException(nameof(dllImportAttribute));
                this.DelegateType = delegateType;
                this.FieldInfo = fieldInfo;
            }

            public MethodInfo Info { get; }
            public RuntimeDllImportAttribute DllImportAttribute { get; }

            public Type? DelegateType { get; set; }
            public FieldInfo? FieldInfo { get; set; }

            public string Name => this.Info.Name;

            public Type ReturnType => this.Info.ReturnType;
        }

        private class LightParameterInfo
        {
            public LightParameterInfo(ParameterInfo info)
            {
                if (info == null) throw new ArgumentNullException(nameof(info));
                this.Type = info.ParameterType;
                this.Name = info.Name;
                this.Attributes = info.Attributes;
            }

            public LightParameterInfo(Type type, string name)
            {
                this.Type = type;
                this.Name = name;
                this.Attributes = ParameterAttributes.HasDefault;
            }

            public Type Type { get; }
            public string? Name { get; }
            public ParameterAttributes Attributes { get; }
        }

        private enum InfoArrayMode
        {
            Invoke,
            BeginInvoke,
            EndInvoke
        }

        private static LightParameterInfo[] GetParameterInfoArray(MethodInfo methodInfo, InfoArrayMode mode = InfoArrayMode.Invoke)
        {
            ParameterInfo[] parameters = methodInfo.GetParameters();
            var infoList = new List<LightParameterInfo>();
            for (var i = 0; i < parameters.Length; i++)
            {
                if (mode != InfoArrayMode.EndInvoke || parameters[i].ParameterType.IsByRef)
                    infoList.Add(new LightParameterInfo(parameters[i]));
            }

            if (mode == InfoArrayMode.BeginInvoke)
            {
                infoList.Add(new LightParameterInfo(typeof(AsyncCallback), "callback"));
                infoList.Add(new LightParameterInfo(typeof(object), "object"));
            }

            if (mode == InfoArrayMode.EndInvoke)
                infoList.Add(new LightParameterInfo(typeof(IAsyncResult), "result"));
            var infoArray = new LightParameterInfo[infoList.Count];
            for (var i = 0; i < infoList.Count; i++)
                infoArray[i] = infoList[i];
            return infoArray;
        }

        private static Type[] GetParameterTypeArray(LightParameterInfo[] infoArray)
        {
            var typeArray = new Type[infoArray.Length];
            for (var i = 0; i < infoArray.Length; i++)
                typeArray[i] = infoArray[i].Type;
            return typeArray;
        }

        #endregion

        #region Name helpers

        private static string GetAssemblyName(Type interfaceType)
        {
            return $"InteropRuntimeImplementer.{GetSubstantialName(interfaceType)}Instance";
        }

        private static string GetImplementationTypeName(string assemblyName, Type interfaceType)
        {
            return $"{assemblyName}.{GetSubstantialName(interfaceType)}Implementation";
        }

        private static string GetSubstantialName(Type interfaceType)
        {
            string name = interfaceType.Name;
            if (name.StartsWith("I"))
                name = name.Substring(1);
            return name;
        }

        private static string GetDelegateName(string assemblyName, MethodInfo methodInfo)
        {
            return $"{assemblyName}.{methodInfo.Name}Delegate";
        }

        #endregion
    }
}