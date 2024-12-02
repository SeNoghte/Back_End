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
    public class AssignTaskToUserCommand : IRequest<AssignTaskToUserResult>
    {
        public Guid TaskId { get; set; }
    }

    public class AssignTaskToUserResult : ResultModel
    {

    }

    public class AssignEventToUserHandler : IRequestHandler<AssignTaskToUserCommand, AssignTaskToUserResult>
    {
        private readonly ApplicationDBContext dBContext;
        private readonly IIdentityService identityService;

        public AssignEventToUserHandler(ApplicationDBContext dBContext, IIdentityService identityService)
        {
            this.dBContext = dBContext;
            this.identityService = identityService;
        }

        public async Task<AssignTaskToUserResult> Handle(AssignTaskToUserCommand request, CancellationToken cancellationToken)
        {
            var result = new AssignTaskToUserResult();

            try
            {
                var UserId = identityService.GetCurrentUserId();

                if (UserId == null)
                {
                    result.ErrorCode = 401;
                    result.Message = "Unauthorized";
                    return result;
                }

                var task = await dBContext.EventTasks.Where(et => et.Id == request.TaskId).FirstOrDefaultAsync();

                if(task == null)
                {
                    result.ErrorCode = 404;
                    result.Message = "تسک پیدا نشد";
                    return result;
                }

                if(task.AssignedUserId != null)
                {
                    result.ErrorCode = 401;
                    result.Message = "تسک به کاربر دیگری داده شده است";
                    return result;
                }

                task.AssignedUserId = UserId;

                await dBContext.SaveChangesAsync();

                result.Success = true;
                result.Message = "عملیات با موفقیت انجام شد";
                return result;
            }              
            catch 
            {
                result.ErrorCode = 500;
                result.Message = "مشکلی پیش آمده است";
                return result;
            }
        }
    }
}
