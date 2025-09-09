<template>
  <div class="stat-card">
    <div class="flex items-center">
      <div class="icon-container">
        <component :is="icon" class="stat-icon" />
      </div>
      <div>
        <p class="card-title">{{ title }}</p>
        <p class="card-value">{{ value }}</p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { defineProps } from 'vue';

defineProps({
  title: {
    type: String,
    required: true,
  },
  value: {
    type: [String, Number],
    required: true,
  },
  icon: {
    type: Object,
    required: true,
  },
});
</script>

<style scoped>
.stat-card {
  background-color: var(--color-background-soft);
  border: 1px solid var(--color-border);
  border-radius: 12px;
  padding: 1.5rem;
  transition: all 0.3s ease;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
}
.dark .stat-card {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.icon-container {
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  width: 3rem;  /* 48px */
  height: 3rem; /* 48px */
  border-radius: 9999px;
  margin-right: 1rem;
  /* Cor para o modo claro */
  background-color: #dbeafe; /* Azul claro, tailwind blue-100 */
  transition: background-color 0.3s ease;
}

/* --- CORREÇÃO DARK MODE AQUI --- */
.dark .icon-container {
  /* Cor de fundo mais suave para o modo escuro */
  background-color: rgba(30, 64, 175, 0.4); /* Azul escuro com transparência */
}

.stat-icon {
  width: 1.5rem;  /* 24px */
  height: 1.5rem; /* 24px */
  /* Cor do ícone para o modo claro */
  color: #3b82f6; /* Azul, tailwind blue-500 */
  transition: color 0.3s ease;
}

.dark .stat-icon {
  /* Cor do ícone para o modo escuro */
  color: #93c5fd; /* Azul mais claro para contraste, tailwind blue-300 */
}
/* --- FIM DA CORREÇÃO --- */

.card-title {
  font-size: 14px;
  color: var(--color-text-mute);
  margin: 0 0 0.25rem 0;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}
.card-value {
  font-size: 2.25rem;
  font-weight: 700;
  color: var(--color-heading);
  margin: 0;
  line-height: 1.1;
}
</style>
