# Questigma

Questigma é um jogo mobile de investigação de mistérios fictícios. Esta issue prepara a API ASP.NET Core e o aplicativo Flutter; as regras do jogo serão adicionadas em issues futuras. Veja [visão do produto](docs/PRODUCT.md) e [decisões de arquitetura](docs/architecture/ADR-001.md).

## Pré-requisitos

- .NET SDK 10
- Docker Desktop ou Docker Engine com Compose
- Flutter SDK com Dart (para Android, Android SDK e emulador ou dispositivo configurado)

## Backend

Na raiz do repositório:

```powershell
dotnet restore backend/Questigma.slnx
dotnet build backend/Questigma.slnx --no-restore
dotnet test backend/tests/Questigma.IntegrationTests/Questigma.IntegrationTests.csproj --no-build
dotnet test backend/tests/Questigma.UnitTests/Questigma.UnitTests.csproj --no-build
dotnet run --project backend/src/Questigma.Api --urls http://localhost:5000
```

Em outro terminal, `curl http://localhost:5000/health` deve retornar HTTP 200. Apenas em `Development`, `curl http://localhost:5000/health/db` retorna HTTP 200 quando a conexão com PostgreSQL funciona e HTTP 503 caso contrário, sem detalhes da conexão na resposta. Em outros ambientes, `/health/db` não é registrado e retorna HTTP 404. A API exige a configuração `ConnectionStrings:Questigma` na inicialização e não executa migrations automaticamente.

## PostgreSQL local

Na raiz do repositório, copie `.env.example` para `.env` e substitua `POSTGRES_PASSWORD` por uma senha local. O arquivo `.env` é ignorado pelo Git. O Compose lê `POSTGRES_DB`, `POSTGRES_USER`, `POSTGRES_PASSWORD` e opcionalmente `POSTGRES_PORT` desse arquivo; o banco fica acessível somente em `127.0.0.1` e seus dados ficam no volume nomeado. A senha do exemplo é apenas um marcador e deve ser trocada.

```powershell
Copy-Item .env.example .env
# Edite .env antes de continuar.
docker compose config --quiet
docker compose up -d --wait
```

O Compose **não** injeta essas variáveis na API executada no host. Configure separadamente `ConnectionStrings__Questigma` no terminal da API, usando os valores escolhidos em `.env` (e a porta de `POSTGRES_PORT`):

```powershell
$env:ConnectionStrings__Questigma = 'Host=127.0.0.1;Port=5432;Database=questigma;Username=questigma;Password=<senha-local>'
dotnet run --project backend/src/Questigma.Api --urls http://localhost:5000
```

Em bash, use `export ConnectionStrings__Questigma='Host=127.0.0.1;Port=5432;Database=questigma;Username=questigma;Password=<senha-local>'`. Não grave a senha em `appsettings*.json`, no código ou em arquivos versionados. Se preferir, use um gerenciador de segredos local. Para parar e reiniciar preservando os dados, execute `docker compose down` e depois `docker compose up -d --wait`. **`docker compose down -v` apaga o volume e todos os dados locais**.

## Migrations EF Core

As versões fixadas são .NET 10, PostgreSQL 17, EF Core/Design/`dotnet-ef` 10.0.15 e Npgsql.EntityFrameworkCore.PostgreSQL 10.0.4. O manifesto `dotnet-tools.json` instala o `dotnet-ef` local. Depois de definir `ConnectionStrings__Questigma` no terminal:

```powershell
dotnet tool restore
dotnet tool run dotnet-ef dbcontext info --project backend/src/Questigma.Infrastructure --startup-project backend/src/Questigma.Api
dotnet tool run dotnet-ef migrations add NomeDaMigration --project backend/src/Questigma.Infrastructure --startup-project backend/src/Questigma.Api --output-dir Persistence/Migrations
dotnet tool run dotnet-ef database update --project backend/src/Questigma.Infrastructure --startup-project backend/src/Questigma.Api
```

A migration `InitialCatalog` cria somente `catalog_cases`. A aplicação de migrations é uma ação explícita; a API não altera o esquema ao iniciar.

Se `docker compose config` reclamar de variáveis ausentes, confira `.env` na raiz e seus três valores obrigatórios. Se o Compose não alcançar o daemon, inicie Docker Desktop/Engine. Se a porta estiver ocupada, altere `POSTGRES_PORT` em `.env` e a porta na connection string da API. Em `Development`, se `/health` retornar 200 mas `/health/db` retornar 503, confira se o contêiner está saudável com `docker compose ps`, a connection string e se a senha corresponde à do volume existente; alterar `POSTGRES_PASSWORD` após criar o volume não redefine o usuário já criado. Se a API apontar `ConnectionStrings__Questigma` ausente, defina essa variável no **mesmo terminal** em que executar `dotnet run`.

O monólito usa quatro camadas: Domain, Application, Infrastructure e Api. As dependências são Application → Domain, Infrastructure → Application e Domain, Api → Application e Infrastructure.

### Catálogo público de casos

`GET /api/v1/cases` lista somente casos publicados com `id`, `title` e `synopsis`. `GET /api/v1/cases/{id}` acrescenta `introduction`. Em `Development`, a especificação OpenAPI está em `/openapi/v1.json`; em `Production`, essa rota não é registrada. Um caso não publicado e um ID inexistente recebem o mesmo HTTP 404 em `application/problem+json`; um ID malformado recebe HTTP 400 no mesmo formato.

`CatalogCase` representa somente o catálogo público: uma tabela pequena (`catalog_cases`) com os textos públicos e `is_published`. A publicação é estado explícito do domínio, iniciado como rascunho por padrão; não há endpoint editorial nesta issue. A lista e os detalhes filtram a publicação no PostgreSQL e projetam diretamente os campos permitidos. `GameCase`, solução, suspeito correto, regras de acusação, deduções e evidências não são carregados nem serializados por esses endpoints. A associação do catálogo ao conteúdo canônico de `GameCase` e a validação de que um caso publicado é jogável serão tratadas posteriormente, junto da persistência de `Investigation`. Textos de título, sinopse e introdução devem ser revisados como conteúdo público antes de publicar.

Os testes de integração sobem PostgreSQL 17 em um contêiner temporário independente do banco de desenvolvimento; Docker precisa estar disponível para executá-los.

## Mobile

```powershell
cd mobile
flutter pub get
flutter analyze
flutter test
flutter run
```

Para gerar um APK Android, use `flutter build apk --debug` após configurar o Android SDK.
