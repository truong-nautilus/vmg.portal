CREATE   PROCEDURE [dbo].[SP_BettingGame_Authenticate_2]
    @_Username VARCHAR(30), 
    @_Password VARCHAR(120),
    @_ClientIP VARCHAR(20) = '',
    @_UIID VARCHAR(150) = '',
    @_PlatformId INT = 0,
    @_MerchantId INT = 0,
    @_ResponseStatus INT OUTPUT 
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @AccID BIGINT;
    DECLARE @StoredPassword VARCHAR(120);
    DECLARE @Status INT;

    SELECT @AccID = AccountID, @StoredPassword = Password, @Status = Status
    FROM Accounts WHERE AccountName = @_Username;

    IF @AccID IS NULL
    BEGIN
        SET @_ResponseStatus = -50; -- Account not exist
        RETURN;
    END

    IF @StoredPassword <> @_Password
    BEGIN
        SET @_ResponseStatus = -53; -- Password invalid
        RETURN;
    END

    IF @Status <> 1
    BEGIN
        SET @_ResponseStatus = -54; -- Account blocked
        RETURN;
    END

    -- Return success and select account info
    SET @_ResponseStatus = 0; -- Success code
    
    -- Map columns to AccountDb properties (AccountName -> UserName)
    SELECT AccountID, AccountName as UserName, TotalXu, TotalCoin, Avatar, IsOtp, ClientIP, MerchantID, PlatformID, NickName as UserFullname, Email as EmailFull, Level, LocationID, PreFix
    FROM Accounts WHERE AccountID = @AccID;
END
