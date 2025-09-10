<template>
  <div class="modal-overlay" @click.self="$emit('close')">
    <div class="modal-content">
      <h3>{{ isEditMode ? 'Editar Contato' : 'Adicionar Contato' }}</h3>
      <form @submit.prevent="submitForm">
        <div class="form-group">
          <label for="name">Nome</label>
          <input type="text" id="name" v-model="form.name" required>
        </div>
        <div class="form-group">
          <label for="email">Email</label>
          <input type="email" id="email" v-model="form.email">
        </div>
        <div class="form-group">
          <label for="phoneNumber">Telefone</label>
          <input type="tel" id="phoneNumber" v-model="form.phoneNumber">
        </div>
        <div class="form-group">
          <label for="whatsAppNumber">WhatsApp</label>
          <input type="tel" id="whatsAppNumber" v-model="form.whatsAppNumber">
        </div>
        <div class="form-group">
          <label for="linkedInUrl">LinkedIn</label>
          <input type="url" id="linkedInUrl" v-model="form.linkedInUrl">
        </div>
        <div class="form-group">
          <label for="facebookUrl">Facebook</label>
          <input type="url" id="facebookUrl" v-model="form.facebookUrl">
        </div>
        <div class="form-group">
          <label for="youTubeChannelUrl">YouTube</label>
          <input type="url" id="youTubeChannelUrl" v-model="form.youTubeChannelUrl">
        </div>
        <div class="form-group">
          <label for="notes">Notas</label>
          <textarea id="notes" v-model="form.notes"></textarea>
        </div>
        <div class="form-actions">
          <button type="button" class="btn-cancel" @click="$emit('close')">
            <i class="fas fa-times"></i> Cancelar
          </button>
          <button type="submit" class="btn-submit">
            <i class="fas fa-save"></i> Salvar
          </button>
        </div>
        <p v-if="errorMessage" class="error-message">{{ errorMessage }}</p>
      </form>
    </div>
  </div>
</template>


<script setup>
import { ref, watchEffect, computed } from 'vue';
import apiService from '@/services/apiService';

const props = defineProps({
  isOpen: Boolean,
  contact: Object
});

const emit = defineEmits(['close', 'saved']);

const form = ref({});
const isSaving = ref(false);
const isEditing = computed(() => props.contact && props.contact.id);

// watchEffect reage às dependências dentro dele (props.contact).
// Ele executa imediatamente e sempre que o 'props.contact' mudar.
watchEffect(() => {
  // Preenche o formulário com os dados do contato ou com strings vazias
  form.value = {
    name: props.contact?.name || '',
    email: props.contact?.email || '',
    phoneNumber: props.contact?.phoneNumber || '',
    whatsAppNumber: props.contact?.whatsAppNumber || '',
    linkedInUrl: props.contact?.linkedInUrl || '',
    facebookUrl: props.contact?.facebookUrl || '',
    youTubeChannelUrl: props.contact?.youTubeChannelUrl || '',
    notes: props.contact?.notes || ''
  };
});

const saveContact = async () => {
  isSaving.value = true;

  // Prepara uma cópia dos dados do formulário para serem enviados
  const payload = { ...form.value };

  // Transforma campos de URL vazios em null
  if (payload.facebookUrl === '') {
    payload.facebookUrl = null;
  }
  if (payload.youTubeChannelUrl === '') {
    payload.youTubeChannelUrl = null;
  }
  // O mesmo pode ser feito para outros campos de URL, se necessário
  if (payload.linkedInUrl === '') {
    payload.linkedInUrl = null;
  }

  try {
    if (isEditing.value) {
      // Envia o payload corrigido
      await apiService.updateNetworkContact(props.contact.id, payload);
    } else {
      // Envia o payload corrigido
      await apiService.createNetworkContact(payload);
    }
    emit('saved');
    emit('close');
  } catch (error) {
    console.error("Erro ao salvar contato:", error);
    // Idealmente, mostrar um alerta de erro para o usuário aqui
  } finally {
    isSaving.value = false;
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
