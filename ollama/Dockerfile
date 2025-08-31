FROM ollama/ollama

# Instala o 'curl' para verificar a saúde do servidor
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copia o script a partir do contexto raiz do repositório
COPY ollama/entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

# Define o script como o ponto de entrada do contêiner
ENTRYPOINT ["./entrypoint.sh"]