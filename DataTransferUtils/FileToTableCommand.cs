using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using DataTransferUtils.FileParsers;

namespace DataTransferUtils
{
    public class FileToTableCommand : Command
    {
        private OutputWriter _errorWriter;

        [OptionList('f', "file-paths", ';', Required = true, HelpText = "A semi-colon (;) delimited list of source file paths.")]
        public IList<string> FilePaths { get; set; }

        [Option('p', "parser-key", Required = true, HelpText = "The key of the parser to use to parse the files.")]
        public string ParserKey { get; set; }

        [Option('c', "target-connection-string", Required = true, HelpText = "The connection string for the target database.")]
        public string TargetConnectionString { get; set; }

        [Option('t', "target-table-name", Required = true, HelpText = "The target table name in the target database.")]
        public string TargetTableName { get; set; }

        public FileToTableCommand(OutputWriter errorWriter)
        {
            _errorWriter = errorWriter;
        }

        public FileToTableCommand()
            : this(new ConsoleOutputWriter())
        {

        }

        public override void Execute(Configuration config)
        {
            var parser = FileParserFactory.Instance.Get(ParserKey);
            parser.ErrorWriter = _errorWriter;

            parser.InitForBatch();

            using (var con = new SqlConnection(TargetConnectionString))
            {
                con.Open();
                SqlBulkCopy bcp = new SqlBulkCopy(con);
                bcp.BulkCopyTimeout = 900;
                bcp.DestinationTableName = TargetTableName;

                foreach (var filePath in FilePaths)
                {
                    parser.InitForFile();
                    var table = parser.ParseFileAsDT(filePath);
                    bcp.WriteToServer(table);
                }
            }
        }
    }
}
