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
    public class JoinGroupCommand : IRequest<JoinGroupResult>
    {
        public Guid GroupId { get; set; }
    }

    public class JoinGroupResult : ResultModel
    {

    }

    public class JoinGroupHandler : IRequestHandler<JoinGroupCommand, JoinGroupResult>
    {
        private readonly IIdentityService identityService;
        private readonly IGeneralServices generalServices;
        private readonly ApplicationDBContext dBContext;

        public JoinGroupHandler(IIdentityService identityService, IGeneralServices generalServices
            , ApplicationDBContext dBContext)
        {
            this.identityService = identityService;
            this.generalServices = generalServices;
            this.dBContext = dBContext;
        }
        public async Task<JoinGroupResult> Handle(JoinGroupCommand request, CancellationToken cancellationToken)
        {
            var result = new JoinGroupResult();

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
                           
                if(member != null)
                {
                    result.ErrorCode = 401;
                    result.Message = "کاربر از قبل عضو گروه است";
                    return result;
                }

                var userGroup = new UserGroup
                {
                    UserId = currentUser.UserId,
                    GroupId = gp.Id,
                    JoinedDate = DateTime.UtcNow,
                };

                await dBContext.UserGroups.AddAsync(userGroup);
                await dBContext.SaveChangesAsync();

                result.Success = true;
                result.Message = "عضویت با موفقیت انجام شد";
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
