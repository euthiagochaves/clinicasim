# ClinicaSim - Tarefa 3

Backend MVP com arquitetura em camadas, EF Core/PostgreSQL, seed idempotente e API REST para fluxo completo de atendimento simulado.

## Pré-requisitos

- .NET SDK 8.0+
- Docker + Docker Compose
- EF Core CLI (`dotnet tool install --global dotnet-ef` se necessário)

## Executar localmente

1. Subir Postgres:

```bash
cd docker
docker compose up -d
```

2. Aplicar migration:

```bash
cd ..
dotnet ef database update --project src/ClinicaSim.Infrastructure --startup-project src/ClinicaSim.Api
```

3. Rodar API:

```bash
dotnet run --project src/ClinicaSim.Api
```

## URLs úteis

- Swagger (Development): `http://localhost:5101/swagger`
- Health: `http://localhost:5101/health`

## Endpoints REST (Tarefa 3)

- `GET /api/cases`
- `POST /api/sessions/start`
- `POST /api/sessions/{sessionCode}/events`
- `GET /api/sessions/{sessionCode}`
- `GET /api/sessions/{sessionCode}/note`
- `POST /api/sessions/{sessionCode}/note`
- `GET /api/sessions/{sessionCode}/differentials`
- `POST /api/sessions/{sessionCode}/differentials`
- `POST /api/sessions/{sessionCode}/finalize`

## Regras principais

- `SessionCode` aleatório, 8-10 chars, alfanumérico (`ABCDEFGHJKLMNPQRSTUVWXYZ23456789`) e único.
- Eventos registram snapshot de seção/categoria/pergunta/resposta.
- Sessão finalizada (`Finalized`) bloqueia novos eventos, nota e diferenciais (retorna `409`).
- Finalização exige nota completa e ao menos 5 diagnósticos diferenciais.
- Mensagens de erro retornadas em espanhol.

## Verificar seed

```bash
docker exec -it clinicasim-postgres psql -U clinicasim -d clinicasim_db -c "select count(*) from clinical_cases;"
```

Esperado: `3`.
