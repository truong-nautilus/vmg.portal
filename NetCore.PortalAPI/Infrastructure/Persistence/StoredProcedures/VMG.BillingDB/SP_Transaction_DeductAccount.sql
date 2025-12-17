
CREATE PROCEDURE [dbo].[SP_Transaction_DeductAccount]
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
    @_Tax INT,
    @_SourceID INT,
    @_ClientIP NVARCHAR(50),
    @_RoomID INT,
    @_WalletOutput BIGINT OUTPUT,
    @_ResponseStatus INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @_ResponseStatus = -1;
    
    -- Check Balance
    DECLARE @_CurrentBalance BIGINT;
    SELECT @_CurrentBalance = Balance FROM Accounts WHERE AccountID = @_AccountID;
    
    IF @_CurrentBalance IS NULL
    BEGIN
        SET @_ResponseStatus = -100;
        RETURN;
    END

    IF @_CurrentBalance < @_Amount
    BEGIN
        SET @_ResponseStatus = -200;
        RETURN;
    END

    -- Deduct
    UPDATE Accounts 
    SET Balance = Balance - @_Amount 
    WHERE AccountID = @_AccountID;
    
    SET @_WalletOutput = @_CurrentBalance - @_Amount;
    SET @_ResponseStatus = 1; 
END
