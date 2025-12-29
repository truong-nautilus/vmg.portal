CREATE OR ALTER PROCEDURE [dbo].[SP_Account_CreateLoginSession]
                        @_AccountID bigint,
                        @_IPAddress varchar(50),
                        @_LoginDate datetime,
                        @_ExpireDate datetime,
                        @_Location nvarchar(255),
                        @_IsActive bit,
                        @_Browser nvarchar(255),
                        @_Device nvarchar(255),
                        @_SessionID varchar(100),
                        @_ResponseStatus int OUTPUT
                    AS
                    BEGIN
                        INSERT INTO Account_LoginSession(AccountID, IPAddress, LoginDate, ExpireDate, Location, IsActive, Browser, Device, SessionID)
                        VALUES (@_AccountID, @_IPAddress, @_LoginDate, @_ExpireDate, @_Location, @_IsActive, @_Browser, @_Device, @_SessionID)

                        SET @_ResponseStatus = 1
                    END