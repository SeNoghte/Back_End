using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class GroupDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsPrivate { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public UserDto Owner { get; set; }
        public List<UserDto> Members { get; set; }
        public string Image { get; set; }
        public bool IsAdmin { get; set; }
    }
}
