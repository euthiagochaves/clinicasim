# ClinicaSim - Tarefa 1

Estrutura inicial do MVP ClinicaSim com separação por camadas, API .NET 8 mínima, Swagger, CORS para Angular local e infraestrutura base com PostgreSQL via Docker Compose.

## Estrutura do projeto

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

- [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) com Docker Compose habilitado

## Subir o PostgreSQL

Na raiz do repositório:

```bash
cd docker
docker compose up -d
```

Para verificar se o container subiu:

```bash
docker compose ps
```

Configuração aplicada no PostgreSQL:

- **Usuário:** `clinicasim`
- **Senha:** `clinicasim_pwd`
- **Database:** `clinicasim_db`
- **Porta:** `5432`
- **Volume persistente:** `clinicasim_pgdata`

## Rodar a API

Na raiz do repositório:

```bash
cd src/ClinicaSim.Api
dotnet run
```

## URLs úteis

Com a API rodando em ambiente Development:

- Swagger: `http://localhost:5101/swagger`
- Health: `http://localhost:5101/health`

Exemplo de retorno esperado em `/health`:

```json
{
  "status": "ok",
  "service": "ClinicaSim.Api"
}
```

## Observações

- A policy de CORS `CorsPolicy` está aplicada globalmente na API.
- Origem permitida para frontend local: `http://localhost:4200`.
- Métodos permitidos: `GET`, `POST`, `PUT`, `DELETE`, `OPTIONS`.
- Headers liberados: qualquer header.
- Nesta etapa não há EF Core, DbContext, migrations, entidades ou regras de negócio.

### Erro comum de Swagger (CS1061)

Se aparecer erro de `AddSwaggerGen`, `UseSwagger` ou `UseSwaggerUI`, execute restauração de pacotes antes do build:

```bash
cd src/ClinicaSim.Api
dotnet restore
```

Depois:

```bash
cd ../..
dotnet build ClinicaSim.sln
```
