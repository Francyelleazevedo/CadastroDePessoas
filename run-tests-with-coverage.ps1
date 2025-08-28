# sonar-analysis-improved.ps1

param(
    [string]$SonarUrl = "http://localhost:9000",
    [string]$ProjectKey = "CadastroDePessoas",
    [string]$ProjectName = "Cadastro de Pessoas",
    [string]$ProjectVersion = "1.0",
    [string]$SonarToken = ""
)

$outputFolder = "TestResults"
$openCoverFile = "$outputFolder/coverage.opencover.xml"

# Verificar se o SonarQube está rodando
try {
    $response = Invoke-WebRequest -Uri $SonarUrl -Method Head -TimeoutSec 10 -ErrorAction Stop
    Write-Host "✅ SonarQube está rodando em $SonarUrl" -ForegroundColor Green
} catch {
    Write-Host "❌ SonarQube não está rodando em $SonarUrl" -ForegroundColor Red
    Write-Host "Execute: docker run -d --name sonarqube -p 9000:9000 sonarqube:latest" -ForegroundColor Yellow
    exit 1
}

# Verificar se as ferramentas estão instaladas
$tools = @(
    @{ Name = "dotnet-sonarscanner"; Command = "dotnet tool install --global dotnet-sonarscanner" },
    @{ Name = "dotnet-reportgenerator-globaltool"; Command = "dotnet tool install --global dotnet-reportgenerator-globaltool" }
)

foreach ($tool in $tools) {
    $toolPath = Get-Command $tool.Name -ErrorAction SilentlyContinue
    if ($null -eq $toolPath) {
        Write-Host "Instalando $($tool.Name)..." -ForegroundColor Yellow
        Invoke-Expression $tool.Command
    }
}

# Verificar se existem projetos de teste
$testProjects = Get-ChildItem -Recurse -Filter "*.csproj" | Where-Object { $_.Name -like "*Test*" -or $_.Name -like "*Teste*" }
if ($testProjects.Count -eq 0) {
    Write-Host "❌ Nenhum projeto de teste encontrado!" -ForegroundColor Red
    Write-Host "Certifique-se de ter projetos com 'Test' ou 'Teste' no nome." -ForegroundColor Yellow
    exit 1
}

Write-Host "📊 Projetos de teste encontrados: $($testProjects.Count)" -ForegroundColor Cyan
$testProjects | ForEach-Object { Write-Host "   - $($_.Name)" -ForegroundColor Gray }

# Executar testes com cobertura
Write-Host "`n🧪 Executando testes com cobertura..." -ForegroundColor Cyan
./run-tests-with-coverage.ps1

# Verificar se o relatório foi gerado
if (!(Test-Path $openCoverFile)) {
    Write-Host "❌ Arquivo de cobertura não foi gerado!" -ForegroundColor Red
    exit 1
}

# Configurar parâmetros do SonarQube
$sonarParameters = @(
    "/k:`"$ProjectKey`"",
    "/n:`"$ProjectName`"",
    "/v:`"$ProjectVersion`"",
    "/d:sonar.host.url=`"$SonarUrl`"",
    "/d:sonar.cs.opencover.reportsPaths=`"$openCoverFile`"",
    "/d:sonar.coverage.exclusions=`"**/*Tests.cs,**/*Testes.cs,**/Program.cs,**/Migrations/**`"",
    "/d:sonar.exclusions=`"**/bin/**,**/obj/**,**/TestResults/**`""
)

if ($SonarToken) {
    $sonarParameters += "/d:sonar.login=`"$SonarToken`""
}

$sonarParamsString = $sonarParameters -join " "

# Análise do SonarQube
Write-Host "`n🔍 Iniciando análise do SonarQube..." -ForegroundColor Cyan
try {
    Invoke-Expression "dotnet sonarscanner begin $sonarParamsString"
    
    Write-Host "🔨 Compilando o projeto..." -ForegroundColor Cyan
    dotnet build --no-incremental --configuration Release
    
    Write-Host "✅ Finalizando análise..." -ForegroundColor Cyan
    if ($SonarToken) {
        dotnet sonarscanner end /d:sonar.login="$SonarToken"
    } else {
        dotnet sonarscanner end
    }
    
    Write-Host "`n🎉 Análise concluída com sucesso!" -ForegroundColor Green
    Write-Host "📊 Dashboard: $SonarUrl/dashboard?id=$ProjectKey" -ForegroundColor Cyan
    Write-Host "📈 Relatório local: $(Resolve-Path "TestResults/CoverageReport/index.html")" -ForegroundColor Cyan
    
} catch {
    Write-Host "❌ Erro durante análise do SonarQube: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}