#!/bin/bash
set -e

echo "Aguardando o banco de dados ficar pronto... (espera de 20s)"
sleep 20

echo "Executando migrações do banco de dados..."
# A CORREÇÃO FINAL: Usa o caminho completo para a ferramenta 'ef'
/root/.dotnet/tools/dotnet-ef database update

echo "Migrações concluídas. Iniciando a aplicação..."
exec dotnet Lead2Buy.API.dll