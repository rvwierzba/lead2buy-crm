<template>
  <div class="contact-detail-page">
    <div v-if="loading" class="loading">Carregando...</div>
    <div v-if="error" class="error">{{ error }}</div>

    <div v-if="contact" class="contact-container">
      <header class="page-header">
        <router-link to="/" class="back-link">&larr; Voltar para a lista</router-link>
        <h1>{{ contact.name }}</h1>
        <span :class="['status-badge', getStatusClass(contact.status)]">{{ contact.status }}</span>
      </header>

      <div class="details-grid">
        <div class="details-panel">
          <h3>Detalhes do Contato</h3>
          <div class="detail-item"><strong>Email:</strong> {{ contact.email || 'Não informado' }}</div>
          <div class="detail-item"><strong>Telefone:</strong> {{ contact.phoneNumber || 'Não informado' }}</div>
          <div class="detail-item"><strong>Origem:</strong> {{ contact.source || 'Não informada' }}</div>
          <div class="detail-item"><strong>Sexo:</strong> {{ contact.gender || 'Não informado' }}</div>
          <div class="detail-item"><strong>Nascimento:</strong> {{ contact.dateOfBirth ? new Date(contact.dateOfBirth).toLocaleDateString('pt-BR') : 'Não informado' }}</div>
          <div class="detail-item address">
            <strong>Endereço:</strong>
            <span>{{ contact.street || 'Não informado' }}, {{ contact.number }}</span>
            <span>{{ contact.neighborhood }}, {{ contact.city }} - {{ contact.state }}</span>
            <span>CEP: {{ contact.cep }}</span>
          </div>
          <div class="detail-item">
            <strong>Observações:</strong>
            <p>{{ contact.observations || 'Nenhuma.' }}</p>
          </div>
          <div class="detail-item">
            <strong>Data de Criação:</strong> {{ new Date(contact.createdAt).toLocaleDateString('pt-BR') }}
          </div>
        </div>

        <div class="right-column">
          <div class="interactions-panel">
            <h3>Histórico de Interações</h3>
            <ul v-if="contact.interactions && contact.interactions.length > 0" class="interaction-list">
              <li v-for="interaction in contact.interactions" :key="interaction.id" class="interaction-item">
                <div class="interaction-header">
                  <strong>{{ interaction.type }}</strong>
                  <span class="interaction-date">{{ new Date(interaction.createdAt).toLocaleString('pt-BR') }}</span>
                </div>
                <p class="interaction-notes">{{ interaction.notes }}</p>
              </li>
            </ul>
            <p v-else>Nenhuma interação registrada.</p>
          </div>

          <TaskList
            :tasks="tasks"
            @add-task="addTask"
            @delete-task="deleteTask"
            @toggle-complete="toggleTaskComplete"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import apiService from '../services/apiService';
import TaskList from '../components/Task/TaskList.vue';

const route = useRoute();
const contact = ref(null);
const tasks = ref([]);
const loading = ref(true);
const error = ref('');

onMounted(() => {
  const contactId = route.params.id;
  fetchContactDetails(contactId);
});

async function fetchContactDetails(contactId) {
  loading.value = true;
  error.value = '';
  try {
    const contactResponse = await apiService.getContactById(contactId);
    contact.value = contactResponse.data;
    await fetchTasks(contactId); // Busca as tarefas depois de carregar o contato
  } catch (err) {
    error.value = "Não foi possível carregar o contato.";
    console.error(err);
  } finally {
    loading.value = false;
  }
}

const fetchTasks = async (contactId) => {
  try {
    const allTasksResponse = await apiService.getTasksForContact(contactId);
    // Filtra no frontend para mostrar apenas as tarefas deste contato
    tasks.value = allTasksResponse.data.filter(t => t.contactId == contactId);
  } catch (err) {
    console.error("Erro ao buscar tarefas:", err);
  }
};

const addTask = async (taskData) => {
  const contactId = contact.value.id;
  try {
    await apiService.createTask({ ...taskData, contactId });
    await fetchTasks(contactId); // Recarrega a lista para pegar a nova tarefa
  } catch (err) {
    console.error("Erro ao adicionar tarefa:", err);
  }
};

const deleteTask = async (taskId) => {
  try {
    await apiService.deleteTask(taskId);
    tasks.value = tasks.value.filter(t => t.id !== taskId);
  } catch (err) {
    console.error("Erro ao deletar tarefa:", err);
  }
};

const toggleTaskComplete = async (task) => {
  try {
    const updatedTaskData = { ...task, isCompleted: !task.isCompleted };
    const response = await apiService.updateTask(task.id, updatedTaskData);
    // Atualiza a tarefa na lista local para reatividade instantânea
    const index = tasks.value.findIndex(t => t.id === task.id);
    if (index !== -1) {
      tasks.value[index] = response.data;
    }
  } catch (err) {
    console.error("Erro ao atualizar tarefa:", err);
  }
};

const getStatusClass = (status) => {
  if (status === 'Convertido') return 'status-converted';
  if (status === 'Oportunidade') return 'status-opportunity';
  return 'status-lead';
};
</script>

<style scoped>
.contact-detail-page {
  padding: 2rem 3rem;
  background-color: var(--bg-color);
  min-height: calc(100vh - 120px); /* Altura da tela menos navbar e footer */
}
.loading, .error {
  text-align: center;
  padding: 2rem;
  font-size: 1.2rem;
  color: var(--text-color-muted);
}
.page-header {
  display: flex;
  align-items: center;
  gap: 1.5rem;
  margin-bottom: 2rem;
}
.page-header h1 {
  color: var(--text-color);
  margin: 0;
}
.back-link {
  color: var(--primary-color);
  text-decoration: none;
  font-weight: bold;
  font-size: 16px;
}
.details-grid {
  display: grid;
  grid-template-columns: 1fr 1.5fr; /* Coluna da direita um pouco maior */
  gap: 2rem;
  align-items: start;
}
.details-panel, .interactions-panel {
  background-color: var(--ui-bg);
  border: 1px solid var(--ui-border);
  padding: 1.5rem 2rem;
  border-radius: 12px;
}
.detail-item {
  margin-bottom: 1.2rem;
  color: var(--text-color);
  font-size: 14px;
}
.detail-item strong {
  display: block;
  color: var(--text-color-muted);
  font-size: 12px;
  margin-bottom: 4px;
}
.detail-item.address span {
  display: block;
}
.detail-item p {
  margin: 0;
}
.right-column {
  display: flex;
  flex-direction: column;
  gap: 2rem;
}
.interactions-panel h3 {
  margin-top: 0;
  color: var(--text-color);
}
.interaction-list {
  list-style: none;
  padding: 0;
  margin: 0;
}
.interaction-item {
  border-bottom: 1px solid var(--ui-border);
  padding: 1rem 0;
}
.interaction-item:first-child {
  padding-top: 0;
}
.interaction-item:last-child {
  border-bottom: none;
  padding-bottom: 0;
}
.interaction-header {
  display: flex;
  justify-content: space-between;
  margin-bottom: 0.5rem;
  color: var(--text-color);
}
.interaction-date {
  font-size: 12px;
  color: var(--text-color-muted);
}
.interaction-notes {
  margin: 0;
  color: var(--text-color);
  white-space: pre-wrap; /* Preserva quebras de linha nas anotações */
}
.status-badge {
  padding: 5px 12px;
  border-radius: 999px;
  font-size: 12px;
  font-weight: bold;
  color: #fff;
}
.status-lead { background-color: #007bff; }
.status-opportunity { background-color: #ffc107; color: #333; }
.status-converted { background-color: #28a745; }
</style>
