using System;
using System.Reflection;
using CommandLine;
using CommandLine.Text;

namespace DataTransferUtils
{
    public class VerbOptions
    {
        [VerbOption("QueryToFile", HelpText = "Write the result of a SQL query to a file")]
        public QueryToFileCommand QueryToFile { get; set; }

        [VerbOption("QueryToXmlFile", HelpText = "Write the result of a SQL query to an XML file")]
        public QueryToXmlFileCommand QueryToXmlFile { get; set; }

        [VerbOption("FileToTable", HelpText = "Parse a list of files and insert them into a SQL table")]
        public FileToTableCommand FileToTable { get; set; }

        [VerbOption("ExchangeEmailAttachmentsToDirectory", HelpText = "Save Exchange email attachments to a directory on the file system. If a matching attachment is found, the email is moved to the 'Collected' folder.")]
        public ExchangeEmailAttachmentsToDirectoryCommand ExchangeEmailAttachmentsToDirectory { get; set; }

        [HelpVerbOption]
        public string GetUsage(string verb)
        {
            if (verb == null)
                return HelpText.AutoBuild(this, null);

            foreach (var prop in GetType().GetProperties())
            {
                var vo = prop.GetCustomAttribute<VerbOptionAttribute>();
                if (vo.LongName == verb)
                    return HelpText.AutoBuild(prop.GetValue(this));
            }

            throw new ArgumentException("Verb could not be found", verb);
        }
    }
}