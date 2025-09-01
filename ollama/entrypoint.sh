#!/bin/sh
set -e

# Inicia o servidor Ollama em background
ollama serve &

# Aguarda um momento para o servidor iniciar
sleep 5

echo "Servidor Ollama iniciado. Tentando baixar o modelo em background..."

# Tenta baixar o modelo em segundo plano. Se falhar, não irá derrubar o serviço.
nohup ollama pull phi4-mini:q4_0 &

# Mantém o contêiner rodando, aguardando o processo principal do servidor
wait