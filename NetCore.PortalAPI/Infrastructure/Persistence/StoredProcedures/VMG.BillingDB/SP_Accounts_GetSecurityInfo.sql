CREATE   PROCEDURE [dbo].[SP_Accounts_GetSecurityInfo]
    @_AccountID BIGINT,
    @_Status INT OUTPUT,         -- Mobile Actived Status (1: Active, 0: Inactive)
    @_Mobile VARCHAR(20) OUTPUT,
    @_IsLoginOTP INT OUTPUT,      -- Is OTP enabled for login
    @_ResponseStatus INT OUTPUT  -- >= 0 Success
    -- Removed @_TeleChatID to be compatible with old backend code
AS
BEGIN
    SET NOCOUNT ON;

    -- Default values
    SET @_ResponseStatus = -1;
    SET @_Status = 0;
    SET @_Mobile = '';
    SET @_IsLoginOTP = 0;

    -- Check if account exists
    IF NOT EXISTS (SELECT 1 FROM Accounts WHERE AccountID = @_AccountID)
    BEGIN
        SET @_ResponseStatus = -50; -- Account Not Found
        RETURN;
    END

    -- Get Security Info
    SELECT 
        @_Mobile = ISNULL(Mobile, ''),
        @_Status = ISNULL(IsMobileConfirmed, 0), -- Map IsMobileConfirmed to Status
        @_IsLoginOTP = ISNULL(IsOtp, 0)
    FROM Accounts 
    WHERE AccountID = @_AccountID;

    SET @_ResponseStatus = 1; -- Success
END
