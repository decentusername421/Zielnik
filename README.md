# 🌿 Zielnik – aplikacja do zarządzania ogrodem

## 📌 Opis projektu
Zielnik to aplikacja webowa umożliwiająca zarządzanie roślinami, ogrodami oraz kategoriami roślin. Użytkownik może tworzyć ogrody, dodawać rośliny oraz przypisywać je do kategorii i lokalizacji.

Projekt został wykonany w technologii **ASP.NET Core Web API** z wykorzystaniem **Entity Framework Core** oraz bazy danych **PostgreSQL**. W projekcie zastosowano również system użytkowników oparty o **ASP.NET Identity**.

---

## ⚙️ Technologie
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL (Npgsql)
- ASP.NET Identity
- Swagger / OpenAPI

---

## 🔐 Użytkownicy
Aplikacja posiada system rejestracji i logowania użytkowników:
- rejestracja konta
- logowanie
- autoryzacja endpointów

---

## 🌱 Funkcjonalności

### Rośliny
- dodawanie roślin
- usuwanie roślin
- przypisywanie kategorii
- pobieranie listy roślin

### Ogrody
- tworzenie ogrodów
- edycja ogrodów
- przypisywanie roślin do ogrodów
- usuwanie roślin z ogrodów

### Kategorie
- tworzenie kategorii roślin
- przypisywanie kategorii do roślin

---

## 🗄️ Baza danych
Projekt korzysta z PostgreSQL oraz Entity Framework Core (ORM).  
Dane inicjalne są dodawane przy starcie aplikacji (SeedData).

---

## 🚀 Uruchomienie
1. Ustaw connection string w `appsettings.json`
2. Uruchom migracje:
