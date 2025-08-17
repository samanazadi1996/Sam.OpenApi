using System;

namespace OpenApi.Helpers
{
    public static class Logger
    {
        public static void LogSuccess(string log)
        {
            Log(log, ConsoleColor.Green);
        }
        public static void LogInfo(string log)
        {
            Log(log, ConsoleColor.DarkCyan);
        }
        public static void LogError(string log)
        {
            Log(log, ConsoleColor.Red);
        }
        public static void Log(string log, ConsoleColor color)
        {
            var tmp = Console.ForegroundColor;

            Console.ForegroundColor = color;

            Console.WriteLine(log);

            Console.ForegroundColor = tmp;

        }

    }
}
