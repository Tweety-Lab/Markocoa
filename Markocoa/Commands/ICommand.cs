
namespace Markocoa.Commands;

/// <summary>
/// Base interface for all commands.
/// </summary>
internal interface ICommand
{
    /// <summary>
    /// Executes the command.
    /// </summary>
    void Execute();
}
