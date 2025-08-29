<template>
  <div class="contact-list-container">
    <h2>Lista de Contatos</h2>
    <table class="contact-table">
      <thead>
        <tr>
          <th>Nome</th>
          <th>Telefone</th>
          <th>Email</th>
          <th>Status</th>
          <th>Ações</th>
        </tr>
      </thead>
      <tbody>
        <tr v-if="contacts.length === 0">
          <td colspan="5">Nenhum contato encontrado.</td>
        </tr>
        <tr v-for="contact in contacts" :key="contact.id">
          <td>{{ contact.name }}</td>
          <td>{{ contact.phoneNumber }}</td>
          <td>{{ contact.email }}</td>
          <td>
            <span :class="['status-badge', getStatusClass(contact.status)]">
              {{ contact.status }}
            </span>
          </td>
          <td>
            <router-link :to="{ name: 'contact-detail', params: { id: contact.id } }" class="action-btn view">
              Ver
            </router-link>
            <button @click="$emit('open-edit-modal', contact)" class="action-btn edit">Editar</button>
            <button @click="openDeleteModal(contact)" class="action-btn delete">Excluir</button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>

  <ConfirmModal
    v-if="showDeleteModal"
    title="Confirmar Exclusão"
    :message="`Tem certeza que deseja excluir o contato '${contactToDelete?.name}'?`"
    @close="showDeleteModal = false"
    @confirm="deleteConfirmed"
  />
</template>

<script setup>
import { ref } from 'vue';
import ConfirmModal from '../ConfirmModal.vue';

// Define os dados que o componente recebe do "pai" (ContactsView)
const props = defineProps({
  contacts: { type: Array, required: true }
});

// Define todos os eventos que este componente pode "emitir" para o "pai"
const emit = defineEmits(['contact-deleted', 'open-edit-modal']);

// --- Lógica para o Modal de Exclusão ---
const showDeleteModal = ref(false);
const contactToDelete = ref(null);

function openDeleteModal(contact) {
  contactToDelete.value = contact;
  showDeleteModal.value = true;
}

function deleteConfirmed() {
  if (contactToDelete.value) {
    // Emite o evento para avisar a ContactsView que o usuário confirmou a exclusão
    emit('contact-deleted', contactToDelete.value.id);
  }
  showDeleteModal.value = false;
}

// --- Lógica para os Status (continua a mesma) ---
const getStatusClass = (status) => {
  if (status === 'Convertido') return 'status-converted';
  if (status === 'Oportunidade') return 'status-opportunity';
  return 'status-lead';
};
</script>

<style scoped>
.contact-list-container {
  background-color: var(--ui-bg);
  padding: 2rem;
  border-radius: 12px;
  border: 1px solid var(--ui-border);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
  transition: all 0.3s ease;
  overflow-x: auto;
}
.contact-list-container h2 {
  color: var(--text-color);
  margin-top: 0;
  margin-bottom: 1rem;
}
.contact-table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 1rem;
}
th, td {
  padding: 12px 15px;
  text-align: left;
  border-bottom: 1px solid var(--ui-border);
  color: var(--text-color);
  white-space: nowrap;
}
th {
  background-color: var(--bg-color);
  font-weight: 600;
  font-size: 14px;
}
tbody tr:hover {
  background-color: rgba(0,0,0,0.02);
}
.dark-mode tbody tr:hover {
  background-color: rgba(255,255,255,0.05);
}
.status-badge {
  padding: 5px 12px;
  border-radius: 999px;
  font-size: 12px;
  font-weight: bold;
  color: #fff;
  text-shadow: 1px 1px 1px rgba(0,0,0,0.1);
}
.status-lead { background-color: #007bff; }
.status-opportunity { background-color: #ffc107; color: #333; }
.status-converted { background-color: #28a745; }

.action-btn {
  padding: 6px 12px;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  margin-right: 5px;
  color: #fff;
  font-weight: 500;
  transition: opacity 0.2s ease;
  text-decoration: none;
}
.action-btn:hover {
  opacity: 0.8;
}
.view { background-color: #17a2b8; }
.edit { background-color: #ffc107; color: #333; }
.delete { background-color: #dc3545; }
</style>
