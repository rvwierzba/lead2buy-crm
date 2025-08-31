#!/bin/sh
set -e

# Inicia o servidor Ollama em background
ollama serve &

# Captura o PID do processo em background
pid=$!

echo "Aguardando o servidor Ollama iniciar..."

# Espera até que o servidor esteja respondendo
while ! curl -s -f http://127.0.0.1:11434/ > /dev/null; do
    sleep 1
done

echo "Servidor Ollama iniciado. Baixando o modelo..."

# Baixa o modelo
ollama pull phi4-mini:q4_0

echo "Modelo baixado com sucesso."

# Traz o processo do servidor para o primeiro plano para manter o contêiner ativo
wait $pid