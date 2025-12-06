USE [Vmg.BillingDB];
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- SP_Agency_CheckExist
IF OBJECT_ID('[dbo].[SP_Agency_CheckExist]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[SP_Agency_CheckExist];
GO
CREATE PROCEDURE [dbo].[SP_Agency_CheckExist]
    @_AccountID INT,
    @_ResponseStatus INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Mock implementation: Assume no one is an agency for now, or check a table if it existed
    -- Logic: If AccountID exists in an Agencies table, return 1, else -1
    -- For now, return -1 (Not an agency) to avoid errors, or 1 if you want to test agency logic
    
    -- Let's say we don't have an Agencies table in this schema yet. 
    -- So strictly returning -1 means "Not an Agency".
    
    SET @_ResponseStatus = -1; 
END
GO

PRINT 'Procedure SP_Agency_CheckExist created successfully';
GO
