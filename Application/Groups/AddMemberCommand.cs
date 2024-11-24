using Application.Common.Models;
using Application.Common.Services.GeneralServices;
using Application.Common.Services.IdentityService;
using DataAccess;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Groups
{
    public class AddMemberCommand : IRequest<AddMemberResult>
    {
        public string GroupId { get; set; }
        public List<string> UsersToAdd { get; set; }
    }

    public class AddMemberResult : ResultModel
    {

    }

    public class AddMemberHandler : IRequestHandler<AddMemberCommand, AddMemberResult>
    {
        private readonly ApplicationDBContext dBContext;
        private readonly IGeneralServices generalServices;
        private readonly IIdentityService identityService;

        public AddMemberHandler(ApplicationDBContext dBContext, IGeneralServices generalServices, IIdentityService identityService)
        {
            this.dBContext = dBContext;
            this.generalServices = generalServices;
            this.identityService = identityService;
        }

        public async Task<AddMemberResult> Handle(AddMemberCommand request, CancellationToken cancellationToken)
        {
            var result = new AddMemberResult();

            try
            {
                var UserId = identityService.GetCurrentUserId();

                if (UserId == null)
                {
                    result.ErrorCode = 401;
                    result.Message = "Unauthorized";
                    return result;
                }

                var CurrentUser = await generalServices.GetUser((Guid)UserId);

                var gp = await generalServices.GetGroup(Guid.Parse(request.GroupId));

                if (gp == null)
                {
                    result.Message = "گروه یافت نشد";
                    result.ErrorCode = 404;
                    return result;
                }

                if (gp.OwnerId != CurrentUser.UserId)
                {
                    result.Message = "فقط مدیر گروه میتواند عضو کند";
                    result.ErrorCode = 403;
                    return result;
                }

                foreach (var user in request.UsersToAdd)
                {
                    var userExist = await generalServices.CheckUserExists(Guid.Parse(user));

                    if (userExist)
                    {
                        var member = await dBContext.UserGroups.Where(ug => ug.UserId == Guid.Parse(user) && ug.GroupId == Guid.Parse(request.GroupId))
                            .FirstOrDefaultAsync();

                        if (member == null)
                        {
                            var membership = new UserGroup
                            {
                                GroupId = Guid.Parse(request.GroupId),
                                UserId = Guid.Parse(user),
                                JoinedDate = DateTime.UtcNow
                            };
                            await dBContext.UserGroups.AddAsync(membership);
                        }
                    }
                }

                await dBContext.SaveChangesAsync();

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
