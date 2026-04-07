using Game.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Game.SharedKernel.Results;

namespace Game.Controllers;

[ApiController]
public abstract class GameApiControllerBase : ControllerBase
{
    protected IActionResult HandleResult<T>(Result<T> result) =>
        result.IsSuccess
            ? Ok(result.Value)
            : StatusCode(result.Error.Code, result.Error.Description);

    protected IActionResult HandleResult(ResultWithoutValue result, int successStatusCode = StatusCodes.Status204NoContent) =>
        result.IsSuccess
            ? StatusCode(successStatusCode)
            : StatusCode(result.Error.Code, result.Error.Description);

    protected string? GetPlayerId() => User.FindFirstValue("PlayerId");
}
