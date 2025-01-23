using Application.Common.Models;
using Application.Common.Services.IdentityService;
using DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Events
{
    public class LeaveEventCommand : IRequest<LeaveEventResult>
    {
        public Guid EventId { get; set; }
    }

    public class LeaveEventResult : ResultModel
    {

    }

    public class LeaveEventHandler : IRequestHandler<LeaveEventCommand, LeaveEventResult>
    {
        private readonly ApplicationDBContext dBContext;
        private readonly IIdentityService identityService;

        public LeaveEventHandler(ApplicationDBContext dBContext, IIdentityService identityService)
        {
            this.dBContext = dBContext;
            this.identityService = identityService;
        }
        public  async Task<LeaveEventResult> Handle(LeaveEventCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = identityService.GetCurrentUserId();

            var result = new LeaveEventResult();

            var evnt = dBContext.Events.Where(e => e.Id == request.EventId).FirstOrDefault();

            if (evnt == null) {
                result.Message = "پیدا نشد";
                result.ErrorCode = 404;
                return result;
            }

            var member = await dBContext.UserEvents
                    .Where(ue => ue.UserId == currentUserId && ue.EventId == evnt.Id).FirstOrDefaultAsync();
                    

            if (member == null)
            {
                result.ErrorCode = 401;
                result.Message = "کاربر عضو ایونت نیست";
                return result;
            }

            dBContext.UserEvents.Remove(member);
            await dBContext.SaveChangesAsync();

            result.Success = true;
            result.Message = "عملیات با موفقیت انجام شد";
            return result;




        }
    }
}
