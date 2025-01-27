using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class EventDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public UserDto Owner { get; set; }
        public string Date { get; set; }  
        public string? Time { get; set; }  
        public Guid GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupImage { get; set; }
        public string ImagePath { get; set; }
        public bool IsPrivate { get; set; } 
        public List<UserDto> Members { get; set; }
        public int? CityId { get; set; }
        public string? Address { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
    }
}
