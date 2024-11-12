﻿using DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
        string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$";   
        private readonly ApplicationDBContext dBContext;

        public GeneralServices(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }


        public bool CheckEmailFromat(string email)
        {
            return Regex.IsMatch(email, emailPattern, RegexOptions.IgnoreCase);
        }

        public bool CheckPasswordFormat(string password)
        {
            var regex = new Regex(passwordPattern);
            return regex.IsMatch(password);
        }

        public async Task<bool> CheckUserExists(Guid userId)
        {
            return await dBContext.Users.AnyAsync(u => u.UserId == userId);
        }
    }
}
