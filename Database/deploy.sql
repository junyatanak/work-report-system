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
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE TABLE [Processes] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_Processes] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE TABLE [WorkClasses] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_WorkClasses] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE TABLE [Workers] (
        [Id] int NOT NULL IDENTITY,
        [WorkerNumber] int NOT NULL,
        [Name] nvarchar(max) NULL,
        CONSTRAINT [PK_Workers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE TABLE [WorkPatterns] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_WorkPatterns] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE TABLE [Products] (
        [Id] int NOT NULL IDENTITY,
        [ProductCode] nvarchar(450) NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [WorkClassId] int NOT NULL,
        CONSTRAINT [PK_Products] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Products_WorkClasses_WorkClassId] FOREIGN KEY ([WorkClassId]) REFERENCES [WorkClasses] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE TABLE [StandardWorkTimes] (
        [Id] int NOT NULL IDENTITY,
        [WorkClassId] int NOT NULL,
        [ProcessId] int NOT NULL,
        [WorkPatternId] int NOT NULL,
        [StandardCycleSeconds] decimal(9,2) NULL,
        CONSTRAINT [PK_StandardWorkTimes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StandardWorkTimes_Processes_ProcessId] FOREIGN KEY ([ProcessId]) REFERENCES [Processes] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_StandardWorkTimes_WorkClasses_WorkClassId] FOREIGN KEY ([WorkClassId]) REFERENCES [WorkClasses] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_StandardWorkTimes_WorkPatterns_WorkPatternId] FOREIGN KEY ([WorkPatternId]) REFERENCES [WorkPatterns] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE TABLE [ProductionOrders] (
        [Id] int NOT NULL IDENTITY,
        [OrderNumber] nvarchar(450) NOT NULL,
        [ProductId] int NOT NULL,
        [OrderQty] int NOT NULL,
        [DueDate] date NOT NULL,
        [RowVersion] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ProductionOrders] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ProductionOrders_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE TABLE [WorkReports] (
        [Id] int NOT NULL IDENTITY,
        [WorkDate] date NOT NULL,
        [ProductionOrderId] int NOT NULL,
        [ProcessId] int NOT NULL,
        [WorkPatternId] int NOT NULL,
        [UserId] nvarchar(450) NOT NULL,
        [RowVersion] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_WorkReports] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_WorkReports_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_WorkReports_Processes_ProcessId] FOREIGN KEY ([ProcessId]) REFERENCES [Processes] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_WorkReports_ProductionOrders_ProductionOrderId] FOREIGN KEY ([ProductionOrderId]) REFERENCES [ProductionOrders] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_WorkReports_WorkPatterns_WorkPatternId] FOREIGN KEY ([WorkPatternId]) REFERENCES [WorkPatterns] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE TABLE [WorkReportWorkers] (
        [Id] int NOT NULL IDENTITY,
        [WorkReportId] int NOT NULL,
        [WorkerId] int NOT NULL,
        [StartAt] datetime2(0) NOT NULL,
        [EndAt] datetime2(0) NOT NULL,
        [BreakMinutes] int NOT NULL,
        [ProducedQty] int NOT NULL,
        CONSTRAINT [PK_WorkReportWorkers] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_WorkReportWorkers_WorkReports_WorkReportId] FOREIGN KEY ([WorkReportId]) REFERENCES [WorkReports] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_WorkReportWorkers_Workers_WorkerId] FOREIGN KEY ([WorkerId]) REFERENCES [Workers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Processes_Name] ON [Processes] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ProductionOrders_OrderNumber] ON [ProductionOrders] ([OrderNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ProductionOrders_ProductId] ON [ProductionOrders] ([ProductId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Products_ProductCode] ON [Products] ([ProductCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Products_WorkClassId] ON [Products] ([WorkClassId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_StandardWorkTimes_ProcessId_WorkPatternId_WorkClassId] ON [StandardWorkTimes] ([ProcessId], [WorkPatternId], [WorkClassId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_StandardWorkTimes_WorkClassId] ON [StandardWorkTimes] ([WorkClassId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_StandardWorkTimes_WorkPatternId] ON [StandardWorkTimes] ([WorkPatternId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_WorkClasses_Name] ON [WorkClasses] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Workers_WorkerNumber] ON [Workers] ([WorkerNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_WorkPatterns_Name] ON [WorkPatterns] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_WorkReports_ProcessId] ON [WorkReports] ([ProcessId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_WorkReports_ProductionOrderId] ON [WorkReports] ([ProductionOrderId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_WorkReports_UserId] ON [WorkReports] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_WorkReports_WorkPatternId] ON [WorkReports] ([WorkPatternId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_WorkReportWorkers_WorkerId] ON [WorkReportWorkers] ([WorkerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_WorkReportWorkers_WorkReportId_WorkerId] ON [WorkReportWorkers] ([WorkReportId], [WorkerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103031903_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261103031903_InitialCreate', N'10.0.9');
END;

COMMIT;
GO

