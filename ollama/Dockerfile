# Imagem base oficial do Ollama
FROM ollama/ollama

# Instala o 'netcat-openbsd' para o script de verificação
RUN apt-get update && apt-get install -y netcat-openbsd procps && rm -rf /var/lib/apt/lists/*

# Copia e dá permissão ao script de inicialização
COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

# Define o script como o comando principal do contêiner
ENTRYPOINT ["/entrypoint.sh"]