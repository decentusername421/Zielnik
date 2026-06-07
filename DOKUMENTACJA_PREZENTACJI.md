# Zielnik - dokumentacja projektu i przygotowanie do prezentacji

## 1. Cel projektu

Zielnik wspiera użytkownika w prowadzeniu własnego ogrodu. Użytkownik może
tworzyć ogrody, dodawać do nich nasadzenia, planować prace ogrodnicze,
oznaczać je jako wykonane oraz analizować historię i statystyki aktywności.

Najważniejszym założeniem jest rozdzielenie danych użytkowników. Ogród,
nasadzenie, zadanie, zbiór lub notatka są dostępne tylko właścicielowi
powiązanego konta. Wyjątkiem jest wspólna baza zatwierdzonych roślin
i kategorii.

## 2. Architektura

Projekt wykorzystuje połączenie wzorca MVC i REST API:

- **Model**: encje w katalogu `Entities` oraz konfiguracja EF Core.
- **View**: widoki Razor w katalogu `Views`.
- **Controller**: kontrolery stron oraz kontrolery API.
- **DTO**: osobne klasy wejścia i wyjścia ograniczające dane przesyłane
  pomiędzy frontendem a backendem.

Widoki Razor odpowiadają za układ strony. JavaScript wywołuje endpointy API
przez `fetch`. Token JWT jest zapisywany w `localStorage` i przesyłany
w nagłówku `Authorization: Bearer ...`.

Strona MVC może zostać otwarta bez tokenu, ale chronione dane nie zostaną
zwrócone, ponieważ właściwe endpointy API posiadają atrybut `[Authorize]`.

## 3. Baza danych i ORM

Projekt używa SQLite oraz Entity Framework Core. Kontekst bazy znajduje się
w `Data/ZielnikDbContextClass.cs`.

EF Core realizuje:

- mapowanie klas C# na tabele,
- relacje i klucze obce,
- operacje CRUD,
- filtrowanie danych po użytkowniku,
- sortowanie zadań i historii,
- agregacje i sumowanie w statystykach,
- migracje schematu bazy.

Przy starcie `Program.cs` przygotowuje schemat bazy przed utworzeniem ról
i seedowaniem. Istniejąca baza jest aktualizowana migracjami, a dla pustej
bazy tworzony jest aktualny schemat. Następnie wykonywany jest
`SeedData.Initialize(context)`.

## 4. Najważniejsze modele i relacje

### Garden

Reprezentuje ogród użytkownika.

- należy do jednego `IdentityUser`,
- zawiera wiele `UserPlant`,
- nazwa ma walidację `Required` i `MaxLength(100)`.

Relacja: użytkownik 1:N ogrody.

### Plant

Reprezentuje pozycję w katalogu roślin.

- zawiera nazwę, gatunek i częstotliwości prac,
- może mieć wiele kategorii,
- może występować w wielu ogrodach przez `UserPlant`,
- posiada status moderacji,
- może przechowywać identyfikator autora propozycji.

Relacje:

- `Plant` N:M `PlantCategory`,
- `Plant` 1:N `UserPlant`,
- autor 1:N prywatne propozycje roślin.

### UserPlant

Jest nasadzeniem, czyli konkretną rośliną umieszczoną w konkretnym ogrodzie.
To model pośredni pomiędzy `Garden` i `Plant`, ale przechowuje także własne
dane biznesowe:

- pseudonim rośliny,
- datę siewu i sadzenia,
- status uprawy,
- ustawienia przypomnienia o zbiorach.

Posiada kolekcje notatek, zabiegów, zbiorów i zdjęć.

### PlantTreatment

Reprezentuje pracę ogrodniczą, np. podlewanie, nawożenie lub oprysk.

W projekcie wykorzystano ten model także do ręcznie planowanych zadań:

- `Notes == "Zaplanowane"` oznacza zadanie oczekujące,
- po wykonaniu znacznik jest usuwany,
- `PerformedAt` zmienia się z terminu planowanego na czas wykonania,
- wykonana pozycja trafia do historii prac.

Takie rozwiązanie upraszcza model i nie wymaga osobnej tabeli dla zadań,
a jednocześnie zachowuje historię wykonania.

### Harvest

Przechowuje datę zbioru, ilość, jednostkę, liczbę owoców i notatkę.
Statystyki sumują `Quantity` dla zbiorów należących do ogrodów użytkownika.

### PlantCategory

Kategorie są wspólne po zatwierdzeniu przez administratora. Użytkownik może
przesłać propozycję. Dopóki `IsApproved` ma wartość `false`, propozycję widzi
administrator na liście oczekujących.

## 5. Uwierzytelnianie i role

Rejestracja i logowanie są obsługiwane przez `AuthController`.

Podczas rejestracji:

1. tworzony jest `IdentityUser`,
2. hasło jest hashowane przez ASP.NET Identity,
3. użytkownik otrzymuje rolę `User`.

Podczas logowania backend:

1. wyszukuje konto po emailu,
2. sprawdza hasło,
3. pobiera role,
4. generuje token JWT.

Token zawiera:

- nazwę użytkownika,
- identyfikator konta,
- role.

Rola administratora pozwala:

- zarządzać wspólną bazą roślin,
- akceptować lub odrzucać propozycje roślin,
- dodawać kategorie bez oczekiwania,
- akceptować lub odrzucać propozycje kategorii.

## 6. Ochrona danych użytkownika

Backend nie przyjmuje identyfikatora właściciela z formularza. Pobiera go
z tokenu:

```csharp
User.FindFirstValue(ClaimTypes.NameIdentifier)
```

Przykładowo aktualizacja ogrodu wyszukuje rekord jednocześnie po `Garden.Id`
i `Garden.UserId`. Znajomość identyfikatora cudzego ogrodu nie wystarcza do
jego odczytania, edycji ani usunięcia.

Podobna kontrola jest stosowana dla:

- nasadzeń,
- ręcznych zadań,
- historii i statystyk,
- notatek, zdjęć, zabiegów i zbiorów.

## 7. Ogrody

Widok: `Views/GardensPage/Index.cshtml`.

Użytkownik może:

- utworzyć ogród,
- zobaczyć swoje ogrody,
- zmienić nazwę,
- usunąć ogród,
- przejść do szczegółów.

Usunięcie ogrodu usuwa dane zależne zgodnie z relacjami EF Core. Operacja
jest potwierdzana w interfejsie, ponieważ obejmuje także nasadzenia i ich
historię.

W szczegółach ogrodu użytkownik wybiera roślinę z katalogu i tworzy
`UserPlant`. Lista katalogowa zawiera rośliny zatwierdzone oraz prywatne
rośliny bieżącego użytkownika.

## 8. Moderacja roślin

Przepływ dla zwykłego użytkownika:

1. użytkownik dodaje roślinę w widoku bazy,
2. roślina otrzymuje `IsApproved = false`,
3. `CreatedByUserId` wskazuje autora,
4. autor widzi ją i może dodać ją do własnego ogrodu,
5. inni użytkownicy jej nie widzą.

Przepływ administratora:

1. administrator otwiera listę oczekujących,
2. może zaakceptować propozycję,
3. zaakceptowana roślina staje się widoczna dla wszystkich,
4. może odrzucić propozycję,
5. odrzucona roślina pozostaje prywatna dla autora.

Status `IsRejected` odróżnia propozycję oczekującą od odrzuconej.

## 9. Moderacja kategorii

Zwykły użytkownik nie dodaje kategorii bezpośrednio do wspólnej bazy.
Formularz tworzy propozycję z `IsApproved = false`.

Administrator widzi osobną listę kategorii oczekujących i może:

- zatwierdzić kategorię,
- odrzucić ją przez usunięcie propozycji.

Administrator tworzący kategorię otrzymuje od razu `IsApproved = true`.

## 10. Harmonogram zadań

Widok: `Views/TasksPage/Index.cshtml`.

System obsługuje dwa źródła zadań:

### Zadania automatyczne

Są obliczane na podstawie:

- daty sadzenia,
- częstotliwości podlewania,
- częstotliwości nawożenia,
- częstotliwości oprysku,
- liczby dni do zbioru.

Nie są osobnymi rekordami przed wykonaniem. Backend generuje je podczas
zapytania o harmonogram.

### Zadania ręczne

Są zapisane w bazie jako `PlantTreatment` ze znacznikiem `Zaplanowane`.
Mają własny identyfikator, dlatego użytkownik może:

- zmienić roślinę,
- zmienić typ zadania,
- zmienić termin,
- usunąć zadanie,
- oznaczyć je jako wykonane.

Edycja i usuwanie wymagają jednocześnie:

- identyfikatora zadania,
- statusu `Zaplanowane`,
- powiązania nasadzenia z ogrodem zalogowanego użytkownika.

Automatyczne przypomnienia można oznaczyć jako wykonane, ale nie edytuje się
ich bezpośrednio, ponieważ wynikają z parametrów rośliny.

## 11. Statystyki

`StatsController` pobiera tylko ogrody zalogowanego użytkownika, a następnie
powiązane nasadzenia i aktywności.

Wyświetlane są m.in.:

- liczba ogrodów,
- liczba nasadzeń,
- liczba aktywnych roślin,
- wykonane zadania,
- zaplanowane zadania,
- podlewania, nawożenia i opryski,
- liczba i łączna ilość zbiorów,
- liczba notatek.

Zastosowano `AsSplitQuery()`, aby EF Core nie tworzył jednego bardzo dużego
zapytania z wieloma kolekcjami. Poprawia to czytelność i wydajność zapytań.

## 12. Frontend

Frontend korzysta z Razor Views i JavaScript.

### `_Layout.cshtml`

Wspólny nagłówek aplikacji:

- linki do ogrodów, roślin, harmonogramu i statystyk,
- nazwa zalogowanego użytkownika odczytana z JWT,
- przycisk logowania lub wylogowania.

### `Account/Login.cshtml` i `Account/Register.cshtml`

Formularze wywołują API uwierzytelniania. Po logowaniu token jest zapisywany
w `localStorage`.

### `GardensPage/Index.cshtml`

Realizuje CRUD ogrodów. Lista jest odświeżana po każdej operacji bez
przeładowania całej strony.

### `GardensPage/Details.cshtml`

Pokazuje nasadzenia w wybranym ogrodzie oraz wspólną i prywatną bazę roślin.
Umożliwia dodanie rośliny do ogrodu i usunięcie konkretnego nasadzenia.

### `PlantsPage/Index.cshtml`

Formularz dodawania roślin, propozycje kategorii oraz listy moderacyjne.
Widoczność przycisków zależy od roli odczytanej z tokenu. Backend niezależnie
sprawdza rolę przez `[Authorize(Roles = "Admin")]`.

### `TasksPage/Index.cshtml`

Wyświetla zadania na dziś, zadania na 30 dni i historię. Ten sam formularz
służy do dodawania i edycji ręcznego zadania.

### `StatsPage/Index.cshtml`

Pobiera zagregowane dane z `/api/Stats` i przedstawia je w prostych kartach.

## 13. Walidacja

Walidacja znajduje się głównie w encjach i DTO:

- `[Required]`,
- `[EmailAddress]`,
- `[MinLength]`,
- `[MaxLength]`,
- `[Range]`.

Frontend wykonuje dodatkowe sprawdzenia dla wygody użytkownika, np. pustej
nazwy ogrodu lub niewybranej daty. Walidacja backendowa pozostaje
ważniejsza, ponieważ żądanie API można wysłać bez używania strony.

## 14. Seed danych

`SeedData` dodaje:

- role `Admin` i `User`,
- konta demonstracyjne,
- kategorie,
- katalog roślin,
- przykładowe ogrody i nasadzenia,
- historię zabiegów, zbiorów, notatek i zdjęć.

Seed sprawdza istnienie najważniejszych danych, aby nie tworzyć ich ponownie
przy każdym uruchomieniu.

## 15. Najważniejsze endpointy

### Uwierzytelnianie

| Metoda | Endpoint | Znaczenie |
|---|---|---|
| POST | `/api/Auth/register` | rejestracja |
| POST | `/api/Auth/login` | logowanie i JWT |

### Ogrody

| Metoda | Endpoint | Znaczenie |
|---|---|---|
| GET | `/api/Gardens` | własne ogrody |
| GET | `/api/Gardens/{id}` | szczegóły własnego ogrodu |
| POST | `/api/Gardens` | utworzenie ogrodu |
| PUT | `/api/Gardens/{id}` | zmiana nazwy |
| DELETE | `/api/Gardens/{id}` | usunięcie ogrodu |

### Rośliny

| Metoda | Endpoint | Znaczenie |
|---|---|---|
| GET | `/api/Plants` | rośliny wspólne i własne |
| POST | `/api/Plants` | dodanie lub propozycja |
| GET | `/api/Plants/pending` | propozycje dla admina |
| POST | `/api/Plants/{id}/approve` | akceptacja |
| POST | `/api/Plants/{id}/reject` | odrzucenie |

### Kategorie

| Metoda | Endpoint | Znaczenie |
|---|---|---|
| GET | `/api/PlantCategories` | zatwierdzone kategorie |
| POST | `/api/PlantCategories` | dodanie lub propozycja |
| GET | `/api/PlantCategories/pending` | propozycje dla admina |
| POST | `/api/PlantCategories/{id}/approve` | akceptacja |
| POST | `/api/PlantCategories/{id}/reject` | odrzucenie propozycji |
| DELETE | `/api/PlantCategories/{id}` | usunięcie kategorii przez admina |

### Zadania

| Metoda | Endpoint | Znaczenie |
|---|---|---|
| GET | `/api/Tasks/today` | zadania na dziś |
| GET | `/api/Tasks/upcoming?days=30` | najbliższe zadania |
| POST | `/api/Tasks/add-manual` | nowe ręczne zadanie |
| PUT | `/api/Tasks/{id}` | edycja ręcznego zadania |
| DELETE | `/api/Tasks/{id}` | usunięcie ręcznego zadania |
| POST | `/api/Tasks/complete` | wykonanie zadania |
| GET | `/api/Tasks/history` | historia prac |

## 16. Możliwe pytania podczas prezentacji

### Dlaczego użyto modelu UserPlant?

Sama roślina opisuje gatunek lub odmianę w katalogu. `UserPlant` opisuje
konkretne nasadzenie w konkretnym ogrodzie i może mieć własne daty, status,
pseudonim i historię.

### Jak zagwarantowano dostęp tylko do własnych danych?

Identyfikator użytkownika jest pobierany z JWT. Zapytania filtrują dane przez
`Garden.UserId`. Backend nie ufa identyfikatorowi właściciela przesłanemu
przez klienta.

### Dlaczego JWT?

API jest bezstanowe. Każde żądanie zawiera podpisany token z identyfikatorem
i rolą. Backend może zweryfikować użytkownika bez przechowywania sesji.

### Jaka jest różnica między Plant i UserPlant?

`Plant` jest pozycją katalogową. `UserPlant` jest egzemplarzem/nasadzeniem
należącym do użytkownika.

### Jak działa rola administratora?

Endpointy administracyjne mają `[Authorize(Roles = "Admin")]`. Frontend
ukrywa niedostępne przyciski, ale właściwe zabezpieczenie znajduje się
na backendzie.

### Dlaczego zadania ręczne są zapisane jako PlantTreatment?

Planowana praca po wykonaniu naturalnie staje się zabiegiem w historii.
Znacznik `Zaplanowane` reprezentuje stan przed wykonaniem i pozwala uniknąć
duplikowania bardzo podobnych modeli.

### Dlaczego nie można edytować automatycznego zadania?

Nie jest ono rekordem w bazie, tylko wynikiem obliczenia częstotliwości.
Zmiana harmonogramu automatycznego powinna wynikać ze zmiany parametrów
rośliny, a nie pojedynczego wygenerowanego wystąpienia.

### Co daje ORM?

EF Core mapuje modele na relacyjną bazę, obsługuje relacje, migracje, CRUD,
filtrowanie, sortowanie, grupowanie i agregacje bez ręcznego budowania SQL.

### Po co są DTO?

Oddzielają kontrakt API od encji bazy, ograniczają możliwe pola wejściowe
i są miejscem walidacji danych przesyłanych z formularzy.

### Jak działa moderacja prywatnej rośliny?

Autor widzi ją dzięki warunkowi `IsApproved || CreatedByUserId == userId`.
Inny użytkownik widzi tylko `IsApproved`. Administrator może zmienić status,
nie odbierając autorowi jego prywatnej rośliny.

### Dlaczego SQLite?

Jest relacyjną bazą danych, współpracuje z EF Core i ułatwia uruchomienie
projektu bez instalowania osobnego serwera. Dla projektu edukacyjnego
zapewnia wszystkie potrzebne mechanizmy relacyjne.

## 17. Proponowany przebieg prezentacji

1. Zalogować się jako zwykły użytkownik.
2. Pokazać nazwę użytkownika i główną nawigację.
3. Utworzyć ogród, zmienić jego nazwę i wejść w szczegóły.
4. Dodać prywatną roślinę i użyć jej we własnym ogrodzie.
5. Dodać zadanie na dziś, zmienić je i oznaczyć jako wykonane.
6. Pokazać historię i statystyki.
7. Zalogować się jako administrator.
8. Zaakceptować lub odrzucić propozycję rośliny i kategorii.
9. W Swaggerze pokazać zabezpieczone endpointy i role.

Podczas prezentacji warto podkreślić, że przyciski na frontendzie poprawiają
UX, ale bezpieczeństwo jest realizowane przez filtrowanie i autoryzację
po stronie backendu.
