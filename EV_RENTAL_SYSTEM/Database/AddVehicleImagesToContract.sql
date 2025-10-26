-- Add Handover_Image and Return_Image columns to Contract table
-- Run this script on your database

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Contract') AND name = 'Handover_Image')
BEGIN
    ALTER TABLE [Contract]
    ADD [Handover_Image] nvarchar(500) NULL;
    PRINT 'Added column Handover_Image to Contract table';
END
ELSE
BEGIN
    PRINT 'Column Handover_Image already exists in Contract table';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Contract') AND name = 'Return_Image')
BEGIN
    ALTER TABLE [Contract]
    ADD [Return_Image] nvarchar(500) NULL;
    PRINT 'Added column Return_Image to Contract table';
END
ELSE
BEGIN
    PRINT 'Column Return_Image already exists in Contract table';
END
GO

PRINT 'Migration completed successfully!';

