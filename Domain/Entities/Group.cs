namespace Domain.Entities
{
    public class Group
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }      

        public ICollection<UserGroup> Members { get; set; }
    }
}
