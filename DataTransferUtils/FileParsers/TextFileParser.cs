using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferUtils.FileParsers
{
    public abstract class TextFileParser<T> : FileParser<T> where T : class
    {
        public override T[] ParseFile(string filePath)
        {
            var values = new List<T>();

            var fullFilePath = Path.GetFullPath(filePath);

            using (var reader = new StreamReader(File.OpenRead(fullFilePath)))
            {
                var context = new FileLineContext(fullFilePath);

                while (!reader.EndOfStream)
                {
                    context.SetNextLine(reader.ReadLine());

                    try
                    {
                        var result = ParseLine(context);

                        if (!result.SkipLine)
                            values.Add(result.Value);
                    }
                    catch (Exception ex)
                    {
                        var pex = new FileLineParseException(context, ex.Message, ex);
                        WriteError(pex);
                        throw pex;
                    }
                }
            }

            return values.ToArray();
        }

        private void WriteError(FileLineParseException pex)
        {
            if (ErrorWriter == null)
                return;

            ErrorWriter.WriteLine(pex.Context.FileName);
            ErrorWriter.WriteLine(pex.Message);
        }

        protected abstract FileLineParseResult<T> ParseLine(FileLineContext context);

        protected FileLineParseResult<T> Success(T result)
        {
            return new FileLineParseResult<T>(result, false);
        }

        protected FileLineParseResult<T> Skip()
        {
            return new FileLineParseResult<T>(null, true);
        }

        protected FileLineParseResult<T> Fail(string errorFormat, params object[] args)
        {
            throw new Exception(string.Format(errorFormat, args));
        }
    }
}
