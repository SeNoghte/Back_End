using Application.Common.Models;
using Application.Common.Services.GeneralServices;
using Application.Common.Services.IdentityService;
using Application.DTO;
using DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users
{
    public class EditProfileCommand : IRequest<EditProfileResult>
    {
        public string Name { get; set; }
        public string? Username { get; set; }
        public string? Image { get; set; }
        public string? AboutMe { get; set; }
    }

    public class EditProfileResult : ResultModel
    {
        public UserDto User { get; set; }
    }

    public class EditProfileHandler : IRequestHandler<EditProfileCommand, EditProfileResult>
    {
        private readonly IIdentityService identityService;
        private readonly ApplicationDBContext dBContext;
        private readonly IGeneralServices generalServices;

        public EditProfileHandler(IIdentityService identityService, ApplicationDBContext dBContext,
            IGeneralServices generalServices)
        {
            this.identityService = identityService;
            this.dBContext = dBContext;
            this.generalServices = generalServices;
        }

        public async Task<EditProfileResult> Handle(EditProfileCommand request, CancellationToken cancellationToken)
        {
            var result = new EditProfileResult();

            try
            {
                var UserId = identityService.GetCurrentUserId();

                if (UserId == null)
                {
                    result.ErrorCode = 401;
                    result.Message = "Unauthorized";
                    return result;
                }

                var user = await dBContext.Users.Where(u => u.UserId == UserId).FirstOrDefaultAsync();

                if (user == null)
                {
                    result.Message = "کاربر پیدا نشد";
                    result.ErrorCode = 404;
                    return result;
                }

                if (request.Name.Length < 2)
                {
                    result.Message = "نام حداقل شامل 3 حرف باشد";
                    result.ErrorCode = 401;
                    return result;
                }

                if (!string.IsNullOrEmpty(request.Username))
                {
                    if (user.Username != request.Username)
                    {
                        var isValidUsername = generalServices.CheckUsernameFormat(request.Username);

                        if (!isValidUsername)
                        {
                            result.Message = "نام کاربری حداقل شامل 2 حرف باشد و فقط حرف انگلیسی،عدد، '_' و'-' مجاز است";
                            result.ErrorCode = 401;
                            return result;
                        }

                        var userExists = await dBContext.Users.AnyAsync(u => u.Username == request.Username);

                        if (userExists)
                        {

                            result.Message = "نام کاربری در سایت ثبت نام شده است";
                            result.ErrorCode = 401;
                            return result;
                        }
                    }
                }

                user.Name = request.Name;
                user.Username = request.Username;
                user.AboutMe = request.AboutMe;

                if (!string.IsNullOrEmpty(request.Image))
                {
                    user.Image = request.Image;
                }

                await dBContext.SaveChangesAsync();

                result.User = new UserDto
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    Name = user.Name,
                    Image = user.Image,
                    JoinedDate = user.JoinedDate,
                    Username = user.Username,
                    AboutMe = user.AboutMe,
                };
                result.Success = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Message = "مشکلی پیش آمده است";
                result.ErrorCode = 500;
                return result;
            }

        }
    }
}
