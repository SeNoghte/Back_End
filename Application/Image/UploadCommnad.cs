using Application.Common.Models;
using Application.Common.Services.CloudService;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Image
{
    public class UploadCommnad : IRequest<UploadResult>
    {
        public IFormFile? Image { get; set; }
        public string Type { get; set; }
    }

    public class UploadResult : ResultModel
    {
        public string ImageId { get; set; }
    }

    public class UploadHandler : IRequestHandler<UploadCommnad, UploadResult>
    {
        private readonly ICloudService cloudService;

        public UploadHandler(ICloudService cloudService)
        {
            this.cloudService = cloudService;
        }

        public async Task<UploadResult> Handle(UploadCommnad request, CancellationToken cancellationToken)
        {
            var result = new UploadResult();

            if(request.Type.ToLower().Contains("group"))
            { 
                request.Type = "groups";
            }
            else if (request.Type.ToLower().Contains("user"))
            {
                request.Type = "users";
            }
            else
            {
                request.Type = "other";
            }

            var imageId = await cloudService.SetImage(request.Image, request.Type);

            if(imageId == null)
            {
                result.Message = "خطا در آپلود تصویر";
                return result;
            }

            result.ImageId = imageId;
            result.Success = true;
            return result;
        }
    }
}
