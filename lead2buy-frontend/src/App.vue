<script setup>
import { onMounted, watch } from 'vue';
import { RouterView } from 'vue-router'
import { useAuthStore } from './stores/authStore';
import signalRService from './services/signalRService';
import { useToast } from 'vue-toastification';

// Importa todos os nossos componentes de layout
import FooterComponent from './components/FooterComponent.vue'
import Navbar from './components/Navbar.vue';
import ChatWidget from './components/Chatbot/ChatWidget.vue'; // <-- O Widget de Chat

const authStore = useAuthStore();
const toast = useToast();

// Função que será chamada quando uma notificação chegar
const showNotification = (message) => {
    toast.info(message, {
        timeout: 5000,
        closeOnClick: true,
        pauseOnFocusLoss: true,
        pauseOnHover: true,
        draggable: true,
        showCloseButtonOnHover: false,
        hideProgressBar: false,
        closeButton: "button",
        icon: true,
    });
};

onMounted(() => {
    if (authStore.isAuthenticated) {
        signalRService.startConnection();
        signalRService.on("ReceiveNotification", showNotification);
    }
});

watch(() => authStore.isAuthenticated, (isAuth) => {
    if (isAuth) {
        signalRService.startConnection();
        signalRService.on("ReceiveNotification", showNotification);
    } else {
        signalRService.stopConnection();
    }
});
</script>

<template>
  <div id="app-wrapper">
    <Navbar v-if="authStore.isAuthenticated" />

    <main class="main-content">
      <RouterView />
    </main>

    <ChatWidget v-if="authStore.isAuthenticated" />

    <FooterComponent v-if="authStore.isAuthenticated" />
  </div>
</template>

<style>
/* Estilos para o layout principal */
#app-wrapper {
  position: relative;
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

/* Garante que o conteúdo principal ocupe o espaço disponível, empurrando o footer para baixo */
.main-content {
  flex: 1;
}

/* Adiciona padding no topo SOMENTE se a navbar estiver visível */
#app-wrapper:has(.navbar) .main-content {
    padding-top: 60px;
}
</style>
