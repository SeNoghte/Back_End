using Application.Common.Models;
using Application.Common.Services.GeneralServices;
using Application.Common.Services.IdentityService;
using DataAccess;
using MediatR;

namespace Application.Groups
{
    public class GroupDeleteCommand : IRequest<GroupDeleteResult>
    {
        public string GroupId { get; set; }
    }

    public class GroupDeleteResult : ResultModel
    {

    }

    public class GroupDeleteHandler : IRequestHandler<GroupDeleteCommand, GroupDeleteResult>
    {
        private readonly ApplicationDBContext dBContext;
        private readonly IGeneralServices generalServices;
        private readonly IIdentityService identityService;

        public GroupDeleteHandler(ApplicationDBContext dBContext, IGeneralServices generalServices
            ,IIdentityService identityService)
        {
            this.dBContext = dBContext;
            this.generalServices = generalServices;
            this.identityService = identityService;
        }

        public async Task<GroupDeleteResult> Handle(GroupDeleteCommand request, CancellationToken cancellationToken)
        {
            var result = new GroupDeleteResult();

            var UserId = identityService.GetCurrentUserId();

            if (UserId == null)
            {
                result.ErrorCode = 401;
                result.Message = "Unauthorized";
                return result;
            }

            var gp = await generalServices.GetGroup(Guid.Parse(request.GroupId));

            if (gp == null)
            {
                result.Message = "گروه پیدا نشد";
                return result;
            }

            if(gp.OwnerId != UserId)
            {
                result.Message = "فقط سازنده اکیپ اجازه حذف دارد";
                return result;
            }

            dBContext.Remove(gp);
            await dBContext.SaveChangesAsync();

            result.Success = true;
            return result;
        }
    }
}
