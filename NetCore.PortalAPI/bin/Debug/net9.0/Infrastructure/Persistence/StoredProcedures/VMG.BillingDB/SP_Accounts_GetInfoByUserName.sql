CREATE OR ALTER PROCEDURE [dbo].[SP_Accounts_GetInfoByUserName]
    @_UserName VARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT *
    FROM Accounts WITH(NOLOCK)
    WHERE AccountName = @_UserName;
END
