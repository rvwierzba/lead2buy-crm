#!/bin/sh

# Inicia o servidor principal do Ollama em segundo plano
ollama serve &

# Espera 5 segundos para o servidor ficar pronto
sleep 5

# Agora que o servidor está no ar, baixa o modelo
echo "Baixando o modelo phi4-mini:q4_0..."
ollama pull phi4-mini:q4_0
echo "Modelo baixado com sucesso!"

# 'wait' impede que o container termine após o download
wait