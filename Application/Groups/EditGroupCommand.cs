using Application.Common.Models;
using Application.Common.Services.CloudService;
using Application.Common.Services.GeneralServices;
using Application.Common.Services.IdentityService;
using DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Groups
{
    public class EditGroupCommand : IRequest<EditGroupResult>
    {
        public Guid GroupId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Image {  get; set; }
    }
    
    public class EditGroupResult : ResultModel
    {

    }

    public class EditGroupHandler : IRequestHandler<EditGroupCommand, EditGroupResult>
    {
        private readonly ApplicationDBContext dBContext;
        private readonly IGeneralServices generalServices;
        private readonly IIdentityService identityService;
        private readonly ICloudService cloudService;

        public EditGroupHandler(ApplicationDBContext dBContext, IGeneralServices generalServices,
            IIdentityService identityService, ICloudService cloudService)
        {
            this.dBContext = dBContext;
            this.generalServices = generalServices;
            this.identityService = identityService;
            this.cloudService = cloudService;
        }

        public async Task<EditGroupResult> Handle(EditGroupCommand request, CancellationToken cancellationToken)
        {
            var result = new EditGroupResult();

            try
            {
                var userId = identityService.GetCurrentUserId();

                if (userId == null)
                {
                    result.ErrorCode = 401;
                    result.Message = "Unauthorized";
                    return result;
                }

                var gp = await dBContext.Groups.FirstOrDefaultAsync(g => g.Id == request.GroupId);

                if (gp == null)
                {
                    result.ErrorCode = 404;
                    result.Message = "گروه پیدا نشد";
                    return result;
                }

                if(gp.OwnerId != userId)
                {
                    result.ErrorCode = 401;
                    result.Message = "تنها سازنده گروه اجازه ویرایش دارد";
                    return result;
                }

                if(string.IsNullOrEmpty(request.Name))
                {
                    result.ErrorCode = 401;
                    result.Message = "نام گروه نمیتواند خالی  نشد";
                    return result;
                }

                if (request.Name.Length < 3)
                {
                    result.Message = "نام گروه حداقل شامل 2 حرف باشد";
                    return result;
                }

                var groupExist = await dBContext.Groups.AnyAsync(g => g.Name == request.Name && g.Id != gp.Id);

                if (groupExist)
                {
                    result.Message = " نام گروه در سایت ثبت شده است";
                    return result;
                }

                gp.Name = request.Name;

                if (request.Description != null)
                {
                    gp.Description = request.Description.Trim() == string.Empty ? null : request.Description;
                }

                if (request.Image != null)
                {
                    gp.Image = request.Image.Trim() == string.Empty ? null : request.Image;
                }

                await dBContext.SaveChangesAsync();

                result.Success = true;
                result.Message = "تغییرات با موفقیت ثبت شد";

                return result;

            }
            catch (Exception ex)
            {
                result.Message = "مشکلی پیش آمده است";
                result.ErrorCode = 500;
                return result;
            }
        }
    }
}
