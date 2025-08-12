# Gerenciador de Propostas de Seguro

Este repositório contém **3 microserviços .NET 8** (Pessoa, Proposta e Contratação) seguindo **DDD + Arquitetura Hexagonal (Ports & Adapters) + Clean Architecture**, com **autenticação via ASP.NET Core Identity/JWT** (no serviço Pessoa) e **bancos SQL Server** dedicados para cada microserviço, orquestrados por **Docker Compose**.

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
- **Docker** e **Docker Compose**
- **DDD + Hexagonal + Clean Architecture**
- **Swagger/OpenAPI**

---

## 🐳 Executando o Docker Compose

### 1) Pré-requisitos
- Docker Desktop (Windows/macOS) ou Docker Engine
- .NET 8 SDK

### 2) Portas utilizadas
- SQL Server:
  - PessoaDb → 14331
  - PropostaDb → 14332
  - ContratacaoDb → 14333
- APIs:
  - Pessoa → 5005
  - Proposta → 5006
  - Contratação → 5007

### 3) Subir containers
docker compose up -d --build

Verificar:
docker ps

### 4) Connection strings (exemplo)
**Dentro do Docker**:
Server=sqlserver-pessoa,1433;Database=PessoaDb;User Id=sa;Password=Senha123;TrustServerCertificate=True;Encrypt=False;
**No host (VS/PMC)**:
Server=localhost,14331;Database=PessoaDb;User Id=sa;Password=Senha123;TrustServerCertificate=True;Encrypt=False;

---

## 🗃️ Migrations (exemplo)

### Criar
```powershell
Add-Migration CriacaoBD -Context PessoaDbContext -Project GPS.PessoaService.Infrastructure -StartupProject GPS.PessoaService.Api
```

### Aplicar
```powershell
Update-Database -Context PessoaDbContext -Project GPS.PessoaService.Infrastructure -StartupProject GPS.PessoaService.Api
```

---

## 🔐 Autenticação

- JWT configurado no **PessoaService**
- Claims incluem `sub` (UserId), `Email`, `PessoaId` e `Tipo`
- Proposta/Contratação validam usando mesmas configs de JWT

---

## 🌐 Endpoints
- Pessoa API: http://localhost:5005/swagger
- Proposta API: http://localhost:5006/swagger
- Contratação API: http://localhost:5007/swagger

---

## 🛠️ Problemas comuns

- **Host não é conhecido** → Usar `localhost,14331/2/3` no host
- **Porta em uso** → Alterar no docker-compose.yml
- **APIs não sobem** → Verificar logs (`docker logs <nome>`)

---
