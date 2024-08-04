using System;

public class Debug
{
    public static void Info(object message){
        Console.WriteLine($"[\x1b[38;5;29m{DateTime.Now:HH:mm:ss}\x1b[37m] [\x1b[38;5;85mINFO\x1b[37m]: {message}");
    }
    public static void Error(object message){
        Console.WriteLine($"[\x1b[38;5;29m{DateTime.Now:HH:mm:ss}\x1b[37m] [\x1b[38;5;9mERRO\x1b[37m]: {message}");
    }
    public static void Warn(object message){
        Console.WriteLine($"[\x1b[38;5;29m{DateTime.Now:HH:mm:ss}\x1b[37m] [\x1b[38;5;222mWARN\x1b[37m]: {message}");
    }
}