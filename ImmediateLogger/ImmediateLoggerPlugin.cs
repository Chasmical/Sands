using System.IO;
using BepInEx;
using BepInEx.Logging;

namespace ImmediateLogger;
[BepInPlugin(@"abbysssal.streetsofrogue.immediatelogger", "S&S: Immediate Logger", "1.0.0")]
public class ImmediateLoggerPlugin : BaseUnityPlugin, ILogListener
{
    private TextWriter writer = null!;
    public void Awake()
    {
        string logFile = Path.Combine(Paths.GameRootPath, "ImmediateLog.txt");
        writer = new StreamWriter(logFile);
        BepInEx.Logging.Logger.Listeners.Add(this);
        writer.WriteLine("LOGGER INITIALIZED");
    }
    public void Dispose()
    {
        writer.WriteLine("LOGGER DISPOSED");
    }
    public void LogEvent(object sender, LogEventArgs eventArgs)
    {
        string line = $"[{eventArgs.Source}: {eventArgs.Level}] {eventArgs.Data}";
        writer.WriteLine(line);
        writer.Flush();
    }
}
