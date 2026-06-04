🌿 ZIELNIK — Aplikacja do zarządzania ogrodem
Zielnik to aplikacja webowa umożliwiająca zarządzanie ogrodami, roślinami, kategoriami oraz roślinami użytkownika.
Projekt zawiera również system przypomnień, zadania „na dziś”, autoryzację JWT, role użytkowników oraz pełne seedowanie danych.

Aplikacja składa się z:

Backendu ASP.NET Core 8 (API + MVC)

Entity Framework Core + PostgreSQL

Autoryzacji JWT + ASP.NET Identity

Widoków MVC (GardensPage, PlantsPage)

Systemu powiadomień i zadań

📌 Funkcjonalności

🔐 Autoryzacja i role użytkowników
Rejestracja użytkownika (/api/Auth/register)

Logowanie i generowanie tokenu JWT (/api/Auth/login)

Automatyczne przypisanie roli User

Token JWT zawiera:

email

userId

role użytkownika

Endpointy zabezpieczone [Authorize]

🏡 Zarządzanie ogrodami
Tworzenie ogrodu

Edycja nazwy ogrodu

Wyświetlanie listy ogrodów

Usuwanie roślin z ogrodu

Widoki MVC:

/GardensPage/Index

/GardensPage/Details/{id}

🌱 Zarządzanie roślinami
Dodawanie roślin

Przypisywanie kategorii

Usuwanie roślin

Pobieranie listy roślin z kategoriami

🏷 Kategorie roślin
CRUD kategorii

Przypisywanie kategorii do roślin

👤 Rośliny użytkownika
Dodawanie rośliny do ogrodu

Przechowywanie:

notatek

zdjęć

zabiegów

zbiorów

⏰ System zadań i powiadomień
Automatyczne wyliczanie zadań „na dziś”

Powiadomienia tekstowe

Logika oparta o:

datę posadzenia

częstotliwość podlewania

Endpointy:

/api/Tasks/today

/api/Tasks/notifications

🌾 Seedowanie danych
Przy starcie aplikacji automatycznie dodawane są:

Kategorie (16):
Warzywa, Owoce, Zioła, Drzewa, Byliny, Jednoroczne, Wieloletnie, Pnącza, Cebulkowe itd.

Rośliny (5):
Black Cherry

Ogórek Śremski

California Wonder

Ligol

Bazylia Genovese

Ogrody (4):
Ogród przydomowy

Tunel foliowy

Balkon

Pergola

Dzięki temu aplikacja działa od razu po uruchomieniu.

🔥 Najważniejsze endpointy API

🔐 Autoryzacja
Metoda	Endpoint	Opis
POST	/api/Auth/register	Rejestracja
POST	/api/Auth/login	Logowanie + JWT


🏡 Ogrody
Metoda	Endpoint	Opis
GET	/api/Gardens	Lista ogrodów
POST	/api/Gardens	Tworzenie ogrodu
PUT	/api/Gardens/{id}	Edycja nazwy
POST	/api/Gardens/{id}/plants/{plantId}	Dodanie rośliny
DELETE	/api/Gardens/{id}/plants/{plantId}	Usunięcie rośliny


🌱 Rośliny
Metoda	Endpoint	Opis
GET	/api/Plants	Lista roślin
POST	/api/Plants	Dodanie rośliny
DELETE	/api/Plants/{id}	Usunięcie rośliny


🏷 Kategorie
Metoda	Endpoint	Opis
GET	/api/PlantCategories	Lista kategorii
POST	/api/PlantCategories	Dodanie kategorii


👤 Rośliny użytkownika
Metoda	Endpoint	Opis
POST	/api/UserPlants	Dodanie rośliny użytkownika


⏰ Zadania i powiadomienia
Metoda	Endpoint	Opis
GET	/api/Tasks/today	Zadania na dziś
GET	/api/Tasks/notifications	Powiadomienia


🧪 Testowanie
Aplikacja została przetestowana:

przez Swagger

przez Postman

przez przeglądarkę (MVC)

testy autoryzacji JWT

testy CRUD

testy seedowania

testy systemu tasków

🚀 Uruchomienie projektu
Skonfiguruj appsettings.json (JWT + PostgreSQL)

Uruchom migracje:

Kod
dotnet ef database update

Uruchom aplikację:

Kod
dotnet run

Wejdź na:

Swagger: https://localhost:7286/swagger

Widoki MVC: https://localhost:7286/GardensPage

📄 Licencja
Projekt edukacyjny — brak ograniczeń w użyciu.
