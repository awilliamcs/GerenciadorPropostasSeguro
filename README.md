# Gerenciador de Propostas de Seguro

Este repositório contém **4 microserviços .NET 8** (Pessoa, Proposta, Contratação e Autenticação) seguindo **DDD + Arquitetura Hexagonal (Ports & Adapters) + Clean Architecture**, com **autenticação centralizada via GPS.AutenticacaoService (ASP.NET Core Identity/JWT)**, **comunicação assíncrona via MassTransit + RabbitMQ**, e **bancos SQL Server** dedicados para cada microserviço, orquestrados por **Docker Compose**. O fluxo de orquestração entre serviços utiliza o padrão **SAGA** para garantir consistência transacional distribuída.

---


## 📂 Estrutura

```
src/
    GPS.AutenticacaoService.Api/
    GPS.AutenticacaoService.Application/
    GPS.AutenticacaoService.Domain/
    GPS.AutenticacaoService.Infrastructure/

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


## 🧰 Tecnologias Utilizadas

- **.NET 8 / C#**
- **ASP.NET Core Web API**
- **Entity Framework Core** (SQL Server)
- **ASP.NET Core Identity** (centralizado no AutenticacaoService) + **JWT**
- **MassTransit** (mensageria e orquestração SAGA)
- **RabbitMQ** (mensageria)
- **Docker** e **Docker Compose**
- **DDD + Hexagonal + Clean Architecture**
- **Swagger/OpenAPI**

---


## 📜 Arquitetura, Camadas e Fluxo SAGA

```mermaid
flowchart LR
    subgraph API[Camada API (Controllers)]
        C0[Controller Autenticacao]
        C1[Controller Pessoa]
        C2[Controller Proposta]
        C3[Controller Contratação]
    end

    subgraph APP[Camada Application (Serviços, Consumers, Sagas)]
        S0[AutenticacaoService]
        S1[PessoaService]
        S2[PropostaService]
        S3[ContratacaoService]
        Saga[SAGA Orquestrador]
    end

    subgraph DOMAIN[Camada Domain (Entidades & Interfaces)]
        E0[Entidades Autenticacao]
        E1[Entidades]
        I1[Interfaces Repositório]
    end

    subgraph INFRA[Camada Infrastructure (Repositórios, DB, Mensageria)]
        R0[Repos Autenticacao]
        R1[Repos Pessoa]
        R2[Repos Proposta]
        R3[Repos Contratação]
        MQ[RabbitMQ]
        DB0[(AutenticacaoDb)]
        DB1[(PessoaDb)]
        DB2[(PropostaDb)]
        DB3[(ContratacaoDb)]
    end

    C0 --> S0
    C1 --> S1
    C2 --> S2
    C3 --> S3

    S0 --> E0
    S1 --> I1
    S2 --> I1
    S3 --> I1

    E0 --> R0
    I1 --> R1
    I1 --> R2
    I1 --> R3

    S0 <-->|Eventos| MQ
    S1 <-->|Eventos| MQ
    S2 <-->|Eventos| MQ
    S3 <-->|Eventos| MQ
    Saga <-->|Orquestração| MQ

    R0 --> DB0
    R1 --> DB1
    R2 --> DB2
    R3 --> DB3
```

**Fluxo SAGA simplificado:**
1. **Controller** recebe requisição HTTP e chama o **Serviço da Application**.
2. Serviço publica evento via **MassTransit** (RabbitMQ) ou chama repositório diretamente.
3. **Saga Orquestrador** coordena múltiplos microserviços, garantindo consistência transacional distribuída.
4. **Consumer** (Application) recebe eventos, aplica regra de negócio e persiste via **Repository**.
5. **Repository** (Infrastructure) acessa banco SQL Server.

---


## 🐳 Executando o Docker Compose

### 1) Pré-requisitos
- Docker Desktop (Windows/macOS) ou Docker Engine
- .NET 8 SDK

### 2) Serviços e portas

**Bancos SQL Server:**
- AutenticacaoDb → `localhost:14330`
- PessoaDb → `localhost:14331`
- PropostaDb → `localhost:14332`
- ContratacaoDb → `localhost:14333`

**RabbitMQ:**
- AMQP → `localhost:5672`
- Painel de administração → http://localhost:15672 (user: `admin`, senha: `admin123`)

**APIs:**
- Autenticacao → http://localhost:5000
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
Server=gps-sqlserver-autenticacao,1433;Database=AutenticacaoDb;User Id=sa;Password=Senha123;TrustServerCertificate=True;Encrypt=False;
Server=gps-sqlserver-pessoa,1433;Database=PessoaDb;User Id=sa;Password=Senha123;TrustServerCertificate=True;Encrypt=False;
Server=gps-sqlserver-proposta,1433;Database=PropostaDb;User Id=sa;Password=Senha123;TrustServerCertificate=True;Encrypt=False;
Server=gps-sqlserver-contratacao,1433;Database=ContratacaoDb;User Id=sa;Password=Senha123;TrustServerCertificate=True;Encrypt=False;
```

**No host (VS/PMC):**
```
Server=localhost,14330;Database=AutenticacaoDb;User Id=sa;Password=Senha123;TrustServerCertificate=True;Encrypt=False;
Server=localhost,14331;Database=PessoaDb;User Id=sa;Password=Senha123;TrustServerCertificate=True;Encrypt=False;
Server=localhost,14332;Database=PropostaDb;User Id=sa;Password=Senha123;TrustServerCertificate=True;Encrypt=False;
Server=localhost,14333;Database=ContratacaoDb;User Id=sa;Password=Senha123;TrustServerCertificate=True;Encrypt=False;
```

**RabbitMQ (MassTransit):**
- **Dentro do Docker:** `Host=gps-rabbitmq`
- **No host:** `Host=localhost`

---


## 📬 Mensageria, Orquestração SAGA e MassTransit + RabbitMQ

- Cada microserviço publica eventos no RabbitMQ via **MassTransit**.
- Os consumers e orquestradores SAGA ficam na camada **Application**, garantindo que a regra de negócio e a consistência transacional distribuída não dependam diretamente da infraestrutura.
- Padrão adotado:
    - Controller → Publica evento
    - Saga → Orquestra e coordena múltiplos microserviços
    - Consumer → Recebe evento, executa regras, aciona o domínio/repositório
- Comunicação assíncrona entre os serviços Autenticacao, Proposta, Contratação e Pessoa.
- O fluxo SAGA garante rollback e compensação em caso de falha entre etapas distribuídas.

---
