# Estágio 1: Construir o Frontend (Vue.js)
FROM node:20-alpine AS frontend-builder

# Define o diretório de trabalho dentro do contêiner
WORKDIR /app

# Copia os arquivos de definição de pacotes do frontend
COPY lead2buy-frontend/package*.json ./

# Instala as dependências do frontend
RUN npm install

# Copia todo o código-fonte do frontend
COPY lead2buy-frontend/ .

# Executa o build de produção do frontend
RUN npm run build

# Estágio 2: Preparar o Servidor Web (Nginx)
FROM nginx:alpine

# Remove a configuração padrão do Nginx
RUN rm /etc/nginx/conf.d/default.conf

# Copia a nossa configuração customizada do Nginx
COPY deployment/nginx/lead2buy-api.conf /etc/nginx/conf.d/default.conf

# Copia os arquivos do frontend (que foram construídos no estágio anterior) para a pasta que o Nginx serve
COPY --from=frontend-builder /app/dist /usr/share/nginx/html

# Expõe as portas 80 (HTTP) e 443 (HTTPS)
EXPOSE 80 443

# Comando para iniciar o Nginx
CMD ["nginx", "-g", "daemon off;"]