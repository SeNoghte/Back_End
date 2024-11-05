using Application.Common.Models;
using Application.Common.Services.IdentityService;
using DataAccess;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Application.Users;

public class SignUpCommand : IRequest<SignUpResult>
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
}

public class SignUpResult : ResultModel
{

}

public class SignUpHandler : IRequestHandler<SignUpCommand, SignUpResult>
{
    public ApplicationDBContext applicationDB { get; set; }
    IIdentityService identityService { get; set; }
    public SignUpHandler(ApplicationDBContext applicationDB, IIdentityService identityService)
    {
        this.applicationDB = applicationDB;
        this.identityService = identityService;
    }
    public async Task<SignUpResult> Handle(SignUpCommand request, CancellationToken cancellationToken)
    {
        var result = new SignUpResult();

        var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        var isValidEmail = Regex.IsMatch(request.Email, emailPattern, RegexOptions.IgnoreCase);

        if (!isValidEmail)
        {
            result.Message = "فرمت ایمیل اشتباه است";
            return result;
        }

        var passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$";
        var regex = new Regex(passwordPattern);
        var isValidPass = regex.IsMatch(request.Password);

        if (!isValidPass)
        {
            result.Message = "رمز عبور باید شامل حداقل 8 حرف یک عدد، یک عدد و حروف بزرگ و کوچک انگبیسی باشد";
            return result;
        }

        if (request.Username.Length < 4)
        {
            result.Message = "نام کاربری حداقل شامل 3 حرف باشد";
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
            Username = request.Username,
            PasswordHash = Convert.ToBase64String(passwordHash),
            PasswordSalt = Convert.ToBase64String(passwordSalt),
            Email = request.Email
        };

        applicationDB.Users.Add(user);
        await applicationDB.SaveChangesAsync();

        result.Success = true;
        return result;
    }
}
