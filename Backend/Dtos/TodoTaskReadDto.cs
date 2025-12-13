namespace TodoApp.Dtos;

public record TodoTaskReadDto(
    int Id,
    string Title,
    string? Description,
    bool IsCompleted,
    DateTime CreatedAtUtc,
    DateTime? DueDateUtc,
    int Priority
);
