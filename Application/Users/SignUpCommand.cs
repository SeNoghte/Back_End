﻿using Application.Common.Models;
using Application.Common.Services.GeneralServices;
using Application.Common.Services.IdentityService;
using DataAccess;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users;

public class SignUpCommand : IRequest<SignUpResult>
{
    public string Name { get; set; }
    public string? Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string VerificationCodeId { get; set; }
}

public class SignUpResult : ResultModel
{

}

public class SignUpHandler : IRequestHandler<SignUpCommand, SignUpResult>
{
    private readonly IGeneralServices _generalServices;

    public ApplicationDBContext applicationDB { get; set; }
    IIdentityService identityService { get; set; }
    public SignUpHandler(ApplicationDBContext applicationDB, IIdentityService identityService,
        IGeneralServices generalServices)
    {
        this.applicationDB = applicationDB;
        this.identityService = identityService;
        _generalServices = generalServices;
    }
    public async Task<SignUpResult> Handle(SignUpCommand request, CancellationToken cancellationToken)
    {
        var result = new SignUpResult();

        var isVerified = await applicationDB.PendingVerifications
            .AnyAsync(x => x.Id.ToString() == request.VerificationCodeId && x.IsVerified);

        if(!isVerified)
        {
            result.Message = "ایمیل تایید نشده است";
            return result;
        }

        
        var isValidEmail = _generalServices.CheckEmailFromat(request.Email);

        if (!isValidEmail)
        {
            result.Message = "فرمت ایمیل اشتباه است";
            return result;
        }

        var isValidPass = _generalServices.CheckPasswordFormat(request.Password);

        if (!isValidPass)
        {
            result.Message = "رمز عبور باید شامل حداقل 8 حرف یک عدد، یک عدد و حروف بزرگ و کوچک انگبیسی باشد";
            return result;
        }

        if (!string.IsNullOrEmpty(request.Username))
        {
            if (request.Username.Length < 3)
            {
                result.Message = "نام کاربری حداقل شامل 2 حرف باشد";
                return result;
            }

            var userExists = await applicationDB.Users.AnyAsync(u => u.Username == request.Username);

            if (userExists)
            {
                result.Message = "نام کاربری در سایت ثبت نام شده است";
                return result;
            }
        }

        if (request.Name.Length < 3)
        {
            result.Message = "نام کاربری حداقل شامل 2 حرف باشد";
            return result;
        }

        var emailExit = await applicationDB.Users.AnyAsync(u => u.Email == request.Email);

        if (emailExit)
        {
            result.Message = "این ایمیل در سایت ثبت نام شده است";
            return result;
        }

        var (passwordHash, passwordSalt) = identityService.CreatePasswordHash(request.Password);

        var user = new User
        {
            Name = request.Name,
            Username = request.Username,
            PasswordHash = Convert.ToBase64String(passwordHash),
            PasswordSalt = Convert.ToBase64String(passwordSalt),
            Email = request.Email,
            JoinedDate = DateTime.Now,       
        };

        applicationDB.Users.Add(user);
        await applicationDB.SaveChangesAsync();

        result.Success = true;
        return result;
    }
}
