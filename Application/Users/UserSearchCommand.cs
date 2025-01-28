using Application.Common.Models;
using Application.Common.Services.CloudService;
using Application.Common.Services.IdentityService;
using Application.DTO;
using DataAccess;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users
{
    public class UserSearchCommand : IRequest<UserSearchResult>
    {
        public string Filter { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class UserSearchResult : ResultModel
    {
        public List<UserDto> FilteredUsers { get; set; }
    }

    public class UserSearchHandler : IRequestHandler<UserSearchCommand, UserSearchResult>
    {
        private readonly ApplicationDBContext dBContext;
        private readonly IIdentityService identityService;
        private readonly ICloudService cloudService;

        public UserSearchHandler(ApplicationDBContext dBContext, IIdentityService identityService
            , ICloudService cloudService)
        {
            this.dBContext = dBContext;
            this.identityService = identityService;
            this.cloudService = cloudService;
        }

        public async Task<UserSearchResult> Handle(UserSearchCommand request, CancellationToken cancellationToken)
        {
            var result = new UserSearchResult();

            try
            {
                var userId = identityService.GetCurrentUserId();

                if (userId == null)
                {
                    result.ErrorCode = 401;
                    result.Message = "Unauthorized";
                    return result;
                }

                var usersQuery = dBContext.Users
                    .Include(u => u.Groups)
                    .ThenInclude(ug => ug.Group)
                    .OrderBy(x => x.Username);

                if (request.Filter != null)
                {
                    usersQuery = usersQuery
                        .Where(u => u.Username.ToLower().Contains(request.Filter.ToLower()) && u.UserId != userId)
                        .OrderBy(u => u.Username);
                }

                usersQuery
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize);

                var users = await usersQuery.ToListAsync();

                result.FilteredUsers = users.Select(u => new UserDto
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Name = u.Name,
                    Email = u.Email,
                    JoinedDate = u.JoinedDate,
                    Image = u.Image
                }).ToList();

                result.Success = true;
                return result;
            }
            catch
            {
                result.Message = "مشکلی پیش آمده است";
                return result;
            }
        }
    }
}
