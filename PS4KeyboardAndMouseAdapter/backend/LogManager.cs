using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace PS4KeyboardAndMouseAdapter.backend
{
    public class LogManager
    {

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

            Logger log = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(levelSwitch)
                .WriteTo.Console()
                .WriteTo.File(LogUtility.GetLogFile())
                .CreateLogger();

            Log.Logger = log;
            Log.Information("Log created");
        }

    }
}
