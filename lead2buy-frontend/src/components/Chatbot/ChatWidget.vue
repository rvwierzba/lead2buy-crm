<template>
  <div>
    <div v-if="isChatOpen" class="chat-window">
      <header class="chat-header">
        <h4>Assistente IA Lead2Buy</h4>
        <button @click="toggleChat" class="close-btn">&times;</button>
      </header>
      <div class="message-area" ref="messageArea">
        <div v-for="(message, index) in messages" :key="index" class="message-bubble-container" :class="message.role">
          <div class="message-bubble">
            {{ message.content }}
          </div>
        </div>
      </div>
      <footer class="chat-footer">
        <form @submit.prevent="sendMessage" class="message-form">
          <button type="button" @click="triggerFileUpload" class="attach-btn">
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor"><path fill-rule="evenodd" d="M18.97 3.659a2.25 2.25 0 00-3.182 0l-10.5 10.5a.75.75 0 001.06 1.06l10.5-10.5a.75.75 0 011.06 0l3.182 3.182a.75.75 0 010 1.06l-10.5 10.5a4.5 4.5 0 11-6.364-6.364l7.091-7.09a2.25 2.25 0 013.182 3.182l-7.09 7.09a.75.75 0 001.06 1.06l7.09-7.09a.75.75 0 011.06 0l3.182 3.182a.75.75 0 010 1.06l-10.5 10.5a4.5 4.5 0 11-6.364-6.364l10.5-10.5a2.25 2.25 0 000-3.182z" clip-rule="evenodd" /></svg>
          </button>
          <input type="file" @change="handleFileSelect" ref="fileInput" style="display: none;" />

          <input v-model="newMessage" placeholder="Digite sua mensagem..." :disabled="isLoading" />
          <button type="submit" :disabled="isLoading">Enviar</button>
        </form>
      </footer>
    </div>

    <button @click="toggleChat" class="fab">
      <span>IA</span>
    </button>
  </div>
</template>

<script setup>
import { ref, onMounted, nextTick } from 'vue';
import apiService from '../../services/apiService';
import signalRService from '../../services/signalRService'; // Importamos o serviço

const isChatOpen = ref(false);
const isLoading = ref(false);
const messages = ref([
  { role: 'assistant', content: 'Olá! Como posso te ajudar? Anexe um arquivo e faça uma pergunta.' }
]);
const newMessage = ref('');
const messageArea = ref(null);
const fileInput = ref(null);
const attachedFile = ref(null);

// Conecta ao SignalR quando o componente é montado
onMounted(() => {
    // Escuta por respostas de chat finalizadas
    signalRService.on("ChatResponseReceived", (data) => {
        console.log("Notificação de chat recebida:", data);

        // Procura a mensagem de "Processando" com o jobId correspondente
        const processingMessageIndex = messages.value.findIndex(m => m.jobId === data.jobId);

        if (processingMessageIndex !== -1) {
            // Substitui a mensagem de "Processando" pela resposta final
            messages.value[processingMessageIndex] = { role: 'assistant', content: data.response };
        } else {
            // Se não encontrar (caso raro), apenas adiciona no final
            messages.value.push({ role: 'assistant', content: data.response });
        }
        scrollToBottom();
    });
});

const toggleChat = () => { isChatOpen.value = !isChatOpen.value; };
const triggerFileUpload = () => { fileInput.value.click(); };

const handleFileSelect = (event) => {
  const file = event.target.files[0];
  if (file) {
    attachedFile.value = file;
    messages.value.push({ role: 'user', content: `Arquivo anexado: ${file.name}` });
    event.target.value = '';
  }
};

const sendMessage = async () => {
  if (newMessage.value.trim() === '' && !attachedFile.value) return;

  if (newMessage.value) {
    messages.value.push({ role: 'user', content: newMessage.value });
  }

  const prompt = newMessage.value;
  const fileToUpload = attachedFile.value;

  newMessage.value = '';
  attachedFile.value = null;
  isLoading.value = true;

  await nextTick();
  scrollToBottom();

  try {
    let response;
    if (fileToUpload) {
      // Chama a API, que agora responde rápido
      response = await apiService.converseWithAttachment(prompt, fileToUpload);

      // Adiciona uma mensagem de "Processando" com o ID do Job
      const processingMessage = {
        role: 'assistant',
        content: `Analisando seu arquivo... Recebi seu pedido (Job ID: ${response.data.jobId}). Te aviso quando terminar.`,
        jobId: response.data.jobId // Guardamos o ID para futura referência
      };
      messages.value.push(processingMessage);

    } else {
      // Conversa simples sem anexo continua igual
      response = await apiService.converseWithChatbot(prompt);
      const assistantMessage = { role: 'assistant', content: response.data.response };
      messages.value.push(assistantMessage);
    }

  } catch (error) {
    const errorMessage = { role: 'assistant', content: 'Desculpe, ocorreu um erro ao enviar sua solicitação.' };
    messages.value.push(errorMessage);
    console.error("Erro no chatbot:", error);
  } finally {
    isLoading.value = false;
    await nextTick();
    scrollToBottom();
  }
};

const scrollToBottom = () => {
  if (messageArea.value) {
    messageArea.value.scrollTop = messageArea.value.scrollHeight;
  }
};
</script>

<style scoped>
/* ESTILO DO BOTÃO FLUTUANTE CORRIGIDO PARA USAR TEXTO */
.fab {
  position: fixed;
  bottom: 2rem;
  right: 2rem;
  width: 60px;
  height: 60px;
  border-radius: 50%;
  background-color: var(--primary-color);
  color: white;
  border: none;
  box-shadow: 0 4px 12px rgba(0,0,0,0.2);
  cursor: pointer;
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
  transition: transform 0.2s ease-in-out;
  /* Estilos para o texto "IA" */
  font-size: 24px;
  font-weight: bold;
  font-family: sans-serif;
}
.fab:hover {
    transform: scale(1.1);
}
.fab span {
    line-height: 1;
}

/* O RESTO DO CSS CONTINUA IGUAL */
.chat-window {
  position: fixed;
  bottom: 6.5rem;
  right: 2rem;
  width: 370px;
  height: 500px;
  background-color: var(--ui-bg);
  border: 1px solid var(--ui-border);
  border-radius: 12px;
  box-shadow: 0 5px 20px rgba(0,0,0,0.2);
  display: flex;
  flex-direction: column;
  z-index: 1000;
}
.chat-header {
  padding: 1rem;
  background-color: var(--bg-color);
  border-bottom: 1px solid var(--ui-border);
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.chat-header h4 { margin: 0; color: var(--text-color); }
.close-btn { background: none; border: none; font-size: 24px; cursor: pointer; color: var(--text-color-muted); }
.message-area {
  flex: 1;
  padding: 1rem;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
}
.message-bubble-container {
  display: flex;
  margin-bottom: 0.5rem;
}
.message-bubble {
  padding: 0.5rem 1rem;
  border-radius: 18px;
  max-width: 80%;
  line-height: 1.4;
}
.message-bubble-container.user {
  justify-content: flex-end;
}
.message-bubble-container.user .message-bubble {
  background-color: var(--primary-color);
  color: white;
  border-bottom-right-radius: 4px;
}
.message-bubble-container.assistant {
  justify-content: flex-start;
}
.message-bubble-container.assistant .message-bubble {
  background-color: var(--bg-color);
  color: var(--text-color);
  border: 1px solid var(--ui-border);
  border-bottom-left-radius: 4px;
}
.chat-footer {
  padding: 0.5rem;
  border-top: 1px solid var(--ui-border);
}
.message-form {
  display: flex;
  gap: 0.5rem;
}
.message-form input {
  flex: 1;
  padding: 10px;
  border: 1px solid var(--ui-border);
  border-radius: 8px;
  background-color: var(--bg-color);
  color: var(--text-color);
}
.message-form button {
  padding: 10px 15px;
  border: none;
  background-color: var(--primary-color);
  color: white;
  border-radius: 8px;
  cursor: pointer;
}
.message-form input:disabled, .message-form button:disabled {
    opacity: 0.5;
    cursor: not-allowed;
}

.attach-btn {
  background: none;
  border: none;
  padding: 0 10px;
  cursor: pointer;
  display: flex;
  align-items: center;
  color: var(--text-color-muted);
}
.attach-btn svg {
  width: 16px;
  height: 16px;
}
.attach-btn:hover {
  color: var(--primary-color);
}
</style>
