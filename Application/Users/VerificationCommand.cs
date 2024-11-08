using Application.Common.Models;
using Application.Common.Services.EmailService;
using Application.Common.Services.GeneralServices;
using DataAccess;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Users
{
    public class VerificationCommand : IRequest<VerificationResult>
    {
        public string Email { get; set; }

    }

    public class VerificationResult : ResultModel
    {
        public Guid VerificationCodeId { get; set; }
    }

    public class VerficationHandler : IRequestHandler<VerificationCommand, VerificationResult>
    {

        private readonly ApplicationDBContext _dbContext;
        private readonly IEmailService _emailService;
        private readonly IGeneralServices generalServices;

        public VerficationHandler(ApplicationDBContext dbContext,IEmailService emailService,
            IGeneralServices generalServices)
        {
            _dbContext = dbContext;
            _emailService = emailService;
            this.generalServices = generalServices;
        }
        public async Task<VerificationResult> Handle(VerificationCommand request, CancellationToken cancellationToken)
        {
            var result = new VerificationResult();
           
            var uExists = _dbContext.Users.Any(u => u.Email == request.Email);

            if (uExists)
            {
                result.Message = "این ایمیل در سایت ثبت نام شده است";
                return result;
            }
            
            var pvExists = _dbContext.PendingVerifications.Any(pv => pv.Email == request.Email);

            if(pvExists)
            {
                result.Message = "ایمیل از قبل برای شما ارسال شده است";
                return result;
            }
           
            var isValidEmail = generalServices.CheckEmailFromat(request.Email);

            if (!isValidEmail)
            {
                result.Message = "فرمت ایمیل اشتباه است";
                return result;
            }

            Random rnd = new Random();
            string code = rnd.Next(100000, 999999).ToString();
            string subject = "کد تایید بچین";

            //await _emailService.SendMail(request.Email, code, subject);

            PendingVerification pv = new PendingVerification()
            {
                Id = Guid.NewGuid(),
                Code = code,
                Email = request.Email,
                Expiration = DateTime.UtcNow.AddMinutes(10)
            };
            _dbContext.Add(pv);
            await _dbContext.SaveChangesAsync();

            result.VerificationCodeId = pv.Id;
            result.Success = true;

            return result;
        }
    }
}
