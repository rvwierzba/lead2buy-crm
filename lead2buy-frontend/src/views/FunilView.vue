<template>
  <div class="funnel-page">
    <header class="page-header">
      <h1>Funil de Vendas</h1>
    </header>
    <div class="funnel-board">
        <FunilColuna
            title="Lead"
            :contacts="leads"
            @drag-end="handleDragEnd"
        />
        <FunilColuna
            title="Oportunidade"
            :contacts="opportunities"
            @drag-end="handleDragEnd"
        />
        <FunilColuna
            title="Convertido"
            :contacts="converted"
            @drag-end="handleDragEnd"
         />
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import apiService from '../services/apiService';
import FunilColuna from '../components/Funil/FunilColuna.vue';

const leads = ref([]);
const opportunities = ref([]);
const converted = ref([]);
const loading = ref(true);

// Mapeamento dos nomes das colunas para os status da API
const statusMap = {
  Lead: leads,
  Oportunidade: opportunities,
  Convertido: converted
};

async function fetchAllContacts() {
  loading.value = true;
  try {
    const response = await apiService.getContacts();
    const allContacts = response.data;

    // Separa os contatos em listas baseadas no status
    leads.value = allContacts.filter(c => c.status === 'Lead');
    opportunities.value = allContacts.filter(c => c.status === 'Oportunidade');
    converted.value = allContacts.filter(c => c.status === 'Convertido');
  } catch (error) {
    console.error("Erro ao buscar contatos para o funil:", error);
  } finally {
    loading.value = false;
  }
}

onMounted(fetchAllContacts);

// A LÓGICA PRINCIPAL ESTÁ AQUI
const handleDragEnd = async (event) => {
  // Pega o ID do contato que foi movido
  const contactId = event.item.getAttribute('data-id');

  // Descobre para qual coluna (status) o card foi movido
  const toColumnTitle = event.to.parentElement.querySelector('.column-title').textContent.split(' ')[0];

  // Encontra o objeto completo do contato que foi movido
  let movedContact = findContactInAllLists(parseInt(contactId));

  if (movedContact && movedContact.status !== toColumnTitle) {
    console.log(`Movendo contato ${contactId} para o status: ${toColumnTitle}`);

    // Cria uma cópia atualizada do contato com o novo status
    const updatedContact = { ...movedContact, status: toColumnTitle };

    try {
      // Chama a API para salvar a mudança no banco de dados
      await apiService.updateContact(contactId, updatedContact);
      console.log(`Contato ${contactId} atualizado com sucesso!`);

      // Para garantir consistência, podemos re-buscar todos os contatos
      // ou apenas atualizar o estado localmente. Re-buscar é mais seguro.
      await fetchAllContacts();

    } catch (error) {
      console.error("Erro ao atualizar o status do contato:", error);
      // Em caso de erro, reverta a mudança visual recarregando os contatos
      await fetchAllContacts();
    }
  }
};

// Função auxiliar para encontrar um contato em qualquer uma das listas
function findContactInAllLists(contactId) {
    const allLists = [...leads.value, ...opportunities.value, ...converted.value];
    return allLists.find(c => c.id === contactId);
}
</script>

<style scoped>
.funnel-page {
  padding: 2rem;
}
.page-header h1 {
    color: var(--text-color);
}
.funnel-board {
  display: flex;
  gap: 1.5rem;
  overflow-x: auto; /* Permite scroll horizontal se as colunas não couberem */
  padding-bottom: 1rem;
}
</style>
