# Cadastro de Pessoas - Aplicação Fullstack

Uma solução completa de gerenciamento de cadastros desenvolvida com .NET 8 (backend) e React 18 (frontend), demonstrando boas práticas de desenvolvimento, arquitetura limpa e utilização de tecnologias modernas.

## 🚀 Tecnologias Utilizadas

### Backend (.NET 8)
- **Framework**: .NET 8 Web API
- **Arquitetura**: Clean Architecture + CQRS + MediatR
- **Banco de Dados**: SQLite (para facilitar execução)
- **ORM**: Entity Framework Core 8
- **Cache**: Redis (opcional) ou MemoryCache
- **Autenticação**: JWT Bearer Token
- **Validação**: FluentValidation
- **Documentação**: Swagger/OpenAPI
- **Testes**: xUnit com cobertura de código

### Frontend (React 18)
- **Framework**: React 18 com Vite
- **UI Framework**: Chakra UI
- **Roteamento**: React Router DOM
- **Formulários**: React Hook Form + Yup
- **Estado Global**: Context API
- **HTTP Client**: Axios
- **Icons**: React Icons

## 🏗️ Estrutura do Projeto

```
CadastroDePessoas/
├── CadastroDePessoas.API/              # Camada de apresentação (Controllers, Middlewares)
├── CadastroDePessoas.Application/      # Camada de aplicação (CQRS, Services, DTOs)
├── CadastroDePessoas.Domain/           # Camada de domínio (Entidades, Interfaces, Validações)
├── CadastroDePessoas.Infra/            # Camada de infraestrutura (Repository, Context, Cache)
├── CadastroDePessoas.Infra.IoC/        # Injeção de dependência
├── CadastroDePessoas.*.Testes/         # Projetos de teste para cada camada
└── cadastrodepessoas-web/              # Frontend React
```

### Arquitetura Backend

O projeto segue os princípios da **Clean Architecture** com separação clara de responsabilidades:

- **API Layer**: Controllers, Middlewares, Filtros, Configurações
- **Application Layer**: CQRS Commands/Queries, Handlers, Validators, Services, DTOs
- **Domain Layer**: Entidades, Enums, Interfaces, Regras de Negócio
- **Infrastructure Layer**: Repositórios, Context, Migrations, Cache

## 🚀 Como Executar o Projeto

### Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) (v16 ou superior)
- [Docker](https://www.docker.com/) (opcional, para Redis)

### 1. Backend (.NET API)

```bash
# 1. Clone o repositório
git clone https://github.com/Francyelleazevedo/CadastroDePessoas.git
cd CadastroDePessoas

# 2. Restaure as dependências
dotnet restore

# 3. Execute o projeto API
cd CadastroDePessoas.API
dotnet run
```

O backend estará disponível em:
- **HTTPS**: `https://localhost:5001`
- **HTTP**: `http://localhost:5000`
- **Swagger**: `https://localhost:5001/swagger`

### 2. Frontend (React)

```bash
# 1. Navegue até a pasta do frontend
cd cadastrodepessoas-web

# 2. Instale as dependências
npm install

# 3. Execute o frontend
npm run dev
```

O frontend estará disponível em `http://localhost:3000`

## 🗄️ Banco de Dados

A aplicação utiliza **SQLite** por padrão para facilitar a execução sem configurações adicionais. O banco de dados é criado automaticamente na primeira execução.

### Localização do Banco
- **Desenvolvimento**: `CadastroDePessoas.Infra/cadastro_pessoas.db`
- **Produção**: Configurado via variáveis de ambiente

### Migrações
```bash
# Adicionar nova migração
dotnet ef migrations add NomeDaMigracao --project CadastroDePessoas.Infra --startup-project CadastroDePessoas.API

# Aplicar migrações
dotnet ef database update --project CadastroDePessoas.Infra --startup-project CadastroDePessoas.API
```

## 🔄 Sistema de Cache

A aplicação suporta dois tipos de cache:

### 1. Cache em Memória (Padrão)
- Configuração automática, sem dependências externas
- Ideal para desenvolvimento e ambientes simples

### 2. Cache com Redis (Opcional)

Para usar Redis como provedor de cache:

```bash
# 1. Execute o Redis com Docker
docker run --name redis-cache -p 6379:6379 -d redis

# 2. Configure no appsettings.json
{
  "ConnectionStrings": {
    "Redis": "localhost:6379"
  },
  "UseRedisCache": "true"
}
```

## 🔐 Autenticação e Autorização

A aplicação implementa autenticação JWT com:

- **Registro de usuários** com validação de senha forte
- **Login** com geração de token JWT
- **Validação de token** em todas as rotas protegidas
- **Middleware de segurança** com rate limiting e validação de origem
- **Perfil de usuário** com alteração de senha

### Endpoints de Autenticação

```
POST /api/v1/auth/registrar    # Registro de usuário
POST /api/v1/auth/login        # Login
GET  /api/v1/auth/perfil       # Perfil do usuário
PUT  /api/v1/auth/alterar-senha # Alterar senha
GET  /api/v1/auth/verificar-token # Verificar token
```

## 📝 API Documentation

A documentação da API está disponível via Swagger com duas versões:

- **V1**: Endereço opcional para pessoas
- **V2**: Endereço obrigatório para pessoas

### Principais Endpoints

#### Pessoas
```
GET    /api/v1/pessoa          # Listar todas as pessoas
GET    /api/v1/pessoa/{id}     # Obter pessoa por ID
POST   /api/v1/pessoa          # Criar nova pessoa
PUT    /api/v1/pessoa          # Atualizar pessoa
DELETE /api/v1/pessoa/{id}     # Excluir pessoa
```

#### Cache
```
POST   /api/v1/pessoa/limpar-cache  # Limpar cache
```

## 🧪 Testes

O projeto possui uma ampla cobertura de testes unitários para todas as camadas:

### Executar Testes
```bash
# Executar todos os testes
dotnet test

# Executar testes com cobertura
./run-tests-with-coverage.ps1
```

### Projetos de Teste
- `CadastroDePessoas.API.Testes`
- `CadastroDePessoas.Application.Testes`
- `CadastroDePessoas.Domain.Testes`
- `CadastroDePessoas.Infra.Testes`

### Relatório de Cobertura

Para gerar relatório de cobertura detalhado:

```powershell
# Windows (PowerShell)
./run-tests-with-coverage.ps1

# O relatório será gerado em: TestResults/CoverageReport/index.html
```

## ✨ Funcionalidades Implementadas

### Funcionalidades Principais
- ✅ **Cadastro completo de pessoas** com validações
- ✅ **Listagem paginada** de pessoas cadastradas
- ✅ **Edição e exclusão** de cadastros
- ✅ **Validação de CPF** com algoritmo oficial
- ✅ **Endereços opcionais** (V1) ou obrigatórios (V2)
- ✅ **Interface responsiva** e amigável

### Funcionalidades Extras
- ✅ **Sistema de Autenticação** JWT completo
- ✅ **Perfil de Usuário** com gerenciamento de senha
- ✅ **Versionamento de API** (V1 e V2)
- ✅ **Cache Redis** para melhor performance
- ✅ **Documentação Swagger** completa
- ✅ **Testes automatizados** com alta cobertura
- ✅ **Validações avançadas** com FluentValidation
- ✅ **Middlewares de segurança** personalizados
- ✅ **Tratamento de erros** robusto
- ✅ **CQRS + MediatR** para separação de responsabilidades
- ✅ **Docker Ready** com Dockerfile incluído

## 🔧 Configurações

### Variáveis de Ambiente (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=cadastro_pessoas.db",
    "Redis": "localhost:6379"
  },
  "UseRedisCache": "false",
  "Jwt": {
    "Chave": "sua-chave-secreta-muito-segura",
    "Emissor": "CadastroPessoasAPI",
    "Audiencia": "CadastroPessoasCliente",
    "ExpiracionHoras": "1"
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "https://localhost:3000"
    ]
  }
}
```

## 🐳 Docker

Para executar com Docker:

```bash
# Construir a imagem
docker build -t cadastro-pessoas-api .

# Executar o container
docker run -p 5000:5000 -p 5001:5001 cadastro-pessoas-api
```

## 🏛️ Padrões Arquiteturais Implementados

### CQRS (Command Query Responsibility Segregation)
- **Commands**: Para operações de escrita (Create, Update, Delete)
- **Queries**: Para operações de leitura (Get, List)
- **Handlers**: Processamento isolado de cada operação

### Repository Pattern
- Abstração da camada de dados
- Facilita testes unitários
- Implementação com Entity Framework

### Dependency Injection
- Configuração centralizada no `InjecaoDependencia.cs`
- Baixo acoplamento entre camadas
- Facilita manutenibilidade e testes

### Validation Pattern
- FluentValidation para regras de negócio
- ValidationBehavior com MediatR Pipeline
- Validações centralizadas e reutilizáveis

## 📊 Validações Implementadas

### Pessoa
- **Nome**: Obrigatório, máximo 100 caracteres
- **CPF**: Validação com algoritmo oficial
- **Email**: Formato válido (quando informado)
- **Data Nascimento**: Entre 01/01/1900 e data atual
- **Endereço**: Opcional (V1) ou Obrigatório (V2)

### Usuário
- **Nome**: Obrigatório, 2-100 caracteres
- **Email**: Formato válido, único
- **Senha**: Mínimo 6 caracteres, com letras, números e símbolos

## 🔒 Segurança

### Middlewares de Segurança
- **CORS**: Configuração de origens permitidas
- **Rate Limiting**: Proteção contra ataques de força bruta
- **Security Headers**: Headers de segurança HTTP
- **JWT Validation**: Validação rigorosa de tokens

### Validação de Entrada
- Sanitização de dados de entrada
- Validação de tipos e formatos
- Proteção contra SQL Injection
- Tratamento seguro de exceções

## 🚨 Tratamento de Erros

### API
- Filtros de exceção globais
- Logs estruturados
- Respostas padronizadas
- Status codes apropriados

### Frontend
- Tratamento de erros de conexão
- Mensagens amigáveis ao usuário
- Redirecionamentos automáticos
- Validação em tempo real

## 📈 Performance

### Cache Strategy
- Cache de consultas frequentes
- Invalidação inteligente
- TTL configurável
- Fallback para memória

### Otimizações Frontend
- Code splitting
- Lazy loading
- Minificação automática
- Chunks otimizados

## 🤝 Contribuição

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/nova-feature`)
3. Commit suas mudanças (`git commit -m 'Adiciona nova feature'`)
4. Push para a branch (`git push origin feature/nova-feature`)
5. Abra um Pull Request

### Padrões de Código
- Use as convenções do C# (.NET)
- Siga os padrões do ESLint (React)
- Mantenha alta cobertura de testes
- Documente métodos públicos

## 📄 Licença

Este projeto está licenciado sob a licença MIT - veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## 👨‍💻 Autor

**Francyelle Azevedo**
- GitHub: [@Francyelleazevedo](https://github.com/Francyelleazevedo)

---

## 📝 Notas de Desenvolvimento

### Scripts Úteis

```bash
# Backend
dotnet build                    # Compilar
dotnet test                     # Executar testes
dotnet run --project CadastroDePessoas.API  # Executar API

# Frontend
npm run dev                     # Desenvolvimento
npm run build                   # Build de produção
npm run lint                    # Verificar código
```

### Estrutura de Pastas Frontend

```
src/
├── components/                 # Componentes reutilizáveis
├── contexts/                   # Context API
├── hooks/                      # Custom hooks
├── pages/                      # Páginas da aplicação
├── services/                   # Serviços de API
├── utils/                      # Utilitários e helpers
└── routes.jsx                  # Configuração de rotas
```

Este projeto demonstra uma implementação completa de uma aplicação moderna, seguindo as melhores práticas de desenvolvimento e arquitetura de software.
