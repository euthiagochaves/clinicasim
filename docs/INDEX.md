# 📚 Documentação ClinicaSim

Bem-vindo à documentação do ClinicaSim! Este é o índice central para toda a documentação do projeto.

## 🗂️ Estrutura de Documentação

### 1. 🚀 [SETUP.md](./SETUP.md)
**Para**: Desenvolvedores configurando o projeto

Contém:
- Pré-requisitos (SDK, DB, ferramentas)
- Setup rápido (5 minutos)
- Configuração de banco de dados
- Migrações do EF Core
- Troubleshooting
- Dicas de desenvolvimento

**Leia este primeiro!** ⭐

---

### 2. 🏗️ [ARCHITECTURE.md](./ARCHITECTURE.md)
**Para**: Entender a estrutura do projeto

Contém:
- Visão geral da arquitetura
- Camadas (Domain, Application, Infrastructure, Presentation)
- Padrões de design (Clean Architecture, DDD)
- Fluxo de dados
- Decisões arquiteturais
- Responsabilidades por camada

**Estude após configurar!** ⭐⭐

---

### 3. 🗄️ [DATABASE.md](./DATABASE.md)
**Para**: Trabalhar com banco de dados

Contém:
- Diagrama ER (Entidade-Relacionamento)
- Descrição detalhada de 9 tabelas
- Relacionamentos e cascatas
- Migrações
- Índices para performance
- Queries comuns
- Backup & recovery
- Tuning

**Consulte ao trabalhar com dados!**

---

### 4. 📋 [BUSINESS_RULES.md](./BUSINESS_RULES.md)
**Para**: Entender regras e validações

Contém:
- 21+ Regras de Negócio (RN-XXX)
- Validações por entidade
- Workflows e transições de estado
- Regras de segurança
- Checklist de implementação
- Exemplos de código

**Consulte ao implementar features!**

---

### 5. 📡 [API.md](./API.md)
**Para**: Usar/testar a API

Contém:
- Base URL e convenções
- Todos os 9 endpoints com exemplos
- Request/Response schemas
- Status codes e erros
- Fluxo recomendado
- Exemplos com cURL
- Swagger link
- Performance recomendações

**Consulte ao integrar frontend!**

---

### 6. 🤝 [CONTRIBUTING.md](./CONTRIBUTING.md)
**Para**: Contribuir com código

Contém:
- Código de Conduta
- Como reportar bugs
- Como sugerir features
- Processo de Pull Request
- Estilo de código (C#, Architecture)
- Padrões de testes
- Documentação esperada
- Checklist pré-commit

**Leia antes de fazer PR!**

---

## 🎯 Guia Rápido por Perfil

### 👨‍💻 Eu sou Desenvolvedor

1. **Primeiro dia?**
   - Leia [SETUP.md](./SETUP.md) e instale
   - Leia [ARCHITECTURE.md](./ARCHITECTURE.md) para entender estrutura
   - Explore o código

2. **Vou implementar feature?**
   - Leia [BUSINESS_RULES.md](./BUSINESS_RULES.md) para validações
   - Leia [DATABASE.md](./DATABASE.md) se for adicionar dados
   - Leia [ARCHITECTURE.md](./ARCHITECTURE.md) para padrões

3. **Vou fazer um bug fix?**
   - Leia a issue no GitHub
   - Consulte [API.md](./API.md) se envolver REST
   - Consulte [BUSINESS_RULES.md](./BUSINESS_RULES.md) para validar fix

4. **Vou fazer PR?**
   - Leia [CONTRIBUTING.md](./CONTRIBUTING.md)
   - Rode testes e formatting
   - Abra PR bem documentado

### 🎨 Eu sou Designer/Frontend

1. **Entender fluxo?**
   - Leia [API.md](./API.md) - vê todos endpoints
   - Leia [ARCHITECTURE.md](./ARCHITECTURE.md) - fluxo de dados
   - Explore [SETUP.md](./SETUP.md) para rodar localmente

2. **Integrar com backend?**
   - Consulte [API.md](./API.md) para schemas exatos
   - Teste endpoints em http://localhost:5000/swagger
   - Veja exemplos de Request/Response

### 📊 Eu sou Product/PM

1. **Entender projeto?**
   - Leia [ARCHITECTURE.md](./ARCHITECTURE.md) - visão geral
   - Leia [BUSINESS_RULES.md](./BUSINESS_RULES.md) - funcionalidades
   - Leia [DATABASE.md](./DATABASE.md) - dados do sistema

2. **Planejar feature?**
   - Consulte [BUSINESS_RULES.md](./BUSINESS_RULES.md)
   - Abra issue no GitHub
   - Discuta com time

---

## 📖 Leitura Progressiva (Recomendado)

### Semana 1
- [ ] [SETUP.md](./SETUP.md) - Instalar e rodar
- [ ] [README.md](../README.md) - Visão geral
- [ ] [ARCHITECTURE.md](./ARCHITECTURE.md) - Estrutura

### Semana 2
- [ ] [DATABASE.md](./DATABASE.md) - Banco de dados
- [ ] [API.md](./API.md) - REST endpoints
- [ ] [BUSINESS_RULES.md](./BUSINESS_RULES.md) - Regras

### Semana 3+
- [ ] [CONTRIBUTING.md](./CONTRIBUTING.md) - Como contribuir
- [ ] Explorar código
- [ ] Fazer primeira contribuição

---

## 🔍 Encontrando Informações

### "Como faço X?"

| Pergunta | Onde Procurar |
|----------|-----------------|
| Como instalar? | [SETUP.md](./SETUP.md) |
| Como iniciar sessão? | [API.md](./API.md) |
| Como testar API? | [API.md](./API.md) - Swagger link |
| Como criar migration? | [SETUP.md](./SETUP.md) ou [DATABASE.md](./DATABASE.md) |
| Como contribuir? | [CONTRIBUTING.md](./CONTRIBUTING.md) |
| Qual regra de negócio de X? | [BUSINESS_RULES.md](./BUSINESS_RULES.md) |
| Qual a estrutura de dados? | [DATABASE.md](./DATABASE.md) |
| Como funciona arquitetura? | [ARCHITECTURE.md](./ARCHITECTURE.md) |
| Qual estilo de código? | [CONTRIBUTING.md](./CONTRIBUTING.md) |

---

## 📚 Conceitos Chave

### Arquitetura Clean Architecture
[ARCHITECTURE.md](./ARCHITECTURE.md) - Seção "Estrutura de Camadas"

4 camadas independentes:
1. **Domain** - Entidades e regras puras
2. **Application** - Use cases e interfaces
3. **Infrastructure** - Acesso a dados
4. **Presentation** - REST API

### Session Codes
[BUSINESS_RULES.md](./BUSINESS_RULES.md) - RN-101

- 10 caracteres alfanuméricos
- Geração criptográfica aleatória
- Únicos no banco
- Impossível adivinhar

### Session Lifecycle
[BUSINESS_RULES.md](./BUSINESS_RULES.md) - RN-100

Estados: `Active` → `Finalized`
Apenas operações em sessão ativa

### Database
[DATABASE.md](./DATABASE.md) - Seção "Tabelas"

9 tabelas com relacionamentos 1:N
Integridade referencial com CASCADE/RESTRICT

---

## 🔗 Links Rápidos

### Na Documentação
- [README principal](../README.md)
- [.env.example](.env.example) - Template de ambiente

### Externo
- [GitHub](https://github.com/euthiagochaves/clinicasim)
- [.NET 8 Docs](https://docs.microsoft.com/dotnet)
- [EF Core Docs](https://learn.microsoft.com/ef/core)
- [PostgreSQL Docs](https://www.postgresql.org/docs)
- [Swagger/OpenAPI](https://swagger.io)

---

## 📞 Precisa de Ajuda?

1. **Consulte documentação** - Comece com este INDEX
2. **Procure nos documentos** - Use Ctrl+F para buscar termo
3. **Veja o código** - O código é auto-documentado
4. **Abra issue** - GitHub Issues para dúvidas/bugs
5. **Email** - thiagochaves@email.com

---

## 🔄 Mantendo Documentação Atualizada

Quando você:
- ✅ Adiciona endpoint → Atualiza [API.md](./API.md)
- ✅ Muda regra → Atualiza [BUSINESS_RULES.md](./BUSINESS_RULES.md)
- ✅ Adiciona tabela → Atualiza [DATABASE.md](./DATABASE.md)
- ✅ Muda arquitetura → Atualiza [ARCHITECTURE.md](./ARCHITECTURE.md)
- ✅ Cria processo novo → Atualiza [CONTRIBUTING.md](./CONTRIBUTING.md)

---

## 📊 Estatísticas da Documentação

| Documento | Linhas | Tópicos | Exemplos |
|-----------|--------|---------|----------|
| SETUP.md | ~400 | 12+ | 30+ |
| ARCHITECTURE.md | ~300 | 10+ | 15+ |
| DATABASE.md | ~500 | 15+ | 10+ |
| BUSINESS_RULES.md | ~600 | 21+ RNs | 40+ |
| API.md | ~400 | 9 endpoints | 20+ |
| CONTRIBUTING.md | ~450 | 20+ | 25+ |
| **TOTAL** | **~2650** | **~100+** | **~140+** |

---

## ✅ Checklist de Leitura

- [ ] Li este INDEX
- [ ] Li [SETUP.md](./SETUP.md) e configurei ambiente
- [ ] Li [ARCHITECTURE.md](./ARCHITECTURE.md)
- [ ] Li [API.md](./API.md) e testei endpoints
- [ ] Li [BUSINESS_RULES.md](./BUSINESS_RULES.md)
- [ ] Li [DATABASE.md](./DATABASE.md)
- [ ] Li [CONTRIBUTING.md](./CONTRIBUTING.md)
- [ ] Explorei código
- [ ] Pronto para contribuir! 🚀

---

<div align="center">

**Documentação ClinicaSim**

Versão 1.0 | Última atualização: March 2026

[Voltar para README](../README.md)

</div>
