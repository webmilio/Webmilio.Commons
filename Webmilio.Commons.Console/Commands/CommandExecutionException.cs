using System;

namespace Webmilio.Commons.Console.Commands
{
    public class CommandExecutionException : Exception
    {
        public CommandExecutionException()
        {
        }

        public CommandExecutionException(string message) : base(message)
        {
        }
    }
}