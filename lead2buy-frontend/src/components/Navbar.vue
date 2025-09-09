<template>
  <nav class="navbar">
    <div class="navbar-brand">
      <router-link to="/">
        <img src="@/assets/l2b_logo.png" alt="Lead2Buy Logo" class="navbar-logo" />
      </router-link>
    </div>

    <div class="navbar-links">
      <router-link to="/" class="nav-link">
        <ChartBarIcon class="h-5 w-5" />
        <span>Dashboard</span>
      </router-link>
      <router-link to="/contacts" class="nav-link">
        <UserGroupIcon class="h-5 w-5" />
        <span>Contatos</span>
      </router-link>
      <router-link to="/funil" class="nav-link">
        <FunnelIcon class="h-5 w-5" />
        <span>Funil</span>
      </router-link>
      <router-link to="/network" class="nav-link">
        <UsersIcon class="h-5 w-5" />
        <span>Network</span>
      </router-link>
    </div>

    <div class="navbar-menu">
      <div class="theme-switcher-container">
        <svg v-if="theme === 'light'" class="icon sun" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor"><path d="M12 2.25a.75.75 0 01.75.75v2.25a.75.75 0 01-1.5 0V3a.75.75 0 01.75-.75zM7.5 12a4.5 4.5 0 119 0 4.5 4.5 0 01-9 0zM18.894 6.106a.75.75 0 00-1.06-1.06l-1.591 1.59a.75.75 0 101.06 1.061l1.591-1.59zM21.75 12a.75.75 0 01-.75.75h-2.25a.75.75 0 010-1.5h2.25a.75.75 0 01.75.75zM17.836 17.836a.75.75 0 00-1.061-1.06l-1.59 1.591a.75.75 0 101.06 1.06l1.59-1.591zM12 18.75a.75.75 0 01.75.75v2.25a.75.75 0 01-1.5 0v-2.25a.75.75 0 01.75-.75zM4.164 17.836a.75.75 0 001.06-1.06l-1.59-1.591a.75.75 0 10-1.061 1.06l1.59 1.591zM3 12a.75.75 0 01-.75.75H.75a.75.75 0 010-1.5h2.25A.75.75 0 013 12zM6.106 6.106a.75.75 0 001.061-1.06l-1.591-1.59a.75.75 0 00-1.06 1.061l1.591 1.59z"></path></svg>
        <svg velse class="icon moon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor"><path fill-rule="evenodd" d="M9.528 1.718a.75.75 0 01.162.819A8.97 8.97 0 009 6a9 9 0 009 9 8.97 8.97 0 003.463-.69.75.75 0 01.981.98 10.503 10.503 0 01-9.694 6.46c-5.799 0-10.5-4.701-10.5-10.5 0-3.833 2.067-7.171 5.144-8.972a.75.75 0 01.818.162z" clip-rule="evenodd"></path></svg>
        <label class="switch">
          <input type="checkbox" @change="toggleTheme" :checked="theme === 'dark'">
          <span class="slider round"></span>
        </label>
      </div>
      <button @click="handleLogout" class="logout-btn">Sair</button>
    </div>
  </nav>
</template>

<script setup>
import { useTheme } from '../composables/useTheme';
import { useAuthStore } from '../stores/authStore';
import { ChartBarIcon, UserGroupIcon, FunnelIcon, UsersIcon } from '@heroicons/vue/24/outline';

const { theme, toggleTheme } = useTheme();
const authStore = useAuthStore();
const handleLogout = () => { authStore.logout(); };
</script>

<style scoped>
.navbar {
  display: flex; justify-content: space-between; align-items: center;
  padding: 0 2rem; height: 60px;
  background-color: var(--color-background-soft);
  border-bottom: 1px solid var(--color-border);
}
.navbar-brand a { display: flex; align-items: center; }
.navbar-logo { height: 40px; }
.navbar-links { display: flex; gap: 0.5rem; }
.nav-link {
  display: flex; align-items: center; gap: 0.5rem;
  padding: 0.5rem 1rem; border-radius: 8px;
  color: var(--color-text); text-decoration: none;
  font-weight: 500; transition: all 0.2s ease;
}
.nav-link:hover { background-color: var(--color-background-mute); }
.router-link-exact-active {
  background-color: var(--primary-color);
  color: white !important;
}
.router-link-exact-active svg { color: white !important; }
.dark .router-link-exact-active { color: var(--color-heading) !important; }
.dark .router-link-exact-active svg { color: var(--color-heading) !important; }
.navbar-menu { display: flex; align-items: center; gap: 1rem; }
.logout-btn {
  background: none; border: 1px solid var(--color-border);
  padding: 8px 15px; border-radius: 8px; cursor: pointer;
  transition: all 0.2s ease-in-out; color: var(--color-text); font-weight: 500;
}
.logout-btn:hover {
  background-color: var(--color-background-mute);
  border-color: var(--color-border-hover);
}
.theme-switcher-container { display: flex; align-items: center; gap: 0.75rem; }
.icon { width: 20px; height: 20px; color: var(--color-text-mute); }
.switch { position: relative; display: inline-block; width: 50px; height: 26px; }
.switch input { opacity: 0; width: 0; height: 0; }
.slider {
  position: absolute; cursor: pointer; top: 0; left: 0; right: 0; bottom: 0;
  background-color: #ccc; transition: .4s;
}
.slider:before {
  position: absolute; content: ""; height: 18px; width: 18px;
  left: 4px; bottom: 4px; background-color: white; transition: .4s;
}
input:checked + .slider { background-color: var(--primary-color); }
input:checked + .slider:before { transform: translateX(24px); }
.slider.round { border-radius: 34px; }
.slider.round:before { border-radius: 50%; }
</style>
