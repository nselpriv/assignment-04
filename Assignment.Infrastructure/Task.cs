

namespace Assignment.Infrastructure;

public class Task
{
    public int id { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    public User? AssignedTo { get; set; }

    [StringLength(int.MaxValue)]
    public string? Description { get; set; }

    [Required]
    public State state { get; set; }

    // Implement 
    public virtual ICollection<Tag> tags { get; set; }


    public DateTime created;
    public DateTime? statusUpdated;
    public Task()
    {

    }


    public Task(string title, User? assignedUser, string? description, State state, ICollection<Tag> tags, DateTime created)
    {
        Title = title;
        this.AssignedTo = assignedUser;
        this.Description = description;
        this.state = state;
        this.tags = tags;
        this.created = created;
        this.statusUpdated = null;
    }
}