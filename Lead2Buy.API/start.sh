#!/bin/bash
set -e

# Inicia o servidor Ollama em background
ollama serve &
sleep 10

echo "Tentando baixar o modelo phi4-mini em background..."
ollama pull phi4-mini:q4_0 &

# Aguarda o Postgres ficar disponível
echo "Aguardando Postgres ficar pronto..."
until pg_isready -h db -U postgres; do
  sleep 2
done

# Força a senha do usuário postgres
echo "Forçando senha do Postgres..."
PGPASSWORD=$POSTGRES_PASSWORD psql -h db -U postgres -d $POSTGRES_DB -c "ALTER USER postgres WITH PASSWORD '$POSTGRES_PASSWORD';" || true

echo "Iniciando a API .NET..."
exec dotnet Lead2Buy.API.dll
