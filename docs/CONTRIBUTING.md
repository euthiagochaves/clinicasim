# Contributing to ClinicaSim

Primeiro, obrigado por querer contribuir! Este guia te ajudará a entender como contribuir com o projeto.

## 📋 Código de Conduta

Este projeto adota um código de conduta para garantir um ambiente acolhedor. Espera-se que todos os contribuidores sigam esses princípios:

- ✅ Ser respeitoso e inclusivo
- ✅ Aceitar críticas construtivas
- ✅ Focar no que é melhor para a comunidade
- ❌ Sem assédio, discriminação ou abuso

---

## 🚀 Como Contribuir

### 1. Issues

#### Reportar Bug
Antes de criar um issue:
- ✅ Pesquise issues existentes
- ✅ Verifique se não foi já reportado

**Título**: Seja descritivo
```
[BUG] Session não finaliza corretamente
```

**Descrição**:
```markdown
## Descrição
Ao clicar no botão "Finalizar", a sessão fica no estado "Active"

## Passos para Reproduzir
1. Iniciar uma sessão
2. Fazer algumas perguntas
3. Clicar em "Finalizar"

## Comportamento Esperado
Sessão deve mudar para "Finalized" e timestamp deve ser preenchido

## Comportamento Atual
Sessão permanece em "Active", sem timestamp

## Ambiente
- .NET 8
- PostgreSQL 14
- Windows 11

## Logs/Errors
[Cole aqui qualquer erro]
```

#### Sugerir Feature
```markdown
## Descrição
Adicionar filtro por diagnóstico nos casos

## Motivação
Usuário quer praticar apenas casos de cardiologia

## Solução Proposta
Adicionar campo `specialty` na tabela `clinical_cases`

## Alternativas
- Permitir tags customizadas
```

### 2. Pull Requests

#### Passo a Passo

1. **Fork o Repositório**
```bash
# No GitHub, clique em "Fork"
git clone https://github.com/SEU_USER/clinicasim.git
cd clinicasim
git remote add upstream https://github.com/euthiagochaves/clinicasim.git
```

2. **Criar Branch**
```bash
# Sempre a partir de main
git fetch upstream
git checkout upstream/main
git checkout -b feature/descricao-da-feature

# Ou bug fix
git checkout -b fix/descricao-do-bug
```

3. **Fazer Mudanças**
- Siga o estilo de código
- Escreva commits claros
- Inclua testes

4. **Testar**
```bash
# Rodar testes
dotnet test

# Testar manualmente
dotnet run

# Verificar migrations
dotnet ef database update
```

5. **Commit**
```bash
# Commit claro e atômico
git commit -m "feat(sessions): add session finalization endpoint

- Add POST /sessions/{code}/finalize endpoint
- Update ConsultationSession.Status to Finalized
- Add validation for active sessions"

# Ou para bug
git commit -m "fix(sessions): prevent duplicate session codes

- Add unique constraint check before generation
- Add retry logic for code generation"
```

6. **Push e Pull Request**
```bash
git push origin feature/descricao-da-feature
```

No GitHub, clique em "Create Pull Request"

#### Template de PR

```markdown
## Descrição
Breve descrição do que foi alterado

## Tipo de Mudança
- [ ] Bug fix
- [ ] Nova feature
- [ ] Breaking change
- [ ] Documentação

## Checklist
- [ ] Código segue style guide
- [ ] Testes adicionados
- [ ] Documentação atualizada
- [ ] Sem merge conflicts

## Screenshots (se aplicável)
[Cole imagens aqui]

## Testing
Passos para testar:
1. ...
2. ...
3. ...

## Referências
Fecha #123 (se for issue)
```

---

## 🎨 Estilo de Código

### C# / .NET

#### Convenções
```csharp
// ✅ Classes PascalCase
public class ClinicalCase { }

// ✅ Métodos PascalCase
public async Task StartAsync() { }

// ✅ Properties PascalCase
public string FullName { get; set; }

// ✅ Private fields camelCase com _
private string _internalField;

// ✅ Constantes UPPER_SNAKE_CASE
private const string ActiveStatus = "Active";

// ✅ Async methods com Async suffix
public async Task<Result> GetAsync() { }

// ✅ Interface com I prefix
public interface ISessionsService { }
```

#### Formatação
```csharp
// ✅ Usar using declarations
using var scope = serviceProvider.CreateScope();

// ✅ Null coalescing
var value = input ?? defaultValue;

// ✅ LINQ quando possível
var items = list.Where(x => x.IsActive).OrderBy(x => x.Name).ToList();

// ✅ Expressão lambda simples
Action<int> log = x => Console.WriteLine(x);
```

#### Validações
```csharp
// ✅ Validar entrada
if (string.IsNullOrWhiteSpace(input))
    throw new AppException("Input inválido", 400);

// ✅ Usar null-forgiving operator com cuidado
var result = data ?? throw new AppException("Not found", 404);

// ✅ Guard clauses
if (session.Status != "Active")
    throw new AppException("Session not active", 409);
```

#### Comentários
```csharp
// ✅ Comentários quando lógica é complexa
// Retry logic para garantir session code único
// Máximo de 10 tentativas antes de falhar
for (int i = 0; i < 10; i++) { ... }

// ❌ Evitar comentários óbvios
// Incrementar i
i++;

// ✅ XML comments para métodos públicos
/// <summary>
/// Starts a new consultation session with a clinical case.
/// </summary>
/// <param name="caseId">The ID of the clinical case</param>
/// <returns>Session information including code</returns>
public async Task<SessionStartResult> StartAsync(Guid caseId)
```

#### Estrutura de Método
```csharp
public async Task<Result> ProcessAsync(string input, CancellationToken cancellationToken)
{
    // 1. Validações
    if (string.IsNullOrWhiteSpace(input))
        throw new AppException("Invalid input", 400);

    // 2. Operações
    var item = await GetItemAsync(input, cancellationToken);

    // 3. Transformações
    return MapToResult(item);
}
```

### Arquitetura

#### Responsabilidades por Camada
```csharp
// Domain (pura lógica de negócio)
public class ClinicalCase
{
    public bool IsValid() => !string.IsNullOrWhiteSpace(FullName);
}

// Application (orquestração)
public interface ICasesService
{
    Task<CaseListItem> GetAsync(Guid id);
}

// Infrastructure (implementação)
public class CasesService : ICasesService
{
    public async Task<CaseListItem> GetAsync(Guid id)
    {
        var entity = await dbContext.ClinicalCases.FindAsync(id);
        return MapToResult(entity);
    }
}

// Presentation (HTTP)
[ApiController]
[Route("api/cases")]
public class CasesController
{
    [HttpGet("{id}")]
    public async Task<ActionResult<CaseDto>> Get(Guid id)
    {
        var result = await service.GetAsync(id);
        return Ok(new CaseDto { ... });
    }
}
```

---

## 🧪 Testes

### Padrão AAA (Arrange, Act, Assert)

```csharp
[Fact]
public async Task StartAsync_WithValidCase_ReturnsSessionCode()
{
    // Arrange
    var caseId = Guid.NewGuid();
    var service = new SessionsService(dbContext);

    // Act
    var result = await service.StartAsync(caseId);

    // Assert
    Assert.NotNull(result.SessionCode);
    Assert.Equal("Active", result.Status);
}

[Fact]
public async Task StartAsync_WithInvalidCase_ThrowsException()
{
    // Arrange
    var invalidCaseId = Guid.NewGuid();
    var service = new SessionsService(dbContext);

    // Act & Assert
    await Assert.ThrowsAsync<AppException>(() =>
        service.StartAsync(invalidCaseId));
}
```

### Cobertura
- ✅ Mínimo 80% de cobertura
- ✅ Testes para happy path
- ✅ Testes para error cases
- ✅ Testes para edge cases

---

## 📝 Documentação

### Ao Adicionar Feature

1. **Código Comentado** (se necessário)
```csharp
/// <summary>
/// Generates a unique session code with retry logic.
/// </summary>
private async Task<string> GenerateUniqueCodeAsync()
```

2. **README.md Atualizado**
Se feature é importante

3. **API.md Atualizado**
Se adicionar/modificar endpoints

4. **BUSINESS_RULES.md Atualizado**
Se mudar regras de negócio

### Exemplo de Documentação
```markdown
## Feature: Session Analytics

### Overview
Rastreia analítica de sessões para melhorar currículo.

### Endpoints Adicionados
- GET /api/sessions/{id}/analytics

### Database Changes
- Adicionada tabela `session_analytics`

### Regra de Negócio
- RN-400: Eventos registrados com timestamp
```

---

## 🔄 Processo de Review

1. **Automated Checks**
   - Build passa
   - Tests passam
   - Linter aprovado

2. **Code Review**
   - Mínimo 1 reviewer
   - Verificar estilo
   - Verificar lógica
   - Verificar testes

3. **Approval**
   - Conversa resolvida
   - Mudanças aprovadas

4. **Merge**
   - Squash commits (1 commit por feature)
   - Delete branch
   - Close related issues

---

## 📊 Checklist Pré-Commit

- [ ] `dotnet format` (auto-format)
- [ ] `dotnet build` (compila)
- [ ] `dotnet test` (testes passam)
- [ ] Sem warnings
- [ ] Documentação atualizada
- [ ] Commits atômicos e descritivos
- [ ] Branch atualizada com main

```bash
# Executar antes de push
dotnet format
dotnet build
dotnet test
```

---

## 🐛 Processo de Bug Fix

1. **Criar Issue** com detalhes
2. **Criar Branch** `fix/descricao`
3. **Criar Teste** que reproduz o bug
4. **Corrigir** o bug
5. **Verificar** que teste passa
6. **PR** com referência ao issue

---

## 🔒 Segurança

### Não faça:
- ❌ Commitar senhas/tokens
- ❌ Submeter dados sensíveis
- ❌ Ignorar validações
- ❌ Usar SQL injection

### Faça:
- ✅ Use `.env.local` para secrets
- ✅ Valide toda entrada
- ✅ Use parameterized queries (EF Core)
- ✅ Reporte vulnerabilidades privadamente

---

## 🚀 Ferramentas Recomendadas

### IDE
- Visual Studio 2022
- VS Code + C# Extension
- JetBrains Rider

### CLI
- `dotnet` CLI
- `git` CLI
- `postgres` CLI

### Testing
- xUnit
- Moq
- FluentAssertions

### Analysis
- SonarQube
- CodeCov
- GitHub CodeQL

---

## 📚 Referências

- [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [Clean Code - Robert C. Martin](https://www.amazon.com/Clean-Code-Handbook-Software-Craftsmanship/dp/0132350882)
- [ASP.NET Core Best Practices](https://docs.microsoft.com/en-us/aspnet/core)
- [Domain-Driven Design - Eric Evans](https://www.domainlanguage.com/ddd/)

---

## 🎓 Primeiras Contribuições

Se é sua primeira vez contribuindo:

1. Procure issues com label `good-first-issue`
2. Comente na issue para ser atribuído
3. Siga este guia
4. Pergunte se tiver dúvidas!

---

## 💬 Perguntas?

- **Abrir Discussion** no GitHub
- **Enviar Email**: thiagochaves@email.com
- **Discord**: [Link do servidor]

---

**Obrigado por contribuir! 🙌**
