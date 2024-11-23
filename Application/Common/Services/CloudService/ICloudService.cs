using Amazon.S3;

namespace Application.Common.Services.CloudService
{
    public interface ICloudService
    {
        Task<string> GetImagePath(string identifier);
        Task<bool> SetImage(string identifier);
    }
}
