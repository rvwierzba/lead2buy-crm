<template>
  <div class="contacts-page">
    <header class="page-header">
      <h1>Meus Contatos</h1>
      <div class="header-actions">
        <button @click="handleDownloadTemplate" class="action-btn template-btn">Baixar Modelo</button>

        <button @click="triggerFileInput" class="action-btn import-btn">Importar CSV</button>
        <input type="file" ref="fileInput" @change="handleFileUpload" style="display: none" accept=".csv" />

        <button @click="handleExport" class="action-btn export-btn">Exportar CSV</button>

        <button @click="showAddModal = true" class="action-btn add-contact-btn">+ Adicionar Contato</button>
      </div>
    </header>

    <div class="filters-container">
      <div class="filter-group">
        <label for="search">Buscar por Nome ou Telefone</label>
        <input
          type="text"
          id="search"
          v-model="searchTerm"
          placeholder="Digite para buscar..."
        />
      </div>
      <div class="filter-group">
        <label for="status">Filtrar por Status</label>
        <select id="status" v-model="statusFilter">
          <option value="">Todos</option>
          <option value="Lead">Lead</option>
          <option value="Oportunidade">Oportunidade</option>
          <option value="Convertido">Convertido</option>
        </select>
      </div>
    </div>

    <ContactList
      :contacts="contacts"
      @contact-deleted="handleContactDeleted"
      @open-edit-modal="openEditModal"
    />

    <AddContactModal
      v-if="showAddModal"
      @close="showAddModal = false"
      @contact-added="handleContactAdded"
    />

    <EditContactModal
      v-if="showEditModal"
      :contact="contactToEdit"
      @close="showEditModal = false"
      @contact-updated="handleContactUpdated"
    />
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue';
import apiService from '../services/apiService';
import ContactList from '../components/Contact/ContactList.vue';
import AddContactModal from '../components/Contact/AddContactModal.vue';
import EditContactModal from '../components/Contact/EditContactModal.vue';

const contacts = ref([]);
const showAddModal = ref(false);
const fileInput = ref(null);

const showEditModal = ref(false);
const contactToEdit = ref(null);

function openEditModal(contact) {
  contactToEdit.value = contact;
  showEditModal.value = true;
}

function handleContactUpdated(updatedContact) {
  const index = contacts.value.findIndex(c => c.id === updatedContact.id);
  if (index !== -1) {
    contacts.value[index] = updatedContact;
  }
}

const searchTerm = ref('');
const statusFilter = ref('');

const fetchContacts = async () => {
  try {
    const response = await apiService.getContacts(searchTerm.value, statusFilter.value);
    contacts.value = response.data;
  } catch (error) {
    console.error("Erro ao buscar contatos:", error);
  }
};

const handleContactAdded = () => {
  fetchContacts();
};

const handleContactDeleted = async (contactId) => {
  try {
    await apiService.deleteContact(contactId);
    contacts.value = contacts.value.filter(c => c.id !== contactId);
  } catch (error) {
    console.error("Erro ao deletar contato:", error);
  }
};

function triggerFileInput() {
  fileInput.value.click();
}

async function handleFileUpload(event) {
  const file = event.target.files[0];
  if (!file) return;

  try {
    const response = await apiService.importContacts(file);
    alert(response.data.message);
    fetchContacts();
  } catch (error) {
    const errorMessage = error.response?.data?.message || 'Ocorreu um erro na importação.';
    const validationErrors = error.response?.data?.errors?.join('\n') || '';
    alert(`${errorMessage}\n${validationErrors}`);
    console.error("Erro ao importar contatos:", error);
  }
  event.target.value = '';
}

async function handleExport() {
  try {
    const response = await apiService.exportContacts();
    const url = window.URL.createObjectURL(new Blob([response.data]));
    const link = document.createElement('a');
    link.href = url;

    const contentDisposition = response.headers['content-disposition'];
    let fileName = 'contatos.csv';
    if (contentDisposition) {
        const fileNameMatch = contentDisposition.match(/filename="(.+)"/);
        if (fileNameMatch && fileNameMatch.length === 2)
            fileName = fileNameMatch[1];
    }

    link.setAttribute('download', fileName);
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  } catch (error) {
    alert('Ocorreu um erro na exportação.');
    console.error("Erro ao exportar contatos:", error);
  }
}

async function handleDownloadTemplate() {
  try {
    const response = await apiService.downloadImportTemplate();
    const url = window.URL.createObjectURL(new Blob([response.data]));
    const link = document.createElement('a');
    link.href = url;
    link.setAttribute('download', 'modelo_importacao_contatos.csv');
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  } catch (error) {
    alert('Ocorreu um erro ao baixar o modelo.');
    console.error("Erro ao baixar modelo:", error);
  }
}

watch(searchTerm, fetchContacts);
watch(statusFilter, fetchContacts);

onMounted(fetchContacts);
</script>

<style scoped>
.contacts-page {
  padding: 2rem 3rem;
}
.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}
.page-header h1 {
  color: var(--text-color);
  font-size: 28px;
}
.header-actions {
  display: flex;
  gap: 1rem;
}
.action-btn {
  padding: 12px 20px;
  border: 1px solid var(--ui-border);
  border-radius: 8px;
  font-size: 14px;
  font-weight: bold;
  cursor: pointer;
  transition: background-color 0.2s ease, opacity 0.2s ease;
}
.action-btn:hover {
  opacity: 0.9;
}
.template-btn {
  background-color: #6c757d;
  color: white;
  border-color: #6c757d;
}
.import-btn {
  background-color: #17a2b8;
  color: white;
  border-color: #17a2b8;
}
.export-btn {
  background-color: #28a745;
  color: white;
  border-color: #28a745;
}
.add-contact-btn {
  background-color: var(--primary-color);
  color: white;
  border: none;
  font-size: 16px;
  padding: 12px 20px;
  font-weight: bold;
  cursor: pointer;
  transition: background-color 0.2s ease;
}
.add-contact-btn:hover {
  background-color: #5C59C8;
}
.filters-container {
  display: flex;
  gap: 1.5rem;
  background-color: var(--ui-bg);
  padding: 1.5rem;
  border-radius: 12px;
  border: 1px solid var(--ui-border);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
  margin-bottom: 2rem;
}
.filter-group {
  flex: 1;
}
.filter-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 600;
  color: var(--text-color);
}
.filter-group input, .filter-group select {
  width: 100%;
  padding: 10px;
  border-radius: 8px;
  background-color: var(--bg-color);
  border: 1px solid var(--ui-border);
  color: var(--text-color);
}
</style>
