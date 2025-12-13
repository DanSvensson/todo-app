const API_BASE_URL = "http://localhost:5218"; // match your .NET API port

export interface TodoTask {
    id: number;
    title: string;
    description: string | null;
    isCompleted: boolean;
    createdAtUtc: string;
    dueDateUtc: string | null;
    priority: number;
}

export interface CreateTodoTask {
    title: string;
    description: string | null;
    dueDateUtc: string | null;
    priority: number;
}

export async function getTasks(
    filterCompleted: boolean | null = null
): Promise<TodoTask[]> {
    const url =
        filterCompleted === null
            ? `${API_BASE_URL}/api/tasks`
            : `${API_BASE_URL}/api/tasks?completed=${filterCompleted}`;

    const res = await fetch(url);
    if (!res.ok) throw new Error("Failed to fetch tasks");
    return res.json();
}

export async function createTask(
    task: CreateTodoTask
): Promise<TodoTask> {
    const res = await fetch(`${API_BASE_URL}/api/tasks`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(task),
    });

    if (!res.ok) throw new Error("Failed to create task");
    return res.json();
}

export async function toggleTask(id: number): Promise<TodoTask> {
    const res = await fetch(`${API_BASE_URL}/api/tasks/${id}/toggle`, {
        method: "PATCH",
    });

    if (!res.ok) throw new Error("Failed to toggle task");
    return res.json();
}

export async function deleteTask(id: number): Promise<void> {
    const res = await fetch(`${API_BASE_URL}/api/tasks/${id}`, {
        method: "DELETE",
    });

    if (!res.ok) throw new Error("Failed to delete task");
}
