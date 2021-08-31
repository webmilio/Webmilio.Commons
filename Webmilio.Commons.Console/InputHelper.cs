using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Webmilio.Commons.Console
{
    public static class InputHelper
    {
        public static bool Choose(string question, bool def, bool otherIsFalse = false)
        {
            bool? answer = default;
            StringBuilder sb = new("(");

            void AddChar(char c, bool surround)
            {
                if (surround)
                    sb.Append('[');
                sb.Append(c);
                if (surround)
                    sb.Append(']');
            }

            AddChar('y', def);
            sb.Append('/');
            AddChar('n', !def);

            if (otherIsFalse)
                sb.Append('!');

            sb.Append(')');

            System.Console.Write("{0} {1}? ", question, sb);

            while (answer == default)
            {
                var response = System.Console.ReadLine();

                if (string.IsNullOrWhiteSpace(response))
                {
                    return def;
                }

                var a = response[0];

                if (a == 'y' || a == 't')
                {
                    answer = true;
                }
                else if (otherIsFalse || a == 'n' || a == 'f')
                {
                    answer = false;
                }
                else
                {
                    System.Console.WriteLine("Value not accepted.");
                }
            }

            return answer.Value;
        }

        public static int EnterNumber(string question, int? min, int? max)
        {
            int? entered = default;

            while (!entered.HasValue)
            {
                System.Console.Write(question);

                if (int.TryParse(System.Console.ReadLine(), out var input))
                {
                    if (min.HasValue && input < min)
                        System.Console.WriteLine("Entered value must be over {0}.", min);
                    else if (max.HasValue && input > max)
                        System.Console.WriteLine("Entered value must be under {0}.", max);
                    else
                        entered = input;
                }
                else
                    System.Console.WriteLine("Input must be numeric.");
            }

            return entered.Value;
        }

        public static DirectoryInfo EnterDirectory(string question)
        {
            DirectoryInfo directory = default;

            while (directory == default)
            {
                System.Console.Write(question);

                string path = System.Console.ReadLine();

                if (Directory.Exists(path))
                    directory = new DirectoryInfo(path);
                else
                    System.Console.WriteLine("Specified path is invalid.");
            }

            return directory;
        }

        public static FileInfo EnterFile(string question)
        {
            FileInfo file = default;

            while (file == default)
            {
                System.Console.Write(question);

                string path = System.Console.ReadLine();

                if (File.Exists(path))
                    file = new FileInfo(path);
                else
                    System.Console.WriteLine("Specified path is invalid.");
            }

            return file;
        }
    }
}