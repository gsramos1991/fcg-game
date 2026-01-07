# FCG Game Microservice

Microsserviço de Jogos usando .NET 8.

## Tecnologias

- .NET 8
- Azure Service Bus
- Kubernetes
- Docker

## Como executar

1. Alterar a connectionString do banco de dados (SQL Server)
```Json
  "ConfigFila": {
    "ConnectionString": "Server=localhost,1433;Database=FCG_Jogos;User Id=[username];Password=[Password];TrustServerCertificate=True;"
  }
```
2. Configurar a ConnectionString do Azure Service Bus 
```Json
  "ConfigFila": {
    "ConnectionString": ""
  }
```
3. Configurar a rota da api de pagamentos
```Json
  "OrderApi": {
    "Url": "http://localhost:5012/"
  }
```
---
## Executar o projeto
1. Rodar a migrations com o comando

```
dotnet ef migrations add AddOrderId --project .\FCG.Game.Infrastructure --startup-project .\FCG.Game.API
```
```
dotnet ef database update
```
2. Executar a aplicação:
```bash
dotnet run --project FCG.Game.API
```

4. Acessar Swagger:
```
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
