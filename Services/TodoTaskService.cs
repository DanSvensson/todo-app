using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Dtos;
using TodoApp.Models;

namespace TodoApp.Services;

public sealed class TodoTaskService : ITodoTaskService
{
    private readonly AppDbContext _db;

    public TodoTaskService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<TodoTaskReadDto>> GetTasksAsync(bool? completed)
    {
        IQueryable<TodoTask> q = _db.TodoTasks;

        if (completed is not null)
            q = q.Where(t => t.IsCompleted == completed);

        var items = await q
            .OrderByDescending(t => t.CreatedAtUtc)
            .ToListAsync();

        return items.Select(ToReadDto).ToList();
    }

    public async Task<TodoTaskReadDto?> GetByIdAsync(int id)
    {
        var task = await _db.TodoTasks.FindAsync(id);
        return task is null ? null : ToReadDto(task);
    }

    public async Task<(bool Success, string? Error, TodoTaskReadDto? Created)> CreateAsync(
        TodoTaskCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            return (false, "Title is required.", null);

        var task = new TodoTask
        {
            Title = dto.Title.Trim(),
            Description = dto.Description,
            DueDateUtc = dto.DueDateUtc,
            Priority = dto.Priority,
            IsCompleted = false,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.TodoTasks.Add(task);
        await _db.SaveChangesAsync();

        return (true, null, ToReadDto(task));
    }

    public async Task<(bool Success, string? Error, TodoTaskReadDto? Updated)> UpdateAsync(
        int id, TodoTaskUpdateDto dto)
    {
        var task = await _db.TodoTasks.FindAsync(id);
        if (task is null) return (false, "NotFound", null);

        if (string.IsNullOrWhiteSpace(dto.Title))
            return (false, "Title is required.", null);

        task.Title = dto.Title.Trim();
        task.Description = dto.Description;
        task.IsCompleted = dto.IsCompleted;
        task.DueDateUtc = dto.DueDateUtc;
        task.Priority = dto.Priority;

        await _db.SaveChangesAsync();
        return (true, null, ToReadDto(task));
    }

    public async Task<TodoTaskReadDto?> ToggleAsync(int id)
    {
        var task = await _db.TodoTasks.FindAsync(id);
        if (task is null) return null;

        task.IsCompleted = !task.IsCompleted;
        await _db.SaveChangesAsync();

        return ToReadDto(task);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var task = await _db.TodoTasks.FindAsync(id);
        if (task is null) return false;

        _db.TodoTasks.Remove(task);
        await _db.SaveChangesAsync();
        return true;
    }

    private static TodoTaskReadDto ToReadDto(TodoTask t) =>
        new(t.Id, t.Title, t.Description, t.IsCompleted, t.CreatedAtUtc, t.DueDateUtc, t.Priority);
}
