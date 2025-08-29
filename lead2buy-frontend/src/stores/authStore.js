import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import apiService from '../services/apiService';

export const useAuthStore = defineStore('auth', () => {
  const token = ref(localStorage.getItem('token') || null);
  const user = ref(null);

  const isAuthenticated = computed(() => !!token.value);

  async function login(credentials) {
    try {
      const response = await apiService.login(credentials);
      const newToken = response.data.token;

      localStorage.setItem('token', newToken);
      token.value = newToken;

      console.log('Token salvo na store e localStorage.');
      return true;
    } catch (error) {
      console.error('Erro na ação de login da store:', error);
      localStorage.removeItem('token');
      token.value = null;
      user.value = null;
      return false;
    }
  }

  // Ação de Logout com a correção para o CSS
  function logout() {
    localStorage.removeItem('token');
    token.value = null;
    user.value = null;

    console.log('Usuário deslogado, token removido.');

    // FORÇA UM RECARREGAMENTO COMPLETO da página de login.
    // Isso garante que o navegador carregue todos os arquivos CSS do zero.
    window.location.href = '/login';
  }

  function checkAuth() {
    const savedToken = localStorage.getItem('token');
    if (savedToken) {
      token.value = savedToken;
    } else {
      token.value = null;
    }
  }

  return {
    token,
    user,
    isAuthenticated,
    login,
    logout,
    checkAuth
  };
});
