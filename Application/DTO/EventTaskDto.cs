using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class EventTaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }

        public Guid? AssignedUserId { get; set; }
        public string? AssignedUserName { get; set;}
    }
}
