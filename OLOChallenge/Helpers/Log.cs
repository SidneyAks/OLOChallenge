using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLOChallenge.Helpers
{
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Fatal
    }

    /// <summary>
    /// This is a quick wrapper to easily log test data. Currently it writes to the console, which is visible via the visual studio test explorer.
    /// If additional logging is needed it could be updated.
    /// </summary>
    public static class Log
    {
        public static void WriteDebug(string Text) => Write(Text, LogLevel.Debug);
        public static void WriteInfo(string Text) => Write(Text, LogLevel.Info);
        public static void WriteWarning(string Text) => Write(Text, LogLevel.Warning);
        public static void WriteFatal(string Text) => Write(Text, LogLevel.Fatal);

        private static void Write(string Text, LogLevel Level)
        {
            Console.WriteLine($"[{Level.ToString().PadLeft(8)}] {DateTime.Now:dd/MM/yy HH:mm:ss} : {Text}");
        }
    }
}
