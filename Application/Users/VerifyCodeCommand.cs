using Application.Common.Models;
using DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users
{
    public class VerifyCodeCommand : IRequest<VerifyCodeResult>
    {
        public string Code { get; set; }
        public string VerificationCodeId { get; set; }

    }

    public class VerifyCodeResult : ResultModel
    {
        public Guid VerificationCodeId { get; set; }
    }

    public class VerifyCodeHandler : IRequestHandler<VerifyCodeCommand, VerifyCodeResult>
    {
        private readonly ApplicationDBContext dBContext;

        public VerifyCodeHandler(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        public async Task<VerifyCodeResult> Handle(VerifyCodeCommand request, CancellationToken cancellationToken)
        {
            var result = new VerifyCodeResult();

            var pv = await dBContext.PendingVerifications
                .Where(x => x.Id.ToString() == request.VerificationCodeId && x.Code == request.Code)
                .FirstOrDefaultAsync();

            if (pv is null)
            {
                result.Message = "کد تایید اشتباه است";
                return result;
            }

            pv.IsVerified = true;
            await dBContext.SaveChangesAsync();

            result.VerificationCodeId = pv.Id;
            result.Success = true;

            return result;
        }
    }
}
