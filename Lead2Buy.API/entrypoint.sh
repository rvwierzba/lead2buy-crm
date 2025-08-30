#!/bin/bash

# Define o script para parar se qualquer comando falhar
set -e

echo "Aguardando o banco de dados ficar pronto..."
# Pequena espera para garantir que o serviço do banco de dados tenha tido tempo de iniciar
sleep 15 

echo "Executando migrações do banco de dados..."
# Roda o comando para atualizar o banco. Se falhar, o script para aqui.
dotnet ef database update

echo "Migrações concluídas. Iniciando a aplicação..."
# Inicia a aplicação .NET
exec dotnet Lead2Buy.API.dll