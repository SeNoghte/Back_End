using Application.Common.Models;
using Application.Common.Services.CloudService;
using Application.Common.Services.GeneralServices;
using Application.Common.Services.IdentityService;
using DataAccess;
using Domain.Entities;
using MediatR;

namespace Application.Events
{
    public class CreateEventCommand : IRequest<CreateEventResult>
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public Guid GroupId { get; set; }
        public string ImagePath { get; set; }
    }

    public class CreateEventResult : ResultModel
    {
        public Guid EventId { get; set; }
    }

    public class CreateEventHandler : IRequestHandler<CreateEventCommand, CreateEventResult>
    {
        private readonly IIdentityService identityService;
        private readonly IGeneralServices generalServices;
        private readonly ICloudService cloudService;
        private readonly ApplicationDBContext dBContext;

        public CreateEventHandler(IIdentityService identityService, IGeneralServices generalServices
            , ICloudService cloudService, ApplicationDBContext dBContext)
        {
            this.identityService = identityService;
            this.generalServices = generalServices;
            this.cloudService = cloudService;
            this.dBContext = dBContext;
        }

        public async Task<CreateEventResult> Handle(CreateEventCommand request, CancellationToken cancellationToken)
        {
            var result = new CreateEventResult();

            try
            {
                var UserId = identityService.GetCurrentUserId();

                if (UserId == null)
                {
                    result.ErrorCode = 401;
                    result.Message = "Unauthorized";
                    return result;
                }

                var currentUser = await generalServices.GetUser((Guid)UserId);
                 
                if (currentUser == null)
                {
                    result.ErrorCode = 404;
                    result.Message = "کاربر پیدا نشد";
                    return result;
                }
                
                var targetGroup = await generalServices.GetGroup(request.GroupId);

                if (targetGroup == null)
                {
                    result.ErrorCode = 401;
                    result.Message = "گروه پیدا نشد";
                    return result;
                }

                DateTime dateTime = new DateTime(request.Date, request.Time, DateTimeKind.Utc);

                if (dateTime < DateTime.UtcNow) 
                {
                    result.ErrorCode = 401;
                    result.Message = "تاریخ یا ساعت نامعتبر";
                    return result;
                }

                var newEvent = new Event
                {
                    Id = Guid.NewGuid(),
                    Title = request.Title,
                    Description = request.Description,
                    StartDate = dateTime,
                    EndDate = dateTime.AddDays(1),
                    CreatedDate = DateTime.UtcNow,
                    ImagePath = request.ImagePath,
                    GroupId = request.GroupId,
                    OwnerId = currentUser.UserId
                };


                var userEvent = new UserEvent
                {
                    UserId = currentUser.UserId,
                    EventId = newEvent.Id,
                    JoinedDate = DateTime.UtcNow,
                };

                await dBContext.Events.AddAsync(newEvent);
                await dBContext.UserEvents.AddAsync(userEvent);
                await dBContext.SaveChangesAsync();
                
                result.Success = true;
                result.EventId = newEvent.Id;
                return result;

            }
            catch (Exception ex)
            {
                result.ErrorCode = 500;
                result.Message = "مشکلی پیش آمده است";
                return result;

            }
        }
    }
}
