using Application.Common.Models;
using Application.Common.Services.IdentityService;
using DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Events
{
    public class RemoveTaskFromUserCommand : IRequest<RemoveTaskFromUserResult>
    {
        public Guid TaskId { get; set; }
    }

    public class RemoveTaskFromUserResult : ResultModel
    {

    }

    public class RemoveTaskFromUserHandler : IRequestHandler<RemoveTaskFromUserCommand, RemoveTaskFromUserResult>
    {
        private readonly ApplicationDBContext dBContext;
        private readonly IIdentityService identityService;

        public RemoveTaskFromUserHandler(ApplicationDBContext dBContext, IIdentityService identityService)
        {
            this.dBContext = dBContext;
            this.identityService = identityService;
        }

        public async Task<RemoveTaskFromUserResult> Handle(RemoveTaskFromUserCommand request, CancellationToken cancellationToken)
        {
            var result = new RemoveTaskFromUserResult();

            try
            {
                var currentUserId = identityService.GetCurrentUserId();

                if (currentUserId == null)
                {
                    result.ErrorCode = 401;
                    result.Message = "Unauthorized";
                    return result;
                }

                var task = await dBContext.EventTasks.Where(et => et.Id == request.TaskId).FirstOrDefaultAsync();

                if (task == null)
                {
                    result.ErrorCode = 404;
                    result.Message = "تسک پیدا نشد";
                    return result;
                }

                if (task.AssignedUserId != null && task.AssignedUserId != currentUserId)
                {
                    result.ErrorCode = 401;
                    result.Message = "تسک به این کاربر تعلق ندارد";
                    return result;
                }

                task.AssignedUserId = null;

                await dBContext.SaveChangesAsync();

                result.Success = true;
                result.Message = "عملیات با موفقیت انجام شد";
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
