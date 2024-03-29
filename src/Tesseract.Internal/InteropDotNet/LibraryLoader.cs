//  Copyright (c) 2014 Andrey Akinshin
//  Project URL: https://github.com/AndreyAkinshin/InteropDotNet
//  Distributed under the MIT License: http://opensource.org/licenses/MIT

namespace InteropDotNet
{
    using System.Reflection;

    using Tesseract.Abstractions;
    using Tesseract.Internal;

    public sealed class LibraryLoader
    {
        private static LibraryLoader? instance;
        private readonly Dictionary<string, IntPtr> loadedAssemblies = new();
        private readonly ILibraryLoaderLogic logic;

        private readonly object syncLock = new();

        private LibraryLoader(ILibraryLoaderLogic logic)
        {
            this.logic = logic ?? throw new ArgumentNullException(nameof(logic));
        }

        public string? CustomSearchPath { get; set; }

        public static LibraryLoader? Instance
        {
            get
            {
                if (instance == null)
                {
                    OperatingSystem operatingSystem = SystemManager.GetOperatingSystem();
                    switch (operatingSystem)
                    {
                        case OperatingSystem.Windows:
                            Logger.TraceInformation("Current OS: Windows");
                            instance = new LibraryLoader(new WindowsLibraryLoaderLogic());
                            break;
                        case OperatingSystem.Unix:
                            Logger.TraceInformation("Current OS: Unix");
                            instance = new LibraryLoader(new UnixLibraryLoaderLogic());
                            break;
                        case OperatingSystem.MacOSX:
                            Logger.TraceInformation("Current OS: MacOsX");
                            instance = new LibraryLoader(new UnixLibraryLoaderLogic());
                            break;
                        default:
                            throw new Exception("Unsupported operation system");
                    }
                }

                return instance;
            }
        }

        public IntPtr LoadLibrary(string fileName, string? platformName = null)
        {
            fileName = this.FixUpLibraryName(fileName);
            lock (this.syncLock)
            {
                if (!this.loadedAssemblies.ContainsKey(fileName))
                {
                    if (platformName == null)
                        platformName = SystemManager.GetPlatformName();

                    Logger.TraceInformation("Current platform: " + platformName);

                    IntPtr dllHandle = this.CheckCustomSearchPath(fileName, platformName);
                    if (dllHandle == IntPtr.Zero)
                        dllHandle = this.CheckExecutingAssemblyDomain(fileName, platformName);
                    if (dllHandle == IntPtr.Zero)
                        dllHandle = this.CheckCurrentAppDomain(fileName, platformName);
                    if (dllHandle == IntPtr.Zero)
                        dllHandle = this.CheckCurrentAppDomainBin(fileName, platformName);
                    if (dllHandle == IntPtr.Zero)
                        dllHandle = this.CheckWorkingDirecotry(fileName, platformName);

                    if (dllHandle != IntPtr.Zero)
                        this.loadedAssemblies[fileName] = dllHandle;
                    else
                        throw new DllNotFoundException($"Failed to find library \"{fileName}\" for platform {platformName}.");
                }

                return this.loadedAssemblies[fileName];
            }
        }

        private IntPtr CheckCustomSearchPath(string fileName, string platformName)
        {
            string? baseDirectory = this.CustomSearchPath;
            if (!string.IsNullOrEmpty(baseDirectory))
            {
                Logger.TraceInformation("Checking custom search location '{0}' for '{1}' on platform {2}.", baseDirectory, fileName, platformName);
                return this.InternalLoadLibrary(baseDirectory, platformName, fileName);
            }

            Logger.TraceInformation("Custom search path is not defined, skipping.");
            return IntPtr.Zero;
        }

        private IntPtr CheckExecutingAssemblyDomain(string fileName, string platformName)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            if (executingAssembly == null)
                // #591 Executing assembly may be null in some cases
                return IntPtr.Zero;

            string? baseDirectory = Path.GetDirectoryName(executingAssembly.Location);
            Logger.TraceInformation("Checking executing application domain location '{0}' for '{1}' on platform {2}.", baseDirectory, fileName, platformName);
            if (baseDirectory != null) return this.InternalLoadLibrary(baseDirectory, platformName, fileName);
            return IntPtr.Zero;
        }

        private IntPtr CheckCurrentAppDomain(string fileName, string platformName)
        {
            string baseDirectory = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
            Logger.TraceInformation("Checking current application domain location '{0}' for '{1}' on platform {2}.", baseDirectory, fileName, platformName);
            return this.InternalLoadLibrary(baseDirectory, platformName, fileName);
        }

        /// <summary>
        ///     Special test for web applications.
        /// </summary>
        /// <remarks>
        ///     Note that this makes a couple of assumptions these being:
        ///     <list type="bullet">
        ///         <item>
        ///             That the current application domain's location for web applications corresponds to the web applications
        ///             root directory.
        ///         </item>
        ///         <item>
        ///             That the tesseract\leptonica dlls reside in the corresponding x86 or x64 directories in the bin directory
        ///             under the apps root directory.
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <param name="fileName"></param>
        /// <param name="platformName"></param>
        /// <returns></returns>
        private IntPtr CheckCurrentAppDomainBin(string fileName, string platformName)
        {
            string baseDirectory = Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory), "bin");
            if (Directory.Exists(baseDirectory))
            {
                Logger.TraceInformation("Checking current application domain's bin location '{0}' for '{1}' on platform {2}.", baseDirectory, fileName, platformName);
                return this.InternalLoadLibrary(baseDirectory, platformName, fileName);
            }

            Logger.TraceInformation("No bin directory exists under the current application domain's location, skipping.");
            return IntPtr.Zero;
        }

        private IntPtr CheckWorkingDirecotry(string fileName, string platformName)
        {
            string baseDirectory = Path.GetFullPath(Environment.CurrentDirectory);
            Logger.TraceInformation("Checking working directory '{0}' for '{1}' on platform {2}.", baseDirectory, fileName, platformName);
            return this.InternalLoadLibrary(baseDirectory, platformName, fileName);
        }

        private IntPtr InternalLoadLibrary(string baseDirectory, string platformName, string fileName)
        {
            string fullPath = Path.Combine(baseDirectory, Path.Combine(platformName, fileName));
            return File.Exists(fullPath) ? this.logic.LoadLibrary(fullPath) : IntPtr.Zero;
        }

        public bool FreeLibrary(string fileName)
        {
            fileName = this.FixUpLibraryName(fileName);
            lock (this.syncLock)
            {
                if (!this.IsLibraryLoaded(fileName))
                {
                    Logger.TraceWarning("Failed to free library \"{0}\" because it is not loaded", fileName);
                    return false;
                }

                if (this.logic.FreeLibrary(this.loadedAssemblies[fileName]))
                {
                    this.loadedAssemblies.Remove(fileName);
                    return true;
                }

                return false;
            }
        }

        public IntPtr GetProcAddress(IntPtr dllHandle, string name)
        {
            IntPtr procAddress = this.logic.GetProcAddress(dllHandle, name);
            if (procAddress == IntPtr.Zero) throw new LoadLibraryException($"Failed to load proc {name}");

            return procAddress;
        }

        public bool IsLibraryLoaded(string fileName)
        {
            fileName = this.FixUpLibraryName(fileName);
            lock (this.syncLock)
            {
                return this.loadedAssemblies.ContainsKey(fileName);
            }
        }

        private string FixUpLibraryName(string fileName)
        {
            return this.logic.FixUpLibraryName(fileName);
        }
    }
}