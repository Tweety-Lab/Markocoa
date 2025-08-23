namespace Markocoa;

using CommandLine;
using Markocoa.Commands;

internal class Program
{
    /// <summary>
    /// All registered commands.
    /// </summary>
    public static readonly Type[] Commands =
    {
        typeof(NewCommand),
        typeof(BuildCommand),
    };

    static void Main(string[] args)
    {
        // Run all commands
        Parser.Default.ParseArguments(args, Commands)
            .WithParsed<ICommand>(cmd => cmd.Execute());
    }
}
