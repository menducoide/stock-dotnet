using System;

namespace stock_dotnet.utils
{
    public interface ILoggerManager
    {
        void LogInfo(string message);
        void LogWarn(string message);
        void LogDebug(string message);
        void LogError(string message);
    }

    public class LoggerManager : ILoggerManager
    {
        private readonly string error = "error: {0}";
        private readonly string debug = "debug: {0}";
        private readonly string warn = "warning: {0}";
        private readonly string info = "info: {0}";
        public void LogDebug(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(string.Format(debug, message));
            Console.ResetColor();
        }

        public void LogError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(string.Format(error, message));
            Console.ResetColor();
        }

        public void LogInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(string.Format(info, message));
            Console.ResetColor();
        }

        public void LogWarn(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(string.Format(warn, message));
            Console.ResetColor();
        }
    }

}