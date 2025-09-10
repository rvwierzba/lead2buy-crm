<template>
  <div class="network-view">
    <header class="view-header">
      <h1><i class="fas fa-network-wired"></i> Minha Rede</h1>
      <button @click="openModal(null)" class="add-btn">
        <i class="fas fa-plus"></i> Adicionar Contato
      </button>
    </header>

    <div class="contact-grid">
      <div v-for="contact in contacts" :key="contact.id" class="contact-card">
        <h3>{{ contact.name }}</h3>
        <p v-if="contact.email"><i class="fas fa-envelope"></i> {{ contact.email }}</p>
        <p v-if="contact.phoneNumber"><i class="fas fa-phone"></i> {{ contact.phoneNumber }}</p>

        <SocialLinks :contact="contact" />

        <div class="card-actions">
          <button @click="openModal(contact)" class="action-btn edit-btn">
            <i class="fas fa-edit"></i>
          </button>
          <button @click="confirmDelete(contact)" class="action-btn delete-btn">
            <i class="fas fa-trash"></i>
          </button>
        </div>
      </div>
    </div>

    <NetworkContactModal
      v-if="isModalVisible"
      :contact="selectedContact"
      @close="closeModal"
      @saved="fetchContacts"
    />

    <ConfirmModal
      v-if="isConfirmModalVisible"
      title="Confirmar Exclusão"
      message="Tem certeza que deseja excluir este contato?"
      @confirm="deleteContact"
      @cancel="closeConfirmModal"
    />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import apiService from '@/services/apiService';
import NetworkContactModal from '@/components/Network/NetworkContactModal.vue';
import ConfirmModal from '@/components/ConfirmModal.vue';
import SocialLinks from '@/components/Network/SocialLinks.vue';

const contacts = ref([]);
// VARIÁVEIS DE CONTROLE QUE ESTAVAM FALTANDO
const isModalVisible = ref(false);
const selectedContact = ref(null);
const isConfirmModalVisible = ref(false);
const contactToDelete = ref(null);

// FUNÇÕES DE CONTROLE QUE ESTAVAM FALTANDO
const openModal = (contact) => {
  selectedContact.value = contact;
  isModalVisible.value = true;
};

const closeModal = () => {
  isModalVisible.value = false;
  selectedContact.value = null;
};

const openConfirmModal = (contact) => {
  contactToDelete.value = contact;
  isConfirmModalVisible.value = true;
};

const closeConfirmModal = () => {
  isConfirmModalVisible.value = false;
  contactToDelete.value = null;
};

const fetchContacts = async () => {
  try {
    const response = await apiService.getNetworkContacts();
    contacts.value = response.data;
  } catch (error) {
    console.error('Erro ao buscar contatos da rede:', error);
  }
};

const confirmDelete = (contact) => {
  openConfirmModal(contact);
};

const deleteContact = async () => {
  try {
    await apiService.deleteNetworkContact(contactToDelete.value.id);
    fetchContacts(); // Atualiza a lista após deletar
  } catch (error) {
    console.error('Erro ao deletar contato:', error);
  } finally {
    closeConfirmModal();
  }
};

onMounted(fetchContacts);
</script>

<style scoped>
/* Seu CSS continua o mesmo, não precisa mudar nada aqui */
.network-view {
  padding: 2rem;
}
.view-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}
.view-header h1 {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  color: var(--color-heading);
}
.add-btn {
  background-color: var(--primary-color);
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 8px;
  cursor: pointer;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}
.contact-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 1.5rem;
}
.contact-card {
  background: var(--color-background-soft);
  border-radius: 12px;
  padding: 1.5rem;
  border: 1px solid var(--color-border);
  display: flex;
  flex-direction: column;
}
.contact-card h3 {
  margin-top: 0;
  color: var(--color-heading);
}
.contact-card p {
  margin: 0.25rem 0;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: var(--color-text-light);
}
.card-actions {
  margin-top: auto;
  padding-top: 1rem;
  display: flex;
  justify-content: flex-end;
  gap: 0.5rem;
}
.action-btn {
  background: none;
  border: 1px solid var(--color-border);
  color: var(--color-text-light);
  width: 36px;
  height: 36px;
  border-radius: 50%;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
}
.action-btn:hover {
  background-color: var(--color-background-mute);
  color: var(--color-text);
}
</style>
