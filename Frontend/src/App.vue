<script setup lang="ts">
    import { ref, onMounted, watch, computed } from "vue";
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

    const openCount = computed(() => tasks.value.filter(t => !t.isCompleted).length);
    const completedCount = computed(() => tasks.value.filter(t => t.isCompleted).length);

    function priorityLabel(p: number) {
      return p === 2 ? "Critical" : p === 1 ? "High" : "Normal";
    }
    function priorityClass(p: number) {
      return p === 2 ? "p-critical" : p === 1 ? "p-high" : "p-normal";
    }
    function isOverdue(t: { dueDateUtc: string | null; isCompleted: boolean }) {
      return !!t.dueDateUtc && !t.isCompleted && new Date(t.dueDateUtc) < new Date();
    }

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
        <header class="header">
            <h1>To-Do Task Manager</h1>
        </header>

        <!-- Create Task -->
        <form class="card" @submit.prevent="addTask">
            <div class="form-grid">
                <label class="field">
                    <span>Title</span>
                    <input v-model="title" placeholder="e.g., Bowling" />
                </label>

                <label class="field">
                    <span>Due date</span>
                    <input type="datetime-local" v-model="dueDate" />
                </label>

                <label class="field desc">
                    <span>Description</span>
                    <textarea v-model="description" placeholder="Optional notes..." />
                </label>

                <label class="field">
                    <span>Priority</span>
                    <select v-model="priority">
                        <option :value="0">Normal</option>
                        <option :value="1">High</option>
                        <option :value="2">Critical</option>
                    </select>
                </label>

                <div class="spacer"></div>

                <button class="btn primary" type="submit">Add Task</button>
            </div>
        </form>

        <!-- Filters -->
        <div class="filters">
            <button type="button" class="btn pillbtn" @click="filter = 'all'">
                All ({{ tasks.length }})
            </button>
            <button type="button" class="btn pillbtn" @click="filter = 'open'">
                Open ({{ openCount }})
            </button>
            <button type="button" class="btn pillbtn" @click="filter = 'completed'">
                Completed ({{ completedCount }})
            </button>
        </div>

        <!-- Status -->
        <p v-if="loading" class="muted">Loading…</p>
        <p v-else-if="error" class="error">{{ error }}</p>
        <p v-else-if="tasks.length === 0" class="muted">No tasks yet. Add one above.</p>

        <!-- List -->
        <ul v-if="tasks.length > 0" class="list">
            <li v-for="t in tasks" :key="t.id" class="task">
                <label class="task-check">
                    <input type="checkbox"
                           :checked="t.isCompleted"
                           @change="toggle(t.id)"
                           :aria-label="`Mark ${t.title} as ${t.isCompleted ? 'incomplete' : 'complete'}`" />
                </label>

                <div class="task-main">
                    <div class="task-top">
                        <div class="task-title" :class="{ done: t.isCompleted }">
                            {{ t.title }}
                        </div>

                        <span class="pill" :class="priorityClass(t.priority)">
                            {{ priorityLabel(t.priority) }}
                        </span>
                    </div>

                    <div class="task-meta">
                        <span v-if="t.dueDateUtc">
                            Due:
                            <strong :class="{ overdue: isOverdue(t) }">
                                {{ formatDate(t.dueDateUtc) }}
                            </strong>
                        </span>
                        <span v-else class="muted">No due date</span>
                        <span class="dot">•</span>
                        <span class="muted">Created: {{ formatDate(t.createdAtUtc) }}</span>
                    </div>

                    <div v-if="t.description" class="task-desc">
                        {{ t.description }}
                    </div>
                </div>

                <div class="task-actions">
                    <button type="button" class="btn subtle" @click="toggle(t.id)">
                        {{ t.isCompleted ? "Reopen" : "Complete" }}
                    </button>
                    <button type="button" class="btn danger" @click="remove(t.id)">
                        Delete
                    </button>
                </div>
            </li>
        </ul>
    </div>
</template>

