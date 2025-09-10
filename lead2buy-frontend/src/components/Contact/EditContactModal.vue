<template>
  <div class="modal-overlay" @click.self="$emit('close')">
    <div class="modal-content">
      <h3>Editar Contato</h3>
      <form @submit.prevent="submitForm">
        <div class="form-group">
          <label for="name">Nome</label>
          <input type="text" v-model="editableContact.name" required />
        </div>
        <div class="form-group">
          <label for="phoneNumber">Telefone</label>
          <input type="text" v-model="editableContact.phoneNumber" required />
        </div>
        <div class="form-group">
          <label for="email">Email</label>
          <input type="email" v-model="editableContact.email" />
        </div>
        <div class="form-group">
          <label for="source">Origem</label>
          <input type="text" v-model="editableContact.source" />
        </div>
        <div class="form-group">
          <label for="status">Status</label>
          <select v-model="editableContact.status">
            <option value="Lead">Lead</option>
            <option value="Oportunidade">Oportunidade</option>
            <option value="Convertido">Convertido</option>
          </select>
        </div>

        <p v-if="errorMessage" class="error-message">{{ errorMessage }}</p>

        <div class="form-actions">
          <button type="button" class="btn-cancel" @click="$emit('close')">
            <i class="fas fa-times"></i> Cancelar
          </button>
          <button type="submit" class="btn-submit">
            <i class="fas fa-save"></i> Salvar Alterações
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref, watch } from 'vue';
import apiService from '@/services/apiService';

const props = defineProps({
  contact: {
    type: Object,
    required: true
  }
});

const emit = defineEmits(['close', 'contact-updated']);

const editableContact = ref({});
const errorMessage = ref('');

watch(() => props.contact, (newVal) => {
  editableContact.value = { ...newVal };
}, { immediate: true });

const submitForm = async () => {
  try {
    await apiService.updateContact(editableContact.value.id, editableContact.value);
    emit('contact-updated');
    emit('close');
  } catch (error) {
    errorMessage.value = 'Falha ao atualizar o contato.';
    console.error('Erro ao atualizar contato:', error);
  }
};
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.7);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.modal-content {
  /* Usando a sua variável para painéis e modais */
  background: var(--ui-bg);
  /* Usando a sua variável de texto principal */
  color: var(--text-color);
  padding: 2rem;
  border-radius: 12px;
  width: 100%;
  max-width: 500px;
  /* Usando a sua variável para bordas */
  border: 1px solid var(--ui-border);
}

h3 {
  /* Usando a sua variável de texto principal (títulos) */
  color: var(--text-color);
  margin-top: 0;
}

.form-group {
  margin-bottom: 1rem;
}

label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 600;
  color: var(--text-color);
}

input, textarea, select {
  width: 100%;
  padding: 10px;
  /* Usando a sua variável para bordas */
  border: 1px solid var(--ui-border);
  /* Usando a sua variável para o fundo principal */
  background-color: var(--bg-color);
  /* Usando a sua variável de texto principal */
  color: var(--text-color);
  border-radius: 8px;
  box-sizing: border-box;
}

textarea {
  resize: vertical;
  min-height: 80px;
}

.form-actions {
  margin-top: 1.5rem;
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
}

.btn-cancel {
  padding: 10px 20px;
  /* Usando a sua variável para bordas */
  border: 1px solid var(--ui-border);
  background-color: transparent;
  color: var(--text-color);
  border-radius: 8px;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.btn-submit {
  padding: 10px 20px;
  border: none;
  /* Usando a sua variável de cor primária */
  background-color: var(--primary-color);
  color: white;
  border-radius: 8px;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.error-message {
  color: #e74c3c;
  margin-top: 1rem;
  text-align: center;
}
</style>
