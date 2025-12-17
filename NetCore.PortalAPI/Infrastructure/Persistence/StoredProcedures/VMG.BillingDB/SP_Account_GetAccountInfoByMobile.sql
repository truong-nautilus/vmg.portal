CREATE   PROCEDURE [dbo].[SP_Account_GetAccountInfoByMobile]
    @_Mobile VARCHAR(20),
    @_AccountID BIGINT OUTPUT,
    @_UserName VARCHAR(50) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @_AccountID = -1;
    SET @_UserName = '';

    SELECT TOP 1 @_AccountID = AccountID, @_UserName = AccountName
    FROM Accounts WITH(NOLOCK)
    WHERE Mobile = @_Mobile;
END
