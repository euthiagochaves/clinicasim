# API Reference - ClinicaSim

## 🌐 Base URL

```
http://localhost:5000/api
```

## 📝 Convenções

- **Content-Type**: `application/json`
- **Autenticação**: Nenhuma (MVP)
- **Errors**: Sempre retornam `ErrorResponse`
- **Timestamps**: ISO 8601 UTC

---

## 📚 Endpoints

### Cases

#### `GET /cases`
Retorna lista de todos os casos clínicos disponíveis.

**Request**:
```http
GET /api/cases HTTP/1.1
```

**Response** (200 OK):
```json
[
  {
    "caseId": "550e8400-e29b-41d4-a716-446655440000",
    "fullName": "João da Silva",
    "age": 45,
    "sex": "M",
    "chiefComplaint": "Dor no peito há 3 dias",
    "triage": "Vermelho"
  },
  {
    "caseId": "550e8400-e29b-41d4-a716-446655440001",
    "fullName": "Maria Santos",
    "age": 32,
    "sex": "F",
    "chiefComplaint": "Febre e tosse",
    "triage": "Amarelo"
  }
]
```

**Error** (500):
```json
{
  "errors": ["Erro ao buscar casos"]
}
```

---

### Sessions

#### `POST /sessions/start`
Inicia uma nova sessão de simulação com um caso.

**Request**:
```http
POST /api/sessions/start HTTP/1.1
Content-Type: application/json

{
  "caseId": "550e8400-e29b-41d4-a716-446655440000"
}
```

**Response** (200 OK):
```json
{
  "sessionCode": "ABC123XYZ9",
  "status": "Active",
  "startedAt": "2024-03-03T14:30:00Z",
  "case": {
    "caseId": "550e8400-e29b-41d4-a716-446655440000",
    "fullName": "João da Silva",
    "age": 45,
    "sex": "M",
    "chiefComplaint": "Dor no peito",
    "triage": "Vermelho"
  },
  "sections": [
    {
      "sectionId": "section-001",
      "name": "Anamnese",
      "categories": [
        {
          "categoryId": "cat-001",
          "name": "Queixa Principal",
          "questions": [
            {
              "questionId": "q-001",
              "text": "Há quanto tempo apresenta essa dor?"
            }
          ]
        }
      ]
    }
  ]
}
```

**Errors**:
- 404: Caso não encontrado
- 400: CaseId inválido

---

#### `POST /sessions/{sessionCode}/events`
Registra uma interação (pergunta feita) na sessão.

**Request**:
```http
POST /api/sessions/ABC123XYZ9/events HTTP/1.1
Content-Type: application/json

{
  "questionId": "q-001"
}
```

**Response** (200 OK):
```json
{
  "eventId": "event-123",
  "occurredAt": "2024-03-03T14:31:00Z",
  "sectionName": "Anamnese",
  "categoryName": "Queixa Principal",
  "questionText": "Há quanto tempo apresenta essa dor?",
  "answerText": "Começou há 3 dias, súbita"
}
```

**Errors**:
- 404: Sessão não encontrada | Pergunta não encontrada
- 409: Sessão já finalizada
- 400: Pergunta não pertence a este caso

---

#### `GET /sessions/{sessionCode}`
Retorna informações da sessão atual.

**Request**:
```http
GET /api/sessions/ABC123XYZ9 HTTP/1.1
```

**Response** (200 OK):
```json
{
  "sessionCode": "ABC123XYZ9",
  "status": "Active",
  "startedAt": "2024-03-03T14:30:00Z",
  "finishedAt": null,
  "case": {
    "caseId": "550e8400-e29b-41d4-a716-446655440000",
    "fullName": "João da Silva",
    "age": 45,
    "sex": "M",
    "chiefComplaint": "Dor no peito",
    "triage": "Vermelho"
  }
}
```

**Errors**:
- 404: Sessão não encontrada

---

#### `GET /sessions/{sessionCode}/note`
Retorna a história clínica preenchida (se existir).

**Request**:
```http
GET /api/sessions/ABC123XYZ9/note HTTP/1.1
```

**Response** (200 OK):
```json
{
  "summaryText": "Paciente relata dor torácica de inicio súbito...",
  "probableDiagnosisText": "Infarto Agudo do Miocárdio",
  "conductStudiesText": "ECG, troponina, perfil lipídico",
  "conductTreatmentText": "Internação em UTI, terapia antitrombótica"
}
```

**Errors**:
- 404: Sessão não encontrada | Nota não encontrada

---

#### `POST /sessions/{sessionCode}/note`
Salva ou atualiza a história clínica (UPSERT).

**Request**:
```http
POST /api/sessions/ABC123XYZ9/note HTTP/1.1
Content-Type: application/json

{
  "summaryText": "Paciente relata dor torácica...",
  "probableDiagnosisText": "Infarto Agudo do Miocárdio",
  "conductStudiesText": "ECG, troponina, perfil lipídico",
  "conductTreatmentText": "Internação em UTI, terapia antitrombótica"
}
```

**Response** (200 OK):
```json
{
  "summaryText": "Paciente relata dor torácica...",
  "probableDiagnosisText": "Infarto Agudo do Miocárdio",
  "conductStudiesText": "ECG, troponina, perfil lipídico",
  "conductTreatmentText": "Internação em UTI, terapia antitrombótica"
}
```

**Errors**:
- 404: Sessão não encontrada
- 409: Sessão já finalizada
- 400: Campo inválido

**Validações**:
- `summaryText`: Min 10 caracteres, obrigatório
- `probableDiagnosisText`: Obrigatório, não vazio
- `conductStudiesText`: Obrigatório, não vazio
- `conductTreatmentText`: Obrigatório, não vazio

---

#### `GET /sessions/{sessionCode}/differentials`
Retorna lista de diagnósticos diferenciais ordenados.

**Request**:
```http
GET /api/sessions/ABC123XYZ9/differentials HTTP/1.1
```

**Response** (200 OK):
```json
[
  {
    "rank": 1,
    "text": "Infarto Agudo do Miocárdio"
  },
  {
    "rank": 2,
    "text": "Angina Pectoris"
  },
  {
    "rank": 3,
    "text": "Embolia Pulmonar"
  }
]
```

**Errors**:
- 404: Sessão não encontrada

---

#### `POST /sessions/{sessionCode}/differentials`
Salva lista de diagnósticos diferenciais ordenados por rank.

**Request**:
```http
POST /api/sessions/ABC123XYZ9/differentials HTTP/1.1
Content-Type: application/json

{
  "items": [
    {
      "rank": 1,
      "text": "Infarto Agudo do Miocárdio"
    },
    {
      "rank": 2,
      "text": "Angina Pectoris"
    },
    {
      "rank": 3,
      "text": "Embolia Pulmonar"
    }
  ]
}
```

**Response** (200 OK):
```json
[
  {
    "rank": 1,
    "text": "Infarto Agudo do Miocárdio"
  },
  {
    "rank": 2,
    "text": "Angina Pectoris"
  },
  {
    "rank": 3,
    "text": "Embolia Pulmonar"
  }
]
```

**Errors**:
- 404: Sessão não encontrada
- 409: Sessão já finalizada
- 400: Ranks não sequenciais | Min 1 diagnóstico | Max 10 diagnósticos

**Validações**:
- Ranks devem ser sequenciais (1, 2, 3...)
- Mínimo 1 diagnóstico
- Máximo 10 diagnósticos
- Cada texto: Min 5 caracteres, obrigatório

---

#### `POST /sessions/{sessionCode}/finalize`
Finaliza a sessão (marca como "Finalized").

**Request**:
```http
POST /api/sessions/ABC123XYZ9/finalize HTTP/1.1
```

**Response** (200 OK):
```json
{
  "sessionCode": "ABC123XYZ9",
  "status": "Finalized",
  "finishedAt": "2024-03-03T14:45:00Z"
}
```

**Errors**:
- 404: Sessão não encontrada
- 409: Sessão já foi finalizada

**Efeitos**:
- Status muda para "Finalized"
- `finishedAt` é preenchido
- Nenhuma operação posterior é permitida

---

#### `GET /sessions/{sessionCode}/pdf`
Gera e retorna relatório PDF da sessão finalizada.

**Request**:
```http
GET /api/sessions/ABC123XYZ9/pdf HTTP/1.1
```

**Response** (200 OK):
```
Content-Type: application/pdf
Content-Disposition: attachment; filename="ClinicaSim_ABC123XYZ9.pdf"

[Binary PDF Content]
```

**Errors**:
- 404: Sessão não encontrada
- 409: Sessão ainda está ativa (não finalizada)

**Conteúdo do PDF**:
- Informações do caso
- Histórico de eventos (perguntas feitas)
- História clínica preenchida
- Diagnósticos diferenciais ordenados
- Timestamp de geração

---

## ⚠️ Error Response

Todos os erros seguem este formato:

```json
{
  "errors": [
    "Mensagem de erro 1",
    "Mensagem de erro 2"
  ]
}
```

### Status Codes Comuns

| Code | Situação |
|------|----------|
| 200 | Success |
| 400 | Validação falhou (Bad Request) |
| 404 | Recurso não encontrado (Not Found) |
| 409 | Conflito de estado (Conflict) |
| 500 | Erro interno do servidor |

---

## 🔄 Fluxo Recomendado

```
1. GET /api/cases
   ↓ (user selects a case)

2. POST /api/sessions/start { caseId }
   ↓ (receive sessionCode)

3. [MULTIPLE] POST /api/sessions/{code}/events { questionId }
   ↓ (user interacts with case)

4. POST /api/sessions/{code}/note { ...fields }
   ↓ (user fills clinical history)

5. POST /api/sessions/{code}/differentials { items }
   ↓ (user orders differentials)

6. POST /api/sessions/{code}/finalize
   ↓ (session ends)

7. GET /api/sessions/{code}/pdf
   ↓ (download report)
```

---

## 🧪 Exemplos com cURL

### Listar casos
```bash
curl -X GET http://localhost:5000/api/cases
```

### Iniciar sessão
```bash
curl -X POST http://localhost:5000/api/sessions/start \
  -H "Content-Type: application/json" \
  -d '{"caseId":"550e8400-e29b-41d4-a716-446655440000"}'
```

### Registrar evento
```bash
curl -X POST http://localhost:5000/api/sessions/ABC123XYZ9/events \
  -H "Content-Type: application/json" \
  -d '{"questionId":"q-001"}'
```

### Salvar história clínica
```bash
curl -X POST http://localhost:5000/api/sessions/ABC123XYZ9/note \
  -H "Content-Type: application/json" \
  -d '{
    "summaryText": "Paciente relata...",
    "probableDiagnosisText": "Infarto",
    "conductStudiesText": "ECG, troponina",
    "conductTreatmentText": "Terapia"
  }'
```

### Finalizar sessão
```bash
curl -X POST http://localhost:5000/api/sessions/ABC123XYZ9/finalize
```

### Baixar PDF
```bash
curl -X GET http://localhost:5000/api/sessions/ABC123XYZ9/pdf \
  -o ClinicaSim_ABC123XYZ9.pdf
```

---

## 📊 Swagger/OpenAPI

A documentação interativa está disponível em:

```
http://localhost:5000/swagger
```

Permite:
- Visualizar todos os endpoints
- Testar endpoints diretamente
- Ver schemas de request/response
- Explorar documentação

---

## 🔐 Segurança (Considerações Futuras)

- [ ] Autenticação (JWT)
- [ ] Autorização por usuário
- [ ] Rate limiting
- [ ] CORS mais restritivo
- [ ] HTTPS obrigatório
- [ ] Validation headers

---

## 📈 Performance

### Recomendações
- Cache de casos (mudam raramente)
- Paginação para muitos eventos
- Índices no banco para queries frequentes
- CDN para PDFs

### Timeouts
- Timeout padrão: 30 segundos
- PDF generation: 5-10 segundos
- Database queries: <500ms

---

## 📚 Referências

- [OpenAPI 3.0 Spec](https://spec.openapis.org/oas/v3.0.3)
- [REST API Best Practices](https://restfulapi.net/)
- [HTTP Status Codes](https://developer.mozilla.org/en-US/docs/Web/HTTP/Status)
