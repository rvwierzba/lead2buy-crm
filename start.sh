#!/bin/bash

# Inicia o servidor Ollama em background
ollama serve &
sleep 10

echo "Tentando baixar o modelo phi4-mini em background..."
ollama pull phi4-mini:q4_0 &

echo "Iniciando a API .NET..."
# Inicia a sua API .NET, que fica como o processo principal do contêiner
dotnet Lead2Buy.API.dll