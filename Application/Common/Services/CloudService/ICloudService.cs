using Amazon.S3;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Services.CloudService
{
    public interface ICloudService
    {
        Task<string> GetImagePath(string identifier);
        Task<string> SetImage(IFormFile image, string ImageType);
    }
}
