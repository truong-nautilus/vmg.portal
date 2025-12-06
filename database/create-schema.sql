-- VMG Portal - Database Schema
-- This script creates the basic tables needed for authentication

USE master;
GO

-- Create database if not exists
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'Vmg.BillingDB')
BEGIN
    CREATE DATABASE [Vmg.BillingDB];
    PRINT 'Database Vmg.BillingDB created successfully';
END
ELSE
BEGIN
    PRINT 'Database Vmg.BillingDB already exists';
END
GO

USE [Vmg.BillingDB];
GO

-- Create Accounts table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Accounts]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Accounts] (
        [AccountID] BIGINT IDENTITY(1,1) PRIMARY KEY,
        [UserName] NVARCHAR(50) NOT NULL UNIQUE,
        [Password] NVARCHAR(255) NOT NULL,
        [NickName] NVARCHAR(50) NULL,
        [Email] NVARCHAR(100) NULL,
        [Mobile] NVARCHAR(20) NULL,
        [PlatformId] INT NOT NULL DEFAULT 1,
        [MerchantId] INT NOT NULL DEFAULT 1,
        [Status] INT NOT NULL DEFAULT 1, -- 1: Active, 0: Inactive, -1: Banned
        [Balance] BIGINT NOT NULL DEFAULT 0,
        [IsOTP] BIT NOT NULL DEFAULT 0,
        [IsAgency] BIT NOT NULL DEFAULT 0,
        [LocationId] INT NULL,
        [PreFix] NVARCHAR(10) NULL,
        [RefCode] NVARCHAR(50) NULL,
        [CurrencyType] INT NOT NULL DEFAULT 1,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [LastLoginDate] DATETIME NULL,
        [LastLoginIP] NVARCHAR(50) NULL,
        [DeviceName] NVARCHAR(100) NULL,
        [Uiid] NVARCHAR(100) NULL,
        [FacebookId] NVARCHAR(100) NULL,
        [AppleId] NVARCHAR(100) NULL,
        [VipLevel] INT NOT NULL DEFAULT 0,
        [VipPoint] BIGINT NOT NULL DEFAULT 0
    );
    
    CREATE INDEX IX_Accounts_UserName ON [dbo].[Accounts]([UserName]);
    CREATE INDEX IX_Accounts_Email ON [dbo].[Accounts]([Email]);
    CREATE INDEX IX_Accounts_Mobile ON [dbo].[Accounts]([Mobile]);
    
    PRINT 'Table Accounts created successfully';
END
ELSE
BEGIN
    PRINT 'Table Accounts already exists';
END
GO

-- Create LoginSessions table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LoginSessions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[LoginSessions] (
        [SessionID] BIGINT IDENTITY(1,1) PRIMARY KEY,
        [AccountID] BIGINT NOT NULL,
        [AccessToken] NVARCHAR(500) NOT NULL,
        [RefreshToken] NVARCHAR(500) NULL,
        [DeviceName] NVARCHAR(100) NULL,
        [IPAddress] NVARCHAR(50) NULL,
        [Uiid] NVARCHAR(100) NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [ExpiredDate] DATETIME NOT NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        FOREIGN KEY ([AccountID]) REFERENCES [dbo].[Accounts]([AccountID])
    );
    
    CREATE INDEX IX_LoginSessions_AccountID ON [dbo].[LoginSessions]([AccountID]);
    CREATE INDEX IX_LoginSessions_AccessToken ON [dbo].[LoginSessions]([AccessToken]);
    
    PRINT 'Table LoginSessions created successfully';
END
ELSE
BEGIN
    PRINT 'Table LoginSessions already exists';
END
GO

-- Create TransactionLogs table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TransactionLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[TransactionLogs] (
        [TransactionID] BIGINT IDENTITY(1,1) PRIMARY KEY,
        [AccountID] BIGINT NOT NULL,
        [TransactionType] INT NOT NULL, -- 1: Deposit, 2: Withdraw, 3: Transfer, 4: Game
        [Amount] BIGINT NOT NULL,
        [BalanceBefore] BIGINT NOT NULL,
        [BalanceAfter] BIGINT NOT NULL,
        [Description] NVARCHAR(500) NULL,
        [ReferenceID] NVARCHAR(100) NULL,
        [Status] INT NOT NULL DEFAULT 1, -- 1: Success, 0: Pending, -1: Failed
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        FOREIGN KEY ([AccountID]) REFERENCES [dbo].[Accounts]([AccountID])
    );
    
    CREATE INDEX IX_TransactionLogs_AccountID ON [dbo].[TransactionLogs]([AccountID]);
    CREATE INDEX IX_TransactionLogs_CreatedDate ON [dbo].[TransactionLogs]([CreatedDate]);
    
    PRINT 'Table TransactionLogs created successfully';
END
ELSE
BEGIN
    PRINT 'Table TransactionLogs already exists';
END
GO

-- Create Locations table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Locations]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Locations] (
        [LocationID] INT IDENTITY(1,1) PRIMARY KEY,
        [LocationName] NVARCHAR(100) NOT NULL,
        [CountryCode] NVARCHAR(10) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1
    );
    
    PRINT 'Table Locations created successfully';
END
ELSE
BEGIN
    PRINT 'Table Locations already exists';
END
GO

-- Create MobileCodes table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MobileCodes]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[MobileCodes] (
        [MobileCodeID] INT IDENTITY(1,1) PRIMARY KEY,
        [CountryCode] NVARCHAR(10) NOT NULL,
        [CountryName] NVARCHAR(100) NOT NULL,
        [DialCode] NVARCHAR(10) NOT NULL,
        [IsActive] BIT NOT NULL DEFAULT 1
    );
    
    PRINT 'Table MobileCodes created successfully';
END
ELSE
BEGIN
    PRINT 'Table MobileCodes already exists';
END
GO

PRINT '';
PRINT '========================================';
PRINT 'Database schema created successfully!';
PRINT '========================================';
PRINT 'Next step: Run seed-database.sql to insert sample data';
GO
