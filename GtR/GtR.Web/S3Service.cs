using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.IO;

namespace GtR.Web
{
    public class S3Service
    {
        private const string bucketName = "gtr-temp";
        public static string UploadZipToS3(byte[] bytes, string filenameWithoutExtension)
        {
            Console.WriteLine($"{DateTime.Now:G}: Starting to upload to S3");
            var client = new AmazonS3Client(
                Environment.GetEnvironmentVariable("S3_AWS_ACCESS_KEY_ID"),
                Environment.GetEnvironmentVariable("S3_AWS_SECRET_ACCESS_KEY"),
                RegionEndpoint.USEast2);
            var transferUtility = new TransferUtility(client);
            var filename = $"{filenameWithoutExtension} {Guid.NewGuid()}.zip";
            using (var memoryStream = new MemoryStream(bytes))
            {
                var request = new TransferUtilityUploadRequest
                {
                    BucketName = bucketName,
                    ContentType = "application/zip",
                    CannedACL = S3CannedACL.PublicRead,
                    InputStream = memoryStream,
                    Key = filename,
                    StorageClass = S3StorageClass.Standard
                };
                transferUtility.Upload(request);
            }
            Console.WriteLine($"{DateTime.Now:G}: Finished upload to S3");
            return $"https://{bucketName}.s3.us-east-2.amazonaws.com/{filename}";
        }
    }
}
