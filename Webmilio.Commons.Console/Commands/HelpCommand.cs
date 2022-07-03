using System;
using System.Threading.Tasks;

namespace Webmilio.Commons.Console.Commands;

public class HelpCommand : Command
{
    internal static string spacing = new(' ', 4);
    private readonly CommandLoader _commands;

    public HelpCommand(CommandLoader commands) : base("help", "?")
    {
        _commands = commands;
    }

    public override Task Execute(object sender, string input, string[] args)
    {
        System.Console.WriteLine("> All commands are case-insensitive, i.e. updateDatabase can be written as uPdAtEDataBasE.");
        System.Console.WriteLine("> Parameters in {} are required, in [] are optional.");
        System.Console.WriteLine();

        if (args.Length < 2)
        {
            foreach (var command in _commands.Generics)
                ShowHelp(command);
        }
        else
        {
            var command = _commands.GetCommand(args[1]);

            if (command == default)
            {
                ConsoleHandler.CommandNotFound(args[1]);
                return Task.CompletedTask;
            }

            ShowHelp(command);
        }

        return Task.CompletedTask;
    }

    private void ShowHelp(Command command)
    {
        System.Console.Write("# {0}", command.Name);

        if (command.Aliases.Length > 0)
        {
            System.Console.Write(" (");

            for (int i = 0; i < command.Aliases.Length; i++)
            {
                System.Console.Write(command.Aliases[i]);

                if (i + 1 < command.Aliases.Length)
                    System.Console.Write(", ");
            }

            System.Console.Write(")");
        }

        if (command.Description != default || command.DefaultValue != default)
        {
            System.Console.WriteLine(":");
            System.Console.Write(spacing);

            if (command.Description != default)
                System.Console.Write("{0}", command.Description);

            if (command.DefaultValue != default)
            {
                if (command.Description != default)
                    System.Console.Write(' ');

                System.Console.Write("[default: {0}]", command.DefaultValue);
            }
        }

        if (command.Usage != default || command.Flags != default)
        {
            System.Console.WriteLine();

            if (command.Usage != default)
                System.Console.WriteLine("{0}Usage: {1}", spacing, command.Usage);

            if (command.Flags != default)
            {
                System.Console.WriteLine("{0}Flags:", spacing);

                foreach (var flag in command.Flags)
                    System.Console.WriteLine("{0}{0}{1}: {2}", spacing, flag.flag, flag.description);
            }
        }

        System.Console.WriteLine(Environment.NewLine);
    }

    public override string Description { get; } = "Shows the list of available commands, their aliases, their description (if present) and default value (if present).";
}