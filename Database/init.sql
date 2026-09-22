CREATE DATABASE IF NOT EXISTS WarehouseDB;
USE WarehouseDB;

-- 1. Створення таблиці Workers (користувачі системи)
CREATE TABLE IF NOT EXISTS Workers (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Username VARCHAR(100) NULL,
    FullName VARCHAR(255) NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    Role VARCHAR(50) NOT NULL DEFAULT 'Worker'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 2. Створення таблиці Products (товари на складі)
CREATE TABLE IF NOT EXISTS Products (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(150) NOT NULL,
    Quantity INT NOT NULL DEFAULT 0,
    Price DECIMAL(18,2) NOT NULL DEFAULT 0.00
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 3. Створення таблиці Orders (дії/замовлення користувачів)
CREATE TABLE IF NOT EXISTS Orders (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    WorkerID INT NOT NULL,
    ProductID INT NOT NULL,
    OrderDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Status VARCHAR(50) NOT NULL DEFAULT 'Pending',
    CONSTRAINT FK_Orders_Workers FOREIGN KEY (WorkerID) REFERENCES Workers(ID) ON DELETE CASCADE,
    CONSTRAINT FK_Orders_Products FOREIGN KEY (ProductID) REFERENCES Products(ID) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 4. Створення індексів (якщо ще не створені)
CREATE INDEX IX_Workers_Email ON Workers(Email);
CREATE INDEX IX_Orders_WorkerID ON Orders(WorkerID);
CREATE INDEX IX_Orders_ProductID ON Orders(ProductID);

-- 5. Очищення та вставка нових оновлених даних
SET FOREIGN_KEY_CHECKS = 0;
TRUNCATE TABLE Orders;
TRUNCATE TABLE Products;
TRUNCATE TABLE Workers;
SET FOREIGN_KEY_CHECKS = 1;

-- Вставка користувачів (з новими поштами, логинами та паролем '0000')
INSERT INTO Workers (Name, Username, FullName, Email, PasswordHash, Role) VALUES 
('Іван Петренко', 'Іван Петренко', 'Іван Петренко', 'ivanpetrenko124@gmail.com', '0000', 'Admin'),
('Олена Коваль', 'Олена Коваль', 'Олена Коваль', 'OlenaKoval@gmail.com', '0000', 'Worker');

-- Вставка товарів
INSERT INTO Products (Name, Quantity, Price) VALUES 
('Ноутбук Dell XPS 15', 10, 45000.00),
('Монітор LG 27"', 25, 8500.00),
('Клавіатура Keychron K2', 50, 3200.00);

-- Вставка замовлень
INSERT INTO Orders (WorkerID, ProductID, OrderDate, Status) VALUES 
(1, 1, NOW(), 'Completed'),
(2, 3, NOW(), 'Pending');

-- 6. Перевірка результату в базі даних
SELECT ID, Name, Username, FullName, Email, Role, PasswordHash FROM Workers;

INSERT INTO Workers (Name, Username, FullName, Email, PasswordHash, Role) 
VALUES ('admin', 'admin', 'System Admin', 'arotar2005@gmail.com', 'Admin123!', 'Admin');

ALTER TABLE Actions ADD COLUMN ActionDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP;
USE WarehouseMS;


CREATE TABLE Actions (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    UserID INT NOT NULL,
    ItemID INT NOT NULL,
    ActionType VARCHAR(50) NOT NULL,
    Quantity INT NOT NULL DEFAULT 1,
    Note VARCHAR(255) NULL,
    ActionDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT FK_Actions_Users FOREIGN KEY (UserID) REFERENCES Users(ID) ON DELETE CASCADE,
    CONSTRAINT FK_Actions_Items FOREIGN KEY (ItemID) REFERENCES Items(ID) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 3. Додаємо тестовий запис для перевірки
INSERT INTO Actions (UserID, ItemID, ActionType, Quantity, Note) 
VALUES (1, 1, 'Income', 10, 'Початковий залишок');

-- 1. Тимчасово вимикаємо безпечний режим оновлення
SET SQL_SAFE_UPDATES = 0;

-- 2. Встановлюємо точне значення пароля для користувача admin
UPDATE users 
SET PasswordHash = 'Admin123!' 
WHERE Username = 'admin' OR Id = 3;

-- 3. Умикаємо режим назад
SET SQL_SAFE_UPDATES = 1;

-- 4. Перевіряємо, що збереглося в базі
SELECT Id, Username, Email, PasswordHash FROM users WHERE Id = 3;

SET SQL_SAFE_UPDATES = 0;

UPDATE users 
SET PasswordHash = 'Admin123!' 
WHERE Username = 'admin';

SET SQL_SAFE_UPDATES = 1;

SELECT * FROM Workers;
SELECT * FROM Products;
SELECT * FROM Actions;

