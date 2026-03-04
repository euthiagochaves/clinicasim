# Regras de Negócio ClinicaSim

## 📋 Overview

Este documento descreve todas as regras de negócio que governam o comportamento da aplicação ClinicaSim. Essas regras devem ser aplicadas em todas as camadas de desenvolvimento.

---

## 🎯 Regras de Casos Clínicos

### RN-001: Estrutura do Caso
**Descrição**: Todo caso clínico deve ter uma estrutura bem definida.

**Regra**:
- ✅ Deve ter exatamente 1 paciente (full_name, sex, age)
- ✅ Deve ter 1 queixa principal (chief_complaint)
- ✅ Deve ter 1 nível de triagem (triage: "Vermelho", "Amarelo", "Verde")
- ✅ Deve ter 1 ou mais seções
- ✅ Cada seção deve ter 1 ou mais categorias
- ✅ Cada categoria deve ter 1 ou mais perguntas
- ✅ Cada pergunta deve ter exatamente 1 resposta pré-definida

**Validações**:
```csharp
if (string.IsNullOrWhiteSpace(clinicalCase.FullName) ||
    clinicalCase.FullName.Length > 200)
    throw new AppException("Nome inválido");

if (!new[] { "M", "F" }.Contains(clinicalCase.Sex))
    throw new AppException("Sexo deve ser M ou F");

if (clinicalCase.Age < 0 || clinicalCase.Age > 150)
    throw new AppException("Idade inválida");

if (!new[] { "Vermelho", "Amarelo", "Verde" }.Contains(clinicalCase.Triage))
    throw new AppException("Triagem inválida");
```

**Responsabilidade**: Application Layer (`CasesService`)

---

### RN-002: Casos São Imutáveis
**Descrição**: Uma vez criado, um caso não pode ser modificado ou deletado.

**Regra**:
- ✅ Nenhuma atualização em `ClinicalCase`
- ✅ Nenhuma exclusão (soft-delete possível no futuro)
- ✅ Novo caso = Nova entrada

**Justificativa**: Garante consistência histórica de simulações anteriores.

**Status**: Validado em serviço (sem UPDATE/DELETE queries)

---

### RN-003: Casos Têm Ordem Definida
**Descrição**: Seções, categorias e perguntas têm ordem de exibição.

**Regra**:
- ✅ `CaseSection.display_order` define a ordem de seções
- ✅ `CaseCategory.display_order` define a ordem de categorias
- ✅ `CaseQuestion.display_order` define a ordem de perguntas
- ✅ Frontend exibe sempre em ordem (ordenado no servidor)

**Implementação**:
```csharp
var sections = clinicalCase.Sections
    .OrderBy(s => s.DisplayOrder)
    .Select(s => new SessionSectionItem(
        s.Id,
        s.Name,
        s.Categories.OrderBy(c => c.DisplayOrder)
            .Select(c => new SessionCategoryItem(
                c.Id,
                c.Name,
                c.Questions
                    .OrderBy(q => q.DisplayOrder)
                    .Select(q => new SessionQuestionItem(q.Id, q.Text))
                    .ToList()))
            .ToList()))
    .ToList();
```

---

## 🎓 Regras de Sessões

### RN-100: Session Lifecycle
**Descrição**: Uma sessão passa por estados bem definidos.

**Estados Possíveis**:
```
[CRIAÇÃO] → [ATIVA] → [FINALIZADA]
   (1)        (*)         (1)
```

- **CRIAÇÃO**: Endpoint POST `/api/sessions/start` chamado
- **ATIVA**: Session code gerado, usuário interage com caso
- **FINALIZADA**: Endpoint POST `/api/sessions/{code}/finalize` chamado

**Regras de Transição**:
- ✅ Criação → Ativa: Automático ao iniciar
- ✅ Ativa → Finalizada: Apenas com comando explícito
- ❌ Qualquer outro caminho: Erro

**Implementação**:
```csharp
public async Task<SessionStartResult> StartAsync(Guid caseId, CancellationToken cancellationToken)
{
    // Valida caso
    var clinicalCase = await GetCaseAsync(caseId);

    // Gera session code único
    var sessionCode = await GenerateUniqueCodeAsync();

    // Cria sessão no status "Active"
    var session = new ConsultationSession
    {
        SessionCode = sessionCode,
        CaseId = caseId,
        Status = "Active",
        StartedAt = DateTimeOffset.UtcNow
    };

    dbContext.ConsultationSessions.Add(session);
    await dbContext.SaveChangesAsync(cancellationToken);

    return MapToResult(session, clinicalCase);
}

public async Task<SessionStartResult> FinalizeAsync(string sessionCode, CancellationToken cancellationToken)
{
    var session = await GetSessionAsync(sessionCode);

    if (session.Status != "Active")
        throw new AppException("Sessão não está ativa", 409);

    session.Status = "Finalized";
    session.FinishedAt = DateTimeOffset.UtcNow;

    await dbContext.SaveChangesAsync(cancellationToken);

    return MapToResult(session);
}
```

**Responsabilidade**: Infrastructure (`SessionsService`)

---

### RN-101: Session Code Geração
**Descrição**: Códigos de sessão devem ser únicos e aleatórios.

**Regra**:
- ✅ Comprimento: 10 caracteres
- ✅ Caracteres: A-Z, 0-9 (excluindo confusos: I, L, O, U)
- ✅ Geração: Aleatória criptográfica
- ✅ Unicidade: Verificada no banco antes de usar

**Formato**: Ex: "ABC123XYZ9", "ZZZ999ABC1"

**Implementação**:
```csharp
private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // 32 chars

private async Task<string> GenerateUniqueCodeAsync(CancellationToken cancellationToken)
{
    string code;
    bool exists;

    do
    {
        var bytes = new byte[10];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(bytes);
        }

        code = new string(bytes.Select(b => Alphabet[b % Alphabet.Length]).ToArray());
        exists = await dbContext.ConsultationSessions
            .AnyAsync(s => s.SessionCode == code, cancellationToken);
    }
    while (exists);

    return code;
}
```

**Responsabilidade**: Infrastructure (`SessionsService`)

---

### RN-102: Operações Apenas em Sessão Ativa
**Descrição**: Apenas sessões no status "Active" aceitam novas operações.

**Regra**:
- ✅ Registrar evento: Apenas se sessão está "Active"
- ✅ Salvar nota: Apenas se sessão está "Active"
- ✅ Salvar DDx: Apenas se sessão está "Active"
- ❌ Após finalizar: Nenhuma modificação permitida

**Validações**:
```csharp
private void EnsureSessionActive(ConsultationSession session)
{
    if (session.Status != "Active")
        throw new AppException("Sessão não está ativa", 409);
}

// Usado em todos os métodos que modificam sessão
public async Task<SessionEventResult> RegisterEventAsync(string sessionCode, Guid questionId, CancellationToken cancellationToken)
{
    var session = await GetSessionEntityAsync(sessionCode, cancellationToken);
    EnsureSessionActive(session);  // ← Validação

    // ... resto do código
}
```

**Responsabilidade**: Infrastructure (`SessionsService`)

---

### RN-103: Perguntas Devem Pertencer ao Caso
**Descrição**: Uma pergunta registrada deve pertencer ao caso da sessão.

**Regra**:
- ✅ `InteractionEvent.questionId` deve estar no `case_id` da sessão
- ✅ Validação antes de registrar evento
- ❌ Perguntas de outros casos: Erro 400

**Validação**:
```csharp
public async Task<SessionEventResult> RegisterEventAsync(string sessionCode, Guid questionId, CancellationToken cancellationToken)
{
    var session = await GetSessionEntityAsync(sessionCode, cancellationToken);
    EnsureSessionActive(session);

    var questionInfo = await dbContext.CaseQuestions
        .Where(q => q.Id == questionId)
        .Select(q => new { q.Category.Section.CaseId, ... })
        .FirstOrDefaultAsync(cancellationToken)
        ?? throw new AppException("Pergunta não encontrada", 404);

    if (questionInfo.CaseId != session.CaseId)
        throw new AppException("Pergunta não pertence a este caso", 400);

    // ... resto
}
```

**Responsabilidade**: Infrastructure (`SessionsService`)

---

### RN-104: Múltiplas Sessões do Mesmo Caso
**Descrição**: Um caso pode ter múltiplas sessões (diferentes usuários/vezes).

**Regra**:
- ✅ Mesmo caso → N sessões diferentes
- ✅ Cada sessão: Código único, histórico isolado
- ✅ Dados do caso: Nunca modificados entre sessões

**Implicação**:
- Estudante A pratica com Caso X → Sessão 1
- Estudante B pratica com Caso X → Sessão 2
- Ambas independentes

---

## 📝 Regras de História Clínica

### RN-200: Historia Clínica UPSERT
**Descrição**: Uma sessão pode ter no máximo 1 história clínica.

**Regra**:
- ✅ Primeira vez: CREATE
- ✅ Atualizações: UPDATE
- ✅ Se não existe: CREATE automático
- ✅ Sempre associada a 1 sessão

**Implementação**:
```csharp
public async Task<ClinicalNoteModel> UpsertNoteAsync(string sessionCode, ClinicalNoteModel model, CancellationToken cancellationToken)
{
    var session = await GetSessionEntityAsync(sessionCode, cancellationToken);
    EnsureSessionActive(session);

    var existingNote = await dbContext.ClinicalNotes
        .FirstOrDefaultAsync(n => n.SessionId == session.Id, cancellationToken);

    if (existingNote == null)
    {
        // CREATE
        var newNote = new ClinicalNote
        {
            SessionId = session.Id,
            SummaryText = model.SummaryText,
            ProbableDiagnosisText = model.ProbableDiagnosisText,
            ConductStudiesText = model.ConductStudiesText,
            ConductTreatmentText = model.ConductTreatmentText
        };
        dbContext.ClinicalNotes.Add(newNote);
    }
    else
    {
        // UPDATE
        existingNote.SummaryText = model.SummaryText;
        existingNote.ProbableDiagnosisText = model.ProbableDiagnosisText;
        existingNote.ConductStudiesText = model.ConductStudiesText;
        existingNote.ConductTreatmentText = model.ConductTreatmentText;
        existingNote.UpdatedAt = DateTimeOffset.UtcNow;
    }

    await dbContext.SaveChangesAsync(cancellationToken);

    return new ClinicalNoteModel(
        existingNote.SummaryText,
        existingNote.ProbableDiagnosisText,
        existingNote.ConductStudiesText,
        existingNote.ConductTreatmentText);
}
```

**Responsabilidade**: Infrastructure (`SessionsService`)

---

### RN-201: Campos de História Clínica Obrigatórios
**Descrição**: Todos os 4 campos da história clínica são obrigatórios.

**Regra**:
- ✅ `SummaryText`: Não nulo, não vazio (min: 10 chars)
- ✅ `ProbableDiagnosisText`: Não nulo, não vazio
- ✅ `ConductStudiesText`: Não nulo, não vazio
- ✅ `ConductTreatmentText`: Não nulo, não vazio

**Validação**:
```csharp
if (string.IsNullOrWhiteSpace(model.SummaryText) || model.SummaryText.Length < 10)
    throw new AppException("Resumo inválido (min: 10 caracteres)", 400);

if (string.IsNullOrWhiteSpace(model.ProbableDiagnosisText))
    throw new AppException("Diagnóstico provável não pode estar vazio", 400);

// ... etc
```

---

## 🔀 Regras de Diagnósticos Diferenciais (DDx)

### RN-300: DDx Ranking
**Descrição**: Diagnósticos diferenciais são ordenados por probabilidade (rank).

**Regra**:
- ✅ Rank começa em 1 (mais provável)
- ✅ Rank é sequencial: 1, 2, 3, 4...
- ✅ Não pode haver gaps: Se há 3 itens, ranks são 1, 2, 3
- ✅ Máximo de DDx: 10 (limitação de UI/UX)
- ✅ Cada sessão tem até 1 DDx por rank

**Validações**:
```csharp
public async Task<IReadOnlyCollection<DifferentialModel>> SaveDifferentialsAsync(
    string sessionCode,
    IReadOnlyCollection<DifferentialModel> items,
    CancellationToken cancellationToken)
{
    var session = await GetSessionEntityAsync(sessionCode, cancellationToken);
    EnsureSessionActive(session);

    // Validar número de itens
    if (items.Count == 0)
        throw new AppException("Deve haver pelo menos 1 diagnóstico diferencial", 400);

    if (items.Count > 10)
        throw new AppException("Máximo de 10 diagnósticos diferenciais", 400);

    // Validar sequência de ranks
    var expectedRanks = Enumerable.Range(1, items.Count).ToHashSet();
    var actualRanks = items.Select(i => i.Rank).ToHashSet();

    if (!expectedRanks.SetEquals(actualRanks))
        throw new AppException("Ranks devem ser sequenciais começando em 1", 400);

    // Validar texto
    foreach (var item in items)
    {
        if (string.IsNullOrWhiteSpace(item.Text) || item.Text.Length < 5)
            throw new AppException($"Diagnóstico rank {item.Rank} inválido", 400);
    }

    // Deletar anteriores e inserir novos
    var existingDdx = await dbContext.DifferentialDiagnoses
        .Where(d => d.SessionId == session.Id)
        .ToListAsync(cancellationToken);

    dbContext.DifferentialDiagnoses.RemoveRange(existingDdx);

    var newDdx = items.Select(i => new DifferentialDiagnosis
    {
        SessionId = session.Id,
        Rank = i.Rank,
        Text = i.Text
    }).ToList();

    dbContext.DifferentialDiagnoses.AddRange(newDdx);
    await dbContext.SaveChangesAsync(cancellationToken);

    return newDdx.Select(d => new DifferentialModel(d.Rank, d.Text)).ToList();
}
```

**Responsabilidade**: Infrastructure (`SessionsService`)

---

### RN-301: DDx Imutável Após Finalizar
**Descrição**: Após finalizar sessão, DDx não pode mudar.

**Regra**:
- ✅ Salvar DDx apenas enquanto sessão está "Active"
- ✅ Após "Finalized": Nenhuma modificação

**Enforcement**: Via `EnsureSessionActive()` (RN-102)

---

## 📊 Regras de Eventos de Interação

### RN-400: Event Logging
**Descrição**: Cada interação do usuário deve ser registrada.

**Regra**:
- ✅ Pergunta feita → `InteractionEvent` criado
- ✅ Timestamp automático (servidor)
- ✅ Não pode ser modificado ou deletado
- ✅ Auditoria permanente

**Campos Capturados**:
- `sessionId`: Referência à sessão
- `occurredAt`: Timestamp exato (UTC)
- `sectionName`: Nome da seção (desnormalizado para auditoria)
- `categoryName`: Nome da categoria
- `questionText`: Texto da pergunta (snapshot)
- `answerText`: Resposta recebida (snapshot)

**Justificativa**:
- Permite análise de padrão de perguntas
- Auditoria completa
- Reconstruir sequência de pensamento do aluno

**Implementação**:
```csharp
public async Task<SessionEventResult> RegisterEventAsync(string sessionCode, Guid questionId, CancellationToken cancellationToken)
{
    var session = await GetSessionEntityAsync(sessionCode, cancellationToken);
    EnsureSessionActive(session);

    var questionInfo = await dbContext.CaseQuestions
        .Where(q => q.Id == questionId)
        .Select(q => new
        {
            Question = q,
            CategoryName = q.Category.Name,
            SectionName = q.Category.Section.Name,
            AnswerText = q.Answer.Text
        })
        .FirstOrDefaultAsync(cancellationToken)
        ?? throw new AppException("Pergunta não encontrada", 404);

    // Criar evento
    var @event = new InteractionEvent
    {
        Id = Guid.NewGuid(),
        SessionId = session.Id,
        OccurredAt = DateTimeOffset.UtcNow,
        SectionName = questionInfo.SectionName,
        CategoryName = questionInfo.CategoryName,
        QuestionText = questionInfo.Question.Text,
        AnswerText = questionInfo.AnswerText
    };

    dbContext.InteractionEvents.Add(@event);
    await dbContext.SaveChangesAsync(cancellationToken);

    return new SessionEventResult(
        @event.Id,
        @event.OccurredAt,
        @event.SectionName,
        @event.CategoryName,
        @event.QuestionText,
        @event.AnswerText);
}
```

**Responsabilidade**: Infrastructure (`SessionsService`)

---

## 🗃️ Regras Gerais

### RN-500: Timezone
**Descrição**: Todo timestamp é armazenado em UTC.

**Regra**:
- ✅ Banco de dados: Sempre UTC
- ✅ Aplicação: Sempre UTC (DateTimeOffset.UtcNow)
- ✅ Frontend: Converte para timezone local

**Implementação**: EF Core automático com `DateTimeOffset`

---

### RN-501: Soft Delete (Futuro)
**Descrição**: Dados críticos nunca são deletados.

**Futura Implementação**:
```csharp
// Adicionar IsDeleted, DeletedAt
public class ClinicalCase
{
    public bool IsDeleted { get; set; } = false;
    public DateTimeOffset? DeletedAt { get; set; }
}

// Query filter em DbContext
modelBuilder.Entity<ClinicalCase>()
    .HasQueryFilter(x => !x.IsDeleted);
```

---

### RN-502: Auditoria
**Descrição**: Rastrear mudanças em entidades críticas.

**Futura Implementação**:
- `CreatedAt` e `UpdatedAt` em todas as entidades
- `CreatedBy` e `UpdatedBy` para rastrear usuário
- Tabela de auditoria separada

---

## 🔐 Regras de Segurança

### RN-600: Isolamento de Dados por Sessão
**Descrição**: Usuário A não pode ver dados de Sessão B.

**Regra**:
- ✅ Session code é único (impossível chutar)
- ✅ Sem autenticação: Apenas código é necessário
- ✅ Futuro: Adicionar usuário para melhor rastreamento

---

### RN-601: Validação de Input
**Descrição**: Toda entrada do usuário é validada.

**Regra**:
- ✅ Strings: Max length, caracteres válidos
- ✅ GUIDs: Formato válido
- ✅ Enums: Valores conhecidos
- ✅ Números: Range válido

---

## 📋 Checklist de Implementação

Para cada nova feature:
- [ ] Regra de negócio definida (RN-XXX)
- [ ] Validação em Application Layer
- [ ] Validação em Infrastructure Layer
- [ ] Validação em Database (constraints)
- [ ] Testes unitários
- [ ] Documentação atualizada

---

## 📚 Referências

- [Domain-Driven Design - Eric Evans](https://www.domainlanguage.com/ddd/)
- [Clean Architecture - Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Validation Best Practices](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/domain-driven-design-microservices)
