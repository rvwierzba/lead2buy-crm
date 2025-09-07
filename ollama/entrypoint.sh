#!/bin/sh
set -e
ollama serve &
pid=$!
echo "Aguardando o servidor Ollama iniciar..."
while ! nc -z localhost 11434; do
  sleep 1
done
echo "Servidor Ollama iniciado. Baixando o modelo..."
ollama pull phi4-mini:q4_0
echo "Modelo baixado."
wait $pid