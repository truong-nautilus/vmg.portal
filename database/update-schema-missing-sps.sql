USE [Vmg.BillingDB];
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- 1. SP_Account_CreateAccounts
IF OBJECT_ID('[dbo].[SP_Account_CreateAccounts]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[SP_Account_CreateAccounts];
GO
CREATE PROCEDURE [dbo].[SP_Account_CreateAccounts]
    @_AccountName NVARCHAR(50),
    @_Password NVARCHAR(255),
    @_NickName NVARCHAR(50) = NULL,
    @_Email NVARCHAR(100) = NULL,
    @_AccountTypeID INT = 0,
    @_MerchantId INT = 0,
    @_PlatformId INT = 0,
    @_ClientIP NVARCHAR(50) = '',
    @_CampaignSource NVARCHAR(50) = NULL,
    @_DeviceID NVARCHAR(100) = NULL,
    @_AccountID BIGINT OUTPUT,
    @_TotalXu BIGINT OUTPUT,
    @_TotalCoin BIGINT OUTPUT,
    @_Avatar INT OUTPUT,
    @_ResponseStatus INT OUTPUT,
    @_LocationID INT = 0,
    @_PreFix NVARCHAR(20) OUTPUT,
    @_WAddress NVARCHAR(100) = NULL,
    @_PartnerUserID NVARCHAR(100) = NULL,
    @_CurrencyType INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @_TotalXu = 0;
    SET @_TotalCoin = 0;
    SET @_Avatar = 0;
    SET @_PreFix = '';
    SET @_CurrencyType = 1;

    IF EXISTS (SELECT 1 FROM Accounts WHERE UserName = @_AccountName)
    BEGIN
        SET @_ResponseStatus = -1; -- ACCOUNT_EXIST
        RETURN;
    END

    INSERT INTO Accounts (UserName, Password, NickName, Email, MerchantId, PlatformId, CreatedDate, Uiid)
    VALUES (@_AccountName, @_Password, @_NickName, @_Email, @_MerchantId, @_PlatformId, GETDATE(), @_DeviceID);

    SET @_AccountID = SCOPE_IDENTITY();
    SET @_ResponseStatus = 1; -- SUCCESS
END
GO

-- 2. SP_Accounts_GetInfo
IF OBJECT_ID('[dbo].[SP_Accounts_GetInfo]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[SP_Accounts_GetInfo];
GO
CREATE PROCEDURE [dbo].[SP_Accounts_GetInfo]
    @_AccountID BIGINT,
    @_UserName NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @_AccountID > 0
        SELECT * FROM Accounts WHERE AccountID = @_AccountID;
    ELSE IF @_UserName IS NOT NULL
        SELECT * FROM Accounts WHERE UserName = @_UserName;
END
GO

-- 3. SP_Account_CreateLoginSession
IF OBJECT_ID('[dbo].[SP_Account_CreateLoginSession]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[SP_Account_CreateLoginSession];
GO
CREATE PROCEDURE [dbo].[SP_Account_CreateLoginSession]
    @_AccountID BIGINT,
    @_AccessToken NVARCHAR(500),
    @_DeviceName NVARCHAR(100) = NULL,
    @_IPAddress NVARCHAR(50) = NULL,
    @_Uiid NVARCHAR(100) = NULL,
    @_ExpiredDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO LoginSessions (AccountID, AccessToken, DeviceName, IPAddress, Uiid, ExpiredDate)
    VALUES (@_AccountID, @_AccessToken, @_DeviceName, @_IPAddress, @_Uiid, @_ExpiredDate);
END
GO

-- 4. SP_BettingGame_VIPCODE_Authenticate
IF OBJECT_ID('[dbo].[SP_BettingGame_VIPCODE_Authenticate]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[SP_BettingGame_VIPCODE_Authenticate];
GO
CREATE PROCEDURE [dbo].[SP_BettingGame_VIPCODE_Authenticate]
    @_ServiceID INT,
    @_HashKey NVARCHAR(100),
    @_UserName NVARCHAR(50),
    @_UniKey NVARCHAR(100),
    @_ClientIP NVARCHAR(50),
    @_EventCoin INT OUTPUT,
    @_IsOTP INT OUTPUT,
    @_IsRedirect INT OUTPUT,
    @_IsViewPayment INT OUTPUT,
    @_IsViewHotLine INT OUTPUT,
    @_IsMD5 BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    -- Mock implementation
    SET @_EventCoin = 0;
    SET @_IsOTP = 0;
    SET @_IsRedirect = 0;
    SET @_IsViewPayment = 1;
    SET @_IsViewHotLine = 1;
    
    DECLARE @AccID BIGINT;
    SELECT @AccID = AccountID FROM Accounts WHERE UserName = @_UserName;
    
    IF @AccID IS NOT NULL
        RETURN @AccID;
    ELSE
        RETURN -1;
END
GO

-- 5. SP_Account_GetBalance_Authenticate
IF OBJECT_ID('[dbo].[SP_Account_GetBalance_Authenticate]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[SP_Account_GetBalance_Authenticate];
GO
CREATE PROCEDURE [dbo].[SP_Account_GetBalance_Authenticate]
    @_AccountID BIGINT,
    @_Balance BIGINT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT @_Balance = Balance FROM Accounts WHERE AccountID = @_AccountID;
    IF @_Balance IS NULL SET @_Balance = 0;
END
GO

-- 6. SP_Accounts_GetInfoByUserName
IF OBJECT_ID('[dbo].[SP_Accounts_GetInfoByUserName]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[SP_Accounts_GetInfoByUserName];
GO
CREATE PROCEDURE [dbo].[SP_Accounts_GetInfoByUserName]
    @_UserName NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        AccountID, UserName, NickName, Avatar = 0, MerchantId, PlatformId, Status, 
        Balance as TotalCoin, 0 as TotalXu, IsOTP, LocationId, PreFix, Email, 
        CreatedDate, LastLoginDate, VipLevel as VipId, IsAgency, CurrencyType
    FROM Accounts WHERE UserName = @_UserName;
END
GO

-- 7. SP_Accounts_GetInfoByAccountID
IF OBJECT_ID('[dbo].[SP_Accounts_GetInfoByAccountID]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[SP_Accounts_GetInfoByAccountID];
GO
CREATE PROCEDURE [dbo].[SP_Accounts_GetInfoByAccountID]
    @_AccountID BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        AccountID, UserName, NickName, Avatar = 0, MerchantId, PlatformId, Status, 
        Balance as TotalCoin, 0 as TotalXu, IsOTP, LocationId, PreFix, Email, 
        CreatedDate, LastLoginDate, VipLevel as VipId, IsAgency, CurrencyType
    FROM Accounts WHERE AccountID = @_AccountID;
END
GO

-- 8. SP_Account_CheckAccountExists
IF OBJECT_ID('[dbo].[SP_Account_CheckAccountExists]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[SP_Account_CheckAccountExists];
GO
CREATE PROCEDURE [dbo].[SP_Account_CheckAccountExists]
    @_UserName NVARCHAR(50),
    @_ResponseStatus INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM Accounts WHERE UserName = @_UserName)
        SET @_ResponseStatus = 1;
    ELSE
        SET @_ResponseStatus = 0;
END
GO

-- 9. SP_Account_AuthenticateOTP (Mock)
IF OBJECT_ID('[dbo].[SP_Account_AuthenticateOTP]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[SP_Account_AuthenticateOTP];
GO
CREATE PROCEDURE [dbo].[SP_Account_AuthenticateOTP]
    @_SysPartnerKey NVARCHAR(100),
    @_AccountID BIGINT,
    @_ServiceID INT,
    @_ClientIP NVARCHAR(50),
    @_OTP NVARCHAR(20),
    @_ResponseStatus INT OUTPUT
AS
BEGIN
    SET @_ResponseStatus = 1; -- Always success for dev
END
GO

-- 10. SP_Accounts_UpdateInfo
IF OBJECT_ID('[dbo].[SP_Accounts_UpdateInfo]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[SP_Accounts_UpdateInfo];
GO
CREATE PROCEDURE [dbo].[SP_Accounts_UpdateInfo]
    @_AccountID BIGINT,
    @_NickName NVARCHAR(50) = NULL,
    @_Email NVARCHAR(100) = NULL,
    @_Mobile NVARCHAR(20) = NULL,
    @_Passport NVARCHAR(50) = NULL,
    @_Avatar INT = NULL,
    @_Gender INT = NULL,
    @_Birthday DATETIME = NULL,
    @_Address NVARCHAR(200) = NULL,
    @_ResponseStatus INT OUTPUT
AS
BEGIN
    UPDATE Accounts 
    SET NickName = ISNULL(@_NickName, NickName),
        Email = ISNULL(@_Email, Email),
        Mobile = ISNULL(@_Mobile, Mobile)
    WHERE AccountID = @_AccountID;
    SET @_ResponseStatus = 1;
END
GO

PRINT 'Additional Stored Procedures created successfully';
GO
