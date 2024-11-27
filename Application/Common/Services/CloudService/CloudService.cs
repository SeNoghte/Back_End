using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Services.CloudService
{
    public class CloudService : ICloudService
    {
        private readonly IConfiguration _configuration;
        private string accessKey;
        private string secretKey;
        private string bucketName;
        private string endPoint;

        public CloudService(IConfiguration configuration)
        {
            _configuration = configuration;
            accessKey = _configuration.GetSection("Liara:Accesskey").Value;
            secretKey = _configuration.GetSection("Liara:SecretKey").Value;
            bucketName = _configuration.GetSection("Liara:BucketName").Value;
            endPoint = _configuration.GetSection("Liara:EndPoint").Value;
        }

        public async Task<string> GetImagePath(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
                return null;

            ListObjectsV2Request r = new ListObjectsV2Request
            {
                BucketName = bucketName
            };

            var credentials = new Amazon.Runtime.BasicAWSCredentials(accessKey, secretKey);
            var config = new AmazonS3Config
            {
                ServiceURL = endPoint,
                ForcePathStyle = true
            };

            using var client = new AmazonS3Client(credentials, config);
            ListObjectsV2Response response = await client.ListObjectsV2Async(r);

            string res;
            var obj = response.S3Objects.Where(o => o.Key.Contains(identifier)).FirstOrDefault();

            if(obj != null)
            {
                GetPreSignedUrlRequest urlRequest = new GetPreSignedUrlRequest
                {
                    BucketName = bucketName,
                    Key = obj.Key,
                    Expires = DateTime.Now.AddHours(1)
                };

                res = client.GetPreSignedURL(urlRequest);
                return res;
            }
            else
            {
                return null;
            }
        }

        public async Task<string> SetImage(IFormFile image, string imageType)
        {
            try
            {
                var credentials = new Amazon.Runtime.BasicAWSCredentials(accessKey, secretKey);
                var config = new AmazonS3Config
                {
                    ServiceURL = endPoint,
                    ForcePathStyle = true
                };

                using var client = new AmazonS3Client(credentials, config);
                using var memoryStream = new MemoryStream();

                await image.CopyToAsync(memoryStream);
                using var fileTransferUtility = new TransferUtility(client);

                string newFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                string objectKey = $"{imageType}/{newFileName}";
                var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                {
                    BucketName = bucketName,
                    InputStream = memoryStream,
                    Key = objectKey
                };
                await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);
                string fileUrl = await GetImagePath(objectKey);
                return fileUrl;    
            }
            catch
            {
                return null;
            }
            
        }
    }
}
