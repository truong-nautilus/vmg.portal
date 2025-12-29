CREATE OR ALTER PROCEDURE [dbo].[SP_BettingGame_Authenticate]
    @_ServiceID INT = 0,
    @_Username NVARCHAR(30), 
    @_Password VARCHAR(120), -- Increased from 50 to 120
    @_Salt VARCHAR(50),      -- Not used in new logic but kept for signature compatibility
    @_ClientIP VARCHAR(20) = '',
    @_UIID NVARCHAR(150) = '',
    @_IsLoginedOTP INT = 0 OUTPUT,
    @_IsMobileConfirmed INT = 0 OUTPUT,
    @_ResponseStatus INT OUTPUT 
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @AccID BIGINT;
    DECLARE @StoredPassword VARCHAR(120);
    DECLARE @Status INT;
    DECLARE @IsOtpDB INT;
    DECLARE @IsMobileConfirmedDB INT;

    SET @_IsLoginedOTP = 0;
    SET @_IsMobileConfirmed = 0;

    SELECT 
        @AccID = AccountID, 
        @StoredPassword = Password, 
        @Status = Status,
        @IsOtpDB = IsOtp,
        @IsMobileConfirmedDB = IsMobileConfirmed
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

    -- Set Output Parameters
    SET @_IsLoginedOTP = @IsOtpDB;
    SET @_IsMobileConfirmed = @IsMobileConfirmedDB;
    SET @_ResponseStatus = 0; -- Success

    -- Return Account Info (Schema matching AccountDb used in older code)
    -- Older code mapped to AccountDb, so we select columns to match
     SELECT AccountID, AccountName as UserName, TotalXu, TotalCoin, Avatar, IsOtp, ClientIP, MerchantID, PlatformID, NickName as UserFullname, Email as EmailFull, Level, LocationID, PreFix
    FROM Accounts WHERE AccountID = @AccID;
END
