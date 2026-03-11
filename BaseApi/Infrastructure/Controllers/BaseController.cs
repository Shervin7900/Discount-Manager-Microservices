using Microsoft.AspNetCore.Mvc;
using BaseApi.Domain.Common;

namespace BaseApi.Infrastructure.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected IActionResult OkResult<T>(T data, string message = "")
    {
        return Ok(Result<T>.Success(data, message));
    }

    protected IActionResult CreatedResult<T>(T data, string message = "")
    {
        return Created("", Result<T>.Success(data, message));
    }

    protected IActionResult FailureResult(string error)
    {
        return BadRequest(Result.Failure(error));
    }
}
