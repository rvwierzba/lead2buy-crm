<template>
  <nav class="navbar">
    <div class="navbar-container">
      <router-link to="/" class="navbar-logo">
        <img src="@/assets/l2b_logo.png" alt="Lead2Buy Logo" class="logo-img">
      </router-link>

      <ul class="nav-menu">
        <li class="nav-item">
          <router-link to="/" class="nav-link">
            <i class="fas fa-tachometer-alt"></i>
            <span>Dashboard</span>
          </router-link>
        </li>
        <li class="nav-item">
          <router-link to="/contacts" class="nav-link">
            <i class="fas fa-users"></i>
            <span>Contatos</span>
          </router-link>
        </li>
        <li class="nav-item">
          <router-link to="/funil" class="nav-link">
            <i class="fas fa-filter"></i>
            <span>Funil</span>
          </router-link>
        </li>
        <li class="nav-item">
          <router-link to="/network" class="nav-link">
            <i class="fas fa-network-wired"></i>
            <span>Network</span>
          </router-link>
        </li>
      </ul>

      <div class="user-actions">
        <button @click="toggleTheme" class="theme-toggle">
          <i :class="isDark ? 'fas fa-sun' : 'fas fa-moon'"></i>
        </button>
        <div class="user-profile">
          <i class="fas fa-user-circle"></i>
          <button @click="logout" class="logout-button">
            Sair
          </button>
        </div>
      </div>
    </div>
  </nav>
</template>

<script setup>
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/authStore';
import { useTheme } from '@/composables/useTheme';

const { isDark, toggleTheme } = useTheme();
const authStore = useAuthStore();
const router = useRouter();

const logout = () => {
  authStore.logout();
  router.push('/login');
};
</script>

<style scoped>
.navbar {
  background-color: var(--color-background);
  border-bottom: 1px solid var(--color-border);
  padding: 0 2rem;
  height: 60px;
  display: flex;
  align-items: center;
  position: sticky;
  top: 0;
  z-index: 999;
}

.navbar-container {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
}

/* ESTILO DA LOGO */
.navbar-logo {
  display: flex;
  align-items: center;
  padding: 5px 0; /* Adiciona um respiro para a logo não colar na borda */
}

.logo-img {
  height: 40px; /* Ajuste a altura conforme necessário */
  width: auto;
}

/* ESTILO DO MENU DE NAVEGAÇÃO */
.nav-menu {
  display: flex;
  list-style: none;
  margin: 0;
  padding: 0;
  gap: 0.5rem; /* Espaçamento entre os itens */
}

.nav-link {
  display: flex;
  align-items: center;
  gap: 0.5rem; /* Espaço entre ícone e texto */
  padding: 0.75rem 1rem;
  border-radius: 6px;
  color: var(--color-text-light);
  text-decoration: none;
  font-weight: 500;
  transition: background-color 0.2s ease, color 0.2s ease;
}

.nav-link:hover {
  background-color: var(--color-background-soft);
  color: var(--color-text);
}

/* A MÁGICA ACONTECE AQUI! */
/* Estilo aplicado APENAS ao link ativo */
.nav-link.router-link-exact-active {
  background-color: var(--color-background-mute);
  color: var(--color-heading);
}

/* ESTILO DAS AÇÕES DO USUÁRIO */
.user-actions {
  display: flex;
  align-items: center;
  gap: 1.5rem;
}

.theme-toggle {
  background: none;
  border: none;
  color: var(--color-text-light);
  cursor: pointer;
  font-size: 1.2rem;
  transition: color 0.2s ease;
}

.theme-toggle:hover {
  color: var(--color-text);
}

.user-profile {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.user-profile i {
  font-size: 1.5rem;
  color: var(--color-text-light);
}

.logout-button {
  background: none;
  border: none;
  color: var(--color-text);
  cursor: pointer;
  font-weight: 500;
}
</style>
