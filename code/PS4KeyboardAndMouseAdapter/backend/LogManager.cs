using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace PS4KeyboardAndMouseAdapter.backend
{
    public class LogManager
    {
        private readonly int THREE_HUNDRED_MB_AS_BYTES = 300 * 1024 * 1024;

        LoggingLevelSwitch levelSwitch;

        public LogEventLevel GetLoggingLevel()
        {
            return levelSwitch.MinimumLevel;
        }

        public void SetLoggingLevel(LogEventLevel level)
        {
            Log.Information("Logger level set to " + level);
            levelSwitch.MinimumLevel = level;
        }

        public void SetupLogger()
        {
            levelSwitch = new LoggingLevelSwitch();
            levelSwitch.MinimumLevel = LogEventLevel.Information;

            // 2021.12.23
            // notes
            // * that fff in timestamp means miliseconds, but logs might not flush in sub seconds times.
            // * in theory max disk space should be 3 gb at worst (but thats unlikely unless we have very verbose logging)
            Logger log = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(levelSwitch)
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss.fff zzz} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(LogUtility.GetLogFile(),
                fileSizeLimitBytes: THREE_HUNDRED_MB_AS_BYTES,
                    retainedFileCountLimit: 10,
                    rollOnFileSizeLimit: true)
                .CreateLogger();

            Log.Logger = log;
            Log.Information("Log created");
        }

    }
}
