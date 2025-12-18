namespace Lox;

public enum TokenType
{
	LParen,
	RParen,
	LBrace,
	RBrace,
	Comma,
	Dot,
	Minus,
	Plus,
	SemiColon,
	Slash,
	Star,

	Bang,
	BangEqual,
	Equal,
	EqualEqual,
	Less,
	LessEqual,

	Identifier,
	String,
	Number,

	And,
	Class,
	Else,
	False,
	Fun,
	For,
	If,
	Nil,
	Or,
	Print,
	Return,
	Super,
	This,
	True,
	Var,
	While,

	Eof,
}

internal static class ExitCodes
{
	public const int Success = 0;
	public const int Usage = 64;    // EX_USAGE
	public const int DataErr = 65;  // EX_DATAERR
	public const int Software = 70; // EX_SOFTWARE
}

static class Lox
{
	static bool hadError = false;
	
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

        return 0;
    }

    private static void RunFile(string path)
    {
        string source = File.ReadAllText(path); // throws
        
        Lox.Run(source);
        if (Lox.hadError)
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
            Lox.hadError = false; // don't kill the session on errors
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
	    Lox.hadError = true;
    }
}
