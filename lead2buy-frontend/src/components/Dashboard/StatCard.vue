<template>
  <div class="stat-card">
    <div class="flex items-center">
      <div class="bg-blue-100 dark:bg-blue-900/50 p-3 rounded-full mr-4">
        <component :is="icon" class="h-6 w-6 text-blue-500 dark:text-blue-400" />
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
