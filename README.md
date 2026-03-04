# 🏥 ClinicaSim

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-14%2B-blue)](https://www.postgresql.org)
[![Angular](https://img.shields.io/badge/Angular-16%2B-red)](https://angular.io)
![Status](https://img.shields.io/badge/status-MVP-green)

**ClinicaSim** é uma plataforma de simulação clínica educacional que permite que estudantes e profissionais de medicina treinem raciocínio clínico através de casos interativos.

## 🎯 Sobre o Projeto

ClinicaSim é um **simulador clínico baseado em casos** que oferece:

- 👨‍⚕️ **Simulação Realística**: Casos clínicos com estrutura de anamnese, exame físico e investigação
- 📝 **História Clínica**: Preenchimento estruturado de diagnóstico e conduta
- 🔀 **Diagnósticos Diferenciais**: Ordenação por probabilidade de acerto
- 📊 **Relatórios PDF**: Exportação completa da sessão
- 🎓 **Educacional**: Ideal para treino de estudantes de medicina e residentes

## ⚡ Quick Start

### 1️⃣ Pré-requisitos
```bash
# .NET 8
dotnet --version  # >= 8.0

# PostgreSQL (ou Docker)
psql --version    # >= 14.0
```

### 2️⃣ Clone e Configure
```bash
git clone https://github.com/euthiagochaves/clinicasim.git
cd clinicasim

# Copie template de ambiente
cp .env.example .env.local
# Edite com suas credenciais
```

### 3️⃣ Rode Backend
```bash
cd src

# Restaurar e executar migrations
dotnet restore
dotnet ef database update -p ClinicaSim.Infrastructure

# Rodar API
dotnet run --project ClinicaSim.Api
```

**API disponível em**: `http://localhost:5000`

### 4️⃣ Teste
```bash
# Swagger interativo
http://localhost:5000/swagger

# Health check
curl http://localhost:5000/health
```

**[→ Documentação completa de setup](./docs/SETUP.md)**

---

## 📚 Documentação Completa

| Documento | Conteúdo |
|-----------|----------|
| **[SETUP.md](./docs/SETUP.md)** | Instalação, configuração, troubleshooting |
| **[ARCHITECTURE.md](./docs/ARCHITECTURE.md)** | Arquitetura, design patterns, decisões técnicas |
| **[DATABASE.md](./docs/DATABASE.md)** | Banco de dados, schemas, migrations, performance |
| **[BUSINESS_RULES.md](./docs/BUSINESS_RULES.md)** | Regras de negócio, validações, workflows |
| **[API.md](./docs/API.md)** | Referência completa de endpoints REST |
| **[CONTRIBUTING.md](./docs/CONTRIBUTING.md)** | Como contribuir, guia de código, PR workflow |

---

## 🏗️ Arquitetura

```
┌────────────────────────────────┐
│  Angular Frontend (4200)       │
└────────────────┬───────────────┘
                 │ REST API
┌────────────────▼───────────────┐
│  .NET 8 API (5000)             │
│  • Controllers                 │
│  • Swagger/OpenAPI             │
└────────────────┬───────────────┘
                 │
    ┌────────────┼────────────┐
    ↓            ↓            ↓
┌─────────┐ ┌──────────┐ ┌──────────┐
│ Domain  │ │ Application
│ (Rules) │ │ (Use Cases)
└─────────┘ └──────────┘ └──────────┘
    │            │            │
    └────────────┼────────────┘
                 ↓
         ┌──────────────────┐
         │ Infrastructure   │
         │ • EF Core        │
         │ • PostgreSQL     │
         │ • Services       │
         └──────────────────┘
```

---

## 📡 API Endpoints

### Cases
```
GET /api/cases                              Lista casos disponíveis
```

### Sessions
```
POST   /api/sessions/start                  Inicia nova sessão
GET    /api/sessions/{code}                 Infos da sessão
POST   /api/sessions/{code}/events          Registra evento
GET    /api/sessions/{code}/note            Lê história clínica
POST   /api/sessions/{code}/note            Salva história clínica
GET    /api/sessions/{code}/differentials   Lista diagnósticos
POST   /api/sessions/{code}/differentials   Salva diagnósticos
POST   /api/sessions/{code}/finalize        Finaliza sessão
GET    /api/sessions/{code}/pdf             Baixa relatório PDF
```

**[→ Referência completa de API](./docs/API.md)**

---

## 🔄 Fluxo de Uso

```
1. GET /api/cases              → Lista de casos
            ↓
2. POST /api/sessions/start    → Inicia com caso escolhido
            ↓
3. POST /api/sessions/{}/events (múltiplas vezes)
            ↓
4. POST /api/sessions/{}/note  → Preenche história clínica
            ↓
5. POST /api/sessions/{}/differentials → Ordena DDx
            ↓
6. POST /api/sessions/{}/finalize → Finaliza
            ↓
7. GET /api/sessions/{}/pdf    → Baixa relatório
```

---

## 📋 Regras Principais

- ✅ **Session Code**: Aleatório, 10 caracteres, único
- ✅ **Casos Imutáveis**: Não podem ser modificados após criação
- ✅ **Operações em Sessão Ativa**: Apenas se `status = "Active"`
- ✅ **Auditoria Completa**: Cada evento registrado com timestamp
- ✅ **Isolamento de Dados**: Usuários não veem sessões uns dos outros

**[→ Todas as 21+ regras de negócio](./docs/BUSINESS_RULES.md)**

---

## 🗄️ Banco de Dados

**PostgreSQL** com 9 tabelas principais:

```
clinical_cases (1) ─── (N) case_sections (1) ─── (N) case_categories
                                                           │
                                                           └─── (N) case_questions (1) ─── (1) case_answers

consultation_sessions (1) ─┬─ (N) interaction_events
                           ├─ (1) clinical_notes
                           └─ (N) differential_diagnoses
```

**[→ Diagrama ER completo e descrição de tabelas](./docs/DATABASE.md)**

---

## 🧪 Testes

```bash
# Rodar todos os testes
dotnet test

# Com cobertura de código
dotnet test /p:CollectCoverage=true

# Teste específico
dotnet test ClinicaSim.Domain.Tests

# Watch mode
dotnet watch test
```

**Objetivo**: 80%+ cobertura

---

## 🛠️ Stack Tecnológico

| Layer | Tecnologia |
|-------|-----------|
| **Frontend** | Angular 16+ |
| **Backend** | .NET 8 C# |
| **ORM** | Entity Framework Core 8 |
| **Database** | PostgreSQL 14+ |
| **API** | REST + OpenAPI/Swagger |
| **Reports** | PDF Generation |
| **Container** | Docker (opcional) |

---

## 📦 Estrutura de Projeto

```
clinicasim/
├── src/
│   ├── ClinicaSim.Api/              # Controllers, DTOs
│   ├── ClinicaSim.Application/      # Use cases, interfaces
│   ├── ClinicaSim.Domain/           # Entidades, regras
│   └── ClinicaSim.Infrastructure/   # Data access, serviços
├── frontend/                         # Angular app
├── docs/                             # Documentação
├── .env.example                      # Template de ambiente
├── README.md                         # Este arquivo
└── ClinicaSim.sln
```

---

## 🚀 Desenvolvimento

### Setup Local
```bash
cd src

# Restaurar dependências
dotnet restore

# Migrations
dotnet ef database update -p ClinicaSim.Infrastructure

# Hot reload
dotnet watch run --project ClinicaSim.Api
```

### Criar Migration
```bash
dotnet ef migrations add NomeMigracao -p ClinicaSim.Infrastructure
dotnet ef database update -p ClinicaSim.Infrastructure
```

### Formatação
```bash
dotnet format
```

**[→ Guia completo de desenvolvimento](./docs/SETUP.md)**

---

## 🤝 Contribuindo

Adoraríamos sua contribuição! Veja [CONTRIBUTING.md](./docs/CONTRIBUTING.md) para:

- ✅ Como reportar bugs
- ✅ Como sugerir features
- ✅ Como fazer pull requests
- ✅ Guia de estilo de código

### Quick Contribute
```bash
# 1. Fork
# 2. Create feature branch
git checkout -b feature/your-feature

# 3. Commit
git commit -m "feat: description"

# 4. Push
git push origin feature/your-feature

# 5. Open Pull Request
```

---

## 📝 Licença

MIT License - veja [LICENSE](LICENSE)

---

## 💬 Suporte

- **Issues**: [GitHub Issues](https://github.com/euthiagochaves/clinicasim/issues)
- **Discussões**: [GitHub Discussions](https://github.com/euthiagochaves/clinicasim/discussions)
- **Email**: thiagochaves@email.com

---

## 🗺️ Roadmap

### MVP (Atual) ✅
- ✅ Backend CRUD de casos
- ✅ Sessões com session codes
- ✅ Rastreamento de eventos
- ✅ História clínica
- ✅ Diagnósticos diferenciais
- ✅ Geração de PDF

### V2 (Planejado)
- [ ] Autenticação JWT
- [ ] Dashboard com estatísticas
- [ ] Importação de casos (CSV)
- [ ] Feedback automático
- [ ] Mobile app (React Native)
- [ ] AI para sugestões de diagnóstico

---

<div align="center">

**Made with ❤️ for medical education**

[**→ Ir para documentação**](./docs/)

</div>
