//  Copyright (c) 2014 Andrey Akinshin
//  Project URL: https://github.com/AndreyAkinshin/InteropDotNet
//  Distributed under the MIT License: http://opensource.org/licenses/MIT

namespace InteropDotNet
{
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    internal sealed class RuntimeDllImportAttribute : Attribute
    {
        public bool BestFitMapping;

        public CallingConvention CallingConvention;

        public CharSet CharSet;
        public string? EntryPoint;

        public bool SetLastError;

        public bool ThrowOnUnmappableChar;

        public RuntimeDllImportAttribute(string libraryFileName)
        {
            if (string.IsNullOrWhiteSpace(libraryFileName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(libraryFileName));
            this.LibraryFileName = libraryFileName;
        }

        public string LibraryFileName { get; private set; }
    }
}