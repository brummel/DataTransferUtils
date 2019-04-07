using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferUtils.FileParsers;

namespace DataTransferUtils
{
    class Program
    {
        static void Main(string[] args)
        {
            // Register file parsers here...
            // FileParserFactory.Instance.Register<Type>("Key");

            (new Program()).Run(args);
        }

        public void Run(string[] args)
        {
            var verbOptions = new VerbOptions();
            Command command = null;
            if (CommandLine.Parser.Default.ParseArgumentsStrict(args, verbOptions, (v, o) => command = ((Command)o)))
            {
                try
                {
                    command.Execute();
                }
                catch (Exception ex)
                {
                    var currentColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("ERROR: {0}", GetExceptionMessageRecursive(ex));
                    Console.ForegroundColor = currentColor;
                    Environment.Exit(-1);
                }
            }
        }

        private string GetExceptionMessageRecursive(Exception ex)
        {
            var messages = new List<string>();
            var currentEx = ex;
            while (currentEx != null)
            {
                messages.Add(currentEx.Message);
                currentEx = currentEx.InnerException;
            }

            return string.Join(" ", messages);
        }
    }
}
