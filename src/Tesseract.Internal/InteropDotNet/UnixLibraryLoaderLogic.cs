﻿//  Copyright (c) 2014 Andrey Akinshin
//  Project URL: https://github.com/AndreyAkinshin/InteropDotNet
//  Distributed under the MIT License: http://opensource.org/licenses/MIT

namespace InteropDotNet
{
    using System.Runtime.InteropServices;

    using Tesseract.Internal;

    internal class UnixLibraryLoaderLogic : ILibraryLoaderLogic
    {
        private const int RTLD_NOW = 2;

        private static readonly string FileExtension = SystemManager.GetOperatingSystem() == OperatingSystem.MacOSX ? ".dylib" : ".so";

        public IntPtr LoadLibrary(string fileName)
        {
            IntPtr libraryHandle = IntPtr.Zero;

            try
            {
                Logger.TraceInformation("Trying to load native library \"{0}\"...", fileName);
                libraryHandle = UnixLoadLibrary(fileName, RTLD_NOW);
                if (libraryHandle != IntPtr.Zero)
                    Logger.TraceInformation("Successfully loaded native library \"{0}\", handle = {1}.", fileName, libraryHandle);
                else
                    Logger.TraceError("Failed to load native library \"{0}\".\r\nCheck windows event log.", fileName);
            }
            catch (Exception e)
            {
                IntPtr lastError = UnixGetLastError();
                Logger.TraceError("Failed to load native library \"{0}\".\r\nLast Error:{1}\r\nCheck inner exception and\\or windows event log.\r\nInner Exception: {2}", fileName, lastError, e.ToString());
            }

            return libraryHandle;
        }

        public bool FreeLibrary(IntPtr libraryHandle)
        {
            return UnixFreeLibrary(libraryHandle) != 0;
        }

        public IntPtr GetProcAddress(IntPtr libraryHandle, string functionName)
        {
            UnixGetLastError(); // Clearing previous errors
            Logger.TraceInformation("Trying to load native function \"{0}\" from the library with handle {1}...",
                functionName, libraryHandle);
            IntPtr functionHandle = UnixGetProcAddress(libraryHandle, functionName);
            IntPtr errorPointer = UnixGetLastError();
            if (errorPointer != IntPtr.Zero)
                throw new Exception("dlsym: " + Marshal.PtrToStringAnsi(errorPointer));
            if (functionHandle != IntPtr.Zero && errorPointer == IntPtr.Zero)
                Logger.TraceInformation("Successfully loaded native function \"{0}\", function handle = {1}.",
                    functionName, functionHandle);
            else
                Logger.TraceError("Failed to load native function \"{0}\", function handle = {1}, error pointer = {2}",
                    functionName, functionHandle, errorPointer);
            return functionHandle;
        }

        public string FixUpLibraryName(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                if (!fileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase))
                    fileName += FileExtension;
                if (!fileName.StartsWith("lib", StringComparison.OrdinalIgnoreCase))
                    fileName = "lib" + fileName;
            }

            return fileName;
        }

        [DllImport("libdl", EntryPoint = "dlopen")]
        private static extern IntPtr UnixLoadLibrary(string fileName, int flags);

        [DllImport("libdl", EntryPoint = "dlclose", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int UnixFreeLibrary(IntPtr handle);

        [DllImport("libdl", EntryPoint = "dlsym", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr UnixGetProcAddress(IntPtr handle, string symbol);

        [DllImport("libdl", EntryPoint = "dlerror", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr UnixGetLastError();
    }
}