#!/bin/bash

# Inicia o servidor Ollama em segundo plano
/bin/ollama serve &

# Aguarda o servidor estar pronto
echo "Aguardando o servidor Ollama iniciar..."
while ! nc -z localhost 11434; do
  sleep 1 # aguarda 1 segundo
done
echo "Servidor Ollama iniciado. Baixando o modelo phi4-mini..."

ollama pull phi4-mini:q4_0

echo "Modelo baixado com sucesso. O contêiner está pronto."

# Mantém o contêiner rodando para que o serviço continue online
tail -f /dev/null
