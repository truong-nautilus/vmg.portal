
-- Stored Procedure: SP_Account_CreateAccounts
CREATE   PROCEDURE [dbo].[SP_Account_CreateAccounts]
    @_AccountName VARCHAR(30),
    @_Password VARCHAR(120),
    @_NickName VARCHAR(30),
    @_Email VARCHAR(150),
    @_AccountTypeID INT = 0,
    @_MerchantId INT = 0,
    @_PlatformId INT = 0,
    @_ClientIP VARCHAR(30) = '',
    @_CampaignSource VARCHAR(100) = NULL,
    @_DeviceID VARCHAR(150) = NULL,
    @_AccountID BIGINT OUTPUT,
    @_TotalXu BIGINT OUTPUT,
    @_TotalCoin BIGINT OUTPUT,
    @_Avatar INT OUTPUT,
    @_ResponseStatus INT OUTPUT,
    @_LocationID INT = 0,
    @_PreFix NVARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if account exists
    IF EXISTS (SELECT 1 FROM Accounts WHERE AccountName = @_AccountName)
    BEGIN
        SET @_ResponseStatus = -1; -- Account Exists
        RETURN;
    END

    -- Check if nickname exists (if provided)
    IF @_NickName IS NOT NULL AND EXISTS (SELECT 1 FROM Accounts WHERE NickName = @_NickName)
    BEGIN
        SET @_ResponseStatus = -2; -- Nickname Exists
        RETURN;
    END

    -- Insert new account
    INSERT INTO Accounts (
        AccountName, Password, NickName, Email, AccountTypeID, MerchantID, PlatformID, 
        ClientIP, CampaignSource, DeviceID, LocationID, PreFix, UIID
    )
    VALUES (
        @_AccountName, @_Password, @_NickName, @_Email, @_AccountTypeID, @_MerchantId, @_PlatformId, 
        @_ClientIP, @_CampaignSource, @_DeviceID, @_LocationID, 'VN', @_DeviceID
    );

    SET @_AccountID = SCOPE_IDENTITY();
    SET @_AccountID = SCOPE_IDENTITY();
    SELECT @_TotalXu = TotalXu, @_TotalCoin = TotalCoin, @_Avatar = Avatar, @_PreFix = PreFix
    FROM Accounts WHERE AccountID = @_AccountID;

    SET @_ResponseStatus = 1; -- Success
END
