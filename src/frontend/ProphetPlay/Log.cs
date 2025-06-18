using Serilog;
using Serilog.Core;

namespace ProphetPlay
{
    /// <summary>
    /// Globale Logger-Klasse für Serilog
    /// </summary>
    public static class LoggerService
    {
        public static Logger Logger { get; private set; }

        static LoggerService()
        {
            Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/log.txt",
                              rollingInterval: RollingInterval.Day,
                              retainedFileCountLimit: 7,
                              fileSizeLimitBytes: 100_000_000)
                .CreateLogger();

            Serilog.Log.Logger = Logger; // globale Serilog-Instanz
        }

        public static void Shutdown()
        {
            Serilog.Log.CloseAndFlush();
        }
    }
}