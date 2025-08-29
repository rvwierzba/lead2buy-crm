FROM ollama/ollama

# Copia nosso script de inicialização para dentro da imagem
COPY entrypoint.sh entrypoint.sh

# Define NOSSO SCRIPT como o ponto de entrada do container
ENTRYPOINT ["entrypoint.sh"]

# Dá permissão de execução para o script
RUN chmod +x entrypoint.sh

