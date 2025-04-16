using System;

namespace GeyserExpandMachine;

public class LogUtil {
    private const string PREFIX = "【间歇泉扩展/GeyserExpand】";
    
    public static void Info(string message) {
        Console.WriteLine($"{PREFIX} [INFO] {message}");
    }

    public static void Warning(string message) {
        Console.WriteLine($"{PREFIX} [WARN] {message}");
    }

    public static void Error(string message) {
        Console.WriteLine($"{PREFIX} [Error] {message}");
    }
}