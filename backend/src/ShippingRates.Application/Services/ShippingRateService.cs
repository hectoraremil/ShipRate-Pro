using Microsoft.Extensions.Logging;
using ShippingRates.Application.Base;
using ShippingRates.Application.DTOs;
using ShippingRates.Application.Interfaces;
using ShippingRates.Domain.Exceptions;
using ShippingRates.Domain.Services;
using ShippingRates.Application.Validators;

namespace ShippingRates.Application.Services;

public sealed class ShippingRateService : IShippingRateService
{
    private readonly IShippingRateRepository shippingRateRepository;
    private readonly IShipmentQuoteRepository shipmentQuoteRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly TariffCalculator tariffCalculator;
    private readonly ILogger<ShippingRateService> logger;

    public ShippingRateService(
        IShippingRateRepository shippingRateRepository,
        IShipmentQuoteRepository shipmentQuoteRepository,
        IUnitOfWork unitOfWork,
        TariffCalculator tariffCalculator,
        ILogger<ShippingRateService> logger)
    {
        this.shippingRateRepository = shippingRateRepository;
        this.shipmentQuoteRepository = shipmentQuoteRepository;
        this.unitOfWork = unitOfWork;
        this.tariffCalculator = tariffCalculator;
        this.logger = logger;
    }

    public async Task<ApplicationResult<IReadOnlyCollection<CountryResponse>>> GetAvailableCountriesAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Iniciando consulta de paises disponibles.");

        try
        {
            var countries = await shippingRateRepository.GetAvailableCountriesAsync(cancellationToken);

            var response = countries
                .Select(country => new CountryResponse
                {
                    CountryCode = country.CountryCode,
                    CountryName = country.CountryName
                })
                .ToArray();

            logger.LogInformation("Consulta de paises completada correctamente. Total: {Count}", response.Length);

            return ApplicationResult<IReadOnlyCollection<CountryResponse>>.Ok(response, "Paises disponibles obtenidos correctamente.");
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Se produjo un error al consultar los paises disponibles.");

            return ApplicationResult<IReadOnlyCollection<CountryResponse>>.Fail(
                "No fue posible obtener los paises disponibles en este momento.",
                ApplicationResultType.UnexpectedError);
        }
    }

    public async Task<ApplicationResult<CalculateQuoteResponse>> CalculateQuoteAsync(
        CalculateQuoteRequest request,
        string? clientIp,
        string? userAgent,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Iniciando calculo de tarifa. CountryCode: {CountryCode}, WeightKg: {WeightKg}",
            request?.CountryCode,
            request?.WeightKg);

        var validationErrors = CalculateQuoteRequestValidator.Validate(request);

        if (validationErrors.Count > 0)
        {
            logger.LogWarning(
                "La solicitud de cotizacion fallo por validacion. Errores: {ValidationErrors}",
                string.Join(" | ", validationErrors.Select(error => $"{error.Field}: {error.Message}")));

            return ApplicationResult<CalculateQuoteResponse>.Fail(
                "La solicitud de cotizacion contiene errores de validacion.",
                ApplicationResultType.ValidationError,
                validationErrors);
        }

        var safeRequest = request!;

        try
        {
            var normalizedCountryCode = safeRequest.CountryCode.Trim().ToUpperInvariant();
            var shippingRate = await shippingRateRepository.GetActiveRateByCountryCodeAsync(normalizedCountryCode, cancellationToken);

            if (shippingRate is null || shippingRate.Country is null || !shippingRate.Country.IsActive)
            {
                logger.LogWarning("No se encontro una tarifa activa para el pais {CountryCode}.", normalizedCountryCode);

                return ApplicationResult<CalculateQuoteResponse>.Fail(
                    "El pais de destino seleccionado no esta disponible.",
                    ApplicationResultType.NotFound);
            }

            var shipmentQuote = tariffCalculator.Calculate(safeRequest.WeightKg, shippingRate, clientIp, userAgent);

            await unitOfWork.ExecuteInTransactionAsync(async transactionCancellationToken =>
            {
                await shipmentQuoteRepository.AddAsync(shipmentQuote, transactionCancellationToken);
            }, cancellationToken);

            var response = new CalculateQuoteResponse
            {
                ShipmentQuoteId = shipmentQuote.ShipmentQuoteId,
                CountryCode = shippingRate.Country.CountryCode,
                CountryName = shippingRate.Country.CountryName,
                WeightKg = shipmentQuote.RequestedWeightKg,
                RatePerKilogram = shipmentQuote.AppliedRatePerKilogram,
                TotalAmount = shipmentQuote.TotalAmount,
                CurrencyCode = shipmentQuote.CurrencyCode,
                RequestedAt = shipmentQuote.RequestedAt
            };

            logger.LogInformation(
                "Cotizacion calculada correctamente. CountryCode: {CountryCode}, TotalAmount: {TotalAmount}",
                response.CountryCode,
                response.TotalAmount);

            return ApplicationResult<CalculateQuoteResponse>.Ok(response, "Tarifa calculada correctamente.");
        }
        catch (DomainValidationException exception)
        {
            logger.LogWarning(exception, "La cotizacion fallo por una regla de negocio.");

            return ApplicationResult<CalculateQuoteResponse>.Fail(
                exception.Message,
                ApplicationResultType.ValidationError,
                [new ValidationError
                {
                    Field = nameof(CalculateQuoteRequest.WeightKg),
                    Message = exception.Message
                }]);
        }
        catch (ResourceNotFoundException exception)
        {
            logger.LogWarning(exception, "No fue posible calcular la tarifa porque el recurso solicitado no existe.");

            return ApplicationResult<CalculateQuoteResponse>.Fail(
                exception.Message,
                ApplicationResultType.NotFound);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Se produjo un error al calcular la tarifa de envio.");

            return ApplicationResult<CalculateQuoteResponse>.Fail(
                "No fue posible calcular la tarifa en este momento.",
                ApplicationResultType.UnexpectedError);
        }
    }

    public async Task<ApplicationResult<CalculateQuoteResponse>> GetQuoteByIdAsync(long shipmentQuoteId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Iniciando consulta de cotizacion. ShipmentQuoteId: {ShipmentQuoteId}", shipmentQuoteId);

        if (shipmentQuoteId <= 0)
        {
            return ApplicationResult<CalculateQuoteResponse>.Fail(
                "El identificador de la cotizacion debe ser mayor que cero.",
                ApplicationResultType.ValidationError,
                [new ValidationError
                {
                    Field = nameof(shipmentQuoteId),
                    Message = "El identificador de la cotizacion debe ser mayor que cero."
                }]);
        }

        try
        {
            var shipmentQuote = await shipmentQuoteRepository.GetByIdAsync(shipmentQuoteId, cancellationToken);

            if (shipmentQuote is null || shipmentQuote.Country is null)
            {
                logger.LogWarning("No se encontro la cotizacion con id {ShipmentQuoteId}.", shipmentQuoteId);

                return ApplicationResult<CalculateQuoteResponse>.Fail(
                    "La cotizacion solicitada no existe.",
                    ApplicationResultType.NotFound);
            }

            var response = new CalculateQuoteResponse
            {
                ShipmentQuoteId = shipmentQuote.ShipmentQuoteId,
                CountryCode = shipmentQuote.Country.CountryCode,
                CountryName = shipmentQuote.Country.CountryName,
                WeightKg = shipmentQuote.RequestedWeightKg,
                RatePerKilogram = shipmentQuote.AppliedRatePerKilogram,
                TotalAmount = shipmentQuote.TotalAmount,
                CurrencyCode = shipmentQuote.CurrencyCode,
                RequestedAt = shipmentQuote.RequestedAt
            };

            return ApplicationResult<CalculateQuoteResponse>.Ok(response, "Cotizacion obtenida correctamente.");
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Se produjo un error al consultar la cotizacion {ShipmentQuoteId}.", shipmentQuoteId);

            return ApplicationResult<CalculateQuoteResponse>.Fail(
                "No fue posible obtener la cotizacion en este momento.",
                ApplicationResultType.UnexpectedError);
        }
    }
}
