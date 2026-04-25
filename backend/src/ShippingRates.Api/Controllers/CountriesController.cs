using Microsoft.AspNetCore.Mvc;
using ShippingRates.Application.DTOs;
using ShippingRates.Application.Interfaces;

namespace ShippingRates.Api.Controllers;

[ApiController]
[Route("api/countries")]
public sealed class CountriesController : ApiControllerBase
{
    private readonly IShippingRateService shippingRateService;

    public CountriesController(IShippingRateService shippingRateService)
    {
        this.shippingRateService = shippingRateService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<CountryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAvailableCountries(CancellationToken cancellationToken)
    {
        var result = await shippingRateService.GetAvailableCountriesAsync(cancellationToken);
        return ToActionResult(result);
    }
}
