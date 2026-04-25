using ShippingRates.Application.Base;
using ShippingRates.Application.DTOs;

namespace ShippingRates.Application.Validators;

public static class CalculateQuoteRequestValidator
{
    public static IReadOnlyCollection<ValidationError> Validate(CalculateQuoteRequest? request)
    {
        var errors = new List<ValidationError>();

        if (request is null)
        {
            errors.Add(new ValidationError
            {
                Field = nameof(CalculateQuoteRequest),
                Message = "La solicitud de cotizacion es obligatoria."
            });

            return errors;
        }

        if (request.WeightKg <= 0)
        {
            errors.Add(new ValidationError
            {
                Field = nameof(CalculateQuoteRequest.WeightKg),
                Message = "Debe ingresar un peso valido mayor que cero."
            });
        }

        if (string.IsNullOrWhiteSpace(request.CountryCode))
        {
            errors.Add(new ValidationError
            {
                Field = nameof(CalculateQuoteRequest.CountryCode),
                Message = "Debe seleccionar un pais de destino."
            });
        }
        else if (request.CountryCode.Trim().Length > 10)
        {
            errors.Add(new ValidationError
            {
                Field = nameof(CalculateQuoteRequest.CountryCode),
                Message = "El codigo del pais no puede exceder los 10 caracteres."
            });
        }

        return errors;
    }
}
