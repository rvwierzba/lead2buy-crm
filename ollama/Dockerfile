FROM ollama/ollama

# Executa todos os comandos de inicialização diretamente aqui
# O comando 'ollama serve &' inicia o servidor em segundo plano
# 'sleep 2' dá um tempo para ele iniciar
# 'ollama pull phi4-mini:q4_0' baixa o modelo
# 'tail -f /dev/null' impede o container de fechar após o download
CMD ["/bin/sh", "-c", "ollama serve & sleep 2 && ollama pull phi4-mini:q4_0 && tail -f /dev/null"]