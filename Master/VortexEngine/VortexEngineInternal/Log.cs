using System.Diagnostics;
using System.Reflection;

namespace VortexEngine;

public enum DebugLevel
{
    None = 0,
    Light = 1,
    Medium = 2,
    Verbose = 3,
}

public static class Log
{
    public static TextWriter logOutput;
    public static TextWriter stdout;

    private static int indent = 0;

    public static bool ShowStackTrace = true;
    
    private static DebugLevel logLevel = DebugLevel.None;

    public static void Indent()
    {
        indent++;
    }

    public static void SetLogLevel(DebugLevel level)
    {
        logLevel = level;
    }

    public static void UnIndent()
    {
        indent--;
        if (indent < 0)
        {
            indent = 0;
        }
    }

    public static void InitLogger()
    {
        TextWriter stdout = Console.Out;
        
        StringWriter debuglog = new StringWriter();
        Console.SetOut(debuglog);

        Log.stdout = stdout;
        logOutput = debuglog;

    }

    private static string GetTrace(StackTrace trace)
    {
        int i = 1;
        while (trace.GetFrame(i).GetMethod().Module.Assembly == Assembly.GetExecutingAssembly())
        {
            i++;
        }
        if (trace.GetFrame(1).GetMethod().Module.Assembly == Assembly.GetExecutingAssembly() && trace.GetFrame(i).GetFileName() != null)
        {
            FileInfo file = new FileInfo(trace.GetFrame(i).GetFileName());
            return (ShowStackTrace ? (" [" + trace.GetFrame(i - 1).GetMethod().Name + " in " + trace.GetFrame(i).GetMethod().DeclaringType.Name + "." + trace.GetFrame(i).GetMethod().Name + " (" + file.Name + ":" + trace.GetFrame(i).GetFileLineNumber() + ")]") : "");
        }
        return (ShowStackTrace ? (" [" + trace.GetFrame(1).GetMethod().Name + " (" + new FileInfo(trace.GetFrame(1).GetFileName()).Name + ":" + trace.GetFrame(1).GetFileLineNumber() + ")]") : "");
    }
    
    public static void Info(string value)
    {
        Console.SetOut(stdout);
        ConsoleColor ogForeground = Console.ForegroundColor;
        ConsoleColor ogBackground = Console.BackgroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        StackTrace trace = new StackTrace(true);
        
        if (trace.GetFrame(1) != null && trace.GetFrame(1).GetFileName() != null)
        {
            Console.Write($"[INFO]{GetTrace(trace)} [{DateTime.Now.ToShortTimeString()}] ");
        }
        else
        {
            Console.Write($"[INFO] [{DateTime.Now.ToShortTimeString()}] ");
        }
        
        Console.ForegroundColor = ogForeground;
        for (int i = 0; i < indent; i++)
        {
            Console.Write("  ");
        }
        Console.WriteLine($"{value}");
        Console.ForegroundColor = ogForeground;
        Console.BackgroundColor = ogBackground;
        Console.SetOut(logOutput);
    }
    
    public static void Debug(string value, DebugLevel level = DebugLevel.Light)
    {
        if ((int)logLevel <= (int)level)
        {
            Console.SetOut(stdout);
            ConsoleColor ogForeground = Console.ForegroundColor;
            ConsoleColor ogBackground = Console.BackgroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            StackTrace trace = new StackTrace(true);
            if (trace.GetFrame(1) != null && trace.GetFrame(1).GetFileName() != null)
            {
                Console.Write($"[INFO]{GetTrace(trace)} [{DateTime.Now.ToShortTimeString()}] ");
            }
            else
            {
                Console.Write($"[DEBUG] [{DateTime.Now.ToShortTimeString()}] ");
            }
            Console.ForegroundColor = ogForeground;
            for (int i = 0; i < indent; i++)
            {
                Console.Write("  ");
            }
            Console.WriteLine($"{value}");
            Console.ForegroundColor = ogForeground;
            Console.BackgroundColor = ogBackground;
            Console.SetOut(logOutput);
        }
        
    }

    public static void Warning(string value)
    {
        Console.SetOut(stdout);
        ConsoleColor ogForeground = Console.ForegroundColor;
        ConsoleColor ogBackground = Console.BackgroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        StackTrace trace = new StackTrace(true);
        if (trace.GetFrame(1) != null && trace.GetFrame(1).GetFileName() != null)
        {
            Console.Write($"[WARN]{GetTrace(trace)} [{DateTime.Now.ToShortTimeString()}] ");
        }
        else
        {
            Console.Write($"[WARN] [{DateTime.Now.ToShortTimeString()}] ");
        }
        for (int i = 0; i < indent; i++)
        {
            Console.Write("  ");
        }
        Console.WriteLine(value);
        Console.ForegroundColor = ogForeground;
        Console.BackgroundColor = ogBackground;
        Console.SetOut(logOutput);
    }

    public static void Error(string value)
    {
        Console.SetOut(stdout);
        ConsoleColor ogForeground = Console.ForegroundColor;
        ConsoleColor ogBackground = Console.BackgroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        StackTrace trace = new StackTrace(true);
        if (trace.GetFrame(1) != null && trace.GetFrame(1).GetFileName() != null)
        {
            Console.Write($"[ERROR]{GetTrace(trace)} [{DateTime.Now.ToShortTimeString()}] ");
        }
        else
        {
            Console.Write($"[ERROR] [{DateTime.Now.ToShortTimeString()}] ");
        }
        for (int i = 0; i < indent; i++)
        {
            Console.Write("  ");
        }
        Console.WriteLine(value);
        Console.ForegroundColor = ogForeground;
        Console.BackgroundColor = ogBackground;
        Console.SetOut(logOutput);
    }

    public static void Fatal(string value)
    {
        Console.SetOut(stdout);
        
        ConsoleColor ogForeground = Console.ForegroundColor;
        ConsoleColor ogBackground = Console.BackgroundColor;
        Console.ForegroundColor = ConsoleColor.DarkRed;
        
        StackTrace trace = new StackTrace(true);
        Console.WriteLine($"[FATAL] [{DateTime.Now.ToShortTimeString()}] {value}");
        Console.ForegroundColor = ogForeground;
        Console.BackgroundColor = ogBackground;
        
        Console.SetOut(logOutput);
        

    }
    
    

}
