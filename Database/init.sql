-- Створення таблиці Users
CREATE DATABASE IF NOT EXISTS WarehouseMS;
USE WarehouseMS;
-- 2. Створення таблиці Users
CREATE TABLE IF NOT EXISTS Users (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    PasswordHash VARCHAR(256) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 3. Створення таблиці Items
CREATE TABLE IF NOT EXISTS Items (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(150) NOT NULL,
    Quantity INT NOT NULL DEFAULT 0,
    Price DECIMAL(18, 2) NOT NULL DEFAULT 0.00,
    UserID INT NOT NULL,
    CONSTRAINT FK_Items_Users FOREIGN KEY (UserID) REFERENCES Users(ID) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 4. Створення таблиці Actions
CREATE TABLE IF NOT EXISTS Actions (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    UserID INT NOT NULL,
    ItemID INT NOT NULL,
    ActionDetails VARCHAR(255) NOT NULL,
    Status VARCHAR(50) NOT NULL,
    CONSTRAINT FK_Actions_Users FOREIGN KEY (UserID) REFERENCES Users(ID),
    CONSTRAINT FK_Actions_Items FOREIGN KEY (ItemID) REFERENCES Items(ID) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 5. Тестові дані: Users
INSERT INTO Users (Name, Email, PasswordHash) VALUES
('Олексій Іванов', 'o.ivanov@chnu.edu.ua', 'hash_pass_123'),
('Марія Петренко', 'm.petrenko@chnu.edu.ua', 'hash_pass_456');

-- 6. Тестові дані: Items
INSERT INTO Items (Name, Quantity, Price, UserID) VALUES
('Сканер штрих-кодів Zebra', 15, 4500.00, 1),
('Принтер етикеток Xprinter', 8, 3200.50, 1),
('Складський стелаж Heavy 200', 4, 8900.00, 2);

-- 7. Тестові дані: Actions
INSERT INTO Actions (UserID, ItemID, ActionDetails, Status) VALUES
(1, 1, 'Прибуття нової партії на склад', 'Completed'),
(2, 3, 'Інвентаризація та перевірка цілісності', 'In Progress'),
(1, 2, 'Списання пошкодженого пристрою', 'Pending');