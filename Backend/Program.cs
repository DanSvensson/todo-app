using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Endpoints;
using TodoApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("TodoDb"));
builder.Services.AddScoped<ITodoTaskService, TodoTaskService>();

builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();
app.UseCors();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapGet("/api/tasks", TodoTasksApi.GetTasks);
app.MapGet("/api/tasks/{id:int}", TodoTasksApi.GetTaskById);
app.MapPost("/api/tasks", TodoTasksApi.CreateTask);
app.MapPut("/api/tasks/{id:int}", TodoTasksApi.UpdateTask);
app.MapPatch("/api/tasks/{id:int}/toggle", TodoTasksApi.ToggleTask);
app.MapDelete("/api/tasks/{id:int}", TodoTasksApi.DeleteTask);

app.Run();
