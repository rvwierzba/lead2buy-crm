FROM ollama/ollama

# Copia nosso script de inicialização para dentro da imagem
COPY entrypoint.sh /entrypoint.sh

# Dá permissão de execução para o script
RUN chmod +x /entrypoint.sh

# Define nosso script como o comando que será executado quando o container iniciar
CMD ["/entrypoint.sh"]