USE TarifasEnvioDb;
GO

INSERT INTO dbo.Countries (CountryCode, CountryName, IsActive)
VALUES
    (N'IN', N'India', 1),
    (N'US', N'Estados Unidos', 1),
    (N'UK', N'Reino Unido', 1);
GO

INSERT INTO dbo.ShippingRates (CountryId, RatePerKilogram, CurrencyCode, IsActive)
SELECT CountryId, RatePerKilogram, N'USD', 1
FROM
(
    VALUES
        (N'IN', CAST(5.00 AS DECIMAL(18,2))),
        (N'US', CAST(8.00 AS DECIMAL(18,2))),
        (N'UK', CAST(10.00 AS DECIMAL(18,2)))
) AS SourceRates (CountryCode, RatePerKilogram)
INNER JOIN dbo.Countries Countries
    ON Countries.CountryCode = SourceRates.CountryCode;
GO
