using Application.Common.Models;
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
    public class GetMyEventsQuery : IRequest<GetMyEventsResult>
    {

    }

    public class GetMyEventsResult : ResultModel
    {
        public List<EventDto> MyEvents { get; set; }
    }

    public class GetMyEventsHandler : IRequestHandler<GetMyEventsQuery, GetMyEventsResult>
    {
        private readonly IIdentityService identityService;
        private readonly IGeneralServices generalServices;
        private readonly ApplicationDBContext dBContext;

        public GetMyEventsHandler(IIdentityService identityService, IGeneralServices generalServices
            , ApplicationDBContext dBContext)
        {
            this.identityService = identityService;
            this.generalServices = generalServices;
            this.dBContext = dBContext;
        }

        public async Task<GetMyEventsResult> Handle(GetMyEventsQuery request, CancellationToken cancellationToken)
        {
            var result = new GetMyEventsResult();

            try
            {
                var UserId = identityService.GetCurrentUserId();

                if (UserId == null)
                {
                    result.ErrorCode = 401;
                    result.Message = "Unauthorized";
                    return result;
                }   

                var currentUser = await dBContext.Users
                .Include(u => u.Events)
                    .ThenInclude(ue => ue.Event)
                .FirstOrDefaultAsync(u => u.UserId == UserId);

                result.MyEvents = currentUser.Events.Select(e => new EventDto
                {
                    Id = e.Event.Id,
                    IsPrivate = e.Event.IsPrivate,
                    Title = e.Event.Title,
                    ImagePath = e.Event.ImagePath
                }).ToList();

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
