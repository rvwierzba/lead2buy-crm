#!/bin/sh

# Inicia o servidor principal do Ollama em segundo plano
ollama serve &

# Captura o ID do processo do servidor que acabou de iniciar
PID=$!

# Espera um pouco para garantir que o servidor esteja pronto
sleep 5

# Agora que o servidor está no ar, baixa o modelo
echo "Baixando o modelo phi4-mini:q4_0..."
ollama pull phi4-mini:q4_0
echo "Modelo baixado com sucesso!"

# Traz o processo do servidor para o primeiro plano para manter o container vivo
wait $PID