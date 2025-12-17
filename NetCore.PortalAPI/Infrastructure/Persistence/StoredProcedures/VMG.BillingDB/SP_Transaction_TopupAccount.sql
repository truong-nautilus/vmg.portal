CREATE OR ALTER PROCEDURE [dbo].[SP_Transaction_TopupAccount]
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
    
    IF (@_CurrencyType = 1 OR @_WalletType = 1) -- Coin
    BEGIN
        SELECT @_CurrentBalance = TotalCoin FROM Accounts WHERE AccountID = @_AccountID;
    END
    ELSE
    BEGIN
        SELECT @_CurrentBalance = TotalXu FROM Accounts WHERE AccountID = @_AccountID;
    END
    
    IF @_CurrentBalance IS NULL
    BEGIN
        SET @_ResponseStatus = -100;
        RETURN;
    END

    -- Add
    IF (@_CurrencyType = 1 OR @_WalletType = 1) -- Coin
    BEGIN
        UPDATE Accounts 
        SET TotalCoin = TotalCoin + @_Amount 
        WHERE AccountID = @_AccountID;
    END
    ELSE
    BEGIN
        UPDATE Accounts 
        SET TotalXu = TotalXu + @_Amount 
        WHERE AccountID = @_AccountID;
    END
    
    SET @_WalletOutput = @_CurrentBalance + @_Amount;
    SET @_ResponseStatus = 1;
END
