#!/bin/bash

/usr/local/bin/ollama serve &
sleep 10
echo "Tentando baixar o modelo phi4-mini em background..."
/usr/local/bin/ollama pull phi4-mini:q4_0 &
echo "Iniciando a API .NET..."
dotnet Lead2Buy.API.dll