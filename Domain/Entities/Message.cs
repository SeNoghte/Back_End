using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class Message
{
    public int MessageId { get; set; }
    public string Text { get; set; }
    public DateTime SentTime { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public Guid GroupId { get; set; }
    public Group Group { get; set; }
}
