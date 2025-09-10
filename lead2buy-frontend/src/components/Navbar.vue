<template>
  <nav class="navbar">
    <div class="navbar-container">
      <router-link to="/" class="navbar-logo">
        <img src="@/assets/l2b_logo.png" alt="Lead2Buy Logo" class="logo-img">
      </router-link>

      <ul class="nav-menu">
        <li class="nav-item">
          <router-link to="/" class="nav-link" exact-active-class="active">
            <i class="fas fa-tachometer-alt"></i>
            <span>Dashboard</span>
          </router-link>
        </li>
        <li class="nav-item">
          <router-link to="/contacts" class="nav-link" exact-active-class="active">
            <i class="fas fa-users"></i>
            <span>Contatos</span>
          </router-link>
        </li>
        <li class="nav-item">
          <router-link to="/funil" class="nav-link" exact-active-class="active">
            <i class="fas fa-filter"></i>
            <span>Funil</span>
          </router-link>
        </li>
        <li class="nav-item">
          <router-link to="/network" class="nav-link" exact-active-class="active">
            <i class="fas fa-network-wired"></i>
            <span>Network</span>
          </router-link>
        </li>
      </ul>

      <div class="user-actions">
        <basic-toggle-switch
          v-model="isDark"
          class="theme-toggle"
          aria-label="Alternar tema"
        >
          <i :class="isDark ? 'fas fa-moon' : 'fas fa-sun'"></i>
        </basic-toggle-switch>

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
import BasicToggleSwitch from './toggle-switch.vue'; // <-- O nome do arquivo deve ser este

// isDark é o estado (true/false), toggleTheme é a função que o altera.
// O v-model no componente filho vai chamar a função automaticamente.
const { isDark, toggleTheme } = useTheme();
const authStore = useAuthStore();
const router = useRouter();

const logout = () => {
  authStore.logout();
  router.push('/login');
};
</script>

<style scoped>
/* O seu CSS original do Navbar.vue está perfeito, não precisa mudar nada. */
/* Cole ele aqui ou mantenha o que você já tinha. */
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
.navbar-logo {
  display: flex;
  align-items: center;
  padding: 5px 0;
}
.logo-img {
  height: 40px;
  width: auto;
}
.nav-menu {
  display: flex;
  list-style: none;
  margin: 0;
  padding: 0;
  gap: 0.5rem;
}
.nav-link {
  display: flex;
  align-items: center;
  gap: 0.5rem;
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
.nav-link.active {
  color: var(--color-heading);
  font-weight: 700;
  background-color: var(--color-background-mute);
}
.user-actions {
  display: flex;
  align-items: center;
  gap: 1.5rem;
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
/* A classe .theme-toggle não é mais necessária aqui,
   pois o estilo está dentro do componente filho.
   Pode remover se quiser. */
</style>
