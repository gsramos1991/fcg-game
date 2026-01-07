# FCG Game Microservice

Microsserviço de Jogos usando .NET 8.

## 📦 Requisitos

- .NET 8
- Azure Service Bus
- Kubernetes
- Docker
- SQL Server

## 🐳 Docker
- Build da imagem da API
  - `docker build -t fcg-games:latest .`
- Subir com docker-compose (SQL Server)
  - `docker compose up -d`

## Como executar

1. Alterar a connectionString do banco de dados (SQL Server)
- ```Json
    "ConnectionStrings": {
      "DefaultConnection": "Server=localhost,1433;Database=FCG_Jogos;User Id=[username];Password=[Password];TrustServerCertificate=True;"
    }
  ```
2. Configurar a ConnectionString do Azure Service Bus 
- ```Json
  "ConfigFila": {
    "ConnectionString": ""
  }
  ```
3. Configurar a rota da api de pagamentos
- ```Json
  "OrderApi": {
    "Url": ""
  }
  ```
---
## 🧱 Migrations (EF Core)

**Gerar uma nova migration:**
- Na raiz do repositório:
  ```bash
  dotnet ef migrations add <NomeDaMigration> -p .\FCG.Game.Infrastructure -s .\FCG.Game.API -c GameDbContext
  ```

**Atualizar o banco de dados:**
- ```bash
  dotnet ef database update -p .\FCG.Game.Infrastructure -s .\FCG.Game.API -c GameDbContext
  ```

**Executar a aplicação**
- ```bash
    dotnet run --project FCG.Game.API
  ```
**Acessar Swagger**
- ```
    http://localhost:5002/swagger
  ```

## Estrutura

- **FCG.Game.API**: Controllers e configuração da API
- **FCG.Game.Application**: Serviços e lógica de aplicação
- **FCG.Game.Domain**: Entidades e eventos de domínio
- **FCG.Game.Infrastructure**: Repositórios e integrações
- **FCG.Game.Tests**: Testes unitários

## API Endpoints

### Games

- `POST /api/Games`: Cria um novo jogo (Requer Role de Administrador)
- `GET /api/Games/{id}`: Busca um jogo por ID
- `GET /api/Games/search`: Procura por jogos com base em um termo
- `GET /api/Games/genre/{genre}`: Busca jogos por gênero
- `GET /api/Games/popular`: Busca os jogos mais populares

### Orders

- `POST /new-order`: Cria um novo pedido
- `POST /{id}/complete`: Completa um pedido

### Payments

- `GET /api/Payments/consultarPagamento`: Consulta um pagamento
- `POST /api/Payments/cancelar`: Cancela um pagamento
