using MediatR;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Common.Services.GeneralServices
{
    public class GeneralServices : IGeneralServices
    {
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        public bool CheckEmailFromat(string email)
        {
            return Regex.IsMatch(email, emailPattern, RegexOptions.IgnoreCase);
        }
    }
}
