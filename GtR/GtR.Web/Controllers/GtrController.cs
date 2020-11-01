using Microsoft.AspNetCore.Mvc;

namespace GtR.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GtrController : ControllerBase
    {
        [HttpPost("[action]")]
        public string GenerateImages([FromBody]GtrConfig config)
        {
            var process = new ImageCreationProcess();
            var bytes = process.Run(config);
            var fileName = "Glory to Rome Images";
            return S3Service.UploadZipToS3(bytes, fileName);
        }
    }
}
