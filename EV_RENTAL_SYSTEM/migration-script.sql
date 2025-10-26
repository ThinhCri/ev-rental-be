BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Transaction]') AND [c].[name] = N'Amount');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Transaction] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Transaction] DROP COLUMN [Amount];
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Order]') AND [c].[name] = N'Notes');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Order] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [Order] DROP COLUMN [Notes];
GO

ALTER TABLE [Transaction] ADD [Status] nvarchar(50) NULL;
GO

ALTER TABLE [Order] ADD [Check_In_Status] nvarchar(50) NULL;
GO

ALTER TABLE [Order] ADD [Rental_Status] nvarchar(50) NULL;
GO

ALTER TABLE [Order] ADD [Verification_License_ImageUrl] nvarchar(500) NULL;
GO

CREATE TABLE [VehicleConditionHistory] (
    [VehicleConditionHistory_Id] int NOT NULL IDENTITY,
    [Order_Id] int NOT NULL,
    [License_Plate_Id] int NULL,
    [Status_Change_Type] nvarchar(50) NOT NULL,
    [Vehicle_Images_Url] nvarchar(500) NULL,
    [Odometer_Reading] decimal(10,2) NULL,
    [Battery_Percentage] decimal(5,2) NULL,
    [Notes] nvarchar(1000) NULL,
    [Condition_Before] nvarchar(500) NULL,
    [Condition_After] nvarchar(500) NULL,
    [Change_Timestamp] datetime2 NOT NULL,
    [Additional_Charges] decimal(10,2) NULL,
    [Additional_Charges_Reason] nvarchar(500) NULL,
    [Staff_Notes] nvarchar(100) NULL,
    CONSTRAINT [PK_VehicleConditionHistory] PRIMARY KEY ([VehicleConditionHistory_Id]),
    CONSTRAINT [FK_VehicleConditionHistory_LicensePlate_License_Plate_Id] FOREIGN KEY ([License_Plate_Id]) REFERENCES [LicensePlate] ([License_plate_Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_VehicleConditionHistory_Order_Order_Id] FOREIGN KEY ([Order_Id]) REFERENCES [Order] ([Order_Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_VehicleConditionHistory_License_Plate_Id] ON [VehicleConditionHistory] ([License_Plate_Id]);
GO

CREATE INDEX [IX_VehicleConditionHistory_Order_Id] ON [VehicleConditionHistory] ([Order_Id]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251026115400_AddRentalWorkflowFeatures', N'8.0.0');
GO

COMMIT;
GO

