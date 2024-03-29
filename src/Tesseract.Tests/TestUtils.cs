namespace Tesseract.Tests
{
    using System.Diagnostics;

    public static class TestUtils
    {
        /// <summary>
        ///     Normalise new line characters to unix (\n) so they are all the same.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string NormaliseNewLine(string text)
        {
            return text
                .Replace("\r\n", "\n")
                .Replace("\r", "\n");
        }

        public static void Cmd(string command, params object[] arguments)
        {
            string argumentStr = string.Join(" ", arguments.Select(x => $"\"{x}\""));
            var processInfo = new ProcessStartInfo(command, argumentStr)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            Process process = Process.Start(processInfo) ?? throw new ArgumentNullException("Process.Start(processInfo)");
            process.WaitForExit();

            // *** Read the streams ***
            // Warning: This approach can lead to deadlocks, see Edit #2
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            int exitCode = process.ExitCode;

            Console.WriteLine("output>>" + (string.IsNullOrEmpty(output) ? "(none)" : output));
            Console.WriteLine("error>>" + (string.IsNullOrEmpty(error) ? "(none)" : error));
            Console.WriteLine("ExitCode: " + exitCode, "ExecuteCommand");
            process.Close();
        }
    }
}