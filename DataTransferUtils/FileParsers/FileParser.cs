using System.Data;
using FileHelpers;

namespace DataTransferUtils.FileParsers
{
    public abstract class FileParser
    {
        public OutputWriter ErrorWriter
        {
            get;
            set;
        }

        public virtual void InitForBatch()
        {

        }

        public virtual void InitForFile()
        {

        }

        public abstract DataTable ParseFileAsDT(string filePath);
    }

    public abstract class FileParser<T> : FileParser where T : class
    {
        public abstract T[] ParseFile(string filePath);

        public override DataTable ParseFileAsDT(string filePath)
        {
            return ParseFile(filePath).ToDataTable();
        }
    }
}