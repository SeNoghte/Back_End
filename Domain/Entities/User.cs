namespace Domain.Entities;

public class User
{
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public string? Username { get; set; }
    public string Email { get; set; }
    public string? Image { get; set; }
    public string? PasswordHash { get; set; }
    public string? PasswordSalt { get; set; }
    public DateTime JoinedDate { get; set; }
    public string? AboutMe { get; set; }

    public ICollection<UserGroup> Groups { get; set; }
    public ICollection<Group> OwnedGroups { get; set; }
    public ICollection<UserEvent> Events { get; set; }
    public ICollection<Event> EventsOwned { get; set; }

}
