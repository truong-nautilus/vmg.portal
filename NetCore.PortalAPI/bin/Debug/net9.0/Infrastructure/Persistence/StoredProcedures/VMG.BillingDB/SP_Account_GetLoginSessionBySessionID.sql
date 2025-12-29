CREATE OR ALTER PROCEDURE [dbo].[SP_Account_GetLoginSessionBySessionID]
                        @_SessionID varchar(100),
                        @_ResponseStatus int OUTPUT
                    AS
                    BEGIN
                        IF EXISTS (SELECT 1 FROM Account_LoginSession WHERE SessionID = @_SessionID AND IsActive = 1)
                        BEGIN
                            SET @_ResponseStatus = 1 -- Found and active
                        END
                        ELSE
                        BEGIN
                            SET @_ResponseStatus = -1 -- Not found or inactive
                        END
                    END