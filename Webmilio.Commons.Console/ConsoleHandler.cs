using System;
using System.Threading;
using System.Threading.Tasks;
using Webmilio.Commons.Console.Commands;

namespace Webmilio.Commons.Console;

public class ConsoleHandler
{
    private readonly CommandLoader _commands;

    public ConsoleHandler(CommandLoader commands)
    {
        _commands = commands;
    }

    public static void CommandNotFound(string commandName)
    {
        WriteLineError($"Command `{commandName}` not found.{Environment.NewLine}For a list of commands, do help");
    }

    public static void WriteLineError(string message)
    {
        var cc = System.Console.ForegroundColor;
        System.Console.ForegroundColor = ConsoleColor.Red;

        System.Console.Error.WriteLine(message);
        System.Console.ForegroundColor = cc;
    }

    public static void WriteLineError(string format, params object[] args)
    {
        WriteLineError(string.Format(format, args));
    }

    protected virtual void PreListen() => System.Console.Write("#> ");

    public async Task ListenToCommands(object origin, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
            await ListenToCommand(origin);
    }

    public Task ListenToCommands(object origin) => ListenToCommands(origin, CancellationToken.None);

    public async Task ListenToCommand(object origin)
    {
        PreListen();
        var input = System.Console.ReadLine();

        await Execute(origin, input);
    }

    public async Task Execute(object origin, string input)
    {
        var args = input.Split(' ');
        string cmd = args[0];

        try
        {
            if (!await _commands.Execute(origin, input, cmd, args))
                CommandNotFound(cmd);
        }
        catch (Exception e)
        {
            WriteLineError($"Error occured while executing command:\n{HelpCommand.spacing}{e.Message}");
        }
    }
}