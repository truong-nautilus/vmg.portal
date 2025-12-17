
                    CREATE   PROCEDURE [dbo].[SP_VMG_GetChipByCurrencyID]
                        @_CurrencyID int
                    AS
                    BEGIN
                        SELECT 
                            CurrencyType,
                            CurrencyCode,
                            CurrencyName,
                            ChipList,
                            MaxBetValue,
                            MinBetValue
                        FROM VMG_Chip
                        WHERE CurrencyType = @_CurrencyID AND IsActive = 1
                    END
                    PRINT 'SP_VMG_GetChipByCurrencyID created.'