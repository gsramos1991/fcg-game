IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Games] (
    [Id] uniqueidentifier NOT NULL,
    [Title] nvarchar(255) NOT NULL,
    [Description] nvarchar(1000) NOT NULL,
    [Genre] nvarchar(100) NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [Publisher] nvarchar(max) NOT NULL,
    [ReleaseDate] datetime2 NOT NULL,
    [PopularityScore] int NOT NULL,
    [TotalSales] int NOT NULL,
    [Tags] nvarchar(max) NOT NULL,
    [CoverImageUrl] nvarchar(max) NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Games] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Orders] (
    [Id] uniqueidentifier NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    [TotalAmount] decimal(18,2) NOT NULL,
    [Status] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CompletedAt] datetime2 NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [OrderItem] (
    [Id] uniqueidentifier NOT NULL,
    [GameId] uniqueidentifier NOT NULL,
    [GameTitle] nvarchar(max) NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [Quantity] int NOT NULL,
    [OrderId] uniqueidentifier NULL,
    CONSTRAINT [PK_OrderItem] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OrderItem_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_OrderItem_OrderId] ON [OrderItem] ([OrderId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251215154059_InitialCreate', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Orders] ADD [PaymentId] uniqueidentifier NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251216012904_AddPaymentIdToOrder', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [OrderItem] DROP CONSTRAINT [FK_OrderItem_Orders_OrderId];
GO

DROP INDEX [IX_OrderItem_OrderId] ON [OrderItem];
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OrderItem]') AND [c].[name] = N'OrderId');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [OrderItem] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [OrderItem] DROP COLUMN [OrderId];
GO

CREATE TABLE [UserLibraryGames] (
    [idLibraryGame] uniqueidentifier NOT NULL,
    [userId] uniqueidentifier NOT NULL,
    [idGame] uniqueidentifier NOT NULL,
    [isActive] bit NOT NULL DEFAULT CAST(0 AS bit),
    [createdAt] datetime2 NOT NULL DEFAULT '2026-01-02T19:21:59.3211527Z',
    CONSTRAINT [PK_UserLibraryGames] PRIMARY KEY ([idLibraryGame])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260102192159_TableUserLibraryGame', N'8.0.0');
GO

COMMIT;
GO

