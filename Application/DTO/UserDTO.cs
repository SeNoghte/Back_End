using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class UserDto
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string? Username { get; set; }
        public string Email { get; set; }
        public DateTime JoinedDate { get; set; }
    }
}
