# Lead2Buy CRM  
## by [RVWtech](https://www.rvwtech.com.br)

Lead2Buy CRM é uma plataforma completa de **Gestão de Relacionamento com o Cliente (CRM)** projetada para otimizar o processo de vendas, engajamento com contatos e automação de processos.  
A aplicação integra um **frontend moderno em Vue.js** com um **backend robusto em .NET**, utilizando **Clean Architecture** e uma **arquitetura de microsserviços conteinerizada com Docker**.

---

## ✨ Funcionalidades Existentes

- **Gestão de Contatos:** Criação, edição, visualização e exclusão de contatos.  
- **Funil de Vendas (Kanban):** Acompanhamento visual do progresso de cada lead.  
- **Dashboard Analítico:** Métricas como total de leads, novos contatos no mês e performance por origem.  
- **Gestão de Tarefas:** Criação e acompanhamento de tarefas vinculadas a contatos.  
- **Interações:** Registro de ligações, e-mails, reuniões e observações.  
- **Chatbot com IA:** Integração com **Ollama** para insights inteligentes.  
- **Importação e Exportação:** Exportação de contatos para CSV e importação a partir de planilhas.  
- **Autenticação Segura:** Registro e login com **JWT (JSON Web Tokens)**.  
- **Notificações em Tempo Real:** Atualizações via **SignalR**.  
- **OCR e Processos Assíncronos:** Processamento em background com feedback em tempo real.  

---

## 🔥 Novas Funcionalidades em Desenvolvimento

- **Múltiplos Usuários e Perfis:** Cada etapa do funil pode ter um responsável (Comercial, Agendamento, Follow-up, Confirmação, Compareceu, Convertido, Remarketing).  
- **Histórico de Eficiência:** Registro de logins e ações para medir produtividade individual e da equipe.  
- **Calendário por Usuário:** Agenda integrada com compromissos vinculados a leads, incluindo origem e interesse.  
- **Timeline do Lead:** Linha do tempo detalhada com todas as interações e movimentações do lead.  
- **Módulo de BI com IA:** Inteligência artificial observando métricas em tempo real e gerando análises automáticas em uma seção exclusiva.  
- **Integração com n8n:** Entrada automática de leads (nome, telefone, origem) via automações conectadas ao **Evolution**.  

---

## 🏗️ Arquitetura do Sistema

O Lead2Buy segue **Clean Architecture** e boas práticas de design:

- **Frontend (Vue.js 3 + Tailwind CSS):**
  - Padrão de design consistente (cores, dark/light mode, tipografia).
  - Componentização reutilizável.
  - Pinia para gerenciamento de estado.
  - Axios para comunicação com a API.

- **Backend (.NET 8 + ASP.NET Core Web API):**
  - **Camada de Domínio:** Entidades (`User`, `Contact`, `CrmTask`, `Interaction`, etc.).
  - **Camada de Aplicação:** Serviços e regras de negócio.
  - **Camada de Infraestrutura:** Persistência com Entity Framework Core (PostgreSQL), Redis, EmailService.
  - **Camada de Apresentação:** Controllers REST, SignalR Hubs, Swagger.

- **Banco de Dados:** PostgreSQL (persistência principal).  
- **Cache e Filas:** Redis (tarefas assíncronas, notificações).  
- **IA:** Ollama (modelo `phi4-mini:q`) para chatbot e análises.  
- **Comunicação em Tempo Real:** SignalR.  
- **Containerização:** Docker e Docker Compose para orquestração.  
- **Servidor Web:** Nginx como proxy reverso.  

---

## 📂 Estrutura do Projeto
```
lead2buy-crm/
│
├── backend/
│   └── Lead2Buy.API/
│       ├── Controllers/        # Endpoints REST
│       ├── Data/               # DbContext e Migrations
│       ├── Hubs/               # SignalR Hubs
│       ├── Models/             # Entidades de Domínio
│       ├── Services/           # Serviços de aplicação
│       ├── Program.cs          # Configuração principal da API
│       └── ...
│
├── frontend/
│   ├── src/
│   │   ├── components/         # Componentes Vue
│   │   ├── store/              # Pinia stores (estado global)
│   │   ├── views/              # Páginas principais
│   │   └── App.vue             # Raiz da aplicação Vue
│   └── ...
│
├── docker-compose.yml          # Orquestração dos containers
├── nginx.conf                  # Configuração do proxy reverso
└── README.md                   # Documentação do projeto
```

## 🏁 Como Começar (Ambiente de Desenvolvimento)

### Pré-requisitos

- [Docker](https://www.docker.com/products/docker-desktop/) e Docker Compose  
- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)  
- [Node.js 20+](https://nodejs.org/)  

### Instalação

1. **Clone o repositório:**
   ```bash
   git clone https://github.com/rvwierzba/lead2buy-crm.git
   cd lead2buy-crm


# Banco de Dados
POSTGRES_USER=postgres
POSTGRES_PASSWORD=sua-senha-super-secreta
POSTGRES_DB=lead2buy_db

# JWT
JWT_KEY=uma-chave-secreta-muito-longa-e-dificil-de-adivinhar-com-pelo-menos-32-caracteres
JWT_ISSUER=Lead2BuyAPI
JWT_AUDIENCE=Lead2BuyApp

# Mailjet
MAILJET_API_KEY=sua-api-key-do-mailjet
MAILJET_SECRET_KEY=sua-secret-key-do-mailjet
MAILJET_FROM_EMAIL=seu-email-verificado-no-mailjet

# Redis
REDIS_CONNECTION_STRING=localhost:6379

# CORS
CORS_ORIGIN=https://crm.rvwtech.com.br

```
docker-compose up -d --build
```

## 🌐 URLs de Acesso em Produção

- **Frontend:** [https://crm.rvwtech.com.br](https://crm.rvwtech.com.br)  
- **API:** [https://crm.rvwtech.com.br/api](https://crm.rvwtech.com.br/api)  
- **Swagger (documentação):** [https://crm.rvwtech.com.br/swagger](https://crm.rvwtech.com.br/swagger)  
- **SignalR Hub:** [https://crm.rvwtech.com.br/notificationHub](https://crm.rvwtech.com.br/notificationHub)  

---

## 📚 Documentação da API

A documentação completa da API está disponível via **Swagger** em:  
[https://crm.rvwtech.com.br/swagger](https://crm.rvwtech.com.br/swagger)

---

## 🌐 Deploy em Produção

- **Servidor:** VPS Debian 13 (LocalWeb)  
- **Domínio:** [crm.rvwtech.com.br](https://crm.rvwtech.com.br)  
- **Proxy reverso:** Nginx configurado para redirecionar tráfego HTTPS para os containers.  
- **Certificado SSL:** Gerenciado via **Let's Encrypt (Certbot)** com renovação automática.  
- **Deploy:**  
  ```bash
  git pull origin main
  docker-compose up -d --build


## 📈 Roadmap

    [x] Gestão de contatos, funil e tarefas

    [x] Autenticação JWT e notificações em tempo real

    [ ] Multiusuário com papéis e histórico de eficiência

    [ ] Calendário por usuário

    [ ] Timeline do lead

    [ ] Integração com n8n (Evolution)

    [ ] Módulo de BI com IA



   ## 📌 Padrões de Arquitetura e Design

    Backend: Seguir Clean Architecture (separação clara entre Domínio, Aplicação, Infraestrutura e Apresentação).

    Frontend: Manter identidade visual já definida (cores, dark/light mode, tipografia).

    Código: Seguir boas práticas de SOLID, DRY e reutilização de componentes.

    Banco: PostgreSQL como persistência principal, sempre versionado via migrations.

    Infraestrutura: Todos os serviços devem ser conteinerizados (Docker).

    Segurança: JWT com chave de no mínimo 32 caracteres, HTTPS obrigatório em produção.


