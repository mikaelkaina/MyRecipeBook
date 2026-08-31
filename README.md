# MyRecipeBook

## Visão geral

O MyRecipeBook permite que usuários criem uma conta, se autentiquem e gerenciem sua coleção pessoal de receitas: criar, atualizar, excluir, filtrar e listar receitas, cada uma com ingredientes, modo de preparo, tipos de prato, tempo de cozimento e uma imagem ilustrativa opcional.

## Arquitetura

A solução está organizada em projetos independentes de responsabilidade única, com uma direção de dependência estrita (`Api → Application / Infrastructure → Domain`), mantendo a camada de Domínio livre de qualquer dependência externa:

```
src/
├── Backend/
│   ├── MyRecipeBook.Api             → Controllers, configuração de JWT/auth, filters, Swagger
│   ├── MyRecipeBook.Application     → Use cases, validação, mapeamento de objetos
│   ├── MyRecipeBook.Domain          → Entidades, enums, contratos de repositório (sem deps externas)
│   └── MyRecipeBook.Infrastructure  → EF Core, repositórios, segurança, blob storage
└── Shared/
    ├── MyRecipeBook.Communication   → DTOs de request/response (contratos da API)
    └── MyRecipeBook.Exception       → Exceções customizadas + mensagens de erro localizadas

tests/
├── UseCase.Tests           → Testes unitários dos use cases da aplicação
├── Validators.Tests        → Testes unitários das regras do FluentValidation
├── WebApi.Tests             → Testes de integração contra um banco de dados real
└── CommoTestsUtilities      → Builders e fixtures de teste compartilhados
```

Cada use case (ex.: `RecipeRegisterUseCase`, `LoginWithEmailAndPasswordUseCase`) é exposto por meio de sua própria interface e resolvido diretamente pelos controllers via `[FromServices]`

## Stack técnica

| Camada | Tecnologias |
|---|---|
| Runtime | .NET 10, ASP.NET Core Web API |
| Banco de dados | SQL Server, EF Core 10 (configurações via Fluent API, migrations code-first) |
| Autenticação | Tokens JWT Bearer, `IAccessTokenGenerator` customizado |
| Hash de senha | Argon2 (`Konscious.Security.Cryptography`) |
| Armazenamento de arquivos | Azure Blob Storage (`Azure.Storage.Blobs`) |
| Validação | FluentValidation |
| Mapeamento de objetos | Mapster |
| Validação de tipo de arquivo | `FileTypeChecker` (valida a assinatura real do arquivo, não só a extensão) |
| Documentação da API | Swagger / OpenAPI com suporte a Bearer auth |
| Testes | xUnit, Shouldly, `WebApplicationFactory`, Coverlet |

## Principais funcionalidades

- **Gerenciamento de conta** — cadastro, consulta/atualização de perfil, troca de senha, upload de foto de perfil.
- **Autenticação** — login com email/senha, recuperação de senha via código de verificação, sessão baseada em JWT.
- **Gerenciamento de receitas** — criar, atualizar, excluir, buscar por id, listar receitas recentes, filtrar por critérios, upload/substituição de imagem ilustrativa.
- **Suporte multi-idioma (i18n)** — `en` e `pt-BR`, resolvido via header `Accept-Language`, com mensagens de erro localizadas a partir de arquivos `.resx`.

## Pontos técnicos de destaque

- **Interfaces de repositório segregadas** — `IUserReadOnlyRepository`, `IUserWriteOnlyRepository`, `IUserUpdateOnlyRepository` (e equivalentes para `Recipe` e `VerificationCode`), separando as responsabilidades de leitura/escrita/atualização na camada de dados.
- **Chaves primárias UUID v7** (`Guid.CreateVersion7()`) — identificadores ordenáveis por tempo, mais amigáveis a índices do que UUIDs aleatórios.
- **Validação real do conteúdo de arquivos** — imagens enviadas são checadas pela assinatura do arquivo (magic bytes), não apenas pela extensão, antes de serem aceitas.
- **Tratamento global de exceções** — um único `ExceptionFilter` mapeia exceções de domínio para os status HTTP corretos e loga exceções não tratadas via `ILogger`.
- **Persistência de enums como string** — `CookTime`, `DishType`, etc. são armazenados como texto (`HasConversion<string>()`) para um schema de banco mais legível e estável.
- **Unit of Work explícito**, coordenando operações de repositório dentro de um limite transacional.
- **Suíte de testes automatizados em camadas** — testes unitários para use cases e validators, além de testes de integração completos subindo a API em memória (`WebApplicationFactory`) contra um banco de dados real.
