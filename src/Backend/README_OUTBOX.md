# Outbox Pattern - Configuração MassTransit + Entity Framework

Este documento explica como o Outbox pattern foi configurado no projeto para garantir que eventos/mensagens só sejam enviados após o `SaveChanges()` do Entity Framework.

## O que é o Outbox Pattern?

O Outbox pattern é uma técnica que garante que eventos sejam publicados de forma confiável junto com mudanças no banco de dados. Ele funciona armazenando os eventos em tabelas temporárias (Outbox) na mesma transação que os dados de negócio, e depois enviando esses eventos para o message broker.

## Configuração Implementada

### 1. Pacotes Adicionados
- `MassTransit.EntityFrameworkCore` versão 8.2.0 foi adicionado aos projetos Infrastructure

### 2. Método de Configuração
Foi criado o método `AddMassTransitWithRabbitMqAndOutbox<TDbContext>()` em `GPS.CrossCutting/Messaging/ServiceCollectionExtensions.cs`:

```csharp
public static IServiceCollection AddMassTransitWithRabbitMqAndOutbox<TDbContext>(
    this IServiceCollection services, 
    IConfiguration configuration, 
    Action<IBusRegistrationConfigurator>? configureConsumers = null)
    where TDbContext : DbContext
```

### 3. DbContexts Atualizados
Todos os DbContexts foram atualizados para incluir as tabelas do Outbox:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    
    // Configurar tabelas do Outbox do MassTransit
    modelBuilder.AddInboxStateEntity();
    modelBuilder.AddOutboxMessageEntity();
    modelBuilder.AddOutboxStateEntity();
    
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(SeuDbContext).Assembly);
}
```

### 4. Migrations Criadas
Foram criadas migrations `AddOutboxTables` em todos os serviços para criar as tabelas necessárias no banco de dados.

## Como Usar

### Exemplo Prático

```csharp
public async Task<Guid> CriarPropostaComEventoAsync(
    Guid idPessoa, 
    TipoProposta tipo,
    decimal valor)
{
    using var transaction = await _context.Database.BeginTransactionAsync();
    
    try
    {
        // 1. Criar e salvar a entidade
        var proposta = new Proposta(idPessoa, tipo, valor);
        await _propostaRepository.SalvarAsync(proposta);

        // 2. Publicar evento - fica no Outbox até SaveChanges()
        var evento = new PropostaCriadaEvent
        {
            PropostaId = proposta.IdProposta,
            Valor = proposta.Valor,
            DataCriacao = proposta.DataSolicitacao,
            ClienteId = proposta.IdPessoa
        };

        await _publishEndpoint.Publish(evento);

        // 3. SaveChanges() - AQUI o evento é enviado para o RabbitMQ
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return proposta.IdProposta;
    }
    catch
    {
        await transaction.RollbackAsync(); // Evento não será enviado
        throw;
    }
}
```

### Benefícios

1. **Garantia de Entrega**: O evento só é enviado se a transação de banco for bem-sucedida
2. **Consistência**: Dados e eventos sempre ficam sincronizados
3. **Recuperação**: Se o envio falhar, o MassTransit tentará novamente
4. **Transacional**: Tudo acontece na mesma transação de banco

### Exemplo de Controller

Foi criado um exemplo em `PropostaOutboxController` mostrando como usar:

```csharp
[HttpPost("criar-com-evento")]
public async Task<IActionResult> CriarPropostaComEvento([FromBody] CriarPropostaRequest request)
{
    var propostaId = await _propostaOutboxService.CriarPropostaComEventoAsync(
        request.IdPessoa,
        request.Tipo,
        request.Valor);

    return Ok(new { PropostaId = propostaId });
}
```

## Configuração nos Serviços

### PropostaService
- ✅ Configurado com `AddMassTransitWithRabbitMqAndOutbox<PropostaDbContext>()`
- ✅ Migration `AddOutboxTables` criada
- ✅ Exemplo de uso implementado

### ContratacaoService
- ✅ Configurado com `AddMassTransitWithRabbitMqAndOutbox<ContratacaoDbContext>()`
- ✅ Migration `AddOutboxTables` criada

### PessoaService
- ✅ Configurado com `AddMassTransitWithRabbitMqAndOutbox<PessoaDbContext>()`
- ✅ Migration `AddOutboxTables` criada

## Tabelas Criadas no Banco

O Outbox pattern criará as seguintes tabelas em cada banco de dados:
- `InboxState` - Para garantir que mensagens não sejam processadas duas vezes
- `OutboxMessage` - Armazena eventos/mensagens antes do envio
- `OutboxState` - Controla o estado do Outbox

## Próximos Passos

1. Execute as migrations: `dotnet ef database update`
2. Teste o endpoint de exemplo
3. Adapte o padrão para seus casos de uso específicos
4. Monitore as tabelas do Outbox para verificar o funcionamento

## Notas Importantes

- O Outbox só funciona quando você usa `SaveChanges()` do Entity Framework
- Eventos publicados fora de uma transação de banco NÃO usarão o Outbox
- É importante sempre usar transações quando quiser garantir a consistência
