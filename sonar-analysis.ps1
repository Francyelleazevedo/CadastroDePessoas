# sonar-analysis.ps1

# Configurações do SonarQube
$sonarUrl = "http://localhost:9000" # Altere para a URL do seu servidor SonarQube
$projectKey = "CadastroDePessoas"
$projectName = "Cadastro de Pessoas"
$projectVersion = "1.0"
$openCoverReportPath = "TestResults/coverage.opencover.xml"
$sonarToken = "" # Se necessário, adicione o token de autenticação

# Verificar se as ferramentas do SonarQube estão instaladas
$sonarScannerPath = Get-Command dotnet-sonarscanner -ErrorAction SilentlyContinue
if ($null -eq $sonarScannerPath) {
    Write-Host "Instalando dotnet-sonarscanner..." -ForegroundColor Yellow
    dotnet tool install --global dotnet-sonarscanner
}

# Primeiro, execute os testes para gerar o relatório de cobertura
Write-Host "Executando testes com cobertura de código..." -ForegroundColor Cyan
./run-tests-with-coverage.ps1

# Verificar se o relatório de cobertura foi gerado
if (!(Test-Path $openCoverReportPath)) {
    Write-Host "Arquivo de cobertura OpenCover não foi gerado. Execute os testes primeiro." -ForegroundColor Red
    exit 1
}

# Configurar parâmetros do SonarQube
$sonarParameters = "/k:""$projectKey"" /n:""$projectName"" /v:""$projectVersion"" /d:sonar.host.url=""$sonarUrl"" /d:sonar.cs.opencover.reportsPaths=""$openCoverReportPath"" /d:sonar.coverage.exclusions=""**/*Tests.cs,**/*Testes.cs,**/Program.cs,**/Migrations/**"""

# Adicionar token se fornecido
if ($sonarToken) {
    $sonarParameters += " /d:sonar.login=""$sonarToken"""
}

# Iniciar análise do SonarQube
Write-Host "Iniciando análise do SonarQube..." -ForegroundColor Cyan
Invoke-Expression "dotnet sonarscanner begin $sonarParameters"

# Compilar o projeto
Write-Host "Compilando o projeto..." -ForegroundColor Cyan
dotnet build --no-incremental

# Finalizar análise do SonarQube
Write-Host "Finalizando análise do SonarQube..." -ForegroundColor Cyan
if ($sonarToken) {
    dotnet sonarscanner end /d:sonar.login="$sonarToken"
} else {
    dotnet sonarscanner end
}

# Exibir resultados
Write-Host "Análise concluída. Verifique os resultados no SonarQube:" -ForegroundColor Green
Write-Host "$sonarUrl/dashboard?id=$projectKey" -ForegroundColor Cyan

# Instruções para executar o SonarQube localmente com Docker
Write-Host "`n= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =" -ForegroundColor Yellow
Write-Host "Para executar o SonarQube localmente com Docker:" -ForegroundColor Yellow
Write-Host "docker run -d --name sonarqube -p 9000:9000 sonarqube:latest" -ForegroundColor Cyan
Write-Host "Acesse: http://localhost:9000 (usuário/senha padrão: admin/admin)" -ForegroundColor Cyan
Write-Host "= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =" -ForegroundColor Yellow
