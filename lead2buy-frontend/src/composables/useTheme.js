import { ref, onMounted, watchEffect } from 'vue';

export function useTheme() {
  // Tenta pegar o tema salvo no localStorage, ou usa 'light' como padrão
  const theme = ref(localStorage.getItem('theme') || 'light');

  // Função para alternar o tema
  function toggleTheme() {
    theme.value = theme.value === 'light' ? 'dark' : 'light';
  }

  // 'watchEffect' é um observador que roda sempre que 'theme.value' muda
  watchEffect(() => {
    // Remove a classe antiga para garantir que apenas uma exista
    document.body.classList.remove('light-mode', 'dark-mode');
    // Adiciona a classe atual ao body
    document.body.classList.add(`${theme.value}-mode`);
    // Salva a nova preferência no localStorage
    localStorage.setItem('theme', theme.value);
  });

  // Garante que o estado inicial seja aplicado quando o app carrega
  onMounted(() => {
    document.body.classList.add(`${theme.value}-mode`);
  });

  // Expõe a variável 'theme' e a função 'toggleTheme' para quem usar o composable
  return {
    theme,
    toggleTheme
  };
}
