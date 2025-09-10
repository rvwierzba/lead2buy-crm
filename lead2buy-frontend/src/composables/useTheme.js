import { ref, onMounted, watchEffect, computed } from 'vue';

export function useTheme() {
  const theme = ref(localStorage.getItem('theme') || 'light');

  // 'isDark' agora é uma propriedade computada.
  // Ela reage a mudanças em 'theme' e pode ser usada com v-model
  // porque tem um 'get' (para ler o valor) e um 'set' (para alterá-lo).
  const isDark = computed({
    get: () => theme.value === 'dark',
    set: (value) => {
      theme.value = value ? 'dark' : 'light';
    }
  });

  // Este watchEffect continua igual e vai funcionar perfeitamente.
  watchEffect(() => {
    document.body.classList.remove('light-mode', 'dark-mode');
    document.body.classList.add(`${theme.value}-mode`);
    localStorage.setItem('theme', theme.value);
  });

  onMounted(() => {
    document.body.classList.add(`${theme.value}-mode`);
  });

  // A função 'toggleTheme' é um pouco redundante agora que o v-model funciona,
  // mas vamos mantê-la caso você precise dela em outro lugar.
  function toggleTheme() {
    isDark.value = !isDark.value;
  }

  // Agora exportamos 'isDark' e 'toggleTheme'
  return {
    isDark,
    toggleTheme
  };
}
