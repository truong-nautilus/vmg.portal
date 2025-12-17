CREATE OR ALTER PROCEDURE [dbo].[SP_Account_RemoveLoginSession]
                        @_ID bigint,
                        @_AccountID bigint,
                        @_ResponseStatus int OUTPUT
                    AS
                    BEGIN
                        UPDATE Account_LoginSession SET IsActive = 0 WHERE ID = @_ID AND AccountID = @_AccountID
                        SET @_ResponseStatus = 1
                    END