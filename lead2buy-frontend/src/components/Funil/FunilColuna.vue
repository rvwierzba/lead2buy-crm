<template>
  <div class="funnel-column">
    <h3 class="column-title">{{ title }} ({{ contacts.length }})</h3>
    <draggable
      :list="contacts"
      group="contacts"
      item-key="id"
      class="card-list"
      @end="$emit('drag-end', $event)"
  >
      <template #item="{ element }">
          <ContatoCard :contact="element" />
      </template>
    </draggable>
  </div>
</template>

<script setup>
import draggable from 'vuedraggable';
import ContatoCard from '../Contact/ContatoCard.vue';

defineProps({
  title: { type: String, required: true },
  contacts: { type: Array, required: true }
});

defineEmits(['drag-end']);
</script>

<style scoped>
.funnel-column {
  flex: 1 0 300px;
  max-width: 350px;
  height: fit-content;

  /* Estilos para parecer um "quadro" */
  background-color: var(--ui-bg);
  border: 1px solid var(--ui-border);
  border-radius: 12px;
  padding: 1rem;

  display: flex;
  flex-direction: column;
}

.column-title {
  color: var(--text-color);
  padding: 0 0.5rem 0.5rem 0.5rem; /* Adicionado padding inferior */
  margin-top: 0;
  border-bottom: 2px solid var(--ui-border); /* Linha divisória para o título */
}

.card-list {
  min-height: 400px;
  padding: 0.5rem;
  border-radius: 8px;
  flex-grow: 1; /* Faz a lista ocupar o espaço disponível na coluna */
}
</style>
