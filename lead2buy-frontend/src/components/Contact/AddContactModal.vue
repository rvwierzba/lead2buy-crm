<template>
  <div class="modal-overlay" @click.self="$emit('close')">
    <div class="modal-content">
      <h3>Adicionar Novo Contato</h3>
      <form @submit.prevent="submitForm">
        <div class="form-group">
          <label for="name">Nome</label>
          <input type="text" v-model="contact.name" required />
        </div>
        <div class="form-group">
          <label for="phoneNumber">Telefone</label>
          <input type="text" v-model="contact.phoneNumber" required />
        </div>
        <div class="form-group">
          <label for="email">Email</label>
          <input type="email" v-model="contact.email" />
        </div>
        <div class="form-group">
          <label for="source">Origem</label>
          <input type="text" v-model="contact.source" />
        </div>

        <p v-if="errorMessage" class="error-message">{{ errorMessage }}</p>

        <div class="form-actions">
          <button type="button" class="btn-cancel" @click="$emit('close')">Cancelar</button>
          <button type="submit" class="btn-submit">Salvar Contato</button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import apiService from '../../services/apiService';

// Define os eventos que este componente pode emitir
const emit = defineEmits(['close', 'contact-added']);

const contact = ref({
  name: '',
  phoneNumber: '',
  email: '',
  source: ''
});
const errorMessage = ref('');

const submitForm = async () => {
  try {
    const response = await apiService.createContact(contact.value);
    // Emite o evento 'contact-added' com o novo contato como dado
    emit('contact-added', response.data);
    // Emite o evento para fechar o modal
    emit('close');
  } catch (error) {
    errorMessage.value = 'Ocorreu um erro ao salvar o contato.';
    console.error("Erro ao criar contato:", error);
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
  background-color: rgba(0, 0, 0, 0.7); /* Fundo mais escuro para o overlay */
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}
.modal-content {
  background: var(--ui-bg); /* Usa a cor de fundo para painéis */
  color: var(--text-color); /* Usa a cor de texto do tema */
  padding: 2rem;
  border-radius: 12px;
  width: 100%;
  max-width: 500px;
  border: 1px solid var(--ui-border);
}
h3 {
    color: var(--text-color); /* Garante que o título use a cor do tema */
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
input, select {
  width: 100%;
  padding: 10px;
  border: 1px solid var(--ui-border);
  background-color: var(--bg-color); /* Fundo dos inputs */
  color: var(--text-color); /* Cor do texto dos inputs */
  border-radius: 8px;
}
.form-actions {
  margin-top: 1.5rem;
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
}
.btn-cancel {
  padding: 10px 20px;
  border: 1px solid var(--ui-border);
  background-color: transparent;
  color: var(--text-color);
  border-radius: 8px;
  cursor: pointer;
}
.btn-submit {
  padding: 10px 20px;
  border: none;
  background-color: var(--primary-color);
  color: white;
  border-radius: 8px;
  cursor: pointer;
}
.error-message {
  color: red;
  margin-top: 1rem;
}
</style>
