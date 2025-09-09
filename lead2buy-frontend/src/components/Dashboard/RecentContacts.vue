<template>
  <div class="card">
    <h3 class="card-header">Últimos Contatos Adicionados</h3>
    <div v-if="loading" class="loading-text">Carregando...</div>
    <div v-else-if="error" class="error-text">{{ error }}</div>
    <ul v-else-if="contacts.length > 0" class="content-list">
      <li v-for="contact in contacts" :key="contact.id" class="list-item">
        <div class="flex items-center">
          <div class="avatar">
            {{ contact.name.charAt(0).toUpperCase() }}
          </div>
          <div>
            <p class="item-title">{{ contact.name }}</p>
            <p class="item-subtitle">{{ contact.email }}</p>
          </div>
        </div>
        <router-link :to="`/contacts/${contact.id}`" class="item-link">
          Ver
        </router-link>
      </li>
    </ul>
    <div v-else class="empty-text">Nenhum contato recente.</div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import apiService from '@/services/apiService';

const contacts = ref([]);
const loading = ref(true);
const error = ref(null);

onMounted(async () => {
  try {
    const response = await apiService.getRecentContacts();
    contacts.value = response.data;
  } catch (err) {
    console.error("Erro ao buscar contatos recentes:", err);
    error.value = "Não foi possível carregar os contatos.";
  } finally {
    loading.value = false;
  }
});
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
  gap: 1rem;
}
.list-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
}
.avatar {
  width: 2.5rem;
  height: 2.5rem;
  border-radius: 9999px;
  background-color: #3b82f6;
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: bold;
  margin-right: 1rem;
}
.item-title {
  font-weight: 600;
  color: var(--color-text);
}
.item-subtitle {
  font-size: 0.875rem;
  color: var(--color-text-mute);
}
.item-link {
  color: #3b82f6;
  font-weight: 500;
  transition: color 0.2s ease;
}
.item-link:hover {
  color: #2563eb;
}
.dark .item-link:hover {
  color: #60a5fa;
}
</style>
