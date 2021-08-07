using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webmilio.Commons.Console.Commands
{
    public class CommandLoader
    {
        private static CommandLoader _instance;
        private readonly List<Command> _generics = new List<Command>();


        private CommandLoader()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                foreach (var type in assembly.DefinedTypes.Where(t => !t.IsAbstract && !t.IsInterface && t.IsSubclassOf(typeof(Command))))
                        _generics.Add(Activator.CreateInstance(type) as Command);
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
            return _generics.FirstOrDefault(command => command.IsCommand(name));
        }


        public IReadOnlyList<Command> Generics => _generics.AsReadOnly();


        public static CommandLoader Instance => _instance ??= new CommandLoader();
    }
}