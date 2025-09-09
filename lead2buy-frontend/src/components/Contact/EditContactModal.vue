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
          <button type="button" class="btn-cancel" @click="$emit('close')">Cancelar</button>
          <button type="submit" class="btn-submit">Salvar Alterações</button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref, watchEffect } from 'vue';
import apiService from '../../services/apiService';

const props = defineProps({
  contact: {
    type: Object,
    required: true
  }
});

const emit = defineEmits(['close', 'contact-updated']);

const editableContact = ref({});
const errorMessage = ref('');

// watchEffect é usado para copiar os dados do prop para uma variável local
// Isso evita que a gente modifique o dado original diretamente (má prática)
watchEffect(() => {
  // Cria uma cópia do contato para edição
  editableContact.value = { ...props.contact };
});

const submitForm = async () => {
  try {
    const response = await apiService.updateContact(editableContact.value.id, editableContact.value);
    emit('contact-updated', response.data);
    emit('close');
  } catch (error) {
    errorMessage.value = 'Ocorreu um erro ao atualizar o contato.';
    console.error("Erro ao atualizar contato:", error);
  }
};
</script>

<style scoped>

.modal-overlay {
  position: fixed; top: 0; left: 0; width: 100%; height: 100%;
  background-color: rgba(0, 0, 0, 0.6); display: flex;
  justify-content: center; align-items: center; z-index: 1000;
}
.modal-content {
  background: var(--ui-bg, #fff); color: var(--text-color);
  padding: 2rem; border-radius: 12px; width: 100%; max-width: 500px;
}
.form-group { margin-bottom: 1rem; }
label { display: block; margin-bottom: 0.5rem; font-weight: 600; }
input, select {
  width: 100%; padding: 10px; border: 1px solid var(--ui-border, #ccc);
  border-radius: 8px; background-color: var(--bg-color); color: var(--text-color);
}
.form-actions {
  margin-top: 1.5rem; display: flex; justify-content: flex-end; gap: 1rem;
}
.btn-cancel {
  padding: 10px 20px; border: 1px solid #ccc;
  background-color: #f8f9fa; border-radius: 8px; cursor: pointer;
}
.btn-submit {
  padding: 10px 20px; border: none; background-color: var(--primary-color);
  color: white; border-radius: 8px; cursor: pointer;
}
.error-message { color: red; margin-top: 1rem; }
</style>
