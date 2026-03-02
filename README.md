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
- `GET /api/sessions/{sessionCode}/pdf` (solo para sesión finalizada)

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


## Probar generación de PDF (Tarea 4)

Flujo mínimo en Swagger:

1. `GET /api/cases`
2. `POST /api/sessions/start`
3. `POST /api/sessions/{code}/events` (registrar algunos eventos)
4. `POST /api/sessions/{code}/note`
5. `POST /api/sessions/{code}/differentials` (5 o más)
6. `POST /api/sessions/{code}/finalize`
7. `GET /api/sessions/{code}/pdf`

Comportamiento esperado de `/pdf`:
- `404` si la sesión no existe.
- `409` con `{ "error": "La sesión no está finalizada. No se puede generar el PDF." }` si está activa.
- `200` con `application/pdf` si está finalizada.
