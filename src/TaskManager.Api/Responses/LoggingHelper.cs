using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace TaskManager.Api.Responses;

public static class LoggingHelper
{
    public static IActionResult LogAndReturnBadRequest(ILogger logger, Exception ex)
    {
        logger.LogError(ex, ex.Message);
        return new BadRequestObjectResult(ex.Message);
    }

    public static IActionResult LogAndReturnNotFound(ILogger logger, Exception ex)
    {
        logger.LogError(ex, ex.Message);
        return new NotFoundObjectResult(ex.Message);
    }
    
    
}