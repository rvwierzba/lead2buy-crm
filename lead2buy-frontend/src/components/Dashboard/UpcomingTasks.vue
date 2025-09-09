<template>
  <div class="card">
    <h3 class="card-header">Próximas Tarefas</h3>
    <div v-if="loading" class="loading-text">Carregando...</div>
    <div v-else-if="error" class="error-text">{{ error }}</div>
    <ul v-else-if="tasks.length > 0" class="content-list">
      <li v-for="task in tasks" :key="task.id" class="task-item">
        <p class="item-title">{{ task.title }}</p>
        <div class="task-details">
          <p class="item-subtitle">Para: {{ task.contactName }}</p>
          <p class="task-duedate" :class="getDueDateColor(task.dueDate)">
            {{ formatDueDate(task.dueDate) }}
          </p>
        </div>
      </li>
    </ul>
    <div v-else class="empty-text">Nenhuma tarefa pendente.</div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import apiService from '@/services/apiService';

const tasks = ref([]);
const loading = ref(true);
const error = ref(null);

onMounted(async () => {
  try {
    const response = await apiService.getUpcomingTasks();
    tasks.value = response.data;
  } catch (err) {
    console.error("Erro ao buscar próximas tarefas:", err);
    error.value = "Não foi possível carregar as tarefas.";
  } finally {
    loading.value = false;
  }
});

const formatDueDate = (dateString) => {
  const date = new Date(dateString);
  return date.toLocaleDateString('pt-BR', { day: '2-digit', month: 'short' });
};

const getDueDateColor = (dateString) => {
  const dueDate = new Date(dateString).setHours(0, 0, 0, 0);
  const today = new Date().setHours(0, 0, 0, 0);
  if (dueDate < today) return 'due-overdue';
  if (dueDate === today) return 'due-today';
  return 'due-future';
};
</script>

<style scoped>
.card {
  background-color: var(--color-background-soft);
  border: 1px solid var(--color-border);
  border-radius: 12px;
  padding: 1.5rem;
  height: 100%;
}
.card-header {
  font-size: 1.125rem;
  font-weight: 600;
  color: var(--color-heading);
  margin-bottom: 1rem;
}
.loading-text, .error-text, .empty-text {
  text-align: center;
  color: var(--color-text-mute);
  padding-top: 2rem;
}
.error-text {
  color: #ef4444;
}
.content-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}
.task-item {
  padding: 0.75rem;
  background-color: var(--color-background);
  border: 1px solid var(--color-border);
  border-radius: 8px;
}
.item-title {
  font-weight: 600;
  color: var(--color-text);
}
.task-details {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 0.25rem;
}
.item-subtitle {
  font-size: 0.875rem;
  color: var(--color-text-mute);
}
.task-duedate {
  font-size: 0.875rem;
  font-weight: 500;
}
.due-overdue {
  color: #ef4444;
}
.dark .due-overdue {
  color: #f87171;
}
.due-today {
  color: #eab308;
}
.dark .due-today {
  color: #facc15;
}
.due-future {
  color: var(--color-text-mute);
}
</style>
