namespace TodoApp.Dtos;

public record TodoTaskUpdateDto(
    string Title,
    string? Description,
    bool IsCompleted,
    DateTime? DueDateUtc,
    int Priority
);
