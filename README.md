# ClinicaSim - Tarefa 2

Base do MVP com arquitetura em camadas + EF Core (PostgreSQL), migração inicial e seed idempotente de 3 casos clínicos fictícios em espanhol.

## Estrutura

```text
.
├── ClinicaSim.sln
├── docker/
│   └── docker-compose.yml
└── src/
    ├── ClinicaSim.Api
    ├── ClinicaSim.Application
    ├── ClinicaSim.Domain
    └── ClinicaSim.Infrastructure
```

## Pré-requisitos

- .NET SDK 8.0+
- Docker + Docker Compose
- EF Core CLI (`dotnet tool install --global dotnet-ef` se necessário)

## 1) Subir Postgres

```bash
cd docker
docker compose up -d
```

## 2) Aplicar migration

A partir da raiz do repositório:

```bash
dotnet ef database update --project src/ClinicaSim.Infrastructure --startup-project src/ClinicaSim.Api
```

## 3) Rodar API

```bash
dotnet run --project src/ClinicaSim.Api
```

## URLs úteis

- Swagger (Development): `http://localhost:5101/swagger`
- OpenAPI JSON (Development): `http://localhost:5101/swagger/v1/swagger.json`
- Health: `http://localhost:5101/health`

## Seed de dados

- O seed é idempotente e roda na inicialização da API em `Development`.
- Se já existir registro em `clinical_cases`, não insere novamente.
- São criados 3 casos clínicos fictícios em espanhol, com seções `Anamnesis` e `Examen Físico`, categorias e perguntas/respostas 1:1.

## Como verificar seed no PostgreSQL

Exemplo com `psql` no container:

```bash
docker exec -it clinicasim-postgres psql -U clinicasim -d clinicasim_db -c "select count(*) from clinical_cases;"
```

Esperado: `3`.

Para conferir estrutura relacionada:

```bash
docker exec -it clinicasim-postgres psql -U clinicasim -d clinicasim_db -c "select count(*) from case_sections;"
docker exec -it clinicasim-postgres psql -U clinicasim -d clinicasim_db -c "select count(*) from case_categories;"
docker exec -it clinicasim-postgres psql -U clinicasim -d clinicasim_db -c "select count(*) from case_questions;"
docker exec -it clinicasim-postgres psql -U clinicasim -d clinicasim_db -c "select count(*) from case_answers;"
```

## Notas

- CORS policy `CorsPolicy` permite `http://localhost:4200` com métodos `GET,POST,PUT,DELETE,OPTIONS` e qualquer header.
- `SessionCode` possui índice único no schema (`consultation_sessions`).
- Nesta tarefa não há endpoints finais de casos/sessões, PDF ou frontend Angular.
