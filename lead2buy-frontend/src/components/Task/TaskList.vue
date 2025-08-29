<template>
  <div class="tasks-panel">
    <h3>Tarefas Agendadas</h3>
    <ul class="task-list">
      <li v-for="task in tasks" :key="task.id" :class="{ completed: task.isCompleted }">
        <div class="task-info">
          <input
            type="checkbox"
            :checked="task.isCompleted"
            @change="$emit('toggle-complete', task)"
          />
          <div class="task-details">
            <span class="task-title">{{ task.title }}</span>
            <span class="task-due-date">Vence em: {{ new Date(task.dueDate).toLocaleDateString('pt-BR', { timeZone: 'UTC' }) }}</span>
          </div>
        </div>
        <div class="task-actions">
          <button @click="$emit('delete-task', task.id)">Excluir</button>
        </div>
      </li>
      <li v-if="tasks.length === 0" class="no-tasks">
        Nenhuma tarefa agendada.
      </li>
    </ul>
    <form @submit.prevent="handleAddTask" class="add-task-form">
      <input type="text" v-model="newTaskTitle" placeholder="Adicionar nova tarefa..." required />
      <input type="date" v-model="newTaskDueDate" required />
      <button type="submit">+ Adicionar</button>
    </form>
  </div>
</template>

<script setup>
import { ref } from 'vue';

defineProps({
  tasks: { type: Array, required: true }
});

const emit = defineEmits(['add-task', 'delete-task', 'toggle-complete']);

const newTaskTitle = ref('');
const newTaskDueDate = ref('');

const handleAddTask = () => {
  if (newTaskTitle.value && newTaskDueDate.value) {
    emit('add-task', {
      title: newTaskTitle.value,
      dueDate: newTaskDueDate.value
    });
    newTaskTitle.value = '';
    newTaskDueDate.value = '';
  }
};
</script>

<style scoped>
.tasks-panel { background-color: var(--ui-bg); border: 1px solid var(--ui-border); padding: 1.5rem; border-radius: 12px; }
h3 { margin-top: 0; color: var(--text-color); }
.task-list { list-style: none; padding: 0; margin: 0 0 1rem 0; }
.task-list li { display: flex; justify-content: space-between; align-items: center; padding: 0.75rem 0; border-bottom: 1px solid var(--ui-border); }
.task-list li.completed .task-title { text-decoration: line-through; color: var(--text-color-muted); }
.task-info { display: flex; align-items: center; gap: 0.75rem; }
.task-details { display: flex; flex-direction: column; }
.task-due-date { font-size: 12px; color: var(--text-color-muted); }
.add-task-form { display: flex; gap: 0.5rem; }
.add-task-form input[type="text"] { flex: 1; }
.add-task-form input, .add-task-form button { padding: 8px; border: 1px solid var(--ui-border); border-radius: 6px; background-color: var(--bg-color); color: var(--text-color); }
.add-task-form button { background-color: var(--primary-color); color: #fff; cursor: pointer; }
</style>
