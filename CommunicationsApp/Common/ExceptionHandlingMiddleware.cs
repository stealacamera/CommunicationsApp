using Serilog;

namespace CommunicationsApp.Web.Common;

public sealed class ExceptionHandlingMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch(TaskCanceledException)
        {
            // Do nothing
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Web app error");
            throw;
        }
    }
}