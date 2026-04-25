using Microsoft.AspNetCore.Mvc;
using ShippingRates.Application.DTOs;
using ShippingRates.Application.Interfaces;

namespace ShippingRates.Api.Controllers;

[ApiController]
[Route("api/shipment-quotes")]
public sealed class ShipmentQuotesController : ApiControllerBase
{
    private readonly IShippingRateService shippingRateService;

    public ShipmentQuotesController(IShippingRateService shippingRateService)
    {
        this.shippingRateService = shippingRateService;
    }

    [HttpGet("{shipmentQuoteId:long}")]
    [ProducesResponseType(typeof(CalculateQuoteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(long shipmentQuoteId, CancellationToken cancellationToken)
    {
        var result = await shippingRateService.GetQuoteByIdAsync(shipmentQuoteId, cancellationToken);
        return ToActionResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CalculateQuoteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(
        [FromBody] CalculateQuoteRequest request,
        CancellationToken cancellationToken)
    {
        var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = Request.Headers.UserAgent.ToString();

        var result = await shippingRateService.CalculateQuoteAsync(request, clientIp, userAgent, cancellationToken);

        if (!result.Success)
        {
            return ToActionResult(result);
        }

        return CreatedAtAction(
            nameof(GetById),
            new { shipmentQuoteId = result.Data!.ShipmentQuoteId },
            result.Data);
    }
}
