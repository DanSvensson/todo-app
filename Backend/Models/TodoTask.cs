namespace TodoApp.Models;

public class TodoTask
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsCompleted { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? DueDateUtc { get; set; }

    public int Priority { get; set; } = 0; // 0 = normal, 1 = high, etc.
}