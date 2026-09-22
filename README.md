# Warehouse Management System (Система управління складом)

Комплексний додаток для обліку товарів на складі, управління користувачами та фіксації операцій (прихід/списання), побудований на базі **ASP.NET Core**, **.NET MAUI** та **MySQL**.

---

## 📌 Предметна область
Система призначена для автоматизації складських процесів підприємства:
- **Облік товарів (`Items` / `Products`):** контроль наявності, кількості, цін та знижок на складі.
- **Управління користувачами (`Users` / `Workers`):** розмежування прав доступу (Manager, Worker, Admin).
- **Фіксація операцій (`Actions` / `UserActions`):** реєстрація надходжень та списань з автоматичною перевіркою залишків та збереженням історії.
- **Інтерфейс:** кросплатформний мобільний/десктопний клієнт на .NET MAUI та вебпанель на ASP.NET Core Razor Pages / REST API.

---

## 📂 Структура проєкту
Репозиторій організовано у вигляді єдиного рішення (`WarehouseManagementSystem.sln`) зі такою структурою:
- **`Client/ClientApp`** — Кліентський кросплатформний застосунок на .NET MAUI:
  - `Models` — моделі даних клієнта.
  - `Services` — сервіси взаємодії з API (`ApiService`).
  - `ViewModels` — бізнес-логіка та стан екранів (включно з `ItemsViewModel` з кешуванням даних).
  - `Views` — інтерфейсні сторінки застосунку.
  - `Platforms` — платформно-специфічний код (Android, Windows тощо).
- **`Server/ServerApp`** — Серверна частина на ASP.NET Core 8.0 Web API:
  - `Controllers` — контролери REST API та вебпанелі.
  - `Models` — моделі сутностей бази даних та контекст `AppDbContext`.
  - `Migrations` — міграції Entity Framework Core.
  - `Views` — Razor Pages для вебпанелі адміністрування.
- **`Database`** — файли та компоненти для конфігурації бази даних.
- **`Docs`** — папка документації проєкту, що містить фінальний SQL-скрипт (`WarehouseDB.sql`).
- **`.gitignore`** — налаштування виключень для тимчасових файлів збірки (`bin`, `obj`) та конфіденційних даних (`appsettings.json`).

---

## ⚙️ Інструкції для запуску

### Передумови
- .NET 8 SDK
- MySQL Server (налаштований на порт `3307`)
- Visual Studio 2022 або Visual Studio Code

### Кроки для запуску
1. **Клонування репозиторію:**
   ```bash
   git clone [https://github.com/Qizman5/WarehouseManagementSystem.git](https://github.com/Qizman5/WarehouseManagementSystem.git)
   cd WarehouseManagementSystem
   