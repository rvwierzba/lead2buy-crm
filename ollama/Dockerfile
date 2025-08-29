FROM ollama/ollama

# Executa todos os comandos de inicialização diretamente aqui.
# 1. Inicia o servidor em segundo plano.
# 2. Espera 5 segundos para o servidor ficar pronto.
# 3. Baixa o modelo de IA.
# 4. Usa 'wait' para manter o container rodando após o download.
CMD ["/bin/sh", "-c", "ollama serve & sleep 5 && ollama pull phi4-mini:q4_0 && wait"]