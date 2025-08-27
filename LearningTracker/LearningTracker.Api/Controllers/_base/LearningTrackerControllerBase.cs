using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;

namespace LearningTracker.Api.Controllers._base;

public abstract class LearningTrackerControllerBase : ControllerBase{
    protected IActionResult HandleResult(Result result) {
        if (result.IsSuccess) {
            return NoContent();
        }

        return BadRequest(result.Error);
    }

    protected IActionResult HandleResult<T>(Result<T> result) {
        if (result.IsSuccess) {
            return new JsonResult(result.Value);
        }

        return BadRequest(result.Error);
    } 
}