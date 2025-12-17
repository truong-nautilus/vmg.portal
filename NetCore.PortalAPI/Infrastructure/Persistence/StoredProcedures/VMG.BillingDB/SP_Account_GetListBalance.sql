
                    CREATE   PROCEDURE [dbo].[SP_Account_GetListBalance]
                        @_AccountID bigint
                    AS
                    BEGIN
                        -- Return balance for each currency type
                        SELECT 
                            CASE 
                                WHEN 1 = 1 THEN N'VND'
                            END as Name,
                            CASE 
                                WHEN 1 = 1 THEN 'VND'
                            END as Code,
                            1 as WalletId,
                            ISNULL(TotalCoin, 0) as Balance,
                            0 as DecimalDigits
                        FROM Accounts
                        WHERE AccountID = @_AccountID
                        
                        UNION ALL
                        
                        SELECT 
                            N'XU' as Name,
                            'XU' as Code,
                            2 as WalletId,
                            ISNULL(TotalXu, 0) as Balance,
                            0 as DecimalDigits
                        FROM Accounts
                        WHERE AccountID = @_AccountID
                    END