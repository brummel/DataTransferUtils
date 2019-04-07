using System;
using System.IO;

namespace DataTransferUtils.FileParsers
{
    public class FileLineContext
    {
        public string FilePath { get; private set; }
        public int LineNumber { get; private set; }
        public string LineValue { get; private set; }

        public string FileName
        {
            get { return Path.GetFileName(FilePath); }
        }

        public FileLineContext(string filePath)
        {
            FilePath = filePath;
            LineNumber = 0;
            LineValue = null;
        }

        public void SetNextLine(string lineValue)
        {
            LineNumber++;
            LineValue = lineValue;
        }

        public string ParseValue(int startIndex, int length)
        {
            return LineValue.Substring(startIndex, length);
        }

        public TValue ParseValue<TValue>(int startIndex, int length, Func<string, TValue> convert = null)
        {
            var valueString = ParseValue(startIndex, length);

            try
            {
                return (convert != null)
                    ? convert(valueString)
                    : (TValue)Convert.ChangeType(valueString, typeof(TValue));
            }
            catch (Exception ex)
            {
                throw new FormatException(
                    string.Format("The string '{0}' (startIndex={1}, length={2} could not be converted to type {3}",
                        valueString, startIndex, length, typeof(TValue).Name),
                    ex
                    );
            }
        }
    }
}