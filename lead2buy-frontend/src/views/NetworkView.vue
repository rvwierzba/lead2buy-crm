<template>
  <div class="page-container">
    <header class="page-header">
      <div>
        <h1 class="page-title">Rede de Contatos</h1>
        <p class="page-subtitle">Gerencie seus contatos de networking profissionais.</p>
      </div>
      <button @click="openModal()" class="btn-primary">
        <PlusIcon class="h-5 w-5 mr-2" />
        Adicionar Contato
      </button>
    </header>

    <div v-if="loading" class="loading-text">Carregando contatos...</div>
    <div v-else-if="error" class="error-text">{{ error }}</div>

    <div v-else class="table-container">
      <table class="data-table">
        <thead>
          <tr>
            <th>Nome</th>
            <th>Email</th>
            <th>Telefone</th>
            <th>Redes Sociais</th>
            <th>Ações</th>
          </tr>
        </thead>
        <tbody v-if="contacts.length > 0">
          <tr v-for="contact in contacts" :key="contact.id">
            <td>{{ contact.name }}</td>
            <td>{{ contact.email || '-' }}</td>
            <td>{{ contact.phoneNumber || '-' }}</td>
            <td><SocialLinks :contact="contact" /></td>
            <td>
              <div class="action-buttons">
                <button @click="openModal(contact)" class="btn-icon">
                  <PencilIcon class="h-5 w-5" />
                </button>
                <button @click="confirmDelete(contact)" class="btn-icon btn-danger">
                  <TrashIcon class="h-5 w-5" />
                </button>
              </div>
            </td>
          </tr>
        </tbody>
        <tbody v-else>
          <tr>
            <td colspan="5" class="empty-text">Nenhum contato na sua rede ainda.</td>
          </tr>
        </tbody>
      </table>
    </div>

    <NetworkContactModal
      :is-open="isModalOpen"
      :contact="selectedContact"
      @close="closeModal"
      @saved="fetchContacts"
    />

    <ConfirmModal
      :is-open="isConfirmModalOpen"
      title="Confirmar Exclusão"
      message="Você tem certeza que deseja excluir este contato da sua rede? Esta ação não pode ser desfeita."
      @confirm="deleteContact"
      @cancel="closeConfirmModal"
    />
  </div>
</template>

<script setup>
// ... (script sem alterações)
import { ref, onMounted } from 'vue';
import apiService from '@/services/apiService';
import NetworkContactModal from '@/components/Network/NetworkContactModal.vue';
import SocialLinks from '@/components/Network/SocialLinks.vue';
import ConfirmModal from '@/components/ConfirmModal.vue';
import { PlusIcon, PencilIcon, TrashIcon } from '@heroicons/vue/24/outline';

const contacts = ref([]);
const loading = ref(true);
const error = ref(null);
const isModalOpen = ref(false);
const selectedContact = ref(null);
const isConfirmModalOpen = ref(false);
const contactToDelete = ref(null);

const fetchContacts = async () => {
  loading.value = true;
  try {
    const response = await apiService.getNetworkContacts();
    contacts.value = response.data;
  } catch (err) {
    console.error("Erro ao buscar contatos da rede:", err);
    error.value = "Não foi possível carregar os contatos.";
  } finally {
    loading.value = false;
  }
};
onMounted(fetchContacts);
const openModal = (contact = null) => {
  selectedContact.value = contact;
  isModalOpen.value = true;
};
const closeModal = () => {
  isModalOpen.value = false;
  selectedContact.value = null;
};
const confirmDelete = (contact) => {
  contactToDelete.value = contact;
  isConfirmModalOpen.value = true;
};
const closeConfirmModal = () => {
  isConfirmModalOpen.value = false;
  contactToDelete.value = null;
};
const deleteContact = async () => {
  if (!contactToDelete.value) return;
  try {
    await apiService.deleteNetworkContact(contactToDelete.value.id);
    await fetchContacts();
  } catch (err) {
    console.error("Erro ao excluir contato:", err);
  } finally {
    closeConfirmModal();
  }
};
</script>

<style scoped>
/* Estilos 100% adaptáveis usando as variáveis do seu tema */
.page-container {
  padding: 2rem;
  background-color: var(--color-background);
}
.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}
.page-title {
  font-size: 2.25rem;
  font-weight: 700;
  color: var(--color-heading);
}
.page-subtitle {
  color: var(--color-text-mute);
  margin-top: 0.25rem;
}
.table-container {
  background-color: var(--color-background-soft);
  border: 1px solid var(--color-border);
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
}
.dark .table-container {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}
.data-table {
  width: 100%;
  border-collapse: collapse;
}
.data-table th, .data-table td {
  padding: 1rem 1.5rem;
  text-align: left;
  border-bottom: 1px solid var(--color-border);
}
.data-table th {
  background-color: var(--color-background-mute);
  color: var(--color-text-mute);
  font-size: 0.75rem;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}
.data-table td {
  color: var(--color-text);
}
.empty-text, .loading-text, .error-text {
  text-align: center;
  padding: 2rem;
  color: var(--color-text-mute);
}
.error-text { color: #ef4444; }
.btn-primary {
  display: inline-flex;
  align-items: center;
  background-color: var(--primary-color);
  color: white;
  padding: 0.75rem 1.5rem;
  border-radius: 8px;
  font-weight: 500;
  transition: background-color 0.2s;
}
.btn-primary:hover {
  background-color: var(--primary-color-dark);
}
.action-buttons {
  display: flex;
  gap: 0.5rem;
}
.btn-icon {
  color: var(--color-text-mute);
  transition: color 0.2s;
}
.btn-icon:hover {
  color: var(--primary-color);
}
.btn-danger:hover {
  color: #ef4444;
}
</style>
