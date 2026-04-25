IF DB_ID(N'TarifasEnvioDb') IS NULL
BEGIN
    CREATE DATABASE TarifasEnvioDb;
END;
GO

USE TarifasEnvioDb;
GO

IF OBJECT_ID(N'dbo.ShipmentQuotes', N'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.ShipmentQuotes;
END;
GO

IF OBJECT_ID(N'dbo.ShippingRates', N'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.ShippingRates;
END;
GO

IF OBJECT_ID(N'dbo.Countries', N'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.Countries;
END;
GO

CREATE TABLE dbo.Countries
(
    CountryId INT IDENTITY(1,1) NOT NULL,
    CountryCode NVARCHAR(10) NOT NULL,
    CountryName NVARCHAR(100) NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Countries_IsActive DEFAULT (1),
    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Countries_CreatedAt DEFAULT (SYSUTCDATETIME()),
    UpdatedAt DATETIME2 NULL,
    CONSTRAINT PK_Countries PRIMARY KEY (CountryId),
    CONSTRAINT UQ_Countries_CountryCode UNIQUE (CountryCode),
    CONSTRAINT UQ_Countries_CountryName UNIQUE (CountryName)
);
GO

CREATE TABLE dbo.ShippingRates
(
    ShippingRateId INT IDENTITY(1,1) NOT NULL,
    CountryId INT NOT NULL,
    RatePerKilogram DECIMAL(18,2) NOT NULL,
    CurrencyCode NVARCHAR(3) NOT NULL CONSTRAINT DF_ShippingRates_CurrencyCode DEFAULT (N'USD'),
    EffectiveFrom DATETIME2 NOT NULL CONSTRAINT DF_ShippingRates_EffectiveFrom DEFAULT (SYSUTCDATETIME()),
    EffectiveTo DATETIME2 NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_ShippingRates_IsActive DEFAULT (1),
    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_ShippingRates_CreatedAt DEFAULT (SYSUTCDATETIME()),
    UpdatedAt DATETIME2 NULL,
    CONSTRAINT PK_ShippingRates PRIMARY KEY (ShippingRateId),
    CONSTRAINT FK_ShippingRates_Countries FOREIGN KEY (CountryId)
        REFERENCES dbo.Countries (CountryId),
    CONSTRAINT CK_ShippingRates_RatePerKilogram CHECK (RatePerKilogram > 0),
    CONSTRAINT CK_ShippingRates_CurrencyCode CHECK (LEN(CurrencyCode) = 3)
);
GO

CREATE TABLE dbo.ShipmentQuotes
(
    ShipmentQuoteId BIGINT IDENTITY(1,1) NOT NULL,
    CountryId INT NOT NULL,
    RequestedWeightKg DECIMAL(18,2) NOT NULL,
    AppliedRatePerKilogram DECIMAL(18,2) NOT NULL,
    TotalAmount DECIMAL(18,2) NOT NULL,
    CurrencyCode NVARCHAR(3) NOT NULL CONSTRAINT DF_ShipmentQuotes_CurrencyCode DEFAULT (N'USD'),
    RequestedAt DATETIME2 NOT NULL CONSTRAINT DF_ShipmentQuotes_RequestedAt DEFAULT (SYSUTCDATETIME()),
    ClientIp NVARCHAR(45) NULL,
    UserAgent NVARCHAR(300) NULL,
    CONSTRAINT PK_ShipmentQuotes PRIMARY KEY (ShipmentQuoteId),
    CONSTRAINT FK_ShipmentQuotes_Countries FOREIGN KEY (CountryId)
        REFERENCES dbo.Countries (CountryId),
    CONSTRAINT CK_ShipmentQuotes_RequestedWeightKg CHECK (RequestedWeightKg > 0),
    CONSTRAINT CK_ShipmentQuotes_AppliedRatePerKilogram CHECK (AppliedRatePerKilogram > 0),
    CONSTRAINT CK_ShipmentQuotes_TotalAmount CHECK (TotalAmount >= 0),
    CONSTRAINT CK_ShipmentQuotes_CurrencyCode CHECK (LEN(CurrencyCode) = 3)
);
GO

CREATE INDEX IX_ShippingRates_CountryId_IsActive
    ON dbo.ShippingRates (CountryId, IsActive, EffectiveFrom, EffectiveTo);
GO

CREATE INDEX IX_ShipmentQuotes_CountryId_RequestedAt
    ON dbo.ShipmentQuotes (CountryId, RequestedAt DESC);
GO
