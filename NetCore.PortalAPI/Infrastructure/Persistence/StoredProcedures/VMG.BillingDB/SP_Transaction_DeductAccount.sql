CREATE OR ALTER PROCEDURE [dbo].[SP_Transaction_DeductAccount]
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
    -- Check Account
    DECLARE @_CurrentBalance BIGINT;
    
    IF (@_CurrencyType = 1 OR @_WalletType = 1) -- Coin
    BEGIN
        SELECT @_CurrentBalance = TotalCoin FROM Accounts WHERE AccountID = @_AccountID;
    END
    ELSE -- Xu
    BEGIN
        SELECT @_CurrentBalance = TotalXu FROM Accounts WHERE AccountID = @_AccountID;
    END
    
    IF @_CurrentBalance IS NULL OR @_CurrentBalance < @_Amount
    BEGIN
        SET @_ResponseStatus = -200; -- Insufficient funds
        RETURN;
    END

    -- Deduct
    IF (@_CurrencyType = 1 OR @_WalletType = 1) -- Coin
    BEGIN
        UPDATE Accounts 
        SET TotalCoin = TotalCoin - @_Amount 
        WHERE AccountID = @_AccountID;
    END
    ELSE
    BEGIN
        UPDATE Accounts 
        SET TotalXu = TotalXu - @_Amount 
        WHERE AccountID = @_AccountID;
    END
    
    SET @_WalletOutput = @_CurrentBalance - @_Amount;
    SET @_ResponseStatus = 1; 
END
