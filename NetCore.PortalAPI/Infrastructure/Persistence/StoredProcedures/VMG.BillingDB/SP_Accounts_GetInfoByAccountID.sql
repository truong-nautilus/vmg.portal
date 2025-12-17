CREATE   PROCEDURE [dbo].[SP_Accounts_GetInfoByAccountID]
    @_AccountID BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT *
    FROM Accounts WITH(NOLOCK)
    WHERE AccountID = @_AccountID;
END
