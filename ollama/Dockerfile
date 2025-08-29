FROM ollama/ollama

# Define o "programa" que vai executar nosso comando
ENTRYPOINT ["/bin/sh", "-c"]

# O comando agora roda o servidor em segundo plano, baixa o modelo,
# e depois usa 'wait' para manter o container vivo e esperando.
CMD ["ollama serve & ollama pull phi4-mini:q4_0 && wait"]