using Application.Common.Models;
using Application.Common.Services.CloudService;
using Application.Common.Services.GeneralServices;
using Application.Common.Services.IdentityService;
using DataAccess;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Events
{
    public class CreateEventCommand : IRequest<CreateEventResult>
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateOnly Date { get; set; }
        public string? Time { get; set; }
        public Guid GroupId { get; set; }
        public string? ImagePath { get; set; }
        public List<string>? Tasks { get; set; }
        public int? CityId { get; set; }
        public string? Address { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
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
                    result.ErrorCode = 404;
                    result.Message = "گروه پیدا نشد";
                    return result;
                }

                var memberOfGroup = await dBContext.UserGroups.Where(ug => ug.GroupId == request.GroupId &&
                                                     ug.UserId == currentUser.UserId).FirstOrDefaultAsync();

                if (memberOfGroup  == null)
                {
                    result.ErrorCode = 401;
                    result.Message = "کاربر عضو گروه نیست";
                    return result;
                }

                DateTime dateTime;       

                if (string.IsNullOrEmpty(request.Time ))
                {                 
                     dateTime = new DateTime(request.Date, TimeOnly.MinValue, DateTimeKind.Utc);
                }
                else
                {
                     dateTime = new DateTime(request.Date, TimeOnly.Parse(request.Time), DateTimeKind.Utc);
                }



                if(new DateTime(request.Date, TimeOnly.MaxValue) < DateTime.UtcNow)
                {
                    result.ErrorCode = 401;
                    result.Message = "تاریخ نامعتبر";
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
                    OwnerId = currentUser.UserId,
                    CityId = request.CityId,
                    Address = request.Address,
                    Latitude = request.Latitude,
                    Longitude = request.Longitude
                };


                var userEvent = new UserEvent
                {
                    UserId = currentUser.UserId,
                    EventId = newEvent.Id,
                    JoinedDate = DateTime.UtcNow,
                };

                var tasks = request.Tasks?.Select(title => new EventTask
                {
                    Id= Guid.NewGuid(),
                    Title = title,
                    EventId = newEvent.Id
                });

                await dBContext.Events.AddAsync(newEvent);
                await dBContext.UserEvents.AddAsync(userEvent);
                if(tasks is not null)
                    await dBContext.EventTasks.AddRangeAsync(tasks);
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
