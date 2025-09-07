# Lead2Buy CRM
## by [RVWtech](www.rvwtech.com.br)

Lead2Buy CRM é uma plataforma completa de Gestão de Relacionamento com o Cliente (CRM) projetada para otimizar o processo de vendas e o engajamento com contatos. A aplicação integra um frontend moderno em Vue.js com um backend robusto em .NET, utilizando uma arquitetura de microsserviços conteinerizada com Docker.

## ✨ Funcionalidades

- **Gestão de Contatos:** Crie, edite, visualize e remova contatos de forma centralizada.
- **Funil de Vendas:** Organize seus contatos em um funil visual estilo Kanban para acompanhar o progresso de cada lead.
- **Dashboard Analítico:** Visualize métricas importantes, como total de leads, novos contatos no mês, e performance por origem.
- **Gestão de Tarefas:** Crie e gerencie tarefas associadas a cada contato para nunca perder um follow-up.
- **Chatbot com IA:** Utilize um chatbot integrado com Ollama para interagir e obter insights.
- **Importação e Exportação:** Exporte sua lista de contatos para CSV e importe novos contatos a partir de planilhas.
- **Autenticação Segura:** Sistema de registro e login com JWT (JSON Web Tokens) para proteger o acesso.
- **Notificações em Tempo Real:** Receba atualizações sobre processos demorados (como OCR) através de SignalR.

## 🚀 Tecnologias Utilizadas

- **Frontend:** Vue.js 3, Pinia, Axios, Tailwind CSS
- **Backend:** .NET 8, ASP.NET Core Web API, Entity Framework Core
- **Banco de Dados:** PostgreSQL
- **Cache e Filas:** Redis
- **Inteligência Artificial:** Ollama com o modelo `phi4-mini:q`
- **Servidor Web e Proxy Reverso:** Nginx
- **Containerização:** Docker e Docker Compose

## 🏁 Como Começar (Ambiente de Desenvolvimento)

Siga os passos abaixo para rodar o projeto localmente.

### Pré-requisitos

- [Docker](https://www.docker.com/products/docker-desktop/) e Docker Compose
- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 20+](https://nodejs.org/)

### Instalação

1.  **Clone o repositório:**
    ```bash
    git clone [https://github.com/rvwierzba/lead2buy-crm.git](https://github.com/rvwierzba/lead2buy-crm.git)
    cd lead2buy-crm
    ```

2.  **Crie o arquivo de configuração de ambiente (`.env`):**
    Na raiz do projeto, crie um arquivo chamado `.env` e adicione as seguintes variáveis. Substitua os valores de exemplo pelas suas credenciais.
    ```env
    # Credenciais do Banco de Dados
    POSTGRES_USER=postgres
    POSTGRES_PASSWORD=sua-senha-super-secreta
    POSTGRES_DB=lead2buy_db

    # Chaves para autenticação JWT
    JWT_KEY=uma-chave-secreta-muito-longa-e-dificil-de-adivinhar-com-pelo-menos-32-caracteres
    JWT_ISSUER=Lead2BuyAPI
    JWT_AUDIENCE=Lead2BuyApp

    # Credenciais do Mailjet (para envio de e-mail)
    MAILJET_API_KEY=sua-api-key-do-mailjet
    MAILJET_SECRET_KEY=sua-secret-key-do-mailjet
    MAILJET_FROM_EMAIL=seu-email-verificado-no-mailjet
    ```

3.  **Construa e inicie os contêineres:**
    Este comando irá construir as imagens da API, do Nginx (com o frontend) e do Ollama, além de iniciar todos os serviços.
    ```bash
    docker-compose up -d --build
    ```

4.  **Acesse a aplicação:**
    Após a conclusão do build, a aplicação estará disponível em `http://localhost`.

## 📚 Documentação da API

A documentação completa da API está disponível via Swagger e pode ser acessada em `http://localhost/swagger` quando o ambiente estiver rodando.

---