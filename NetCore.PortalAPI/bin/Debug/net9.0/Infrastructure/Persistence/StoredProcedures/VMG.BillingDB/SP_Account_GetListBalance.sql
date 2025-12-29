CREATE OR ALTER PROCEDURE [dbo].[SP_Account_GetListBalance]
    @_AccountID BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Return balance for each wallet/currency type
    -- Columns must match ListBalanceAccount model: Name, Code, WalletId, Balance, DecimalDigits
    SELECT
        N'VND' AS Name,
        N'VND' AS Code,
        1 AS WalletId,
        ISNULL(TotalCoin, 0) AS Balance,
        0 AS DecimalDigits
    FROM Accounts 
    WHERE AccountID = @_AccountID
    
    UNION ALL
    
    SELECT
        N'XU' AS Name,
        N'XU' AS Code,
        2 AS WalletId,
        ISNULL(TotalXu, 0) AS Balance,
        0 AS DecimalDigits
    FROM Accounts 
    WHERE AccountID = @_AccountID
END