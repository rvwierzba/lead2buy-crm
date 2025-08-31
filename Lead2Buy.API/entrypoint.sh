#!/bin/bash

# Saia imediatamente se um comando falhar
set -e

# Aguarde o banco de dados ficar pronto e execute as migrações
echo "Aguardando o banco de dados ficar pronto e executando migrações..."
dotnet ef database update

# Inicie a aplicação principal
echo "Iniciando a aplicação..."
dotnet Lead2Buy.API.dll