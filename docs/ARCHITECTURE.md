# Arquitetura ClinicaSim

## 📋 Visão Geral

ClinicaSim é um **simulador clínico educacional** construído com **Clean Architecture** e **Domain-Driven Design (DDD)**. A aplicação permite que estudantes e profissionais de medicina treinem raciocínio clínico através de casos interativos.

## 🏗️ Estrutura de Camadas

```
┌─────────────────────────────────────────┐
│     Presentation Layer (API)            │
│     ClinicaSim.Api                      │
│  Controllers + DTOs + Swagger           │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│     Application Layer                   │
│     ClinicaSim.Application              │
│  Use Cases + Interfaces + Models        │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│     Domain Layer                        │
│     ClinicaSim.Domain                   │
│  Entities + Business Rules              │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│     Infrastructure Layer                │
│     ClinicaSim.Infrastructure           │
│  Data Access + External Services        │
└─────────────────────────────────────────┘
```

## 📦 Camadas em Detalhes

### 1. **Domain Layer** (`ClinicaSim.Domain`)
**Responsabilidade**: Lógica de negócio pura, independente de frameworks.

**Entidades**:
- `ClinicalCase` - Caso clínico (estrutura de paciente)
- `CaseSection` - Seções do caso (Anamnese, Exame Físico, etc)
- `CaseCategory` - Categorias dentro de seções
- `CaseQuestion` - Perguntas do caso
- `CaseAnswer` - Respostas pré-definidas
- `ConsultationSession` - Sessão ativa de simulação
- `InteractionEvent` - Evento de interação do usuário
- `ClinicalNote` - Histórico clínico preenchido
- `DifferentialDiagnosis` - Diagnósticos diferenciais ordenados

**Características**:
- ✅ Sem dependências externas
- ✅ Regras de negócio encapsuladas
- ✅ Testável isoladamente

---

### 2. **Application Layer** (`ClinicaSim.Application`)
**Responsabilidade**: Orquestração de use cases e definição de contratos.

**Interfaces de Serviço**:
```csharp
ICasesService
├── GetCasesAsync() → IEnumerable<CaseListItem>

ISessionsService
├── StartAsync(caseId) → SessionStartResult
├── RegisterEventAsync(code, questionId) → SessionEventResult
├── GetSessionAsync(code) → SessionInfoResult
├── GetNoteAsync(code) → ClinicalNoteModel
├── UpsertNoteAsync(code, note) → ClinicalNoteModel
├── GetDifferentialsAsync(code) → IEnumerable<DifferentialModel>
├── SaveDifferentialsAsync(code, items) → IEnumerable<DifferentialModel>
└── FinalizeAsync(code) → SessionStartResult

IPdfReportService
└── GenerateSessionPdfAsync(code) → byte[]
```

**Models** (DTOs de transferência):
- `CaseListItem` - Item de caso para listagem
- `SessionStartResult` - Resultado ao iniciar sessão
- `SessionEventResult` - Resultado de evento registrado
- `SessionInfoResult` - Informações da sessão
- `ClinicalNoteModel` - Dados de história clínica
- `DifferentialModel` - Diagnóstico diferencial

**Exceções**:
- `AppException` - Exceção padrão com status HTTP

---

### 3. **Infrastructure Layer** (`ClinicaSim.Infrastructure`)
**Responsabilidade**: Implementação de dados, serviços externos, configuração.

**Subpacotes**:

#### `Persistence`
- `ClinicaSimDbContext` - DbContext do EF Core com mapeamentos fluentes
- `ClinicaSimDbContextFactory` - Factory para migrations
- `DbSeeder` - Dados iniciais para desenvolvimento

#### `Services`
- `CasesService` - Implementação de ICasesService
- `SessionsService` - Implementação de ISessionsService (lógica complexa)
- `PdfReportService` - Geração de relatórios PDF

#### `Migrations`
- `20260901000000_InitialCreate` - Estrutura inicial do banco

#### `DependencyInjection`
- `ServiceCollectionExtensions` - Registro de serviços no IoC

---

### 4. **Presentation Layer** (`ClinicaSim.Api`)
**Responsabilidade**: Expor endpoints REST, validação de entrada, mapear DTOs.

**Controllers**:

#### `CasesController` [`/api/cases`]
```
GET / → Lista todos os casos disponíveis
```

#### `SessionsController` [`/api/sessions`]
```
POST /start                           → Inicia nova sessão
POST /{code}/events                   → Registra evento
GET /{code}                           → Infos da sessão
GET /{code}/note                      → Lê história clínica
POST /{code}/note                     → Salva história clínica
GET /{code}/differentials             → Lista diagnósticos
POST /{code}/differentials            → Salva diagnósticos
POST /{code}/finalize                 → Finaliza sessão
GET /{code}/pdf                       → Baixa relatório PDF
```

**DTOs**:
- Separados por contexto (Requests, Responses)
- Mapeados de/para Models da Application Layer

**Configuração** (`Program.cs`):
- Registro de DbContext
- Configuração CORS
- Swagger/OpenAPI
- Auto-migrations em desenvolvimento

---

## 🔄 Fluxo de Dados

```
Client Request (Angular)
        ↓
   [Controller]
        ↓
   [Map DTO → Model]
        ↓
   [Service Interface Call]
        ↓
   [Service Implementation]
        ↓
   [DbContext (EF Core)]
        ↓
   [PostgreSQL Database]
        ↓
   [Reverse Flow com Response]
```

---

## 📐 Padrões de Design Utilizados

### 1. **Clean Architecture**
- Independência de frameworks
- Testabilidade
- Separação de responsabilidades

### 2. **Dependency Injection**
- Serviços registrados no `ServiceCollectionExtensions`
- Injeção via construtor
- Facilita testes

### 3. **Repository Pattern** (via EF Core)
- DbContext atua como repository
- Queries através de LINQ

### 4. **Service Layer**
- Orquestração de lógica
- Transações
- Validações de negócio

### 5. **DTO Pattern**
- Separação entre Domain Entities e API Contracts
- Segurança (não expõe todos os campos)
- Versionamento de API facilitado

---

## 🔐 Responsabilidades por Camada

| Aspecto | Domain | Application | Infrastructure | Presentation |
|--------|--------|-------------|-----------------|--------------|
| **Validações de Negócio** | ✅ | ✅ | ❌ | ❌ |
| **Regras de Workflow** | ✅ | ✅ | ❌ | ❌ |
| **Acesso a Dados** | ❌ | ❌ | ✅ | ❌ |
| **HTTP/REST** | ❌ | ❌ | ❌ | ✅ |
| **Mapeamento de DTOs** | ❌ | ✅ | ❌ | ✅ |
| **Exceções** | ✅ | ✅ | ✅ | ✅ |

---

## 🚀 Fluxo de Desenvolvimento Recomendado

### Para Nova Feature

1. **Domain** → Criar entidade ou adicionar método
2. **Application** → Criar interface e model
3. **Infrastructure** → Implementar serviço
4. **Presentation** → Criar endpoint e DTOs
5. **Testes** → Cobertura por camada

### Para Novo Endpoint

1. Definir contrato (DTOs de request/response)
2. Criar método em IService
3. Implementar em Service
4. Criar action no Controller
5. Documentar no Swagger

---

## ⚙️ Stack Tecnológico

- **.NET 8** - Framework
- **C# 12** - Linguagem
- **Entity Framework Core 8** - ORM
- **PostgreSQL** - Banco (via Supabase)
- **Npgsql** - Driver PostgreSQL
- **Swagger/Swashbuckle** - API Documentation
- **Angular** - Frontend (separado)

---

## 📊 Decisões Arquiteturais

### 1. **Usar Clean Architecture**
**Por quê**: Escalabilidade, testabilidade, baixo acoplamento

### 2. **EF Core com PostgreSQL**
**Por quê**: Produtividade, migrations automáticas, Supabase para hospedagem

### 3. **REST API**
**Por quê**: Padrão industry, fácil integração com frontend

### 4. **DTOs**
**Por quê**: Versionar API sem quebrar Domain, segurança

### 5. **Session Codes Aleatórios**
**Por quê**: Privacidade, impossível adivinhar código de outra sessão

---

## 🔗 Referências

- [Clean Architecture - Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design - Eric Evans](https://www.domainlanguage.com/ddd/)
- [Microsoft .NET Architecture Guide](https://docs.microsoft.com/en-us/dotnet/architecture/)
