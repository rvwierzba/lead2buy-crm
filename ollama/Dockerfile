FROM ollama/ollama

# Executa todos os comandos de inicialização diretamente aqui.
# O ENTRYPOINT da imagem base do Ollama já é "ollama", então passamos os comandos como argumentos.
# O shell (-c) é necessário para rodar múltiplos comandos com '&'.
CMD ["/bin/sh", "-c", "ollama serve & ollama pull phi4-mini:q4_0 && wait"]