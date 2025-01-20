using Application.Common.Models;
using Application.Common.Services.CloudService;
using Application.Common.Services.GeneralServices;
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

namespace Application.Events
{
    public class GetEventQuery : IRequest<GetEventResult>
    {
        public Guid EventId { get; set; }
    }

    public class GetEventResult : ResultModel
    {
        public EventDto Event { get; set; }
        public List<EventTaskDto> Tasks { get; set; }
        public List<EventTagDto> Tags { get; set; }
    }

    public class GetEventHandler : IRequestHandler<GetEventQuery, GetEventResult>
    {
        private readonly ApplicationDBContext dBContext;
        private readonly IGeneralServices generalServices;
        private readonly IIdentityService identityService;   
        public GetEventHandler(ApplicationDBContext dBContext, IGeneralServices generalServices,
            IIdentityService identityService)
        {
            this.dBContext = dBContext;
            this.generalServices = generalServices;
            this.identityService = identityService;
        }

        public async Task<GetEventResult> Handle(GetEventQuery request, CancellationToken cancellationToken)
        {
            var result = new GetEventResult();

            try
            {
                var userId = identityService.GetCurrentUserId();

                if (userId == null)
                {
                    result.ErrorCode = 401;
                    result.Message = "Unauthorized";
                    return result;
                }

                var e = await dBContext.Events
                    .Where(ev => ev.Id == request.EventId)
                    .Include(ev=> ev.Tasks)
                    .ThenInclude(evt => evt.AssignedUser)
                    .Include(ev => ev.Tags)
                    .Include(ev => ev.EventMembers)
                    .ThenInclude(ue => ue.User)       
                    .FirstOrDefaultAsync();

                if (e == null)
                {
                    result.ErrorCode = 404;
                    result.Message = "اینونت یافت نشد";
                    return result;
                }

                result.Event = new EventDto
                {
                    Title = e.Title,
                    Description = e.Description,
                    Date = e.StartDate.ToString("yyyy-MM-dd"),
                    Time = e.StartDate.ToString("HH:mm:ss"),
                    GroupId = e.GroupId,
                    ImagePath = e.ImagePath,   
                    IsPrivate = e.IsPrivate,
                    Members = e.EventMembers.Select(em => new UserDto
                    {
                        UserId = em.User.UserId,
                        Username = em.User.Username,
                        Name = em.User.Name,
                        Email = em.User.Email,
                        Image = em.User.Image

                    }).ToList()
                };

                result.Tasks = e.Tasks.Select(t => new EventTaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    AssignedUserId = t.AssignedUserId,
                    AssignedUserName = t.AssignedUser?.Name,
                }).ToList();

                result.Tags = e.Tags.Select(t => new EventTagDto
                {
                    Id = t.Id,
                    Tag = t.Tag,
                }).ToList();

                result.Success = true;
                return result;
            }
            catch
            {
                result.Message = "مشکلی پیش آمده است";
                result.ErrorCode = 500;
                return result;
            }
        }
    }
}
