using Application.Common.Models;
using Application.Common.Services.EmailService;
using Application.Common.Services.GeneralServices;
using DataAccess;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users
{
    public class SendVerificationCodeCommand : IRequest<SendVerificationCodeResult>
    {
        public string Email { get; set; }
        public bool IsForPasswordRecovery { get; set; } = false;
    }

    public class SendVerificationCodeResult : ResultModel
    {
        public string VerificationCodeId { get; set; }
    }

    public class SendVerificationCodeCommandHandler : IRequestHandler<SendVerificationCodeCommand, SendVerificationCodeResult>
    {

        private readonly ApplicationDBContext _dbContext;
        private readonly IEmailService _emailService;
        private readonly IGeneralServices generalServices;

        public SendVerificationCodeCommandHandler(ApplicationDBContext dbContext, IEmailService emailService,
            IGeneralServices generalServices)
        {
            _dbContext = dbContext;
            _emailService = emailService;
            this.generalServices = generalServices;
        }
        public async Task<SendVerificationCodeResult> Handle(SendVerificationCodeCommand request, CancellationToken cancellationToken)
        {
            var result = new SendVerificationCodeResult();

            var isValidEmail = generalServices.CheckEmailFromat(request.Email);

            if (!isValidEmail)
            {
                result.Message = "فرمت ایمیل اشتباه است";
                return result;
            }
            
            var uExists = _dbContext.Users.Any(u => u.Email == request.Email);
            
            if (uExists)
            {
                if (!request.IsForPasswordRecovery)
                {
                    result.Message = "این ایمیل در سایت ثبت نام شده است";
                    return result;
                }
            }
            else
            {
                if(request.IsForPasswordRecovery)
                {
                    result.Message = "این ایمیل در سایت ثبت نام نشده است";
                    return result;
                }
            }
            

            Random rnd = new Random();
            string code = "12345";
            string subject = "کد تایید بچین";

            //await _emailService.SendMail(request.Email, code, subject);

            PendingVerification pv = new PendingVerification()
            {
                Id = Guid.NewGuid(),
                Code = code,
                Email = request.Email,
                Expiration = DateTime.UtcNow.AddMinutes(10),
                IsVerified = false
            };
            var pvExists = await _dbContext.PendingVerifications.FirstOrDefaultAsync(pv => pv.Email == request.Email);

            if (pvExists != null)
            {
                pvExists.Code = code;
                pvExists.Expiration = DateTime.UtcNow.AddMinutes(10);
                pvExists.IsVerified = false;

                _dbContext.Update(pvExists);

                pv.Id = pvExists.Id;
            }
            else
            {
                _dbContext.Add(pv);
            }

            await _dbContext.SaveChangesAsync();

            result.VerificationCodeId = pv.Id.ToString();
            result.Success = true;

            return result;
        }
    }
}
