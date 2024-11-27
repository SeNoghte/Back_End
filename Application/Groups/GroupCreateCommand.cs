using Application.Common.Models;
using Application.Common.Services.CloudService;
using Application.Common.Services.GeneralServices;
using Application.Common.Services.IdentityService;
using DataAccess;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Groups
{

    public class GroupCreateCommand : IRequest<GroupCreateResult>
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? ImageId { get; set; }
        public List<Guid>? MembersToAdd { get; set; }
    }

    public class GroupCreateResult : ResultModel
    {
        public string GroupId { get; set; }
    }

    public class GroupCreateHandler : IRequestHandler<GroupCreateCommand, GroupCreateResult>
    {
        private readonly ApplicationDBContext dBContext;
        private readonly ICloudService cloudService;
        private readonly IGeneralServices generalServices;

        IIdentityService identityService {  get; set; }

        public GroupCreateHandler(ApplicationDBContext dBContext, IIdentityService identityService,
            ICloudService cloudService, IGeneralServices generalServices)
        {
            this.dBContext = dBContext;
            this.identityService = identityService;
            this.cloudService = cloudService;
            this.generalServices = generalServices;
        }

        public async Task<GroupCreateResult> Handle(GroupCreateCommand request, CancellationToken cancellationToken)
        {

            var result = new GroupCreateResult();
            try
            {
                var UserId = identityService.GetCurrentUserId();

                if (UserId == null)
                {
                    result.ErrorCode = 401;
                    result.Message = "Unauthorized";
                    return result;
                }

                var ownerExist = await generalServices.CheckUserExists((Guid)UserId);

                if (!ownerExist)
                {
                    result.Message = "کاربر پیدا نشد";
                    return result;
                }

                if (request.Name.Length < 3)
                {
                    result.Message = "نام گروه حداقل شامل 2 حرف باشد";
                    return result;
                }

                var groupExist = await dBContext.Groups.AnyAsync(gp => gp.Name == request.Name);

                if (groupExist)
                {
                    result.Message = " نام گروه در سایت ثبت شده است";
                    return result;
                }


                var gp = new Group
                {
                    Name = request.Name,
                    Description = request.Description,
                    CreatedDate = DateTime.UtcNow,
                    OwnerId = (Guid)UserId,
                    Image = request.ImageId,
                };

                await dBContext.AddAsync(gp);
                await dBContext.SaveChangesAsync();

                gp = await dBContext.Groups.FirstOrDefaultAsync(gp => gp.Name == request.Name);

                //owner
                var userGroup = new UserGroup
                {
                    UserId = (Guid)UserId,
                    GroupId = gp.Id,
                    JoinedDate = DateTime.UtcNow
                };

                await dBContext.UserGroups.AddAsync(userGroup);

                foreach (var userId in request.MembersToAdd.Distinct())
                {
                    var usrExist = await generalServices.CheckUserExists(userId);
                    if (usrExist)
                    {
                        var memberGroup = new UserGroup
                        {
                            UserId = userId,
                            GroupId = gp.Id,
                            JoinedDate = DateTime.UtcNow
                        };
                        await dBContext.UserGroups.AddAsync(memberGroup);
                    }
                }

                await dBContext.SaveChangesAsync();

                result.GroupId = gp.Id.ToString();
                result.Success = true;
                return result;
            }
            catch
            {
                result.Message = "مشکلی پیش آمده است";
                result.ErrorCode = 500;
                return result;
            }
        }
    }
}
