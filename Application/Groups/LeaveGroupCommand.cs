using Application.Common.Models;
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
    public class LeaveGroupCommand : IRequest<LeaveGroupResult>
    {
        public Guid GroupId { get; set; }
    }

    public class LeaveGroupResult : ResultModel
    {

    }

    public class HandleLeaveGroup : IRequestHandler<LeaveGroupCommand, LeaveGroupResult>
    {
        private readonly IIdentityService identityService;
        private readonly IGeneralServices generalServices;
        private readonly ApplicationDBContext dBContext;

        public HandleLeaveGroup(IIdentityService identityService, IGeneralServices generalServices
            , ApplicationDBContext dBContext)
        {
            this.identityService = identityService;
            this.generalServices = generalServices;
            this.dBContext = dBContext;
        }

        public async Task<LeaveGroupResult> Handle(LeaveGroupCommand request, CancellationToken cancellationToken)
        {
            var result = new LeaveGroupResult();

            try
            {
                var UserId = identityService.GetCurrentUserId();

                if (UserId == null)
                {
                    result.ErrorCode = 401;
                    result.Message = "Unauthorized";
                    return result;
                }

                var currentUser = await generalServices.GetUser((Guid)UserId);

                var gp = await generalServices.GetGroup(request.GroupId);

                if (gp == null)
                {
                    result.Message = "گروه یافت نشد";
                    result.ErrorCode = 404;
                    return result;
                }

                var member = await dBContext.UserGroups
                    .Where(ug => ug.UserId == currentUser.UserId && ug.GroupId == gp.Id)
                    .FirstOrDefaultAsync();

                if (member == null)
                {
                    result.ErrorCode = 401;
                    result.Message = "کاربر عضو گروه نیست";
                    return result;
                }

                dBContext.UserGroups.Remove(member);
                await dBContext.SaveChangesAsync();

                result.Success = true;
                result.Message = "عملیات با موفقیت انجام شد";
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
