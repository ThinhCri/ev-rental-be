-- Add Notes and Station_Id columns to User table
USE EV_Rental_System;

-- Check if Notes column exists, if not add it
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('User') AND name = 'Notes')
BEGIN
    ALTER TABLE [User] ADD [Notes] nvarchar(500) NULL;
    PRINT 'Added Notes column to User table';
END
ELSE
BEGIN
    PRINT 'Notes column already exists in User table';
END

-- Check if Station_Id column exists, if not add it
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('User') AND name = 'Station_Id')
BEGIN
    ALTER TABLE [User] ADD [Station_Id] int NULL;
    PRINT 'Added Station_Id column to User table';
END
ELSE
BEGIN
    PRINT 'Station_Id column already exists in User table';
END

-- Add foreign key constraint if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_User_Station_Station_Id')
BEGIN
    ALTER TABLE [User] 
    ADD CONSTRAINT [FK_User_Station_Station_Id] 
    FOREIGN KEY ([Station_Id]) REFERENCES [Station]([Station_Id]);
    PRINT 'Added foreign key constraint FK_User_Station_Station_Id';
END
ELSE
BEGIN
    PRINT 'Foreign key constraint FK_User_Station_Station_Id already exists';
END

-- Create index if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_User_Station_Id')
BEGIN
    CREATE INDEX [IX_User_Station_Id] ON [User] ([Station_Id]);
    PRINT 'Created index IX_User_Station_Id';
END
ELSE
BEGIN
    PRINT 'Index IX_User_Station_Id already exists';
END

PRINT 'Script completed successfully!';

