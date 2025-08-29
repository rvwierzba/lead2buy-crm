<template>
  <nav class="navbar">
    <div class="navbar-brand">
      <span><img src="@/assets/l2b_logo.png" alt="Lead2Buy Logo" class="navbar-logo" /></span>
    </div>
    <div class="navbar-links">
      <router-link to="/">Dashboard</router-link>
      <router-link to="/contacts">Contatos</router-link>
      <router-link to="/funil">Funil</router-link>
    </div>
    <div class="navbar-menu">
     <div class="theme-switcher-container">
      <label class="switch">
        <input type="checkbox" @change="toggleTheme" :checked="theme === 'dark'">
        <span class="slider round"></span>
      </label>
      <svg v-if="theme === 'light'" class="icon sun" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor"><path d="M12 2.25a.75.75 0 01.75.75v2.25a.75.75 0 01-1.5 0V3a.75.75 0 01.75-.75zM7.5 12a4.5 4.5 0 119 0 4.5 4.5 0 01-9 0zM18.894 6.106a.75.75 0 00-1.06-1.06l-1.591 1.59a.75.75 0 101.06 1.061l1.591-1.59zM21.75 12a.75.75 0 01-.75.75h-2.25a.75.75 0 010-1.5h2.25a.75.75 0 01.75.75zM17.836 17.836a.75.75 0 00-1.061-1.06l-1.59 1.591a.75.75 0 101.06 1.06l1.59-1.591zM12 18.75a.75.75 0 01.75.75v2.25a.75.75 0 01-1.5 0v-2.25a.75.75 0 01.75-.75zM4.164 17.836a.75.75 0 001.06-1.06l-1.59-1.591a.75.75 0 10-1.061 1.06l1.59 1.591zM3 12a.75.75 0 01-.75.75H.75a.75.75 0 010-1.5h2.25A.75.75 0 013 12zM6.106 6.106a.75.75 0 001.061-1.06l-1.591-1.59a.75.75 0 00-1.06 1.061l1.591 1.59z"></path></svg>
      <svg v-else class="icon moon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor"><path fill-rule="evenodd" d="M9.528 1.718a.75.75 0 01.162.819A8.97 8.97 0 009 6a9 9 0 009 9 8.97 8.97 0 003.463-.69.75.75 0 01.981.98 10.503 10.503 0 01-9.694 6.46c-5.799 0-10.5-4.701-10.5-10.5 0-3.833 2.067-7.171 5.144-8.972a.75.75 0 01.818.162z" clip-rule="evenodd"></path></svg>
    </div>
      <button @click="handleLogout" class="logout-btn">Sair</button>
    </div>
  </nav>
</template>

<script setup>
  import { useTheme } from '../composables/useTheme';
  import { useAuthStore } from '../stores/authStore';

  // Pega as funções e variáveis do nosso composable de tema
  const { theme, toggleTheme } = useTheme();

  // Pega a store de autenticação para o logout
  const authStore = useAuthStore();

  const handleLogout = () => {
    authStore.logout();
  };
</script>

<style scoped>
  .navbar {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 0 2rem;
    height: 60px;
    background-color: var(--ui-bg, #ffffff); /* Usa a variável, com branco como padrão */
    box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    border-bottom: 1px solid var(--ui-border, #e9ecef);
    transition: background-color 0.3s ease;
  }
  .navbar-brand {
    display: flex;
    align-items: center;
    gap: 10px;
    font-weight: bold;
    font-size: 20px;
    color: var(--text-color, #333);
  }
  .navbar-logo {
    height: 40px;
  }
  .navbar-links {
    display: flex;
    gap: 1.5rem;
}
.navbar-links a {
    color: var(--text-color);
    text-decoration: none;
    font-weight: 600;
}
.navbar-links a.router-link-exact-active {
    color: var(--primary-color);
}
  .navbar-menu {
    display: flex;
    align-items: center;
    gap: 1rem;
  }
  .theme-switcher, .logout-btn {
    background: none;
    border: 1px solid #ccc;
    padding: 8px 15px;
    border-radius: 8px;
    cursor: pointer;
    transition: all 0.2s ease-in-out;
  }
  .logout-btn {
    background-color: #f8f9fa;
  }
  .theme-switcher:hover, .logout-btn:hover {
      background-color: #e9ecef;
      border-color: #adb5bd;
  }
/*---------------------------------------------------------*/
/* ESTILOS PARA O TOGGLE SWITCH */
  .switch {
    position: relative;
    display: inline-block;
    width: 50px;
    height: 26px;
  }
  .switch input {
    opacity: 0;
    width: 0;
    height: 0;
  }
  .slider {
    position: absolute;
    cursor: pointer;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background-color: #ccc;
    transition: .4s;
  }
  .slider:before {
    position: absolute;
    content: "";
    height: 18px;
    width: 18px;
    left: 4px;
    bottom: 4px;
    background-color: white;
    transition: .4s;
  }
  input:checked + .slider {
    background-color: var(--primary-color);
  }
  input:focus + .slider {
    box-shadow: 0 0 1px var(--primary-color);
  }
  input:checked + .slider:before {
    transform: translateX(24px);
  }
  .slider.round {
    border-radius: 34px;
  }
  .slider.round:before {
    border-radius: 50%;
  }
  .theme-switcher-container {
  display: flex;
  align-items: center;
  gap: 0.5rem; /* Espaço entre o ícone e a chave */
}
.icon {
  width: 24px;
  height: 24px;
  color: var(--text-color);
  transition: color 0.3s ease;
}
</style>
