<template>
  <div class="login-container">
    <form @submit.prevent="handleLogin" class="login-form">
      <img src="@/assets/l2b_logo.png" alt="Lead2Buy Logo" class="logo">

      <div class="form-group">
        <label for="email">Email</label>
        <input type="email" id="email" v-model="email" required placeholder="seuemail@exemplo.com" />
      </div>

      <div class="form-group">
        <label for="password">Senha</label>
        <input type="password" id="password" v-model="password" required placeholder="••••••••" />
      </div>

      <button type="submit">Entrar</button>

      <p v-if="errorMessage" class="error-message">{{ errorMessage }}</p>

      <p class="register-link">
        Não tem uma conta? <router-link to="/register">Crie uma agora</router-link>
      </p>
    </form>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '../stores/authStore';

const email = ref('');
const password = ref('');
const errorMessage = ref('');
const authStore = useAuthStore();
const router = useRouter();

const handleLogin = async () => {
  errorMessage.value = '';
  const credentials = {
    email: email.value,
    password: password.value
  };

  const success = await authStore.login(credentials);

  if (success) {
    // A navegação agora é controlada pelo router guard
    router.push('/');
  } else {
    errorMessage.value = 'Email ou senha inválidos. Tente novamente.';
  }
};
</script>

<style scoped>
/* ESTILO COMPLETO E CORRIGIDO PARA ESPELHAR O REGISTERVIEW */
.login-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background-color: var(--bg-color);
  padding: 20px 0;
  transition: background-color 0.3s ease;
}
.logo {
  max-width: 250px;
  margin-bottom: 30px;
}
.login-form {
  padding: 40px;
  background: var(--ui-bg);
  border-radius: 12px;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.08);
  width: 100%;
  max-width: 400px;
  display: flex;
  flex-direction: column;
  align-items: center;
  border: 1px solid var(--ui-border);
}
.form-group {
  margin-bottom: 20px;
  width: 100%;
  text-align: left;
}
label {
  display: block;
  margin-bottom: 8px;
  color: var(--text-color);
  font-weight: 600;
}
input {
  width: 100%;
  padding: 12px;
  border: 1px solid var(--ui-border);
  background-color: var(--bg-color);
  color: var(--text-color);
  border-radius: 8px;
  box-sizing: border-box;
}
button {
  width: 100%;
  padding: 14px;
  background-color: var(--primary-color);
  color: white;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-size: 16px;
  font-weight: bold;
}
button:hover {
  background-color: #3A378A;
}
.error-message {
  color: #E74C3C;
  margin-top: 15px;
}
.register-link {
  margin-top: 20px;
  font-size: 14px;
  color: var(--text-color);
}
.register-link a {
  color: var(--primary-color);
  font-weight: bold;
  text-decoration: none;
}
</style>
