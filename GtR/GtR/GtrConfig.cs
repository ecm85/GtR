using Newtonsoft.Json;
using System;
using System.IO;

namespace GtR
{
    public class GtrConfig
    {
        public float CardShortSideInInches { get; set; }
        public float CardLongSideInInches { get; set; }
        public float BleedSizeInInches { get; set; }
        public float BorderPaddingInInches { get; set; }
        public SaveConfiguration SaveConfiguration { get; set; }

        public static readonly GtrConfig Current = LoadGtrConfig();

        private static GtrConfig LoadGtrConfig()
        {
            var configurationFile = "GtrConfig.json";
            var config = LoadConfigFromFile(configurationFile);
            if (config == null)
                throw new InvalidOperationException("Invalid configuration file!");
            if (config.CardShortSideInInches == default)
                throw new InvalidOperationException("Invalid CardShortSideInInches in configuration file!");
            if (config.CardLongSideInInches == default)
                throw new InvalidOperationException("Invalid CardLongSideInInches in configuration file!");
            if (config.BleedSizeInInches == default)
                throw new InvalidOperationException("Invalid BleedSizeInInches in configuration file!");
            if (config.BorderPaddingInInches == default)
                throw new InvalidOperationException("Invalid BorderPaddingInInches in configuration file!");
            if (config.SaveConfiguration == default)
                throw new InvalidOperationException("Invalid SaveConfiguration in configuration file!");
            return config;
        }

        private static GtrConfig LoadConfigFromFile(string configurationFile)
        {
            if (!File.Exists(configurationFile))
                throw new InvalidOperationException($"Configuration file does not exist, looked for {configurationFile}.");
            var text = File.ReadAllText(configurationFile);
            Console.WriteLine("Loaded configuration text: ");
            Console.WriteLine(text);
            try
            {
                return JsonConvert.DeserializeObject<GtrConfig>(text);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
