# Gerenciador de Propostas de Seguro

Este repositório contém **3 microserviços .NET 8** (Pessoa, Proposta e Contratação) seguindo **DDD + Arquitetura Hexagonal (Ports & Adapters) + Clean Architecture**, com **autenticação via ASP.NET Core Identity/JWT** (no serviço Pessoa), **comunicação assíncrona via MassTransit + RabbitMQ**, e **bancos SQL Server** dedicados para cada microserviço, orquestrados por **Docker Compose**.

---

## 📂 Estrutura

```
src/
  GPS.PessoaService.Api/
  GPS.PessoaService.Application/
  GPS.PessoaService.Domain/
  GPS.PessoaService.Infrastructure/

  GPS.PropostaService.Api/
  GPS.PropostaService.Application/
  GPS.PropostaService.Domain/
  GPS.PropostaService.Infrastructure/

  GPS.ContratacaoService.Api/
  GPS.ContratacaoService.Application/
  GPS.ContratacaoService.Domain/
  GPS.ContratacaoService.Infrastructure/

docker-compose.yml
```

---

## 🧰 Tecnologias

- **.NET 8 / C#**
- **ASP.NET Core Web API**
- **Entity Framework Core** (SQL Server)
- **ASP.NET Core Identity** (no PessoaService) + **JWT**
- **MassTransit** (abstração de mensageria)
- **RabbitMQ** (mensageria)
- **Docker** e **Docker Compose**
- **DDD + Hexagonal + Clean Architecture**
- **Swagger/OpenAPI**

---

## 📜 Arquitetura e Camadas

```mermaid
flowchart LR
    %% Direção
    %% Camadas com IDs simples e rótulos claros

    subgraph API["Camada API (Controllers)"]
        C1[Controller Pessoa]
        C2[Controller Proposta]
        C3[Controller Contratação]
    end

    subgraph APP["Camada Application (Serviços e Consumers)"]
        S1[PessoaService]
        S2[PropostaService]
        S3[ContratacaoService]
    end

    subgraph DOMAIN["Camada Domain (Entidades e Interfaces)"]
        E1[Entidades]
        I1[Interfaces de Repositório]
    end

    subgraph INFRA["Camada Infrastructure (Repositórios, DB, Mensageria)"]
        R1[Repos Pessoa]
        R2[Repos Proposta]
        R3[Repos Contratação]
        MQ[(RabbitMQ)]
        DB1[(PessoaDb)]
        DB2[(PropostaDb)]
        DB3[(ContratacaoDb)]
    end

    %% Fluxo API -> Application
    C1 --> S1
    C2 --> S2
    C3 --> S3

    %% Application -> Domain (ports)
    S1 --> I1
    S2 --> I1
    S3 --> I1

    %% Domain -> Infrastructure (adapters)
    I1 --> R1
    I1 --> R2
    I1 --> R3

    %% Mensageria (eventos)
    S1 <-->|Eventos| MQ
    S2 <-->|Eventos| MQ
    S3 <-->|Eventos| MQ

    %% Persistência
    R1 --> DB1
    R2 --> DB2
    R3 --> DB3
```

**Fluxo simplificado:**
1. **Controller** recebe requisição HTTP e chama o **Serviço da Application**.
2. Serviço publica evento via **MassTransit** (RabbitMQ) ou chama repositório diretamente.
3. **Consumer** (Application) recebe eventos, aplica regra de negócio e persiste via **Repository**.
4. **Repository** (Infrastructure) acessa banco SQL Server.

---

## 🐳 Executando o Docker Compose

### 1) Pré-requisitos
- Docker Desktop (Windows/macOS) ou Docker Engine
- .NET 8 SDK

### 2) Serviços e portas

**Bancos SQL Server:**
- PessoaDb → `localhost:14331`
- PropostaDb → `localhost:14332`
- ContratacaoDb → `localhost:14333`

**RabbitMQ:**
- AMQP → `localhost:5672`
- Painel de administração → http://localhost:15672 (user: `admin`, senha: `admin123`)

**APIs:**
- Pessoa → http://localhost:5001
- Proposta → http://localhost:5002
- Contratação → http://localhost:5003

---

### 3) Subir containers
```bash
docker compose up -d --build
```

Verificar se todos subiram:
```bash
docker ps
```

---

### 4) Connection strings

**Dentro do Docker (entre containers):**
```
Server=gps-sqlserver-pessoa,1433;Database=PessoaDb;User Id=sa;Password=Senha123;TrustServerCertificate=True;Encrypt=False;
Server=gps-sqlserver-proposta,1433;Database=PropostaDb;User Id=sa;Password=Senha123;TrustServerCertificate=True;Encrypt=False;
Server=gps-sqlserver-contratacao,1433;Database=ContratacaoDb;User Id=sa;Password=Senha123;TrustServerCertificate=True;Encrypt=False;
```

**No host (VS/PMC):**
```
Server=localhost,14331;Database=PessoaDb;User Id=sa;Password=Senha123;TrustServerCertificate=True;Encrypt=False;
Server=localhost,14332;Database=PropostaDb;User Id=sa;Password=Senha123;TrustServerCertificate=True;Encrypt=False;
Server=localhost,14333;Database=ContratacaoDb;User Id=sa;Password=Senha123;TrustServerCertificate=True;Encrypt=False;
```

**RabbitMQ (MassTransit):**
- **Dentro do Docker:** `Host=gps-rabbitmq`
- **No host:** `Host=localhost`

---

## 📬 Mensageria com MassTransit + RabbitMQ

- Cada microserviço publica eventos no RabbitMQ via **MassTransit**.
- Os consumers ficam na camada **Application**, garantindo que a regra de negócio não dependa diretamente da infraestrutura.
- Padrão adotado:
  - Controller → Publica evento
  - Consumer → Recebe evento, executa regras, aciona o domínio/repositório
- Comunicação assíncrona entre os serviços Proposta, Contratação e Pessoa.

---
