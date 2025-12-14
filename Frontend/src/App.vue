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
                All ({{ allTasks.length }})
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
        <p v-else-if="allTasks.length === 0" class="muted">
            No tasks yet. Add one above.
        </p>

        <!-- List -->
        <ul v-if="visibleTasks.length > 0" class="list">
            <li v-for="t in visibleTasks" :key="t.id" class="task">
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
                        <span class="muted">
                            Created: {{ formatDate(t.createdAtUtc) }}
                        </span>
                    </div>

                    <div v-if="t.description" class="task-desc">
                        {{ t.description }}
                    </div>
                </div>

                <div class="task-actions">
                    <button type="button"
                            class="btn subtle"
                            @click="toggle(t.id)">
                        {{ t.isCompleted ? "Reopen" : "Complete" }}
                    </button>
                    <button type="button"
                            class="btn danger"
                            @click="remove(t.id)">
                        Delete
                    </button>
                </div>
            </li>
        </ul>
    </div>
</template>

<script setup lang="ts">
    import { ref, computed, onMounted } from "vue";
    import {
        getTasks,
        createTask,
        toggleTask,
        deleteTask,
        type TodoTask
    } from "./apiClient";

    type Filter = "all" | "open" | "completed";

    const allTasks = ref<TodoTask[]>([]);
    const filter = ref<Filter>("all");

    const title = ref("");
    const description = ref("");
    const dueDate = ref<string | null>(null);
    const priority = ref(0);

    const loading = ref(false);
    const error = ref<string | null>(null);

    /* Derived state */
    const visibleTasks = computed(() => {
        if (filter.value === "open")
            return allTasks.value.filter(t => !t.isCompleted);
        if (filter.value === "completed")
            return allTasks.value.filter(t => t.isCompleted);
        return allTasks.value;
    });

    const openCount = computed(
        () => allTasks.value.filter(t => !t.isCompleted).length
    );

    const completedCount = computed(
        () => allTasks.value.filter(t => t.isCompleted).length
    );

    /* Lifecycle */
    onMounted(loadTasks);

    /* API */
    async function loadTasks() {
        loading.value = true;
        error.value = null;
        try {
            allTasks.value = await getTasks(null);
        } catch {
            error.value = "Failed to load tasks.";
        } finally {
            loading.value = false;
        }
    }

    async function addTask() {
        if (!title.value.trim()) {
            error.value = "Title is required.";
            return;
        }

        error.value = null;

        const created = await createTask({
            title: title.value,
            description: description.value || null,
            dueDateUtc: dueDate.value,
            priority: priority.value
        });

        allTasks.value.unshift(created);

        title.value = "";
        description.value = "";
        dueDate.value = null;
        priority.value = 0;
    }

    async function toggle(id: number) {
        const updated = await toggleTask(id);
        allTasks.value = allTasks.value.map(t =>
            t.id === updated.id ? updated : t
        );
    }

    async function remove(id: number) {
        await deleteTask(id);
        allTasks.value = allTasks.value.filter(t => t.id !== id);
    }

    /* Helpers */
    function formatDate(value: string) {
        return new Date(value).toLocaleString();
    }

    function priorityLabel(p: number) {
        return p === 2 ? "Critical" : p === 1 ? "High" : "Normal";
    }

    function priorityClass(p: number) {
        return p === 2 ? "p-critical" : p === 1 ? "p-high" : "p-normal";
    }

    function isOverdue(t: { dueDateUtc: string | null; isCompleted: boolean }) {
        return !!t.dueDateUtc && !t.isCompleted && new Date(t.dueDateUtc) < new Date();
    }
</script>
