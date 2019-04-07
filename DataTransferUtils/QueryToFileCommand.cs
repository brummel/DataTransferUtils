using System;
using System.Data.SqlClient;
using System.IO;
using CommandLine;

namespace DataTransferUtils
{
    public class QueryToFileCommand : Command
    {
        [Option('q', "query", Required = true, HelpText = "The SQL query to execute against the default connection.")]
        public string Query { get; set; }

        [Option('f', "file", Required = true, HelpText = "The path of the destination file relative to the default root directory.")]
        public string RelativeFilePath { get; set; }

        [Option('s', "seperator", Required = false, DefaultValue = ",", HelpText = "The field seperator to be used in the destination file.")]
        public string Seperator { get; set; }

        [Option('x', "exclude-column-name-header", Required = false, HelpText = "Flag to exclude the column name header from the destination file.", DefaultValue = false)]
        public bool ExcludeColumnNameHeader { get; set; }

        [Option('r', "seperator-replacement", Required = false, DefaultValue = " ", HelpText = "The replacement used for field seperators occuring in the data.")]
        public string SeperatorReplacement { get; set; }

        public override void Execute(Configuration config)
        {
            if (Seperator == SeperatorReplacement)
                throw new Exception($"The seperator and its replacement cannot be the same ({Seperator}).");

            Console.WriteLine("Opening connection '{0}'...", config.DefaultConnectionString);
            using (var con = new SqlConnection(config.DefaultConnectionString))
            {
                con.Open();
                var command = new SqlCommand(Query, con);
                command.CommandTimeout = 900; // 900 seconds = 15 minutes

                Console.WriteLine("Executing query...");
                var reader = command.ExecuteReader();

                var absoluteFilePath = Path.Combine(config.DefaultRootDirectory, RelativeFilePath);

                Console.WriteLine("Writing results to destination file '{0}'...", absoluteFilePath);

                int columnCount = reader.FieldCount;

                using (var writer = new StreamWriter(File.Create(absoluteFilePath)))
                {
                    if (!ExcludeColumnNameHeader)
                    {
                        WriteDelimitedLine(writer, columnCount, reader.GetName, Seperator);
                    }

                    int dataRowCount = 0;

                    while (reader.Read())
                    {
                        WriteDelimitedLine(writer, columnCount, i => GetValue(reader, i, Seperator, SeperatorReplacement), Seperator);
                        dataRowCount++;
                    }

                    Console.WriteLine("Writing complete. {0:N0} data rows written.", dataRowCount);
                }
            }
        }

        private static string GetValue(SqlDataReader reader, int index, string seperator, string seperatorReplacement)
        {
            var value = reader[index].ToString();
            if (seperator == "")
                return value;
            var sanitisedValue = value.Replace(seperator, seperatorReplacement);
            return sanitisedValue;
        }

        private static void WriteDelimitedLine(StreamWriter writer, int columnCount, Func<int, string> valueFunct, string delimiter)
        {
            for (int i = 0; i < columnCount; i++)
            {
                writer.Write(valueFunct(i));

                if (i < columnCount - 1)
                {
                    writer.Write(delimiter);
                }
                else
                {
                    writer.WriteLine();
                }
            }
        }
    }
}