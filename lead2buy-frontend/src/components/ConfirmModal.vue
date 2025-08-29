<template>
  <div class="modal-overlay" @click.self="$emit('close')">
    <div class="modal-content">
      <h3>{{ title }}</h3>
      <p>{{ message }}</p>
      <div class="form-actions">
        <button type="button" class="btn-cancel" @click="$emit('close')">Cancelar</button>
        <button type="button" class="btn-confirm" @click="$emit('confirm')">Confirmar</button>
      </div>
    </div>
  </div>
</template>

<script setup>
defineProps({
  title: { type: String, required: true },
  message: { type: String, required: true }
});
defineEmits(['close', 'confirm']);
</script>

<style scoped>
.modal-overlay {
  position: fixed; top: 0; left: 0; width: 100%; height: 100%;
  background-color: rgba(0, 0, 0, 0.6); display: flex;
  justify-content: center; align-items: center; z-index: 1001;
}
.modal-content {
  background: var(--ui-bg, #fff); color: var(--text-color);
  padding: 2rem; border-radius: 12px; width: 100%; max-width: 400px;
  text-align: center;
}
p {
  margin: 1rem 0;
}
.form-actions {
  margin-top: 1.5rem; display: flex; justify-content: center; gap: 1rem;
}
.btn-cancel {
  padding: 10px 20px; border: 1px solid #ccc;
  background-color: #f8f9fa; border-radius: 8px; cursor: pointer;
}
.btn-confirm {
  padding: 10px 20px; border: none;
  background-color: #dc3545; /* Vermelho de perigo */
  color: white; border-radius: 8px; cursor: pointer;
}
</style>
