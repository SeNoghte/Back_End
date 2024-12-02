using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class EventTask
    {
        public Guid Id { get; set; }
        public string Title { get; set; }

        public Guid EventId { get; set; }
        public Event Event { get; set; }

        public Guid? AssginedUserId { get; set; }
        public User AssignedUser { get; set; }
    }
}
