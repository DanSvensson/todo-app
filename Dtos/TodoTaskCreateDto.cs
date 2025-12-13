namespace TodoApp.Dtos;

public record TodoTaskCreateDto(
    string Title,
    string? Description,
    DateTime? DueDateUtc,
    int Priority
);