using Application.Common.Models;
using DataAccess;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Groups
{

    public class GroupCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class GroupCreateCommand : IRequest<GroupCreateResult>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string OwnerId { get; set; }
    }

    public class GroupCreateResult : ResultModel
    {
        public string GroupId { get; set; }
    }

    public class GroupCreateHandler : IRequestHandler<GroupCreateCommand, GroupCreateResult>
    {
        private readonly ApplicationDBContext dBContext;

        public GroupCreateHandler(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        public async Task<GroupCreateResult> Handle(GroupCreateCommand request, CancellationToken cancellationToken)
        {
            var result = new GroupCreateResult();

            var ownerExist = await dBContext.Users.AnyAsync(u => u.UserId.ToString() == request.OwnerId);

            if (!ownerExist)
            {
                result.Message = "کاربر پیدا نشد";
                return result;
            }

            if(request.Name.Length < 3)
            {
                result.Message = "نام گروه حداقل شامل 2 حرف باشد";
                return result;
            }

            var groupExist = await dBContext.Groups.AnyAsync(gp => gp.Name ==  request.Name);

            if (groupExist)
            {
                result.Message = " نام گروه در سایت ثبت شده است";
                return result;
            }

            var gp = new Group
            {
                Name = request.Name,
                Description = request.Description,
                CreatedDate = DateTime.UtcNow,
                OwnerId = Guid.Parse(request.OwnerId)
            };

            await dBContext.AddAsync(gp);
            await dBContext.SaveChangesAsync();

            gp = await dBContext.Groups.FirstOrDefaultAsync(gp => gp.Name == request.Name);

            result.GroupId = gp.Id.ToString();
            result.Success = true;
            return result;
        }
    }
}
