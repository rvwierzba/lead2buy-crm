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

const props = defineProps({ isOpen: Boolean, contact: Object });
const emit = defineEmits(['close', 'saved']);
const form = ref({});
const isSaving = ref(false);
const isEditing = computed(() => props.contact && props.contact.id);

watch(() => props.isOpen, (newVal) => {
  if (newVal) {
    form.value = {
      name: props.contact?.name || '', email: props.contact?.email || '',
      phoneNumber: props.contact?.phoneNumber || '', whatsAppNumber: props.contact?.whatsAppNumber || '',
      linkedInUrl: props.contact?.linkedInUrl || '', facebookUrl: props.contact?.facebookUrl || '',
      youTubeChannelUrl: props.contact?.youTubeChannelUrl || '', notes: props.contact?.notes || '',
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
.form-group input, .form-group textarea {
  width: 100%; padding: 10px;
  border: 1px solid var(--color-border);
  border-radius: 8px;
  background-color: var(--color-background);
  color: var(--color-text);
  transition: all 0.2s ease;
}
.form-group input:focus, .form-group textarea:focus {
  outline: none; border-color: var(--primary-color);
  box-shadow: 0 0 0 2px var(--primary-color-soft);
}
.btn-secondary, .btn-primary {
  padding: 10px 20px; border-radius: 8px;
  cursor: pointer; font-weight: 500;
  border: 1px solid transparent;
}
.btn-secondary {
  border-color: var(--color-border);
  background-color: transparent;
  color: var(--color-text);
}
.btn-secondary:hover { background-color: var(--color-background-mute); }
.btn-primary {
  background-color: var(--primary-color);
  color: white;
}
.btn-primary:hover { background-color: var(--primary-color-dark); }
.btn-primary:disabled { opacity: 0.5; cursor: not-allowed; }
.error-message { color: red; margin-top: 1rem; }
</style>
