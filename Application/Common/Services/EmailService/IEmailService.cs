using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Services.EmailService
{
    public interface IEmailService
    {
        public Task SendMail(string email, string subject, string body);
    }
}
