namespace Meedu.Middleware
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        public ILogger<ErrorHandlingMiddleware> Logger { get; }

        public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
        {
            Logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch(Exception e)
            {
                Logger.LogError(e, e.Message);

                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Something went wrong");
            }
        }
    }
}
