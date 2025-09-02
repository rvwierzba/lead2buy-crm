#!/bin/bash

echo "Iniciando o servidor Ollama em background..."
# Inicia o Ollama e o deixa rodando em segundo plano
/bin/ollama serve &

# Aguarda um pouco para garantir que o Ollama iniciou
sleep 10

echo "Tentando baixar o modelo phi4-mini em background..."
# Tenta baixar o modelo. Se falhar, a aplicação não quebra.
ollama pull phi4-mini:q4_0 &

echo "Iniciando a API .NET..."
# Inicia a sua API .NET, que fica como o processo principal
# A variável ASPNETCORE_URLS será fornecida pelo Render
dotnet Lead2Buy.API.dll