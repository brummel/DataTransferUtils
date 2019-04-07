using System;
using System.Collections.Generic;
using System.Reflection;

namespace DataTransferUtils.FileParsers
{
    public class FileParserFactory
    {
        private Dictionary<string, Type> _availableParsers;

        private static FileParserFactory _instance;

        public static FileParserFactory Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new FileParserFactory();

                return _instance;
            }
        }

        private FileParserFactory()
        {
            _availableParsers = new Dictionary<string, Type>();
        }

        public void Register<TFileParser>(string key) where TFileParser : FileParser
        {
            _availableParsers.Add(key, typeof(TFileParser));
        }

        public FileParser Get(string key)
        {
            if (!_availableParsers.ContainsKey((key)))
                throw new KeyNotFoundException(string.Format(
                    "A file parser with the key '{0}' has not been registered", key));

            return (FileParser)Activator.CreateInstance(_availableParsers[key]);
        }
    }
}