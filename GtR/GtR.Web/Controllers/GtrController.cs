using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace GtR.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GtrController : ControllerBase
    {
        [HttpPost("[action]")]
        public FileContentResult GenerateImages(
            [FromForm]float cardShortSideInInches,
            [FromForm]float cardLongSideInInches,
            [FromForm]float bleedSizeInInches,
            [FromForm]float borderPaddingInInches,
            [FromForm]SaveConfiguration saveConfiguration)
        {
            var config = new GtrConfig
            {
                BleedSizeInInches = bleedSizeInInches,
                BorderPaddingInInches = borderPaddingInInches,
                CardLongSideInInches = cardLongSideInInches,
                CardShortSideInInches = cardShortSideInInches,
                SaveConfiguration = saveConfiguration
            };
            var process = new ImageCreationProcess();
            var bytes = process.Run(config);
            var dateStamp = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss", CultureInfo.InvariantCulture);
            var fileName = $"Glory to Rome {dateStamp}.zip";
            return File(bytes, "application/zip", fileName);
        }
    }
}
