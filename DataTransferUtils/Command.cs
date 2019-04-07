using CommandLine;
using CommandLine.Text;

namespace DataTransferUtils
{
    public abstract class Command
    {
        [ParserState]
        public IParserState ParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this);
        }

        public abstract void Execute(Configuration config);

        public void Execute()
        {
            Execute(new Configuration());
        }
    }
}