using System;
using System.Threading.Tasks;
using Webmilio.Commons.Console.Commands;

namespace Webmilio.Commons.Console
{
    public static class ConsoleHelper
    {
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

        public static async Task ListenToCommand(object origin)
        {
            // We wait for the user's input to kill the console.
            string input;

            while (!(input = System.Console.ReadLine()).Equals("exit", StringComparison.CurrentCultureIgnoreCase))
            {
                await Execute(origin, input);
            }
        }

        public static async Task Execute(object origin, string input)
        {
            var args = input.Split(' ');
            string cmd = args[0];

            try
            {
                if (!await CommandLoader.Instance.Execute(origin, input, cmd, args))
                    CommandNotFound(cmd);
            }
            catch (Exception e)
            {
                WriteLineError($"Error occured while executing command:\n{HelpCommand.spacing}{e.Message}");
            }
        }
    }
}