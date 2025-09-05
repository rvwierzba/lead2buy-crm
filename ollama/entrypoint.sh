#!/bin/sh

set -e

# Inicia o servidor Ollama em background
ollama serve &
pid=$!

echo "Aguardando o servidor Ollama iniciar na porta 11434..."

# Espera o servidor ficar pronto antes de continuar
while ! nc -z localhost 11434; do
  sleep 1
done

echo "Servidor Ollama iniciado com sucesso."
echo "Baixando o modelo phi4-mini:q4_0..."
ollama pull phi4-mini:q4_0
echo "Modelo baixado com sucesso."

# Traz o processo do servidor para o primeiro plano para manter o contêiner rodando
wait $pid