using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class EventTag
    {
        public Guid Id { get; set; }    
        public string Tag { get; set; }

        public Event Event { get; set; }
        public Guid EventId { get; set; }
    }
}
