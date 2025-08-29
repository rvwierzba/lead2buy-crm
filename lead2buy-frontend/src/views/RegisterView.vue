<template>
  <div class="register-container">
    <form @submit.prevent="handleRegister" class="register-form">
      <img src="@/assets/l2b_logo.png" alt="Lead2Buy Logo" class="logo">

      <div class="form-group">
        <label for="name">Nome Completo</label>
        <input type="text" id="name" v-model="name" required placeholder="Seu nome" />
      </div>

      <div class="form-group">
        <label for="email">Email</label>
        <input type="email" id="email" v-model="email" required placeholder="seuemail@exemplo.com" />
      </div>

      <div class="form-group">
        <label for="password">Senha</label>
        <input type="password" id="password" v-model="password" required placeholder="Mínimo 6 caracteres" />
      </div>

      <button type="submit">Registrar</button>

      <p v-if="successMessage" class="success-message">{{ successMessage }}</p>
      <p v-if="errorMessage" class="error-message">{{ errorMessage }}</p>

      <p class="login-link">
        Já tem uma conta? <router-link to="/login">Faça o login</router-link>
      </p>
    </form>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import apiService from '../services/apiService';

const name = ref('');
const email = ref('');
const password = ref('');
const errorMessage = ref('');
const successMessage = ref('');
const router = useRouter();

const handleRegister = async () => {
  errorMessage.value = '';
  successMessage.value = '';

  if (password.value.length < 6) {
    errorMessage.value = 'A senha deve ter no mínimo 6 caracteres.';
    return;
  }

  try {
    const userData = {
      name: name.value,
      email: email.value,
      password: password.value
    };

    await apiService.register(userData);

    successMessage.value = 'Registro bem-sucedido! Redirecionando para o login...';

    setTimeout(() => {
      router.push('/login');
    }, 2000);

  } catch (error) {
    if (error.response && error.response.data) {
      errorMessage.value = error.response.data;
    } else {
      errorMessage.value = 'Ocorreu um erro no registro. Tente novamente.';
    }
    console.error('Erro no registro:', error);
  }
};
</script>

<style scoped>
/* Estilos agora usando as variáveis de tema globais */
.register-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background-color: var(--bg-color); /* <-- MUDANÇA */
  padding: 20px 0;
  transition: background-color 0.3s ease;
}
.logo {
  max-width: 250px;
  margin-bottom: 30px;
}
.register-form {
  padding: 40px;
  background: var(--ui-bg, #fff); /* <-- MUDANÇA */
  border-radius: 12px;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.08);
  width: 100%;
  max-width: 400px;
  display: flex;
  flex-direction: column;
  align-items: center;
  border: 1px solid var(--ui-border, #e9ecef); /* <-- MUDANÇA */
}
.form-group {
  margin-bottom: 20px;
  width: 100%;
  text-align: left;
}
label {
  display: block;
  margin-bottom: 8px;
  color: var(--text-color); /* <-- MUDANÇA */
  font-weight: 600;
}
input {
  width: 100%;
  padding: 12px;
  border: 1px solid var(--ui-border, #ccc); /* <-- MUDANÇA */
  background-color: var(--bg-color); /* <-- MUDANÇA */
  color: var(--text-color); /* <-- MUDANÇA */
  border-radius: 8px;
  box-sizing: border-box;
}
button {
  width: 100%;
  padding: 14px;
  background-color: #28a745; /* Verde para diferenciar do login */
  color: white;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-size: 16px;
  font-weight: bold;
}
button:hover {
  background-color: #218838;
}
.error-message {
  color: #E74C3C;
  margin-top: 15px;
}
.success-message {
  color: #28a745;
  margin-top: 15px;
}
.login-link {
  margin-top: 20px;
  font-size: 14px;
  color: var(--text-color); /* <-- MUDANÇA */
}
.login-link a {
  color: var(--primary-color); /* <-- MUDANÇA */
  font-weight: bold;
  text-decoration: none;
}
</style>
