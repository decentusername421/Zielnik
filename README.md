# Zielnik

Zielnik to aplikacja webowa do zarządzania ogrodem, planowania nasadzeń
i monitorowania prac ogrodniczych. Projekt został wykonany w ASP.NET Core 8
z wykorzystaniem MVC, REST API, Entity Framework Core, SQLite, ASP.NET
Identity oraz uwierzytelniania JWT.

## Najważniejsze funkcje

- rejestracja i logowanie użytkownika,
- role `User` i `Admin`,
- tworzenie, edycja i usuwanie własnych ogrodów,
- dodawanie roślin do ogrodu,
- katalog roślin wspólnych i prywatnych,
- moderacja roślin oraz propozycji kategorii,
- automatyczne i ręcznie planowane zadania,
- oznaczanie zadań jako wykonane,
- edycja i usuwanie ręcznie zaplanowanych zadań,
- historia wykonanych prac,
- statystyki ogrodów, nasadzeń, zadań i zbiorów,
- izolacja danych pomiędzy kontami użytkowników.

## Technologie

- .NET 8 / ASP.NET Core MVC i Web API
- Entity Framework Core
- SQLite
- ASP.NET Core Identity
- JWT Bearer Authentication
- Razor Views, HTML, CSS i JavaScript
- Swagger / OpenAPI

## Uruchomienie

```powershell
dotnet restore
dotnet run
```

Przy uruchomieniu aplikacja automatycznie stosuje migracje i uzupełnia bazę
danymi demonstracyjnymi.

Domyślne adresy:

- aplikacja: `http://localhost:5031`
- Swagger: `http://localhost:5031/swagger`

Konta demonstracyjne mają hasło `Zielnik123!`:

- `admin@zielnik.pl` - administrator,
- `anna@zielnik.pl` - użytkownik,
- `jan@zielnik.pl` - użytkownik.

## Struktura

- `Entities/` - modele bazy danych,
- `DTOs/` - obiekty wymiany danych i walidacja wejścia,
- `Data/` - DbContext, konfiguracja relacji i seed danych,
- `Controllers/` - kontrolery API i kontrolery stron MVC,
- `Views/` - interfejs użytkownika w Razor,
- `Migrations/` - historia zmian schematu bazy,
- `wwwroot/` - pliki statyczne i skrypty.

Szczegółowy opis architektury, funkcji i możliwych pytań znajduje się
w pliku [DOKUMENTACJA_PREZENTACJI.md](DOKUMENTACJA_PREZENTACJI.md).
