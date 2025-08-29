<template>
  <div class="contact-card" :data-id="contact.id">
    <p class="card-name">{{ contact.name }}</p>
    <p class="card-phone">{{ contact.phoneNumber }}</p>
  </div>
</template>

<script setup>
defineProps({
  contact: {
    type: Object,
    required: true
  }
});
</script>

<style scoped>
.contact-card {
  background-color: var(--ui-bg);
  border: 1px solid var(--ui-border);
  padding: 1rem;
  border-radius: 8px;
  margin-bottom: 0.5rem;
  cursor: grab;
  transition: all 0.2s ease;
}
.contact-card:hover {
    box-shadow: 0 4px 8px rgba(0,0,0,0.1);
    transform: translateY(-2px);
}
.card-name {
  font-weight: bold;
  margin: 0 0 0.25rem 0;
  color: var(--text-color);
}
.card-phone {
  font-size: 12px;
  color: var(--text-color-muted);
  margin: 0;
}
</style>
