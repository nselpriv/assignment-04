

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
    public State State { get; set; }

    // Implement 
    public virtual ICollection<Tag> tags { get; set; }


    public DateTime created;
    public DateTime? statusUpdated;

    // HAS TO BE HERE OTHERWISE WE GET AN ERROR CREATING THE DATABASE USING DEPENDENCY INJECTION
    public Task()
    {

    }
    public Task(string title, User? assignedUser, string? description, ICollection<Tag> tags)
    {
        Title = title;
        this.AssignedTo = assignedUser;
        this.Description = description;
        this.tags = tags;
        this.State = State.New;
        this.created = DateTime.UtcNow;
        this.statusUpdated = null;
    }
}
