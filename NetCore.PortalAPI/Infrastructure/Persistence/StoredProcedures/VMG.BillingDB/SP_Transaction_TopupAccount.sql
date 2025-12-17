
CREATE PROCEDURE [dbo].[SP_Transaction_TopupAccount]
    @_ServiceID INT,
    @_ServiceKey NVARCHAR(50),
    @_CurrencyType INT,
    @_WalletType INT,
    @_AccountID BIGINT,
    @_Username NVARCHAR(50),
    @_RelatedAccountID BIGINT,
    @_RelatedUsername NVARCHAR(50),
    @_Amount BIGINT,
    @_Description NVARCHAR(250),
    @_ReferenceID BIGINT,
    @_MerchantID INT,
    @_PriceId INT,
    @_AgencyID INT,
    @_Tax INT,
    @_RoomID INT,
    @_SourceID INT,
    @_WalletOutput BIGINT OUTPUT,
    @_ClientIP NVARCHAR(50),
    @_ResponseStatus INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @_ResponseStatus = -1;
    
    -- Check Account
    DECLARE @_CurrentBalance BIGINT;
    SELECT @_CurrentBalance = Balance FROM Accounts WHERE AccountID = @_AccountID;
    
    IF @_CurrentBalance IS NULL
    BEGIN
        SET @_ResponseStatus = -100;
        RETURN;
    END

    -- Add
    UPDATE Accounts 
    SET Balance = Balance + @_Amount 
    WHERE AccountID = @_AccountID;
    
    SET @_WalletOutput = @_CurrentBalance + @_Amount;
    SET @_ResponseStatus = 1;
END
