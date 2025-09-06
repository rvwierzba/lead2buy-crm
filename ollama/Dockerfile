# Imagem base oficial do Ollama
FROM ollama/ollama

# A imagem do Ollama é minimalista. Precisamos instalar o 'netcat-openbsd'
# para que o nosso script entrypoint.sh consiga verificar o status do servidor.
RUN apt-get update && apt-get install -y netcat-openbsd procps && rm -rf /var/lib/apt/lists/*

# Copia o nosso script de inicialização para dentro da imagem
COPY entrypoint.sh /entrypoint.sh

# Dá permissão de execução para o script
RUN chmod +x /entrypoint.sh

# Define o nosso script como o comando de entrada principal do contêiner
ENTRYPOINT ["/entrypoint.sh"]