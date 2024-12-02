using Application.Common.Models;
using Application.Common.Services.GeneralServices;
using Application.Common.Services.IdentityService;
using DataAccess;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users
{
    public class ForgetPasswordCommand : IRequest<ForgetPasswordResult>
    {
        public string NewPassword { get; set; }
        public string NewPasswordAgain { get; set; }
        public string VerificationCodeId { get; set; }
    }

    public class ForgetPasswordResult : ResultModel
    {

    }

    public class ForgetPasswordHandler : IRequestHandler<ForgetPasswordCommand, ForgetPasswordResult>
    {
        private readonly IIdentityService identityService;
        private readonly ApplicationDBContext dBContext;
        private readonly IGeneralServices generalServices;

        public ForgetPasswordHandler(IIdentityService identityService,ApplicationDBContext dBContext,
            IGeneralServices generalServices)
        {
            this.identityService = identityService;
            this.dBContext = dBContext;
            this.generalServices = generalServices;
        }

        public async Task<ForgetPasswordResult> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
        {
            var result = new ForgetPasswordResult();

            try
            {
                var userId = identityService.GetCurrentUserId();

                if (userId == null)
                {
                    result.ErrorCode = 401;
                    result.Message = "Unauthorized";
                    return result;
                }

                if(request.NewPassword != request.NewPasswordAgain)
                {
                    result.ErrorCode = 401;
                    result.Message = "عدم تطابق رمز ها";
                    return result;
                }

                var isValidPass = generalServices.CheckPasswordFormat(request.NewPassword);

                if (!isValidPass)
                {
                    result.Message = "رمز عبور باید شامل حداقل 8 حرف باشد";
                    return result;
                }

                var currentUser = await generalServices.GetUser((Guid)userId);

                var (passwordHash, passwordSalt) = identityService.CreatePasswordHash(request.NewPassword);

                currentUser.PasswordHash = Convert.ToBase64String(passwordHash);
                currentUser.PasswordSalt = Convert.ToBase64String(passwordSalt);

                await dBContext.SaveChangesAsync();

                result.Success = true;

                return result;
            }
            catch 
            {
                result.Message = "مشکلی پیش آمده است";
                return result;
            }
        }
    }
}
