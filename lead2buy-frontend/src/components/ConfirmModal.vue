<template>
  <div v-if="isOpen" class="modal-overlay" @click.self="$emit('cancel')">
    <div class="modal-content">
      <h3 class="modal-title">{{ title }}</h3>
      <p class="modal-message">{{ message }}</p>
      <div class="modal-actions">
        <button @click="$emit('cancel')" class="btn-secondary">Cancelar</button>
        <button @click="$emit('confirm')" class="btn-danger">Confirmar Exclusão</button>
      </div>
    </div>
  </div>
</template>

<script setup>
defineProps({
  isOpen: Boolean,
  title: String,
  message: String,
});

defineEmits(['confirm', 'cancel']);
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.6);
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
  max-width: 450px;
  text-align: center;
}
.modal-title {
  font-size: 1.25rem;
  font-weight: 600;
  color: var(--color-heading);
  margin-bottom: 0.5rem;
}
.modal-message {
  color: var(--color-text);
  margin-bottom: 1.5rem;
}
.modal-actions {
  display: flex;
  justify-content: center;
  gap: 1rem;
}
.btn-secondary {
    padding: 0.6rem 1.2rem;
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
.btn-danger {
  padding: 0.6rem 1.2rem;
  border-radius: 8px;
  font-weight: 500;
  transition: background-color 0.2s;
  background-color: #ef4444;
  color: white;
}
.btn-danger:hover {
  background-color: #dc2626;
}
</style>
