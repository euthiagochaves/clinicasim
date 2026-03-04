# Banco de Dados ClinicaSim

## 🗄️ Overview

**Engine**: PostgreSQL (via Supabase)
**ORM**: Entity Framework Core 8
**Versioning**: Migrations automáticas
**Environment**: Development = Auto-migrate + Seed

---

## 📊 Diagrama Entidade-Relacionamento (ER)

```
┌──────────────────────┐
│   CLINICAL_CASES     │
├──────────────────────┤
│ id (PK)              │
│ full_name            │
│ sex                  │
│ age                  │
│ chief_complaint      │
│ triage               │
└──────┬───────────────┘
       │ 1:N
       │
       ↓
┌──────────────────────┐
│   CASE_SECTIONS      │
├──────────────────────┤
│ id (PK)              │
│ case_id (FK)         │
│ name                 │
└──────┬───────────────┘
       │ 1:N
       │
       ↓
┌──────────────────────┐
│  CASE_CATEGORIES     │
├──────────────────────┤
│ id (PK)              │
│ section_id (FK)      │
│ name                 │
└──────┬───────────────┘
       │ 1:N
       │
       ↓
┌──────────────────────┐
│  CASE_QUESTIONS      │
├──────────────────────┤
│ id (PK)              │
│ category_id (FK)     │
│ text                 │
└──────┬───────────────┘
       │ 1:1
       │
       ↓
┌──────────────────────┐
│  CASE_ANSWERS        │
├──────────────────────┤
│ id (PK)              │
│ question_id (FK,UQ)  │
│ text                 │
└──────────────────────┘


┌──────────────────────────┐
│ CONSULTATION_SESSIONS    │
├──────────────────────────┤
│ id (PK)                  │
│ session_code (UQ, 10)    │
│ case_id (FK)             │
│ started_at               │
│ finished_at              │
│ status (Active/Final)    │
└──────┬───────────────────┘
       │ 1:N
       ├─────────────────────────┬──────────────────────┐
       │                         │                      │
       ↓                         ↓                      ↓
┌─────────────────┐   ┌──────────────────┐   ┌──────────────────────┐
│INTERACTION_EVENTS│   │CLINICAL_NOTES     │   │DIFFERENTIAL_DIAGNOSES│
├─────────────────┤   ├──────────────────┤   ├──────────────────────┤
│ id (PK)         │   │ id (PK)           │   │ id (PK)              │
│ session_id (FK) │   │ session_id (FK,UQ)│   │ session_id (FK)      │
│ occurred_at     │   │ summary_text      │   │ rank                 │
│ section_name    │   │ probable_diagnosis│   │ text                 │
│ category_name   │   │ conduct_studies   │   │ (UQ: session+rank)   │
│ question_text   │   │ conduct_treatment │   │                      │
│ answer_text     │   │                   │   │                      │
└─────────────────┘   └──────────────────┘   └──────────────────────┘
```

---

## 📋 Tabelas Detalhadas

### 1. **clinical_cases**
Armazena definições de casos clínicos para treino.

```sql
CREATE TABLE clinical_cases (
    id UUID PRIMARY KEY,
    full_name VARCHAR(200) NOT NULL,           -- Ex: "João da Silva"
    sex VARCHAR(20) NOT NULL,                  -- Ex: "M", "F"
    age INT NOT NULL,                          -- Ex: 45
    chief_complaint VARCHAR(300) NOT NULL,     -- Ex: "Dor no peito"
    triage VARCHAR(20) NOT NULL,               -- Ex: "Vermelho", "Amarelo"
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP DEFAULT NOW()
);
```

**Índices**:
- PRIMARY KEY: `id`
- Nenhum índice adicional (queries por id)

**Relacionamentos**:
- 1:N com `case_sections`

---

### 2. **case_sections**
Agrupa categorias de perguntas (ex: Anamnese, Exame Físico).

```sql
CREATE TABLE case_sections (
    id UUID PRIMARY KEY,
    case_id UUID NOT NULL REFERENCES clinical_cases(id) ON DELETE CASCADE,
    name VARCHAR(100) NOT NULL,                -- Ex: "Anamnese"
    display_order INT,                         -- Ordem de exibição
    created_at TIMESTAMP DEFAULT NOW()
);
```

**Índices**:
- PRIMARY KEY: `id`
- FOREIGN KEY: `case_id`

**Restrições**:
- `case_id` é obrigatório
- Deleção em cascata quando caso é deletado

---

### 3. **case_categories**
Categorias dentro de seções (ex: "Queixa Principal", "Antecedentes").

```sql
CREATE TABLE case_categories (
    id UUID PRIMARY KEY,
    section_id UUID NOT NULL REFERENCES case_sections(id) ON DELETE CASCADE,
    name VARCHAR(100) NOT NULL,                -- Ex: "Antecedentes Pessoais"
    display_order INT,                         -- Ordem de exibição
    created_at TIMESTAMP DEFAULT NOW()
);
```

**Índices**:
- PRIMARY KEY: `id`
- FOREIGN KEY: `section_id`

---

### 4. **case_questions**
Perguntas do caso clínico.

```sql
CREATE TABLE case_questions (
    id UUID PRIMARY KEY,
    category_id UUID NOT NULL REFERENCES case_categories(id) ON DELETE CASCADE,
    text TEXT NOT NULL,                        -- Texto da pergunta
    display_order INT,
    created_at TIMESTAMP DEFAULT NOW()
);
```

**Índices**:
- PRIMARY KEY: `id`
- FOREIGN KEY: `category_id`

---

### 5. **case_answers**
Respostas pré-definidas para perguntas (relação 1:1).

```sql
CREATE TABLE case_answers (
    id UUID PRIMARY KEY,
    question_id UUID NOT NULL UNIQUE REFERENCES case_questions(id) ON DELETE CASCADE,
    text TEXT NOT NULL,                        -- Resposta do "paciente virtual"
    created_at TIMESTAMP DEFAULT NOW()
);
```

**Índices**:
- PRIMARY KEY: `id`
- UNIQUE: `question_id` (1:1 relationship)

---

### 6. **consultation_sessions**
Sessão ativa de simulação do usuário.

```sql
CREATE TABLE consultation_sessions (
    id UUID PRIMARY KEY,
    session_code VARCHAR(10) NOT NULL UNIQUE, -- Ex: "ABC123XYZ9" (gerado aleatoriamente)
    case_id UUID NOT NULL REFERENCES clinical_cases(id) ON DELETE RESTRICT,
    started_at TIMESTAMP NOT NULL,             -- Quando começou
    finished_at TIMESTAMP,                     -- Quando terminou (NULL se ativa)
    status VARCHAR(20) NOT NULL,               -- "Active" | "Finalized"
    created_at TIMESTAMP DEFAULT NOW()
);
```

**Índices**:
- PRIMARY KEY: `id`
- UNIQUE: `session_code`
- INDEX: `case_id`
- INDEX: `status` (para queries de sessões ativas)

**Restrições**:
- `case_id` tem DELETE RESTRICT (não deleta caso se há sessão referenciando)
- `session_code` é único (impossível dois códigos iguais)

**Lógica de Negócio**:
- Status pode ser "Active" ou "Finalized"
- `finished_at` é preenchido apenas ao finalizar

---

### 7. **interaction_events**
Rastreia cada interação do usuário (pergunta feita, resposta vista).

```sql
CREATE TABLE interaction_events (
    id UUID PRIMARY KEY,
    session_id UUID NOT NULL REFERENCES consultation_sessions(id) ON DELETE CASCADE,
    occurred_at TIMESTAMP NOT NULL,            -- Quando aconteceu
    section_name VARCHAR(100) NOT NULL,        -- Ex: "Anamnese"
    category_name VARCHAR(100) NOT NULL,       -- Ex: "Antecedentes"
    question_text TEXT NOT NULL,               -- Pergunta feita
    answer_text TEXT NOT NULL,                 -- Resposta recebida
    created_at TIMESTAMP DEFAULT NOW()
);
```

**Índices**:
- PRIMARY KEY: `id`
- INDEX: `session_id`
- INDEX: `(session_id, occurred_at)` (para ordenação temporal)

**Uso**:
- Auditoria de interações
- Análise de padrão de perguntas do aluno
- Reconstruir fluxo da sessão

---

### 8. **clinical_notes**
História clínica preenchida pelo usuário.

```sql
CREATE TABLE clinical_notes (
    id UUID PRIMARY KEY,
    session_id UUID NOT NULL UNIQUE REFERENCES consultation_sessions(id) ON DELETE CASCADE,
    summary_text TEXT NOT NULL,                -- Resumo da história
    probable_diagnosis_text TEXT NOT NULL,     -- Diagnóstico provável
    conduct_studies_text TEXT NOT NULL,        -- Estudos a conduzir
    conduct_treatment_text TEXT NOT NULL,      -- Tratamento indicado
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP DEFAULT NOW()
);
```

**Índices**:
- PRIMARY KEY: `id`
- UNIQUE: `session_id` (1:1 com sessão)

**Lógica**:
- Criada quando sessão é finalizada
- Pode ser atualizada antes de finalizar (UPSERT)

---

### 9. **differential_diagnoses**
Diagnósticos diferenciais ordenados pelo usuário.

```sql
CREATE TABLE differential_diagnoses (
    id UUID PRIMARY KEY,
    session_id UUID NOT NULL REFERENCES consultation_sessions(id) ON DELETE CASCADE,
    rank INT NOT NULL,                         -- 1, 2, 3... (ordem de probabilidade)
    text TEXT NOT NULL,                        -- Ex: "Infarto Agudo do Miocárdio"
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP DEFAULT NOW()
);
```

**Índices**:
- PRIMARY KEY: `id`
- INDEX: `session_id`
- UNIQUE: `(session_id, rank)` (não há dois diagnósticos com mesmo rank na sessão)

**Lógica**:
- `rank` começa em 1 (mais provável)
- Pode ser atualizado (re-ordenado)

---

## 🔄 Relacionamentos e Cascatas

| Origem | Destino | Tipo | On Delete |
|--------|---------|------|-----------|
| `case_sections` | `clinical_cases` | N:1 | CASCADE |
| `case_categories` | `case_sections` | N:1 | CASCADE |
| `case_questions` | `case_categories` | N:1 | CASCADE |
| `case_answers` | `case_questions` | 1:1 | CASCADE |
| `consultation_sessions` | `clinical_cases` | N:1 | RESTRICT |
| `interaction_events` | `consultation_sessions` | N:1 | CASCADE |
| `clinical_notes` | `consultation_sessions` | 1:1 | CASCADE |
| `differential_diagnoses` | `consultation_sessions` | N:1 | CASCADE |

---

## 🚀 Migrações

### Initial Create (20260901000000)
- Cria todas as tabelas
- Configura índices
- Define constraints e relacionamentos
- Seed de dados de exemplo (em desenvolvimento)

### Como Rodar
```bash
# Migrations automáticas em desenvolvimento
dotnet run

# Ou manual
dotnet ef database update
```

---

## 📈 Escalabilidade

### Índices Críticos para Performance
```sql
-- Sessões ativas
CREATE INDEX idx_sessions_status ON consultation_sessions(status)
WHERE status = 'Active';

-- Eventos por sessão (ordenados)
CREATE INDEX idx_events_session_time ON interaction_events(session_id, occurred_at);

-- Diagnósticos únicos por sessão
CREATE UNIQUE INDEX idx_ddx_session_rank ON differential_diagnoses(session_id, rank);
```

### Particionamento (Futuro)
Se houver volumes grandes, particionar `interaction_events` por `session_id` ou data.

---

## 🔒 Segurança & Constraints

### Integridade Referencial
- ✅ Foreign keys com DELETE CASCADE/RESTRICT
- ✅ Unique constraints onde necessário

### Dados Sensíveis
- ❌ Nenhum CPF/RG/credencial no banco
- ✅ Session codes aleatórios (não sequenciais)

### Validações em Banco
```sql
-- Enums para status
ALTER TABLE consultation_sessions ADD CONSTRAINT ck_status
CHECK (status IN ('Active', 'Finalized'));

-- Rank positivo
ALTER TABLE differential_diagnoses ADD CONSTRAINT ck_rank
CHECK (rank > 0);
```

---

## 📊 Seed Data

Desenvolvido em `DbSeeder.cs`:
- 3-5 casos clínicos de exemplo
- Estrutura completa (seções, categorias, perguntas, respostas)
- Usado apenas em `Environment.IsDevelopment()`

```csharp
// Exemplo
var case = new ClinicalCase
{
    FullName = "João da Silva",
    Sex = "M",
    Age = 45,
    ChiefComplaint = "Dor no peito",
    Triage = "Vermelho"
};
```

---

## 🔍 Queries Comuns

### Listar Casos com Estrutura Completa
```csharp
dbContext.ClinicalCases
    .Include(c => c.Sections)
        .ThenInclude(s => s.Categories)
        .ThenInclude(c => c.Questions)
        .ThenInclude(q => q.Answer)
    .ToListAsync();
```

### Obter Sessão com Histórico
```csharp
dbContext.ConsultationSessions
    .Include(s => s.Events)
    .Include(s => s.Note)
    .Include(s => s.Differentials)
    .FirstOrDefaultAsync(s => s.SessionCode == code);
```

### Sessões Ativas de um Caso
```csharp
dbContext.ConsultationSessions
    .Where(s => s.CaseId == caseId && s.Status == "Active")
    .ToListAsync();
```

---

## 📝 Connection String

```json
{
  "ConnectionStrings": {
    "ClinicaSim": "Host=localhost;Database=clinicasim;Username=postgres;Password=password"
  }
}
```

**Supabase**:
```
postgresql://[user]:[password]@[host]:5432/[database]
```

---

## ⚙️ Tuning & Monitoramento

### EXPLAIN ANALYZE (PostgreSQL)
```sql
EXPLAIN ANALYZE
SELECT * FROM consultation_sessions
WHERE status = 'Active'
ORDER BY started_at DESC;
```

### Vacuum & Analyze
```sql
VACUUM ANALYZE clinical_cases;
```

---

## 🗑️ Backup & Disaster Recovery

**Supabase**:
- Backups automáticos
- Point-in-time recovery
- Exports semanais para arquivo

**Local Dev**:
```bash
# Dump
pg_dump -U postgres clinicasim > backup.sql

# Restore
psql -U postgres clinicasim < backup.sql
```

---

## 📚 Referências

- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [Entity Framework Core Docs](https://learn.microsoft.com/en-us/ef/core/)
- [Database Design Best Practices](https://en.wikipedia.org/wiki/Database_normalization)
