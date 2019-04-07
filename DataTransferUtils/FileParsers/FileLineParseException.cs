using System;

namespace DataTransferUtils.FileParsers
{
    public class FileLineParseException : Exception
    {
        public FileLineContext Context { get; private set; }

        internal FileLineParseException(FileLineContext context, string errorMessage, Exception innerException)
            : base(string.Format("Error parsing line {0} of file '{1}': {2}", context.LineNumber, context.FilePath, errorMessage), innerException)
        {
            Context = context;
        }
    }
}