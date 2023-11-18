using System;

namespace LibreHardwareMonitor.Warpper;

public class Logger
{

    public enum Level
    {
        Debug,
        Info,
        Error
    }

    private static bool _isDebug = false;
    private const string LogMsgFormat = "LibreHardwareMonitor {0} [{1}]: {2}";
    public static void Debug(string logMsg)
    {
        Print(Level.Debug, logMsg);
    }
    public static void Info(string logMsg)
    {
        Print(Level.Info, logMsg);
    }
    public static void Error(string logMsg)
    {
        Print(Level.Error, logMsg);
    }

    public static void Print(Level level, string logMsg)
    {
        var date = DateTime.Now.ToString("HH:mm:ss.fff");
        var levelString = level.ToString();

        // TODO: 简单实现
        if (_isDebug || level == Level.Error)
        {
            Console.WriteLine(string.Format(LogMsgFormat, date, levelString, logMsg));
        }
    }

    public static object SetLevel(string level)
    {
        if (Enum.TryParse<Level>(level, out Level outLevel))
        {
            if (outLevel == Level.Debug)
            {
                _isDebug = true;
            }
        }
        else
        {
            Error($"Failed to parser level: {level}");
        }
        return null;
    }
}
