using ExpenseTrackerEntity.ViewModel;
using System.Net;

namespace ExpenseTracker.ExceptionMiddleware
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;


        public ExceptionHandler(RequestDelegate next)
        {

            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync(new ExceptionInfo()
            {
                StatusCode = context.Response.StatusCode,
                Message = exception.Message
            }.ToString());
        }
    }
}