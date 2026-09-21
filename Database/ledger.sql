-- ============================================
-- دفتر الأستاذ العام - General Ledger
-- SQL Server - مع دعم اللغة العربية
-- ============================================

USE master;
GO

-- 1. حذف قاعدة البيانات القديمة
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'AccountingDB')
BEGIN
    ALTER DATABASE AccountingDB SET OFFLINE WITH ROLLBACK IMMEDIATE;
    ALTER DATABASE AccountingDB SET ONLINE;
    DROP DATABASE AccountingDB;
    PRINT N'تم حذف قاعدة البيانات القديمة';
END
GO

-- 2. إنشاء قاعدة البيانات الجديدة
CREATE DATABASE AccountingDB COLLATE Arabic_CI_AS;
GO

USE AccountingDB;
GO

-- ============================================
-- 3. إنشاء الجداول
-- ============================================

CREATE TABLE ChartOfAccounts (
    AccountID INT PRIMARY KEY IDENTITY(1,1),
    AccountCode NVARCHAR(20) NOT NULL,
    AccountName NVARCHAR(100) NOT NULL,
    AccountType NVARCHAR(50) NOT NULL,
    AccountCategory NVARCHAR(50),
    ParentAccountID INT NULL,
    Balance DECIMAL(18, 2) DEFAULT 0,
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE JournalEntries (
    EntryID INT PRIMARY KEY IDENTITY(1,1),
    EntryNumber NVARCHAR(20) NOT NULL,
    TransactionDate DATE NOT NULL,
    Description NVARCHAR(200),
    Reference NVARCHAR(50),
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE GeneralLedger (
    LineID INT PRIMARY KEY IDENTITY(1,1),
    EntryID INT NOT NULL,
    EntryNumber NVARCHAR(20) NOT NULL,
    TransactionDate DATE NOT NULL,
    AccountID INT NOT NULL,
    Description NVARCHAR(200),
    Debit DECIMAL(18, 2) DEFAULT 0,
    Credit DECIMAL(18, 2) DEFAULT 0,
    Reference NVARCHAR(50),
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (EntryID) REFERENCES JournalEntries(EntryID),
    FOREIGN KEY (AccountID) REFERENCES ChartOfAccounts(AccountID)
);
GO

CREATE TABLE AccountBalances (
    BalanceID INT PRIMARY KEY IDENTITY(1,1),
    AccountID INT NOT NULL,
    BalanceDate DATE NOT NULL,
    OpeningBalance DECIMAL(18, 2) DEFAULT 0,
    TotalDebit DECIMAL(18, 2) DEFAULT 0,
    TotalCredit DECIMAL(18, 2) DEFAULT 0,
    ClosingBalance DECIMAL(18, 2) DEFAULT 0,
    BalanceType NVARCHAR(10),
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (AccountID) REFERENCES ChartOfAccounts(AccountID)
);
GO

-- ============================================
-- 4. إدخال البيانات التجريبية
-- ============================================

INSERT INTO ChartOfAccounts (AccountCode, AccountName, AccountType, AccountCategory) VALUES
('1001', N'النقدية والصندوق', N'Asset', N'Cash'),
('1002', N'البنك', N'Asset', N'Cash'),
('1101', N'الذمم المدينة', N'Asset', N'AccountsReceivable'),
('1201', N'المخزون', N'Asset', N'Inventory'),
('1501', N'الأصول الثابتة', N'Asset', N'FixedAssets'),
('2001', N'الذمم الدائنة', N'Liability', N'AccountsPayable'),
('2101', N'القروض قصيرة الأجل', N'Liability', N'ShortTermLoans'),
('2201', N'القروض طويلة الأجل', N'Liability', N'LongTermLoans'),
('3001', N'رأس المال', N'Equity', N'ShareCapital'),
('3101', N'الأرباح المبقاة', N'Equity', N'RetainedEarnings'),
('4001', N'المبيعات', N'Revenue', N'Sales'),
('4002', N'إيرادات أخرى', N'Revenue', N'OtherIncome'),
('5001', N'المشتريات', N'Expense', N'Purchases'),
('5002', N'الرواتب', N'Expense', N'Salaries'),
('5003', N'الإيجار', N'Expense', N'Rent'),
('5004', N'مرافق', N'Expense', N'Utilities'),
('5005', N'مصروفات إدارية', N'Expense', N'AdminExpenses'),
('5006', N'الاستهلاك', N'Expense', N'Depreciation');
GO

INSERT INTO JournalEntries (EntryNumber, TransactionDate, Description, Reference) VALUES
('J-001', '2026-01-01', N'رأس المال الأولي', N'CAP-001');
INSERT INTO GeneralLedger (EntryID, EntryNumber, TransactionDate, AccountID, Description, Debit, Credit, Reference) VALUES
(1, 'J-001', '2026-01-01', 1, N'رأس المال الأولي', 100000, 0, N'CAP-001'),
(1, 'J-001', '2026-01-01', 9, N'رأس المال الأولي', 0, 100000, N'CAP-001');

INSERT INTO JournalEntries (EntryNumber, TransactionDate, Description, Reference) VALUES
('J-002', '2026-01-05', N'شراء معدات', N'INV-001');
INSERT INTO GeneralLedger (EntryID, EntryNumber, TransactionDate, AccountID, Description, Debit, Credit, Reference) VALUES
(2, 'J-002', '2026-01-05', 5, N'شراء معدات', 30000, 0, N'INV-001'),
(2, 'J-002', '2026-01-05', 2, N'شراء معدات', 0, 30000, N'INV-001');

INSERT INTO JournalEntries (EntryNumber, TransactionDate, Description, Reference) VALUES
('J-003', '2026-01-10', N'مبيعات نقدية', N'SAL-001');
INSERT INTO GeneralLedger (EntryID, EntryNumber, TransactionDate, AccountID, Description, Debit, Credit, Reference) VALUES
(3, 'J-003', '2026-01-10', 1, N'مبيعات نقدية', 50000, 0, N'SAL-001'),
(3, 'J-003', '2026-01-10', 11, N'مبيعات نقدية', 0, 50000, N'SAL-001');

INSERT INTO JournalEntries (EntryNumber, TransactionDate, Description, Reference) VALUES
('J-004', '2026-01-15', N'شراء مخزون', N'PUR-001');
INSERT INTO GeneralLedger (EntryID, EntryNumber, TransactionDate, AccountID, Description, Debit, Credit, Reference) VALUES
(4, 'J-004', '2026-01-15', 4, N'شراء مخزون', 20000, 0, N'PUR-001'),
(4, 'J-004', '2026-01-15', 6, N'شراء مخزون', 0, 20000, N'PUR-001');

INSERT INTO JournalEntries (EntryNumber, TransactionDate, Description, Reference) VALUES
('J-005', '2026-01-20', N'رواتب يناير', N'PAY-001');
INSERT INTO GeneralLedger (EntryID, EntryNumber, TransactionDate, AccountID, Description, Debit, Credit, Reference) VALUES
(5, 'J-005', '2026-01-20', 14, N'رواتب يناير', 15000, 0, N'PAY-001'),
(5, 'J-005', '2026-01-20', 1, N'رواتب يناير', 0, 15000, N'PAY-001');

INSERT INTO JournalEntries (EntryNumber, TransactionDate, Description, Reference) VALUES
('J-006', '2026-01-25', N'إيجار يناير', N'RENT-001');
INSERT INTO GeneralLedger (EntryID, EntryNumber, TransactionDate, AccountID, Description, Debit, Credit, Reference) VALUES
(6, 'J-006', '2026-01-25', 15, N'إيجار يناير', 5000, 0, N'RENT-001'),
(6, 'J-006', '2026-01-25', 1, N'إيجار يناير', 0, 5000, N'RENT-001');

INSERT INTO JournalEntries (EntryNumber, TransactionDate, Description, Reference) VALUES
('J-007', '2026-01-31', N'استهلاك يناير', N'DEP-001');
INSERT INTO GeneralLedger (EntryID, EntryNumber, TransactionDate, AccountID, Description, Debit, Credit, Reference) VALUES
(7, 'J-007', '2026-01-31', 18, N'استهلاك يناير', 1000, 0, N'DEP-001'),
(7, 'J-007', '2026-01-31', 5, N'استهلاك يناير', 0, 1000, N'DEP-001');

INSERT INTO JournalEntries (EntryNumber, TransactionDate, Description, Reference) VALUES
('J-008', '2026-01-15', N'الحصول على قرض', N'LOAN-001');
INSERT INTO GeneralLedger (EntryID, EntryNumber, TransactionDate, AccountID, Description, Debit, Credit, Reference) VALUES
(8, 'J-008', '2026-01-15', 2, N'الحصول على قرض', 50000, 0, N'LOAN-001'),
(8, 'J-008', '2026-01-15', 8, N'الحصول على قرض', 0, 50000, N'LOAN-001');

INSERT INTO JournalEntries (EntryNumber, TransactionDate, Description, Reference) VALUES
('J-009', '2026-01-30', N'سداد قرض', N'LOAN-002');
INSERT INTO GeneralLedger (EntryID, EntryNumber, TransactionDate, AccountID, Description, Debit, Credit, Reference) VALUES
(9, 'J-009', '2026-01-30', 8, N'سداد قرض', 10000, 0, N'LOAN-002'),
(9, 'J-009', '2026-01-30', 2, N'سداد قرض', 0, 10000, N'LOAN-002');
GO

PRINT N'تم إنشاء قاعدة بيانات AccountingDB وتعبئتها بالبيانات التجريبية بنجاح';
GO
