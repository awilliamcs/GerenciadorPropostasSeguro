# GPS API Gateway

Este é o API Gateway (BFF - Backend for Frontend) do sistema GPS (Gerenciador de Propostas de Seguro), implementado usando [Ocelot](https://github.com/ThreeMammals/Ocelot).

## Funcionalidades

- **Roteamento Centralizado**: Ponto único de acesso para todos os microserviços
- **Rate Limiting**: Controle de taxa de requisições (100 requisições por minuto)
- **CORS**: Configurado para aceitar requisições de qualquer origem
- **Health Check**: Endpoint para verificar a saúde do gateway
- **Documentação de Rotas**: Endpoint que lista todas as rotas disponíveis

## Microserviços Integrados

O gateway roteia requisições para os seguintes serviços:

1. **AutenticacaoService** (`/api/autenticacao`)
   - Porta de desenvolvimento: 5000
   - Container: `autenticacao-api:8080`

2. **PessoaService** (`/api/pessoa`)
   - Porta de desenvolvimento: 5001
   - Container: `pessoa-api:8080`

3. **PropostaService** (`/api/proposta`)
   - Porta de desenvolvimento: 5002
   - Container: `proposta-api:8080`

4. **ContratacaoService** (`/api/contratacao`)
   - Porta de desenvolvimento: 5003
   - Container: `contratacao-api:8080`

## Endpoints do Gateway

### Health Check
```
GET /api/gateway/health
```
Retorna informações sobre a saúde do gateway.

### Documentação de Rotas
```
GET /api/gateway/routes
```
Lista todos os serviços disponíveis e suas rotas.

### Swagger Documentation
```
GET /swagger
```
Documentação interativa da API (disponível apenas em desenvolvimento).

## Configuração

### Arquivos de Configuração

- `ocelot.json`: Configuração base para produção (localhost)
- `ocelot.Development.json`: Configuração para ambiente de desenvolvimento (Docker containers)

### Configuração de Rate Limiting

```json
"RateLimitOptions": {
  "EnableRateLimiting": true,
  "Period": "1m",
  "PeriodTimespan": 60,
  "Limit": 100
}
```

## Executando o Projeto

### Desenvolvimento Local

```bash
cd Backend/GPS.ApiGateway
dotnet run
```

O gateway estará disponível em `http://localhost:5004`

### Docker

```bash
docker-compose up api-gateway
```

O gateway estará disponível em `http://localhost:5004`

## Exemplos de Uso

### Chamando o Serviço de Autenticação através do Gateway
```bash
# Ao invés de chamar diretamente: http://localhost:5000/api/autenticacao/login
# Chame através do gateway:
curl -X POST http://localhost:5004/api/autenticacao/login \
  -H "Content-Type: application/json" \
  -d '{"email": "user@example.com", "password": "senha123"}'
```

### Chamando o Serviço de Pessoa através do Gateway
```bash
# Ao invés de chamar diretamente: http://localhost:5001/api/pessoa
# Chame através do gateway:
curl -X GET http://localhost:5004/api/pessoa \
  -H "Authorization: Bearer {token}"
```

## Estrutura de Arquivos

```
GPS.ApiGateway/
├── Controllers/
│   └── GatewayController.cs    # Controller com endpoints do gateway
├── ocelot.json                 # Configuração base do Ocelot
├── ocelot.Development.json     # Configuração para Docker
├── Program.cs                  # Configuração da aplicação
├── appsettings.json           # Configurações da aplicação
├── Dockerfile                 # Configuração Docker
└── README.md                  # Este arquivo
```

## Observações

- O gateway automaticamente carrega a configuração apropriada baseada no ambiente (`Development`, `Production`, etc.)
- As rotas são configuradas para aceitar todos os métodos HTTP (`GET`, `POST`, `PUT`, `DELETE`, `PATCH`)
- O padrão `{everything}` captura qualquer path adicional nos endpoints dos microserviços
- CORS está configurado para aceitar todas as origens (adequado para desenvolvimento)
