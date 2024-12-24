namespace Domain.Entities
{
    public class Group
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }      
        public string? Image { get; set; }
        public bool IsPrivate { get; set; }      
        public ICollection<UserGroup> Members { get; set; }

        public Guid OwnerId { get; set; }
        public User Owner { get; set; }


        public ICollection<Event> Events { get; set; }

    }
}
