using Microsoft.EntityFrameworkCore;
using ShippingRates.Domain.Entities;

namespace ShippingRates.Persistence.Context;

public sealed class ShippingRatesDbContext : DbContext
{
    public ShippingRatesDbContext(DbContextOptions<ShippingRatesDbContext> options) : base(options)
    {
    }

    public DbSet<Country> Countries => Set<Country>();
    public DbSet<ShippingRate> ShippingRates => Set<ShippingRate>();
    public DbSet<ShipmentQuote> ShipmentQuotes => Set<ShipmentQuote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Country>(entity =>
        {
            entity.ToTable("Countries");
            entity.HasKey(country => country.CountryId);

            entity.Property(country => country.CountryCode)
                .HasMaxLength(10)
                .IsRequired();

            entity.Property(country => country.CountryName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(country => country.CreatedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasIndex(country => country.CountryCode).IsUnique();
            entity.HasIndex(country => country.CountryName).IsUnique();
        });

        modelBuilder.Entity<ShippingRate>(entity =>
        {
            entity.ToTable("ShippingRates");
            entity.HasKey(rate => rate.ShippingRateId);

            entity.Property(rate => rate.RatePerKilogram)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(rate => rate.CurrencyCode)
                .HasMaxLength(3)
                .IsRequired();

            entity.Property(rate => rate.EffectiveFrom)
                .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.Property(rate => rate.CreatedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasOne(rate => rate.Country)
                .WithMany(country => country.ShippingRates)
                .HasForeignKey(rate => rate.CountryId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<ShipmentQuote>(entity =>
        {
            entity.ToTable("ShipmentQuotes");
            entity.HasKey(quote => quote.ShipmentQuoteId);

            entity.Property(quote => quote.RequestedWeightKg)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(quote => quote.AppliedRatePerKilogram)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(quote => quote.TotalAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(quote => quote.CurrencyCode)
                .HasMaxLength(3)
                .IsRequired();

            entity.Property(quote => quote.ClientIp)
                .HasMaxLength(45);

            entity.Property(quote => quote.UserAgent)
                .HasMaxLength(300);

            entity.Property(quote => quote.RequestedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasOne(quote => quote.Country)
                .WithMany()
                .HasForeignKey(quote => quote.CountryId)
                .OnDelete(DeleteBehavior.NoAction);
        });
    }
}
