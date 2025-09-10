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
          <button type="button" @click="$emit('close')" class="btn-cancel">Cancelar</button>
          <button type="submit" class="btn-primary" :disabled="isSaving">
            {{ isSaving ? 'Salvando...' : 'Salvar Contato' }}
          </button>
        </div>
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
  background-color: rgba(0, 0, 0, 0.7); /* Fundo mais escuro para o overlay */
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}
.modal-content {
  background: var(--ui-bg); /* Usa a cor de fundo para painéis */
  color: var(--text-color); /* Usa a cor de texto do tema */
  padding: 2rem;
  border-radius: 12px;
  width: 100%;
  max-width: 500px;
  border: 1px solid var(--ui-border);
}
h3 {
    color: var(--text-color); /* Garante que o título use a cor do tema */
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
input, select {
  width: 100%;
  padding: 10px;
  border: 1px solid var(--ui-border);
  background-color: var(--bg-color); /* Fundo dos inputs */
  color: var(--text-color); /* Cor do texto dos inputs */
  border-radius: 8px;
}
.form-actions {
  margin-top: 1.5rem;
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
}

/* Ajuste para campos que ocupam duas colunas */
.form-group.col-span-2 {
  grid-column: span 2 / span 2;
}

/* Estilo para cada grupo de formulário (label + input) */
.form-group {
  display: flex;
  flex-direction: column;
  margin-bottom: 0; /* Remove margin-bottom individual, o gap do grid já gerencia */
}

/* Estilo para os rótulos dos campos */
.form-group label {
  margin-bottom: 0.5rem;
  font-weight: 600;
  font-size: 0.875rem;
  color: var(--color-text-light); /* Cor do label, um pouco mais suave */
}

/* Estilo para os campos de input e textarea */
.form-group input,
.form-group textarea {
  width: 100%;
  padding: 10px;
  border: 1px solid var(--color-border-input); /* Borda do input mais clara/escura */
  border-radius: 8px;
  background-color: var(--color-background-input); /* Fundo do input */
  color: var(--color-text); /* Cor do texto digitado */
  transition: all 0.2s ease;
}

/* Estilo para campos de input e textarea quando focados */
.form-group input:focus,
.form-group textarea:focus {
  outline: none;
  border-color: var(--primary-color); /* Borda primária ao focar */
  box-shadow: 0 0 0 2px var(--primary-color-soft); /* Sombra suave ao focar */
}

/* Estilo para o grupo de anotações */
.form-group.mt-4 {
  margin-top: 1.5rem; /* Espaçamento superior para as anotações */
}

/* Estilo para a seção de botões de ação do modal */
.modal-actions {
  margin-top: 2rem; /* Mais espaço para os botões */
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
}

/* Estilo base para os botões */
.btn-cancel,
.btn-primary {
  padding: 10px 20px;
  border-radius: 8px;
  cursor: pointer;
  font-weight: 500;
  border: 1px solid transparent;
  transition: all 0.2s ease;
}

/* Estilo para o botão de cancelar */
.btn-cancel {
  border-color: var(--color-border); /* Borda padrão */
  background-color: var(--color-background-button-secondary); /* Fundo secundário */
  color: var(--color-text);
}

.btn-cancel:hover {
  background-color: var(--color-background-button-secondary-hover);
  border-color: var(--color-border-hover);
}

/* Estilo para o botão primário (salvar) */
.btn-primary {
  background-color: var(--primary-color); /* Cor primária */
  color: white; /* Texto branco */
}

.btn-primary:hover {
  background-color: var(--primary-color-dark); /* Tom mais escuro da cor primária */
}

/* Estilo para botões desabilitados */
.btn-primary:disabled {
  opacity: 0.6;
  cursor: not-allowed;
  background-color: var(--primary-color); /* Mantém a cor primária, mas com opacidade */
  pointer-events: none; /* Impede eventos de mouse */
}

/* Media query para telas menores (ex: celulares) */
@media (max-width: 640px) {
  .modal-content {
    max-width: 95%; /* Ocupa mais largura em telas pequenas */
    padding: 1rem;
  }
  .form-grid {
    grid-template-columns: 1fr; /* Coluna única em telas pequenas */
  }
  .form-group.col-span-2 {
    grid-column: span 1 / span 1; /* Ajusta para coluna única */
  }
  .modal-actions {
    flex-direction: column; /* Botões empilhados em telas pequenas */
    gap: 0.75rem;
  }
  .btn-cancel, .btn-primary {
    width: 100%; /* Botões ocupam largura total */
  }
}

</style>
