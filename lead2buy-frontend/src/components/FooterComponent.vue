<template>
  <footer class="app-footer">
    Desenvolvido por
    <a href="http://www.rvwtech.com.br" target="_blank" rel="noopener noreferrer">
      RVWtech
    </a>
  </footer>
</template>

<script setup>
// Nenhum script necessário para este componente simples
</script>

<style scoped>
.app-footer {
  width: 100%;
  text-align: center;
  padding: 20px 0;
  /* Usa as variáveis de tema para fundo e borda */
  background-color: var(--navbar-footer-bg, #f8f9fa);
  border-top: 1px solid var(--navbar-footer-border, #dee2e6);
  color: var(--text-color-muted, #6c757d); /* Cor de texto sutil */
  font-size: 14px;
  position: absolute;
  bottom: 0;
  left: 0;
  transition: all 0.3s ease; /* Transição suave */
}

.app-footer a {
  /* Usa a variável para a cor do texto principal */
  color: var(--text-color, #495057);
  font-weight: 600;
  text-decoration: none;
  transition: color 0.3s;
}

.app-footer a:hover {
  color: var(--primary-color); /* Nossa cor primária no hover */
}
</style>
