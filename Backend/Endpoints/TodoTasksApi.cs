using TodoApp.Dtos;
using TodoApp.Services;

namespace TodoApp.Endpoints;

public static class TodoTasksApi
{
    public static async Task<IResult> GetTasks(
        ITodoTaskService svc, bool? completed) =>
        Results.Ok(await svc.GetTasksAsync(completed));

    public static async Task<IResult> GetTaskById(
        ITodoTaskService svc, int id)
    {
        var task = await svc.GetByIdAsync(id);
        return task is null ? Results.NotFound() : Results.Ok(task);
    }

    public static async Task<IResult> CreateTask(
        ITodoTaskService svc, TodoTaskCreateDto dto)
    {
        var (ok, err, created) = await svc.CreateAsync(dto);
        if (!ok) return Results.BadRequest(err);
        return Results.Created($"/api/tasks/{created!.Id}", created);
    }

    public static async Task<IResult> UpdateTask(
        ITodoTaskService svc, int id, TodoTaskUpdateDto dto)
    {
        var (ok, err, updated) = await svc.UpdateAsync(id, dto);
        if (!ok && err == "NotFound") return Results.NotFound();
        if (!ok) return Results.BadRequest(err);
        return Results.Ok(updated);
    }

    public static async Task<IResult> ToggleTask(
        ITodoTaskService svc, int id)
    {
        var updated = await svc.ToggleAsync(id);
        return updated is null ? Results.NotFound() : Results.Ok(updated);
    }

    public static async Task<IResult> DeleteTask(
        ITodoTaskService svc, int id)
    {
        var ok = await svc.DeleteAsync(id);
        return ok ? Results.NoContent() : Results.NotFound();
    }
}
