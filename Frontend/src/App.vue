<script setup lang="ts">
import { ref, onMounted, watch } from "vue";
import type { TodoTask } from "./apiClient";
import {
  getTasks,
  createTask,
  toggleTask,
  deleteTask,
} from "./apiClient";

type Filter = "all" | "open" | "completed";

const tasks = ref<TodoTask[]>([]);
const filter = ref<Filter>("all");

const title = ref("");
const description = ref("");
const dueDate = ref("");
const priority = ref(0);

const loading = ref(false);
const error = ref("");

async function loadTasks() {
  loading.value = true;
  error.value = "";

  let completed: boolean | null = null;
  if (filter.value === "open") completed = false;
  if (filter.value === "completed") completed = true;

  try {
    tasks.value = await getTasks(completed);
  } catch {
    error.value = "Failed to load tasks.";
  } finally {
    loading.value = false;
  }
}

onMounted(loadTasks);
watch(filter, loadTasks);

async function addTask() {
  if (!title.value.trim()) {
    error.value = "Title is required.";
    return;
  }

  try {
    const created = await createTask({
      title: title.value.trim(),
      description: description.value || null,
      dueDateUtc: dueDate.value
        ? new Date(dueDate.value).toISOString()
        : null,
      priority: priority.value,
    });

    tasks.value.unshift(created);

    title.value = "";
    description.value = "";
    dueDate.value = "";
    priority.value = 0;
  } catch {
    error.value = "Failed to create task.";
  }
}

async function toggle(id: number) {
  try {
    const updated = await toggleTask(id);
    tasks.value = tasks.value.map((t) =>
      t.id === updated.id ? updated : t
    );
  } catch {
    error.value = "Failed to update task.";
  }
}

async function remove(id: number) {
  if (!confirm("Delete this task?")) return;

  try {
    await deleteTask(id);
    tasks.value = tasks.value.filter((t) => t.id !== id);
  } catch {
    error.value = "Failed to delete task.";
  }
}

function formatDate(value: string | null) {
  return value ? new Date(value).toLocaleString() : "";
}
</script>

<template>
    <div class="app">
        <h1>To-Do Task Manager</h1>

        <form class="card" @submit.prevent="addTask">
            <input v-model="title" placeholder="Task title" />
            <textarea v-model="description" placeholder="Description (optional)" />
            <input type="datetime-local" v-model="dueDate" />
            <select v-model="priority">
                <option :value="0">Normal</option>
                <option :value="1">High</option>
                <option :value="2">Critical</option>
            </select>
            <button class="primary">Add Task</button>
        </form>

        <div class="filters">
            <button @click="filter = 'all'">All</button>
            <button @click="filter = 'open'">Open</button>
            <button @click="filter = 'completed'">Completed</button>
        </div>

        <p v-if="loading">Loading…</p>
        <p v-if="error" class="error">{{ error }}</p>

        <ul>
            <li v-for="t in tasks" :key="t.id">
                <div class="task-left">
                    <input type="checkbox" :checked="t.isCompleted" @change="toggle(t.id)" />
                    <div>
                        <div class="task-title" :class="{ done: t.isCompleted }">{{ t.title }}</div>
                        <div class="task-meta" v-if="t.dueDateUtc">Due: {{ formatDate(t.dueDateUtc) }}</div>
                    </div>
                </div>
                <button @click="remove(t.id)">Delete</button>
            </li>
        </ul>
    </div>
</template>
