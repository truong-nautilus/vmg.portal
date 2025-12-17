CREATE OR ALTER PROCEDURE [dbo].[SP_Account_GetListBalance]
                        @_AccountID bigint
                    AS
                    BEGIN
                        -- Return balance for each currency type
                        SELECT
                            1 AS CurrencyID,
                            TotalCoin AS Balance,
                            N'VND' AS CurrencyName 
                        FROM Accounts 
                        WHERE AccountID = @_AccountID
                        
                        UNION ALL
                        
                        SELECT
                            2 AS CurrencyID,
                            TotalXu AS Balance,
                            N'XU' AS CurrencyName 
                        FROM Accounts 
                        WHERE AccountID = @_AccountID
                    END