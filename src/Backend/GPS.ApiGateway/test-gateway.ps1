# Script para testar o API Gateway GPS
param(
    [string]$GatewayUrl = "http://localhost:5004"
)

Write-Host "=== Testando GPS API Gateway ===" -ForegroundColor Green
Write-Host "Gateway URL: $GatewayUrl" -ForegroundColor Yellow
Write-Host ""

# Função para fazer requisições HTTP
function Test-Endpoint {
    param(
        [string]$Url,
        [string]$Description,
        [string]$Method = "GET"
    )
    
    Write-Host "Testando: $Description" -ForegroundColor Cyan
    Write-Host "URL: $Url" -ForegroundColor Gray
    
    try {
        $response = Invoke-RestMethod -Uri $Url -Method $Method -ErrorAction Stop
        Write-Host "✅ Sucesso!" -ForegroundColor Green
        if ($response) {
            $response | ConvertTo-Json -Depth 3 | Write-Host
        }
        Write-Host ""
        return $true
    }
    catch {
        Write-Host "❌ Erro: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host ""
        return $false
    }
}

# Testa se o gateway está respondendo
Write-Host "1. Verificando saúde do Gateway" -ForegroundColor Magenta
Test-Endpoint -Url "$GatewayUrl/api/gateway/health" -Description "Health Check do Gateway"

# Testa documentação de rotas
Write-Host "2. Verificando documentação das rotas" -ForegroundColor Magenta
Test-Endpoint -Url "$GatewayUrl/api/gateway/routes" -Description "Documentação das Rotas"

# Testa rotas dos microserviços (estas podem falhar se os serviços não estiverem rodando)
Write-Host "3. Testando roteamento para microserviços" -ForegroundColor Magenta
Write-Host "Nota: Estes testes podem falhar se os microserviços não estiverem executando" -ForegroundColor Yellow
Write-Host ""

Test-Endpoint -Url "$GatewayUrl/api/autenticacao" -Description "Rota para AutenticacaoService"
Test-Endpoint -Url "$GatewayUrl/api/pessoa" -Description "Rota para PessoaService"
Test-Endpoint -Url "$GatewayUrl/api/proposta" -Description "Rota para PropostaService"
Test-Endpoint -Url "$GatewayUrl/api/contratacao" -Description "Rota para ContratacaoService"

Write-Host "=== Teste do API Gateway Concluído ===" -ForegroundColor Green
Write-Host ""
Write-Host "Para testar com Docker:" -ForegroundColor Yellow
Write-Host "docker-compose up" -ForegroundColor Gray
Write-Host ""
Write-Host "Para acessar o Swagger:" -ForegroundColor Yellow
Write-Host "$GatewayUrl/swagger" -ForegroundColor Gray
