//  Copyright (c) 2014 Andrey Akinshin
//  Project URL: https://github.com/AndreyAkinshin/InteropDotNet
//  Distributed under the MIT License: http://opensource.org/licenses/MIT

namespace Tesseract.Internal
{
    using System.Diagnostics;
    using System.Globalization;

    public static class Logger
    {
        private static readonly TraceSource Trace = new("Tesseract");

        public static void TraceInformation(string format, params object?[] args)
        {
            Trace.TraceEvent(TraceEventType.Information, 0, string.Format(CultureInfo.CurrentCulture, format, args));
        }

        public static void TraceError(string format, params object[] args)
        {
            Trace.TraceEvent(TraceEventType.Error, 0, string.Format(CultureInfo.CurrentCulture, format, args));
        }

        public static void TraceWarning(string format, params object[] args)
        {
            Trace.TraceEvent(TraceEventType.Warning, 0, string.Format(CultureInfo.CurrentCulture, format, args));
        }
    }
}