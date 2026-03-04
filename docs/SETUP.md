# Setup & Development Guide

## 📋 Pré-requisitos

### Essencial
- **.NET 8 SDK** - [Download](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- **PostgreSQL 14+** ou **Supabase Account** - [Supabase](https://supabase.com)
- **Git** - [Download](https://git-scm.com)
- **Node.js 18+** (para frontend Angular) - [Download](https://nodejs.org)

### Opcional (Recomendado)
- **Visual Studio 2022** ou **VS Code**
- **pgAdmin** (para gerenciar PostgreSQL)
- **Postman** ou **Insomnia** (para testar API)
- **Docker** (para PostgreSQL local)

---

## 🚀 Setup Rápido (5 minutos)

### 1. Clone o Repositório
```bash
git clone https://github.com/euthiagochaves/clinicasim.git
cd clinicasim
```

### 2. Configurar Variáveis de Ambiente
```bash
# Copiar template
cp .env.example .env.local

# Editar com suas credenciais
# Windows
notepad .env.local
# macOS/Linux
nano .env.local
```

**Conteúdo esperado**:
```env
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__ClinicaSim=postgresql://user:password@localhost:5432/clinicasim
```

### 3. Restaurar Dependências e Rodar Migrations
```bash
cd src

# Restaurar pacotes
dotnet restore

# Aplicar migrations (cria banco e tabelas)
dotnet ef database update -p ClinicaSim.Infrastructure

# Seed de dados (casos exemplo)
# Automático ao rodar em Development
```

### 4. Rodar Backend
```bash
cd ClinicaSim.Api
dotnet run

# Ou com hot reload
dotnet watch run
```

**Output esperado**:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
```

### 5. Testar API
```bash
# Em outra aba de terminal
curl http://localhost:5000/api/cases

# Ou abrir no navegador
http://localhost:5000/swagger
```

### 6. Rodar Frontend (Angular)
```bash
# Nova aba
cd ../../../frontend  # ou onde está o Angular

npm install
ng serve --open

# Abre em http://localhost:4200
```

---

## 🗄️ Database Setup

### Opção 1: PostgreSQL Local (Docker - Recomendado)

```bash
# Criar container PostgreSQL
docker run --name clinicasim-postgres \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=password \
  -e POSTGRES_DB=clinicasim \
  -p 5432:5432 \
  -d postgres:15

# Verificar
docker ps | grep clinicasim-postgres

# Connection String
postgresql://postgres:password@localhost:5432/clinicasim
```

### Opção 2: PostgreSQL Instalado Localmente

```bash
# Windows (WSL) / macOS / Linux
createdb -U postgres clinicasim

# Connection String
postgresql://postgres:PASSWORD@localhost:5432/clinicasim
```

### Opção 3: Supabase (Cloud - Mais Fácil)

1. Ir em [supabase.com](https://supabase.com)
2. Criar projeto novo
3. Copiar connection string
4. Usar em `.env.local`

---

## 🔄 Migrações

### Criar Nova Migration
```bash
cd src

# Depois de alterar entidades
dotnet ef migrations add NomeDaMigracao -p ClinicaSim.Infrastructure

# Revisar arquivo gerado em: ClinicaSim.Infrastructure/Migrations/
# Editar se necessário
```

### Aplicar Migrations
```bash
# Aplicar todas pendentes
dotnet ef database update -p ClinicaSim.Infrastructure

# Ou específica
dotnet ef database update 20260901000000_InitialCreate -p ClinicaSim.Infrastructure
```

### Remover Migration (se ainda não aplicada)
```bash
dotnet ef migrations remove -p ClinicaSim.Infrastructure
```

### Ver Histórico
```bash
dotnet ef migrations list -p ClinicaSim.Infrastructure
```

---

## 💻 Estrutura de Diretórios

```
clinicasim/
├── src/
│   ├── ClinicaSim.Api/           # REST API
│   │   ├── Controllers/           # Endpoints
│   │   ├── Dtos/                  # Data Transfer Objects
│   │   └── Program.cs             # Configuration
│   │
│   ├── ClinicaSim.Application/    # Use Cases
│   │   ├── Interfaces/            # Service contracts
│   │   ├── Models/                # DTOs internos
│   │   └── Common/                # Exceções, helpers
│   │
│   ├── ClinicaSim.Domain/         # Entidades
│   │   └── Entities/              # Domain models
│   │
│   └── ClinicaSim.Infrastructure/ # Data Access
│       ├── Persistence/           # DbContext, Factory
│       ├── Services/              # Implementações
│       ├── Migrations/            # EF Core migrations
│       └── DependencyInjection/   # IoC setup
│
├── frontend/                       # Angular app (separado)
├── docs/                           # Documentação
└── README.md
```

---

## 🔧 Configuração de Ambiente

### Development (.env.local)
```env
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://localhost:5000
ConnectionStrings__ClinicaSim=postgresql://postgres:password@localhost:5432/clinicasim
```

### Production (.env)
```env
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=https://api.clinicasim.com
ConnectionStrings__ClinicaSim=postgresql://[user]:[password]@[host]:5432/clinicasim
```

### Arquivos de Configuração
```
├── appsettings.json          # Default
├── appsettings.Development.json
├── appsettings.Production.json
└── appsettings.local.json    # Local (não commitar)
```

---

## 🧪 Testes

### Rodar Testes Unitários
```bash
cd src

# Rodar todos
dotnet test

# Ou específico
dotnet test ClinicaSim.Domain.Tests

# Com cobertura
dotnet test /p:CollectCoverage=true
```

### Estrutura de Testes (Futuro)
```
tests/
├── ClinicaSim.Domain.Tests/
├── ClinicaSim.Application.Tests/
├── ClinicaSim.Infrastructure.Tests/
└── ClinicaSim.Api.Tests/
```

---

## 🐛 Troubleshooting

### "Database connection failed"
```bash
# Verificar connection string
echo $ConnectionStrings__ClinicaSim

# Testar conexão
psql -U postgres -d clinicasim -c "SELECT 1"

# Se usando Docker
docker logs clinicasim-postgres
```

### "EF Core migration error"
```bash
# Limpar migrations não aplicadas
dotnet ef migrations remove -p ClinicaSim.Infrastructure --force

# Resetar banco (cuidado!)
dotnet ef database drop -p ClinicaSim.Infrastructure
dotnet ef database update -p ClinicaSim.Infrastructure
```

### "Port 5000 already in use"
```bash
# Windows
netstat -ano | findstr :5000
taskkill /PID [PID] /F

# macOS/Linux
lsof -i :5000
kill -9 [PID]

# Ou usar porta diferente
dotnet run -- --urls http://localhost:5001
```

### "Swagger não aparece"
```bash
# Verificar Program.cs tem:
app.UseSwagger();
app.UseSwaggerUI();

# Acessar em
http://localhost:5000/swagger
```

### "CORS error no frontend"
```bash
# Verificar Program.cs tem:
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

app.UseCors("DevCors");
```

---

## 📦 Dependências Principais

| Pacote | Versão | Uso |
|--------|--------|-----|
| `Microsoft.EntityFrameworkCore` | 8.0 | ORM |
| `Npgsql.EntityFrameworkCore.PostgreSQL` | 8.0 | Driver PostgreSQL |
| `Swashbuckle.AspNetCore` | 6.x | Swagger/OpenAPI |
| `Microsoft.AspNetCore.Cors` | 8.0 | CORS |

### Instalar Novo Pacote
```bash
dotnet add ClinicaSim.Infrastructure package NomeDoPackote --version 1.0.0
```

---

## 🚀 Deploy

### Localmente (IIS)
```bash
# Build para publicação
dotnet publish -c Release -o ./publish

# Copiar para wwwroot do IIS
# Configurar site no IIS Manager
```

### Docker
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY publish .
EXPOSE 5000
ENTRYPOINT ["dotnet", "ClinicaSim.Api.dll"]
```

### Supabase + Azure/AWS
1. Build release
2. Push para repositório
3. CI/CD pipeline (GitHub Actions, etc)
4. Deploy automático

---

## 📚 Documentação Adicional

- [ARCHITECTURE.md](./ARCHITECTURE.md) - Arquitetura do projeto
- [DATABASE.md](./DATABASE.md) - Banco de dados
- [BUSINESS_RULES.md](./BUSINESS_RULES.md) - Regras de negócio
- [API.md](./API.md) - Referência da API
- [CONTRIBUTING.md](./CONTRIBUTING.md) - Como contribuir

---

## 💡 Dicas de Desenvolvimento

### Hot Reload
```bash
dotnet watch run

# Ao salvar arquivo, app reinicia automaticamente
```

### EF Core SQL
```bash
# Ver SQL gerado
dotnet ef dbcontext script -p ClinicaSim.Infrastructure
```

### Seed Manual
```bash
# Em Program.cs, desenvolvimento automático seed
# Para adicionar mais casos, editar DbSeeder.cs
```

### VS Code Extensions (Recomendadas)
- C# (Dev Kit)
- REST Client
- SQLTools

---

## 🤝 Próximos Passos

1. ✅ Setup concluído
2. Ler [ARCHITECTURE.md](./ARCHITECTURE.md)
3. Explorar código com IDE
4. Rodar aplicação localmente
5. Testar endpoints via Swagger
6. Ver [CONTRIBUTING.md](./CONTRIBUTING.md) para contribuir

---

## 📞 Suporte

- **Issues**: GitHub Issues
- **Docs**: Pasta `/docs`
- **Email**: thiagochaves@email.com

---

**Sucesso! 🚀**
