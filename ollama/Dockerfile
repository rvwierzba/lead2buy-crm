FROM ollama/ollama

# Instala o 'curl' para verificar a saúde do servidor
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copia e torna o script de inicialização executável
COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

# Define o script como o ponto de entrada do contêiner
ENTRYPOINT ["/entrypoint.sh"]