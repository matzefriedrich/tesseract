namespace Tesseract.Interop
{
    using Abstractions;

    using InteropDotNet;

    internal static class LeptonicaApi
    {
        private static ILeptonicaApiSignatures? native;

        public static ILeptonicaApiSignatures? Native
        {
            get
            {
                if (native == null)
                    Initialize();
                return native;
            }
        }

        public static void Initialize()
        {
            if (native == null) native = InteropRuntimeImplementer.CreateInstance<ILeptonicaApiSignatures>();
        }
    }
}