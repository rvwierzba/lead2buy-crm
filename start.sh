#!/bin/bash

echo "Iniciando o servidor Ollama em background..."
# Chama 'ollama' diretamente, sem o caminho completo
ollama serve &

# Aguarda 10 segundos para dar tempo ao Ollama de iniciar
sleep 10

echo "Tentando baixar o modelo phi4-mini em background..."
ollama pull phi4-mini:q4_0 &

echo "Iniciando a API .NET..."
dotnet Lead2Buy.API.dll