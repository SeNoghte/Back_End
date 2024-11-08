using Application.Common.Models;
using DataAccess;
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
        public byte[]? HashCode { get; set; }
    }

    public class VerficationHandler : IRequestHandler<VerificationCommand, VerificationResult>
    {

        private readonly ApplicationDBContext _dbContext;

        public Task<VerificationResult> Handle(VerificationCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
