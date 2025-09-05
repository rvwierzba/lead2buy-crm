#!/bin/sh

# Encerra o script imediatamente se qualquer comando falhar
set -e

# Inicia o servidor Ollama em background para podermos executar outros comandos
ollama serve &

# Captura o ID do processo do servidor que está em background
pid=$!

echo "Aguardando o servidor Ollama iniciar na porta 11434..."

# Loop que espera o servidor ficar pronto antes de continuar
# Ele tenta se conectar a cada segundo.
while ! nc -z localhost 11434; do
  sleep 1
done

echo "Servidor Ollama iniciado com sucesso."

# Agora que o servidor está no ar, baixa o modelo
echo "Baixando o modelo phi4-mini:q4_0..."
ollama pull phi4-mini:q4_0
echo "Modelo baixado com sucesso."

# Traz o processo do servidor para o primeiro plano.
# Isso garante que o contêiner continue rodando enquanto o servidor estiver ativo.
wait $pid