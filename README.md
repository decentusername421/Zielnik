Podział pracy na 3 osoby
Osoba 1 — Backend i baza danych
Zakres prac
konfiguracja projektu ASP.NET Core,
konfiguracja bazy danych,
stworzenie modeli ORM,
migracje,
relacje między modelami,
walidatory modeli,
przygotowanie REST API,
filtrowanie i sortowanie danych,
zabezpieczenie endpointów.
Odpowiedzialność

Najważniejsza logika backendowa oraz poprawne działanie bazy danych.

Główne pliki
Models/
Controllers/
DTOs/
Services/
Repositories/
Data/ApplicationDbContext.cs
Program.cs
Osoba 2 — Frontend i interfejs użytkownika
Zakres prac
stworzenie widoków aplikacji,
formularze dodawania i edycji,
dashboard użytkownika,
lista ogrodów,
lista roślin,
lista zadań,
obsługa komunikacji z API,
responsywny wygląd aplikacji,
obsługa błędów formularzy.
Odpowiedzialność

Cały interfejs użytkownika i UX.

Widoki
logowanie/rejestracja,
panel główny,
szczegóły ogrodu,
szczegóły rośliny,
harmonogram zadań.
Osoba 3 — System użytkowników, powiadomienia i testy
Zakres prac
autoryzacja i role użytkowników,
zabezpieczenia dostępu,
system przypomnień,
zadania „na dziś”,
opcjonalne powiadomienia email,
testowanie aplikacji,
przygotowanie seed danych,
dokumentacja projektu,
README,
przygotowanie prezentacji.
Odpowiedzialność

Bezpieczeństwo, stabilność projektu i finalne dopracowanie.

Wspólne zasady pracy
Git
każdy pracuje na osobnym branchu,
pull requesty do main,
częste commity,
sensowne nazwy commitów.
Proponowana struktura branchy
main
backend
frontend
auth-notifications
