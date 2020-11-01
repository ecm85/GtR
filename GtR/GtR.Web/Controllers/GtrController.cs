using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GtR.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GtrController : ControllerBase
    {
        private static GtrConfig LoadGtrConfig()
        {
            var configurationFile = "GtRConfig.json";
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
            if (!System.IO.File.Exists(configurationFile))
                throw new InvalidOperationException($"Configuration file does not exist, looked for {configurationFile}.");
            var text = System.IO.File.ReadAllText(configurationFile);
            try
            {
                return JsonConvert.DeserializeObject<GtrConfig>(text);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost("[action]")]
        public FileContentResult GenerateImages()
        {
            var config = LoadGtrConfig();
            var process = new ImageCreationProcess();
            var bytes = process.Run(config);
            var dateStamp = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss", CultureInfo.InvariantCulture);
            var fileName = $"Glory to Rome {dateStamp}.zip";
            return File(bytes, "application/zip", fileName);
        }
    }
}
