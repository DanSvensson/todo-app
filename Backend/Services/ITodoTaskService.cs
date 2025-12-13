using TodoApp.Dtos;

namespace TodoApp.Services;

public interface ITodoTaskService
{
    Task<IReadOnlyList<TodoTaskReadDto>> GetTasksAsync(bool? completed);
    Task<TodoTaskReadDto?> GetByIdAsync(int id);
    Task<(bool Success, string? Error, TodoTaskReadDto? Created)> CreateAsync(TodoTaskCreateDto dto);
    Task<(bool Success, string? Error, TodoTaskReadDto? Updated)> UpdateAsync(int id, TodoTaskUpdateDto dto);
    Task<TodoTaskReadDto?> ToggleAsync(int id);
    Task<bool> DeleteAsync(int id);
}
