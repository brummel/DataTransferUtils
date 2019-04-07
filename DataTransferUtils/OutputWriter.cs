namespace DataTransferUtils
{
    public abstract class OutputWriter
    {
        public abstract void WriteLine(string format, params object[] args);
    }
}