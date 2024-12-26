﻿namespace Domain.Entities
{
    public class Event
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? ImagePath { get; set; }
        public int MaxMembers { get; set; }
        public bool IsPrivate { get; set; }

        public Guid GroupId { get; set; }
        public Group Group { get; set; }

        public Guid OwnerId { get; set; }
        public User Owner { get; set; }
        public int? CityId { get; set; }
        public string? Address { get; set; }
        public decimal? Longitude {  get; set; }
        public decimal? Latitude {  get; set; }
        public ICollection<UserEvent> EventMembers { get; set; }

        public ICollection<EventTask>? Tasks { get; set; }

        public ICollection<EventTag>? Tags { get; set; }

    }
}
