using Application.Common.Models;
using DataAccess;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users
{
    public class VerificationCommand : IRequest<VerificationResult>
    {
        public string? Email { get; set; }

    }

    public class VerificationResult : ResultModel
    {
        public Guid VerificationCodeId { get; set; }
    }

    public class VerficationHandler : IRequestHandler<VerificationCommand, VerificationResult>
    {

        private readonly ApplicationDBContext _dbContext;

        public VerficationHandler(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<VerificationResult> Handle(VerificationCommand request, CancellationToken cancellationToken)
        {
            var result = new VerificationResult();

            //... logic for sending email
            PendingVerification pv = new PendingVerification()
            {
                Id = Guid.NewGuid(),
                Code = "11211",
                Expiration = DateTime.UtcNow.AddMinutes(1)
            };
            _dbContext.Add(pv);
            await _dbContext.SaveChangesAsync();

            result.VerificationCodeId = pv.Id;
            result.Success = true;

            return result;
        }
    }
}
