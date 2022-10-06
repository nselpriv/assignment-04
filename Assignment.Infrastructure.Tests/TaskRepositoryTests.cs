using System.Collections.ObjectModel;
using Microsoft.Data.Sqlite;

namespace Assignment.Infrastructure.Tests;



public sealed class TaskRepositoryTests : IDisposable
{
    private readonly KanbanContext _context;
    private readonly TaskRepository _repository;

    private readonly User testUser1;
    private readonly User testUser2;
    private readonly Task task1;
    private readonly Task task2;

    private readonly List<Task> tasklist;

    private readonly ICollection<Tag> allTags;

    public TaskRepositoryTests()
    {
        //
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>()
        .UseSqlite(connection);
        var context = new KanbanContext(builder.Options);
        context.Database.EnsureCreated();

        //
        var tag1 = new Tag("hello") { id = 1 };
        var tag2 = new Tag("KindaImportant") { id = 2 };
        var allTags = new List<Tag> { tag1, tag2 };
        var testUser1 = new User("Oscar", "Oggelicious@svenskamail.dk") { id = 1 };
        var testUser2 = new User("Gotham", "nicolejs@tuborg.dk") { id = 2 };
        var task1 = new Task("hello", testUser1, "Lets go ahead and do it", State.Active, allTags!) { id = 1 };
        var task2 = new Task("crazy important task", testUser2, "Long description", State.Removed, allTags!.Where(b => b.Name == "Hello").ToList()) { id = 2 };


        tasklist = new() { task1, task2 };
        //
        context.Tags.AddRange(tag1, tag2);
        context.Users.AddRange(testUser1, testUser2);
        context.Tasks.AddRange(task1, task2);
        context.SaveChanges();

        _context = context;
        _repository = new TaskRepository(_context);


        this.testUser1 = _context!.Users.FirstOrDefault(c => c.id == 1)!;
        this.testUser2 = _context!.Users.FirstOrDefault(c => c.id == 2)!;
        this.task1 = _context!.Tasks.FirstOrDefault(c => c.id == 1)!;
        this.task2 = _context!.Tasks.FirstOrDefault(c => c.id == 2)!;
        this.allTags = _context.Tags.ToList();

    }


    [Fact]
    public void Create_ConfirmThatTaskHasBeenCreated_ByResponseAndByID()
    {
        // Given

        var (response, created) = _repository.Create(new TaskCreateDTO("Water the garden", 1, "", new Collection<String> { "Very important", "dontforget" }));

        response.Should().Be(Response.Created);

        created.Should().Be(3);

    }

    [Fact]
    public void read_CallMethodWithTestDb_GetCollectionOfAllTestTasks()
    {
        // Given
        var suspectedTasks = tasklist.Select(c => new TaskDTO(c.id, c.Title, c.AssignedTo!.Name, c.tags.Select(c => c.Name).ToList(), c.state, c.created, c.statusUpdated));

        // When
        var allTasks = _repository.ReadAll();

        // Then
        allTasks.Should().BeEquivalentTo(suspectedTasks);
    }


    [Fact]
    public void read_GivenId1_shouldBeTask_Task1()
    {

        // Given
        var suspectedTasks = new TaskDetailsDTO(1, task1.Title, task1.Description, task1.created, testUser1.Name, allTags.Select(c => c.Name).ToList(), State.Active, task2.statusUpdated);

        // When
        var outputTask = _repository.Read(1);
        // Then
        outputTask.Should().BeEquivalentTo(suspectedTasks);
    }

    [Fact]
    public void Update_shouldReturnName_TotallyImportant_forHelloChangeToHelloButNowUpdated_AndResponseIs_Updated()
    {
        // Given
        var originalTask = _repository.Read(1);
        // When
        var response = _repository.Update(new TaskUpdateDTO(task1.id, "hello but now updated", task1.AssignedTo.id, "", allTags.Select(c => c.Name).ToList(), State.Active, task1.created));
        var updatedTask = _repository.Read(1);
        // Then
        response.Should().Be(Response.Updated);
        originalTask.Should().NotBe(updatedTask);
        updatedTask.StateUpdated.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(10));
    }


    [Fact]
    public void Delete_deleteShouldReturnResponseDeleted_ReadingId1ShouldReturnStateRemoved_ForDeletingTaskWithID1()
    {
        var preDeletion = _repository.Read(1);
        var response = _repository.Delete(1);
        response.Should().Be(Response.Deleted);
        var deletedTask = _repository.Read(1);
        deletedTask.State.Should().Be(State.Removed);

    }


    public void Dispose()
    {
        _context.Dispose();
    }


}
