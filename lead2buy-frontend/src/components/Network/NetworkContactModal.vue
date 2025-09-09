<template>
  <div v-if="isOpen" class="modal-overlay" @click.self="$emit('close')">
    <div class="modal-content">
      <h2 class="modal-title">{{ isEditing ? 'Editar Contato' : 'Adicionar Contato à Rede' }}</h2>

      <form @submit.prevent="saveContact">
        <div class="form-grid">
          <div class="form-group col-span-2">
            <label for="name">Nome Completo</label>
            <input type="text" id="name" v-model="form.name" required>
          </div>
          <div class="form-group">
            <label for="email">Email</label>
            <input type="email" id="email" v-model="form.email">
          </div>
          <div class="form-group">
            <label for="phone">Telefone</label>
            <input type="tel" id="phone" v-model="form.phoneNumber">
          </div>
          <div class="form-group">
            <label for="whatsapp">WhatsApp</label>
            <input type="tel" id="whatsapp" v-model="form.whatsAppNumber" placeholder="+55 (11) 99999-9999">
          </div>
          <div class="form-group">
            <label for="linkedin">LinkedIn URL</label>
            <input type="url" id="linkedin" v-model="form.linkedInUrl" placeholder="https://linkedin.com/in/seu-perfil">
          </div>
          <div class="form-group">
            <label for="facebook">Facebook URL</label>
            <input type="url" id="facebook" v-model="form.facebookUrl" placeholder="https://facebook.com/seu-perfil">
          </div>
          <div class="form-group">
            <label for="youtube">YouTube URL</label>
            <input type="url" id="youtube" v-model="form.youTubeChannelUrl" placeholder="https://youtube.com/seu-canal">
          </div>
        </div>

        <div class="form-group mt-4">
          <label for="notes">Anotações</label>
          <textarea id="notes" v-model="form.notes" rows="3"></textarea>
        </div>

        <div class="modal-actions">
          <button type="button" @click="$emit('close')" class="btn-secondary">Cancelar</button>
          <button type="submit" class="btn-primary" :disabled="isSaving">
            {{ isSaving ? 'Salvando...' : 'Salvar Contato' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref, watch, computed } from 'vue';
import apiService from '@/services/apiService';

const props = defineProps({
  isOpen: Boolean,
  contact: Object,
});
const emit = defineEmits(['close', 'saved']);
const form = ref({});
const isSaving = ref(false);
const isEditing = computed(() => props.contact && props.contact.id);

watch(() => props.isOpen, (newVal) => {
  if (newVal) {
    form.value = {
      name: props.contact?.name || '',
      email: props.contact?.email || '',
      phoneNumber: props.contact?.phoneNumber || '',
      whatsAppNumber: props.contact?.whatsAppNumber || '',
      linkedInUrl: props.contact?.linkedInUrl || '',
      facebookUrl: props.contact?.facebookUrl || '',
      youTubeChannelUrl: props.contact?.youTubeChannelUrl || '',
      notes: props.contact?.notes || '',
    };
  }
});

const saveContact = async () => {
  isSaving.value = true;
  try {
    if (isEditing.value) {
      await apiService.updateNetworkContact(props.contact.id, form.value);
    } else {
      await apiService.createNetworkContact(form.value);
    }
    emit('saved');
    emit('close');
  } catch (error) {
    console.error("Erro ao salvar contato:", error);
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
  /* CORREÇÃO MODAL: Fundo suave e com desfoque */
  background-color: rgba(0, 0, 0, 0.5);
  backdrop-filter: blur(4px);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}
.modal-content {
  background-color: var(--color-background-soft);
  padding: 2rem;
  border-radius: 12px;
  width: 100%;
  max-width: 600px;
  max-height: 90vh;
  overflow-y: auto;
  border: 1px solid var(--color-border);
  box-shadow: 0 10px 25px rgba(0,0,0,0.1);
}
.dark .modal-content {
  box-shadow: 0 10px 25px rgba(0,0,0,0.3);
}
.modal-title {
  font-size: 1.5rem;
  font-weight: 600;
  color: var(--color-heading);
  margin-bottom: 1.5rem;
}
.form-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 1rem;
}
.form-group {
  display: flex;
  flex-direction: column;
}
.form-group.col-span-2 {
  grid-column: span 2 / span 2;
}
.form-group label {
  margin-bottom: 0.5rem;
  font-weight: 500;
  font-size: 0.875rem;
  color: var(--color-text);
}
/* CORREÇÃO MODAL: Estilo dos campos do formulário */
.form-group input, .form-group textarea {
  background-color: var(--color-background);
  border: 1px solid var(--color-border);
  border-radius: 8px;
  padding: 0.75rem;
  color: var(--color-text);
  transition: all 0.2s ease;
}
.form-group input:focus, .form-group textarea:focus {
  outline: none;
  border-color: var(--primary-color);
  box-shadow: 0 0 0 2px var(--primary-color-soft);
}
.modal-actions {
  margin-top: 2rem;
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
}
.btn-secondary {
  padding: 0.75rem 1.5rem;
  border-radius: 8px;
  font-weight: 500;
  transition: background-color 0.2s;
  background-color: var(--color-background-mute);
  color: var(--color-text);
  border: 1px solid var(--color-border);
}
.btn-secondary:hover {
  background-color: var(--color-border-hover);
}
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
.btn-primary:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}
</style>
