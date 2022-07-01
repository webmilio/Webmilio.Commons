using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Webmilio.Commons.Console.Commands;
using Webmilio.Commons.DependencyInjection;
using Webmilio.Commons.Extensions.Reflection;

namespace Webmilio.Commons.Console.Commands;

public class CommandLoader
{
    private readonly IServiceProvider _services;
    private readonly List<Command> _generics = new();

    public CommandLoader(IServiceProvider services)
    {
        _services = services;
        Generics = _generics.AsReadOnly();
    }

    public void Initialize()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            foreach (var type in assembly.Concrete<Command>())
                _generics.Add(_services.Make(type) as Command);
    }

    public async Task<bool> Execute(object sender, string input, string cmd, string[] args)
    {
        var command = GetCommand(cmd);

        if (command == default)
            return false;

        if (args.Length - 1 < command.RequiredArgsCount)
            throw new CommandExecutionException($"Incorrect amount of parameters specified:\n{HelpCommand.spacing}Usage: {command.Usage}\nFor more information about this command, do help/? <command>");

        await command?.Execute(sender, input, args);
        return true;
    }

    public Command GetCommand(string name)
    {
        var generic = _generics.FirstOrDefault(command => command.IsCommand(name));
        if (generic == null) return null;

        return _services.Make(generic.GetType()) as Command;
    }


    public ReadOnlyCollection<Command> Generics { get; }
}