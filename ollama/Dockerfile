FROM ollama/ollama

# Executa todos os comandos de inicialização diretamente aqui.
# O ENTRYPOINT força o uso do shell, resolvendo o erro de "comando desconhecido".
ENTRYPOINT ["/bin/sh", "-c"]

# O comando agora roda o servidor em segundo plano, baixa o modelo,
# e depois usa 'wait' para manter o container vivo.
CMD ["ollama serve & sleep 5 && ollama pull phi4-mini:q4_0 && wait"]