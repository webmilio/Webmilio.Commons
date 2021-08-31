using System;
using System.Threading.Tasks;

namespace Webmilio.Commons.Console.Commands
{
    public abstract class Command
    {
        protected Command(string name, params string[] aliases)
        {
            Name = name;
            Aliases = aliases;
        }


        public abstract Task Execute(object sender, string input, string[] args);

        public bool IsCommand(string cmd)
        {
            if (cmd.Equals(Name, StringComparison.InvariantCultureIgnoreCase))
                return true;

            for (int i = 0; i < Aliases.Length; i++)
                if (cmd.Equals(Aliases[i], StringComparison.InvariantCultureIgnoreCase))
                    return true;

            return false;
        }


        public string Name { get; }
        public string[] Aliases { get; }

        public virtual string Description { get; }
        public virtual object DefaultValue { get; }

        public virtual string Usage { get; }
        public virtual int RequiredArgsCount { get; }
        public virtual (string flag, string description)[] Flags { get; }
    }
}