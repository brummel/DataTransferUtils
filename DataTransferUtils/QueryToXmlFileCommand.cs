using System;
using System.Data.SqlClient;
using System.IO;
using CommandLine;
using System.Xml;
using System.Collections.Generic;

namespace DataTransferUtils
{
    public class QueryToXmlFileCommand : Command
    {
        [Option('q', "query", Required = true, HelpText = "The SQL query to execute against the default connection.")]
        public string Query { get; set; }

        [Option('f', "file", Required = true, HelpText = "The path of the destination file relative to the default root directory.")]
        public string RelativeFilePath { get; set; }

        [Option('r', "root", Required = true, HelpText = "The name of the root element in the XML file.")]
        public string RootElementName { get; set; }

        [Option('e', "element", Required = true, HelpText = "The element name for rows returned in the XML file.")]
        public string ElementName { get; set; }

        public override void Execute(Configuration config)
        {
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

                var columnNames = new Dictionary<int, string>();
                for (int i = 0; i < columnCount; i++)
                    columnNames[i] = reader.GetName(i);

                var settings = new XmlWriterSettings();
                settings.Indent = true;

                using (var writer = System.Xml.XmlWriter.Create(File.Create(absoluteFilePath), settings))
                {
                    int dataRowCount = 0;

                    writer.WriteStartDocument();
                    writer.WriteStartElement(RootElementName);

                    while (reader.Read())
                    {
                        writer.WriteStartElement(ElementName);

                        for (int i = 0; i < columnCount; i++)
                        {
                            writer.WriteElementString(columnNames[i], reader.GetValue(i).ToString());
                        }

                        writer.WriteEndElement();

                        dataRowCount++;
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();

                    Console.WriteLine("Writing complete. {0:N0} data rows written.", dataRowCount);
                }
            }
        }

        public class Element
        {
            public string Name { get; }
            public List<Element> Children { get; set; }

            public Element(string name)
            {
                this.Name = name;
                this.Children = new List<Element>();
            }

            public Element FindChild(string name) // Note: Not recursive
            {
                foreach (var child in this.Children)
                {
                    if (child.Name == name)
                        return child;
                }

                return null;
            }
        }

        public class ValueElement : Element
        {
            public int FieldIndex { get; }

            public ValueElement(string name, int fieldIndex)
                : base(name)
            {
                this.FieldIndex = fieldIndex;
            }
        }

        public class ElementTreeFactory
        {
            public Element CreateElementTree(string rowElementName, string[] columnNames)
            {
                var tree = new Element(rowElementName);

                for (int i = 0; i < columnNames.Length; i++)
                {
                    var elementNames = columnNames[i].Split('|');

                    var currentElement = tree;

                    for (int j = 0; j < elementNames.Length; j++)
                    {
                        var elementName = elementNames[j];
                        var isLastElementName = j + 1 == elementNames.Length;

                        if (isLastElementName)
                        {
                            currentElement.Children.Add(new ValueElement(elementName, i));
                        }
                        else
                        {
                            var child = currentElement.FindChild(elementName);
                            if (child == null)
                                child = new Element(elementName);

                        }
                    }
                }

                return tree;
            }
        }
    }
}