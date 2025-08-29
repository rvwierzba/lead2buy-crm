#!/bin/sh

# Inicia o servidor principal do Ollama em segundo plano
ollama serve &

# Captura o ID do processo do servidor que acabou de iniciar
PID=$!

# Espera um pouco para garantir que o servidor esteja pronto para receber comandos
sleep 5

# Agora que o servidor está no ar, executa o comando para baixar o modelo
echo "Baixando o modelo phi4-mini..."
ollama pull phi4-mini:q4_0
echo "Modelo baixado com sucesso!"

# Traz o processo do servidor para o primeiro plano.
# Isso impede que o script termine e o container morra.
wait $PID