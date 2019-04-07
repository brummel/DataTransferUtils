using System;

namespace DataTransferUtils
{
    public class ConsoleOutputWriter : OutputWriter
    {
        public override void WriteLine(string format, params object[] args)
        {
            Console.WriteLine(format, args);
        }
    }
}