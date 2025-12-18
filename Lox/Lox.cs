using System;
using System.IO;

namespace Lox;

internal static class ExitCodes
{
	public const int Success = 0;
	public const int Usage = 64;    // EX_USAGE
	public const int DataErr = 65;  // EX_DATAERR
}

static class Lox
{
	private static bool _hadError = false;
	
    public static int Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.Error.WriteLine($"Usage: Lox [script]");
            return ExitCodes.Usage;
        }

        if (args.Length == 1)
        {
            Lox.RunFile(args[0]);
        }
        else
        {
            Lox.RunPrompt();
        }

        return ExitCodes.Success;
    }

    private static void RunFile(string path)
    {
        string source = File.ReadAllText(path); // throws
        
        Lox.Run(source);
        if (Lox._hadError)
        {
	        Environment.Exit(ExitCodes.DataErr);
        }
    }

    private static void RunPrompt()
    {
        while (true)
        {
            Console.Write("> ");
            Console.Out.Flush();
            string? line = Console.ReadLine();
            if (line == null)
            {
                return;
            }

            Lox.Run(line);
            Lox._hadError = false; // don't kill the session on errors
        }
    }

    private static void Run(string command)
    {
        Console.WriteLine($"Running:\n{command}");
    }

    static void Error(int line, string message)
    {
	    Lox.Report(line, "", message);
    }

    private static void Report(int line, string where, string message)
    {
	    Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
	    Lox._hadError = true;
    }
}
