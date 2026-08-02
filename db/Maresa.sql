/* 
  Scripts de bases de Datos para la aplicación Maresa. 
  Este script crea la base de datos MaresaDb y las tablas necesarias para la aplicación, 
  incluyendo PedidosCabecera, LogsAuditoria y PedidoDetalles. 
*/

IF DB_ID(N'MaresaDb') IS NULL
BEGIN
    CREATE DATABASE [MaresaDb];
END;
GO

USE [MaresaDb];
GO

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
    WHERE [MigrationId] = N'20260801223725_InitialCreate'
)
BEGIN
    CREATE TABLE [PedidosCabecera] (
        [Id] int NOT NULL IDENTITY,
        [ClienteId] int NOT NULL,
        [Fecha] datetime2 NOT NULL,
        [Total] decimal(18,2) NOT NULL,
        [Usuario] nvarchar(100) NOT NULL,
        [Estado] nvarchar(20) NOT NULL,
        CONSTRAINT [PK_PedidosCabecera] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260801223725_InitialCreate'
)
BEGIN
    CREATE TABLE [LogsAuditoria] (
        [Id] int NOT NULL IDENTITY,
        [PedidoId] int NULL,
        [Fecha] datetime2 NOT NULL,
        [Evento] nvarchar(100) NOT NULL,
        [Descripcion] nvarchar(500) NOT NULL,
        CONSTRAINT [PK_LogsAuditoria] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LogsAuditoria_PedidosCabecera_PedidoId] FOREIGN KEY ([PedidoId]) REFERENCES [PedidosCabecera] ([Id]) ON DELETE SET NULL
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260801223725_InitialCreate'
)
BEGIN
    CREATE TABLE [PedidoDetalles] (
        [Id] int NOT NULL IDENTITY,
        [PedidoId] int NOT NULL,
        [ProductoId] int NOT NULL,
        [Cantidad] int NOT NULL,
        [Precio] decimal(18,2) NOT NULL,
        CONSTRAINT [PK_PedidoDetalles] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PedidoDetalles_PedidosCabecera_PedidoId] FOREIGN KEY ([PedidoId]) REFERENCES [PedidosCabecera] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260801223725_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_LogsAuditoria_PedidoId] ON [LogsAuditoria] ([PedidoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260801223725_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_PedidoDetalles_PedidoId] ON [PedidoDetalles] ([PedidoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260801223725_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260801223725_InitialCreate', N'10.0.10');
END;

COMMIT;
GO

