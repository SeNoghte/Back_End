using Application.Common.Models;
using Application.Common.Services.CloudService;
using Application.Common.Services.GeneralServices;
using Application.Common.Services.IdentityService;
using DataAccess;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Events
{
    public class JoinEventCommand : IRequest<JoinEvenResult>
    {
        public Guid EventId { get; set; }
    }

    public class JoinEvenResult : ResultModel
    {

    }

    public class JoinEventCommadHandler : IRequestHandler<JoinEventCommand, JoinEvenResult>
    {
        private readonly IIdentityService identityService;
        private readonly IGeneralServices generalServices;
        private readonly ICloudService cloudService;
        private readonly ApplicationDBContext dBContext;

        public JoinEventCommadHandler(IIdentityService identityService, IGeneralServices generalServices
            , ICloudService cloudService, ApplicationDBContext dBContext)
        {
            this.identityService = identityService;
            this.generalServices = generalServices;
            this.cloudService = cloudService;
            this.dBContext = dBContext;
        }

        public async Task<JoinEvenResult> Handle(JoinEventCommand request, CancellationToken cancellationToken)
        {
            var result = new JoinEvenResult();

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

                if (currentUser == null)
                {
                    result.ErrorCode = 404;
                    result.Message = "کاربر پیدا نشد";
                    return result;
                }

                var targetEvent = await dBContext.Events.FirstOrDefaultAsync(e => e.Id == request.EventId);

                if (targetEvent == null)
                {
                    result.ErrorCode = 404;
                    result.Message = "برنامه مورد نظر پیدا نشد";
                    return result;
                }

                if (targetEvent.StartDate > DateTime.UtcNow)
                {
                    result.ErrorCode = 404;
                    result.Message = "این برنامه منقضی شده است";
                    return result;
                }

                var isJoinedToEvent = await dBContext.UserEvents.AnyAsync(ue => ue.EventId == request.EventId && ue.UserId == currentUser.UserId);
                if (isJoinedToEvent)
                {
                    result.ErrorCode = 404;
                    result.Message = "کاربر از قبل عضو برنامه است";
                    return result;
                }

                if (targetEvent.IsPrivate)
                {
                    var isJoinedToGroup = await dBContext.UserGroups.AnyAsync(ug => ug.GroupId == targetEvent.GroupId && ug.UserId == currentUser.UserId);
                    if (!isJoinedToGroup)
                    {
                        result.ErrorCode = 403;
                        result.Message = "برای عضویت در برنامه خصوصی باید عضو گروه هم باشید";
                        return result;
                    }
                }


                var userEvent = new UserEvent
                {
                    UserId = currentUser.UserId,
                    EventId = targetEvent.Id,
                    JoinedDate = DateTime.UtcNow,
                };

                await dBContext.UserEvents.AddAsync(userEvent);
                await dBContext.SaveChangesAsync();

                result.Success = true;
                result.Message = "عضویت با موفقیت انجام شد";
                return result;

            }
            catch (Exception ex)
            {
                result.ErrorCode = 500;
                result.Message = "مشکلی پیش آمده است";
                return result;
            }
        }
    }
}
