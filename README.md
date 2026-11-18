# Questigma

Questigma é um jogo mobile de investigação de mistérios fictícios. Esta issue prepara a API ASP.NET Core e o aplicativo Flutter; as regras do jogo serão adicionadas em issues futuras. Veja [visão do produto](docs/PRODUCT.md) e [decisões de arquitetura](docs/architecture/ADR-001.md).

## Pré-requisitos

- .NET SDK 10
- Flutter SDK com Dart (para Android, Android SDK e emulador ou dispositivo configurado)

## Backend

Na raiz do repositório:

```powershell
dotnet restore backend/Questigma.slnx
dotnet build backend/Questigma.slnx --no-restore
dotnet test backend/Questigma.slnx --no-build
dotnet run --project backend/src/Questigma.Api --urls http://localhost:5000
```

Em outro terminal, `curl http://localhost:5000/health` deve retornar HTTP 200.

O monólito usa quatro camadas: Domain, Application, Infrastructure e Api. As dependências são Application → Domain, Infrastructure → Application e Domain, Api → Application e Infrastructure. Dentro de cada camada, as funcionalidades futuras serão agrupadas em Investigation, Activities, Progression e Identity. Esses diretórios estão vazios por enquanto.

## Mobile

```powershell
cd mobile
flutter pub get
flutter analyze
flutter test
flutter run
```

Para gerar um APK Android, use `flutter build apk --debug` após configurar o Android SDK.
