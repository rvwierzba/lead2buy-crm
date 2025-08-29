<template>
  <div class="stat-card">
    <div class="card-content">
      <h3 class="card-title">{{ title }}</h3>
      <p class="card-value">{{ value }}</p>
    </div>
  </div>
</template>

<script setup>
defineProps({
  title: { type: String, required: true },
  value: { type: [String, Number], required: true }
});
</script>

<style scoped>
.stat-card {
  background-color: var(--ui-bg);
  border: 1px solid var(--ui-border);
  border-radius: 12px;
  padding: 1.5rem;
  transition: all 0.3s ease;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
}
.card-title {
  font-size: 14px;
  color: var(--text-color-muted);
  margin: 0 0 0.5rem 0;
}
.card-value {
  font-size: 2.5rem;
  font-weight: bold;
  color: var(--text-color);
  margin: 0;
}
</style>
