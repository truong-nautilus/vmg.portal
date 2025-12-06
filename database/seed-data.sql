-- VMG Portal - Seed Data
-- This script inserts sample data including admin account

USE [Vmg.BillingDB];
GO

PRINT 'Inserting seed data...';
PRINT '';

-- Insert Locations
IF NOT EXISTS (SELECT * FROM [dbo].[Locations] WHERE [LocationID] = 1)
BEGIN
    SET IDENTITY_INSERT [dbo].[Locations] ON;
    
    INSERT INTO [dbo].[Locations] ([LocationID], [LocationName], [CountryCode], [IsActive])
    VALUES 
        (1, N'Việt Nam', 'VN', 1),
        (2, N'Thailand', 'TH', 1),
        (3, N'Laos', 'LA', 1),
        (4, N'Cambodia', 'KH', 1),
        (5, N'Myanmar', 'MM', 1);
    
    SET IDENTITY_INSERT [dbo].[Locations] OFF;
    PRINT '✅ Locations inserted';
END
ELSE
BEGIN
    PRINT '⚠️  Locations already exist';
END
GO

-- Insert Mobile Codes
IF NOT EXISTS (SELECT * FROM [dbo].[MobileCodes] WHERE [MobileCodeID] = 1)
BEGIN
    SET IDENTITY_INSERT [dbo].[MobileCodes] ON;
    
    INSERT INTO [dbo].[MobileCodes] ([MobileCodeID], [CountryCode], [CountryName], [DialCode], [IsActive])
    VALUES 
        (1, 'VN', N'Vietnam', '+84', 1),
        (2, 'TH', N'Thailand', '+66', 1),
        (3, 'LA', N'Laos', '+856', 1),
        (4, 'KH', N'Cambodia', '+855', 1),
        (5, 'MM', N'Myanmar', '+95', 1),
        (6, 'US', N'United States', '+1', 1),
        (7, 'CN', N'China', '+86', 1),
        (8, 'JP', N'Japan', '+81', 1),
        (9, 'KR', N'South Korea', '+82', 1),
        (10, 'SG', N'Singapore', '+65', 1);
    
    SET IDENTITY_INSERT [dbo].[MobileCodes] OFF;
    PRINT '✅ Mobile codes inserted';
END
ELSE
BEGIN
    PRINT '⚠️  Mobile codes already exist';
END
GO

-- Insert Admin Account
-- Password: admin (hashed with MD5 for demo - in production use proper hashing)
-- MD5 hash of "admin" = 21232f297a57a5a743894a0e4a801fc3

IF NOT EXISTS (SELECT * FROM [dbo].[Accounts] WHERE [UserName] = 'admin')
BEGIN
    INSERT INTO [dbo].[Accounts] (
        [UserName],
        [Password],
        [NickName],
        [Email],
        [Mobile],
        [PlatformId],
        [MerchantId],
        [Status],
        [Balance],
        [IsOTP],
        [IsAgency],
        [LocationId],
        [CurrencyType],
        [CreatedDate],
        [VipLevel],
        [VipPoint]
    )
    VALUES (
        'admin',
        '21232f297a57a5a743894a0e4a801fc3', -- MD5 hash of "admin"
        N'Administrator',
        'admin@vmg.vn',
        '+84901234567',
        1, -- Android platform
        1, -- Merchant ID
        1, -- Active
        10000000, -- 10M balance for testing
        0, -- No OTP
        1, -- Is Agency
        1, -- Vietnam
        1, -- Currency type
        GETDATE(),
        10, -- VIP Level 10
        1000000 -- 1M VIP points
    );
    
    PRINT '✅ Admin account created';
    PRINT '   Username: admin';
    PRINT '   Password: admin';
    PRINT '   Balance: 10,000,000';
    PRINT '   VIP Level: 10';
END
ELSE
BEGIN
    PRINT '⚠️  Admin account already exists';
END
GO

-- Insert Test User Accounts
IF NOT EXISTS (SELECT * FROM [dbo].[Accounts] WHERE [UserName] = 'testuser')
BEGIN
    INSERT INTO [dbo].[Accounts] (
        [UserName],
        [Password],
        [NickName],
        [Email],
        [PlatformId],
        [MerchantId],
        [Status],
        [Balance],
        [LocationId],
        [VipLevel]
    )
    VALUES 
        ('testuser', '21232f297a57a5a743894a0e4a801fc3', N'Test User', 'test@vmg.vn', 1, 1, 1, 1000000, 1, 1),
        ('player001', '21232f297a57a5a743894a0e4a801fc3', N'Player 001', 'player001@vmg.vn', 1, 1, 1, 500000, 1, 2),
        ('player002', '21232f297a57a5a743894a0e4a801fc3', N'Player 002', 'player002@vmg.vn', 2, 1, 1, 750000, 1, 3);
    
    PRINT '✅ Test user accounts created';
    PRINT '   - testuser (password: admin)';
    PRINT '   - player001 (password: admin)';
    PRINT '   - player002 (password: admin)';
END
ELSE
BEGIN
    PRINT '⚠️  Test user accounts already exist';
END
GO

-- Insert sample transaction logs for admin
DECLARE @AdminAccountID BIGINT;
SELECT @AdminAccountID = [AccountID] FROM [dbo].[Accounts] WHERE [UserName] = 'admin';

IF @AdminAccountID IS NOT NULL AND NOT EXISTS (SELECT * FROM [dbo].[TransactionLogs] WHERE [AccountID] = @AdminAccountID)
BEGIN
    INSERT INTO [dbo].[TransactionLogs] (
        [AccountID],
        [TransactionType],
        [Amount],
        [BalanceBefore],
        [BalanceAfter],
        [Description],
        [Status],
        [CreatedDate]
    )
    VALUES 
        (@AdminAccountID, 1, 5000000, 5000000, 10000000, N'Initial deposit', 1, DATEADD(day, -7, GETDATE())),
        (@AdminAccountID, 3, 100000, 10000000, 9900000, N'Transfer to player001', 1, DATEADD(day, -5, GETDATE())),
        (@AdminAccountID, 1, 100000, 9900000, 10000000, N'Refund', 1, DATEADD(day, -3, GETDATE()));
    
    PRINT '✅ Sample transaction logs created';
END
GO

PRINT '';
PRINT '========================================';
PRINT 'Seed data inserted successfully!';
PRINT '========================================';
PRINT '';
PRINT 'Test Accounts:';
PRINT '  Username: admin     | Password: admin | Balance: 10,000,000';
PRINT '  Username: testuser  | Password: admin | Balance: 1,000,000';
PRINT '  Username: player001 | Password: admin | Balance: 500,000';
PRINT '  Username: player002 | Password: admin | Balance: 750,000';
PRINT '';
PRINT 'You can now test the API with these accounts!';
PRINT '';
GO
