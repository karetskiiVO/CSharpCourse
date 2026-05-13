using System.Drawing;
using System.Runtime.InteropServices;

namespace TaskHub.Utils;

public static class Logging {
    public enum LogLevel : byte {
        All,
        Trace,
        Debug,
        Info,
        Warning,
        Error,
        Fatal,
    }

    public static LogLevel Level { get; set; } = LogLevel.Debug;

    private static void Log(ConsoleColor color, LogLevel msgLevel, params object?[]? args) {
        if (msgLevel < Level) return;

        Console.Write($"{DateTime.Now} [");
        var originalFore = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.Write(msgLevel);
        Console.ForegroundColor = originalFore;
        Console.Write("] ");
        Console.WriteLine(args != null ? string.Join(" ", args) : string.Empty);
    }

    private static void Logf(ConsoleColor color, LogLevel msgLevel, string format, params object?[]? args) {
        if (msgLevel < Level) return;

        Console.Write($"{DateTime.Now} [");
        var originalFore = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.Write(msgLevel);
        Console.ForegroundColor = originalFore;
        Console.Write("] ");
        Console.WriteLine(format, args);
    }

    public static void Trace(params object?[]? args) => Log(ConsoleColor.DarkCyan, LogLevel.Trace, args);
    public static void Tracef(string format, params object?[]? args) => Logf(ConsoleColor.DarkCyan, LogLevel.Trace, format, args);


    public static void Debug(params object?[]? args) => Log(ConsoleColor.DarkGreen, LogLevel.Debug, args);
    public static void Debugf(string format, params object?[]? args) => Logf(ConsoleColor.DarkGreen, LogLevel.Debug, format, args);

    public static void Info(params object?[]? args) => Log(ConsoleColor.DarkGray, LogLevel.Info, args);
    public static void Infof(string format, params object?[]? args) => Logf(ConsoleColor.DarkGray, LogLevel.Info, format, args);

    public static void Warning(params object?[]? args) => Log(ConsoleColor.Yellow, LogLevel.Warning, args);
    public static void Warningf(string format, params object?[]? args) => Logf(ConsoleColor.Yellow, LogLevel.Warning, format, args);

    public static void Error(params object?[]? args) => Log(ConsoleColor.Red, LogLevel.Error, args);
    public static void Errorf(string format, params object?[]? args) => Logf(ConsoleColor.Red, LogLevel.Error, format, args);

    public static void Fatal(params object?[]? args) {
        Log(ConsoleColor.Red, LogLevel.Fatal, args);
        Environment.Exit(-1);
    }
    public static void Fatalf(string format, params object?[]? args) {
        Logf(ConsoleColor.Red, LogLevel.Error, format, args);
        Environment.Exit(-1);
    }
}

