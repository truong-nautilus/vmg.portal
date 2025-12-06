USE [Vmg.BillingDB];
GO

-- Create SP_BettingGame_Authenticate_2
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SP_BettingGame_Authenticate_2]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[SP_BettingGame_Authenticate_2];
GO

CREATE PROCEDURE [dbo].[SP_BettingGame_Authenticate_2]
    @_Username          VARCHAR(30), 
    @_Password          VARCHAR(50),
    @_ClientIP          VARCHAR(20) = '',
    @_UIID              VARCHAR(150) = '',
    @_PlatformId        INT = 0, -- 1: android, 2: ios, 3: windows-pc, 4: web
    @_MerchantId        INT = 0,
    @_ResponseStatus    INT OUTPUT 
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @AccountID BIGINT;
    DECLARE @StoredPassword NVARCHAR(255);
    DECLARE @Status INT;
    DECLARE @PlatformID_DB INT;

    -- Check if account exists
    SELECT TOP 1 
        @AccountID = AccountID,
        @StoredPassword = Password,
        @Status = Status,
        @PlatformID_DB = PlatformId
    FROM [dbo].[Accounts]
    WHERE UserName = @_Username;

    IF @AccountID IS NULL
    BEGIN
        SET @_ResponseStatus = -51; -- ACCOUNT_NOT_EXIST (mapped to 1013 in code logic usually, but using negative for SP convention)
        RETURN;
    END

    -- Check status
    IF @Status = -1
    BEGIN
        SET @_ResponseStatus = -54; -- ACCOUNT_BLOCKED
        RETURN;
    END

    -- Check password (compare hash)
    IF @StoredPassword <> LOWER(CONVERT(VARCHAR(32), HashBytes('MD5', @_Password), 2))
    BEGIN
        SET @_ResponseStatus = -53; -- PASSWORD_INVALID
        RETURN;
    END

    -- Login success, return Account info
    -- IsLoginedOTP, IsMobileConfirmed, ResponseStatus
    
    -- Update LastLogin
    UPDATE [dbo].[Accounts]
    SET LastLoginDate = GETDATE(),
        LastLoginIP = @_ClientIP,
        Uiid = @_UIID
    WHERE AccountID = @AccountID;
    
    -- Select Account Data for DAO mapping
    SELECT 
        AccountID,
        UserName,
        NickName,
        Avatar = 0, -- Default
        MerchantId,
        PlatformId,
        Status,
        CAST(Balance AS DECIMAL(20, 0)) as TotalCoin, -- Mapping Balance to TotalCoin (Decimal)
        CAST(0 AS DECIMAL(20, 0)) as TotalXu,       -- (Decimal)
        IsOTP,
        LocationId,
        PreFix,
        Email,
        CreatedDate,
        LastLoginDate,
        VipLevel as VipId, -- Mapping
        CAST(IsAgency AS INT) as IsAgency, -- Cast BIT to INT
        CurrencyType
    FROM [dbo].[Accounts]
    WHERE AccountID = @AccountID;

    SET @_ResponseStatus = 0; -- SUCCESS
END
GO

PRINT 'Procedure SP_BettingGame_Authenticate_2 created successfully';
GO
