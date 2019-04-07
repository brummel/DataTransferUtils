using FX.Configuration;

namespace DataTransferUtils
{
    public class Configuration : AppConfiguration
    {
        public string DefaultConnectionString { get; set; }
        public string DefaultRootDirectory { get; set; }
        public string ExchangeServiceUrl { get; set; }
        public string ExchangeUsername { get; set; }
        public string ExchangePassword { get; set; }
    }
}