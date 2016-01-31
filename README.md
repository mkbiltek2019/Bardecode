# Bardecode
first repository - cmd 

Store Procedure in MS SQL Server

USE TimerSQL;
/*
GO
ALTER PROCEDURE ReturnPageCountNull 
    @_JobNo int,
	@_Barcode nvarchar(50)
AS 

    SET NOCOUNT ON;
    SELECT 1
    FROM Pablo_ScanData
    WHERE Pagecount is Null and DrawingCount is Null  and Box is not Null  AND JobNo = @_JobNo AND Barcode=@_Barcode;
GO
*/

exec ReturnPageCountNull 1839