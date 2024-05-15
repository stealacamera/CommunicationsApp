using CommunicationsApp.Infrastructure.Logger;

namespace CommunicationsApp.Web.Common;

public sealed class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILoggerService _logger;

    public ExceptionHandlingMiddleware(ILoggerService logger)
        => _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch(Exception exc)
        {
            //_logger.LogErrorAsync(exc);
            throw;
        }
    }
}
