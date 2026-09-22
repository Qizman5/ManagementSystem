-- Створення бази даних, якщо вона ще не існує
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

-- 2. Створення таблиці Products (товари на складі, з урахуванням знижки та ціни)
CREATE TABLE IF NOT EXISTS Products (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(150) NOT NULL,
    Quantity INT NOT NULL DEFAULT 0,
    Price DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    Discount DECIMAL(18,2) NOT NULL DEFAULT 0.00
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 3. Створення таблиці Orders (замовлення)
CREATE TABLE IF NOT EXISTS Orders (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    WorkerID INT NOT NULL,
    ProductID INT NOT NULL,
    OrderDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Status VARCHAR(50) NOT NULL DEFAULT 'Pending',
    CONSTRAINT FK_Orders_Workers FOREIGN KEY (WorkerID) REFERENCES Workers(ID) ON DELETE CASCADE,
    CONSTRAINT FK_Orders_Products FOREIGN KEY (ProductID) REFERENCES Products(ID) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 4. Створення таблиці Actions (історія операцій: хто, що взяв, кількість та ціна на момент операції)
CREATE TABLE IF NOT EXISTS Actions (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    WorkerID INT NOT NULL,
    ProductID INT NOT NULL,
    ActionType VARCHAR(50) NOT NULL,        -- Наприклад: 'Take' (взяв) або 'Income' (прихід)
    Quantity INT NOT NULL DEFAULT 1,
    PriceAtMoment DECIMAL(18,2) NOT NULL,   -- Ціна товару на момент операції
    Note VARCHAR(255) NULL,
    ActionDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT FK_Actions_Workers FOREIGN KEY (WorkerID) REFERENCES Workers(ID) ON DELETE CASCADE,
    CONSTRAINT FK_Actions_Products FOREIGN KEY (ProductID) REFERENCES Products(ID) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 5. Створення індексів для прискорення пошуку
CREATE INDEX IX_Workers_Email ON Workers(Email);
CREATE INDEX IX_Orders_WorkerID ON Orders(WorkerID);
CREATE INDEX IX_Orders_ProductID ON Orders(ProductID);
CREATE INDEX IX_Actions_WorkerID ON Actions(WorkerID);
CREATE INDEX IX_Actions_ProductID ON Actions(ProductID);

-- 6. Вставка початкових даних (робітники та адміністратор)
INSERT INTO Workers (Name, Username, FullName, Email, PasswordHash, Role) VALUES 
('System Admin', 'admin', 'System Admin', 'arotar2005@gmail.com', 'Admin123!', 'Admin'),
('Іван Петренко', 'ivan', 'Іван Петренко', 'ivanpetrenko124@gmail.com', '0000', 'Worker'),
('Олена Коваль', 'olena', 'Олена Коваль', 'OlenaKoval@gmail.com', '0000', 'Worker');

-- 7. Вставка товарів
INSERT INTO Products (Name, Quantity, Price, Discount) VALUES 
('Ноутбук Dell XPS 15', 10, 45000.00, 0.00),
('Монітор LG 27"', 25, 8500.00, 5.00),
('Клавіатура Keychron K2', 50, 3200.00, 0.00);

-- 8. Вставка тестових дій (наприклад, Іван узяв ноутбук)
INSERT INTO Actions (WorkerID, ProductID, ActionType, Quantity, PriceAtMoment, Note) VALUES 
(2, 1, 'Take', 1, 45000.00, 'Видача працівнику');

-- 9. ЗАПИТ ДЛЯ ПЕРЕВІРКИ: показує логін, пошту, пароль користувача, назву товару, кількість та виставлену ціну
SELECT 
    w.Username AS `Логін`,
    w.Email AS `Пошта`,
    w.PasswordHash AS `Пароль`,
    p.Name AS `НазваТовару`,
    a.Quantity AS `Кількість`,
    a.PriceAtMoment AS `ВиставленаЦіна`,
    a.ActionType AS `Дія`,
    a.ActionDate AS `Дата/Час`
FROM Actions a
JOIN Workers w ON a.WorkerID = w.ID
JOIN Products p ON a.ProductID = p.ID;
