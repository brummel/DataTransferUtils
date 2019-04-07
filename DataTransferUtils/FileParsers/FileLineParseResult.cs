namespace DataTransferUtils.FileParsers
{
    public class FileLineParseResult<T> where T : class
    {
        public T Value { get; set; }
        public bool SkipLine { get; set; }

        public FileLineParseResult(T value, bool skipLine)
        {
            Value = value;
            SkipLine = skipLine;
        }
    }
}