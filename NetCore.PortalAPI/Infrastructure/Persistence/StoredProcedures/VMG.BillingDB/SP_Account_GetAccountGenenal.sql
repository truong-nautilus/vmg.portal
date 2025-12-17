
                    CREATE   PROCEDURE [dbo].[SP_Account_GetAccountGenenal]
                        @_AccountID bigint
                    AS
                    BEGIN
                        SELECT 
                            AccountID,
                            AccountName as UserName,
                            NickName as UserFullname,
                            Email as UserEmail,
                            LocationID,
                            Mobile,
                            CAST(1 as bit) as UseMK,
                            Avatar
                        FROM Accounts
                        WHERE AccountID = @_AccountID
                    END