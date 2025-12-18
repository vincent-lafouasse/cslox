namespace Lox;

static class Program
{
    public static int Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.Error.WriteLine($"Usage: Lox [script]");
            return 1;
        }
        
        if (args.Length == 1)
        {
            RunFile(args[0]);
        }
        else
        {
            RunPrompt();
        }

        return 0;
    }

    private static void RunFile(string path)
    {
        string source = File.ReadAllText(path); // throws
        Run(source);
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
            
            Run(line);
        }
    }

    private static void Run(string command)
    {
        Console.WriteLine($"Running:\n{command}");
    }
}