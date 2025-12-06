USE [Vmg.BillingDB];
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- SP_Accounts_GetSecurityInfo
IF OBJECT_ID('[dbo].[SP_Accounts_GetSecurityInfo]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[SP_Accounts_GetSecurityInfo];
GO
CREATE PROCEDURE [dbo].[SP_Accounts_GetSecurityInfo]
    @_AccountID BIGINT,
    @_Status INT OUTPUT,
    @_Mobile VARCHAR(20) OUTPUT,
    @_IsLoginOTP INT OUTPUT,
    @_ResponseStatus INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Mock implementation
    -- Get Mobile and IsOTP from Accounts table
    SELECT 
        @_Mobile = Mobile,
        @_IsLoginOTP = CASE WHEN IsOTP = 1 THEN 1 ELSE 0 END,
        @_Status = 1 -- Assume active if basic checks pass
    FROM Accounts 
    WHERE AccountID = @_AccountID;

    IF @@ROWCOUNT > 0
    BEGIN
        SET @_ResponseStatus = 1;
    END
    ELSE
    BEGIN
        SET @_Status = 0;
        SET @_Mobile = '';
        SET @_IsLoginOTP = 0;
        SET @_ResponseStatus = -1;
    END
END
GO

PRINT 'Procedure SP_Accounts_GetSecurityInfo created successfully';
GO
