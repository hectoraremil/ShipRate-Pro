using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using ShippingRates.Application.Base;

namespace ShippingRates.Api.Controllers;

public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult ToActionResult<T>(ApplicationResult<T> result)
    {
        if (result.Success)
        {
            return Ok(result.Data);
        }

        return result.ResultType switch
        {
            ApplicationResultType.ValidationError => BuildValidationProblem(result),
            ApplicationResultType.NotFound => NotFound(new ProblemDetails
            {
                Title = result.Message,
                Status = StatusCodes.Status404NotFound,
                Extensions = { ["traceId"] = HttpContext.TraceIdentifier }
            }),
            _ => StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = result.Message,
                Status = StatusCodes.Status500InternalServerError,
                Extensions = { ["traceId"] = HttpContext.TraceIdentifier }
            })
        };
    }

    private BadRequestObjectResult BuildValidationProblem<T>(ApplicationResult<T> result)
    {
        var modelState = new ModelStateDictionary();

        foreach (var error in result.ValidationErrors)
        {
            modelState.AddModelError(error.Field, error.Message);
        }

        var problemDetails = new ValidationProblemDetails(modelState)
        {
            Title = result.Message,
            Status = StatusCodes.Status400BadRequest
        };

        problemDetails.Extensions["traceId"] = HttpContext.TraceIdentifier;

        return BadRequest(problemDetails);
    }
}
