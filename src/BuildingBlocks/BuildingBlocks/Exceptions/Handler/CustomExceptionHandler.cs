using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace BuildingBlocks.Exceptions.Handler
{
    public class CustomExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<CustomExceptionHandler> logger;
        public CustomExceptionHandler(ILogger<CustomExceptionHandler> logger)
        {
            this.logger = logger;
        }
        public async ValueTask<bool> TryHandleAsync(
            HttpContext context,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var exceptionMessage = exception.Message;
            //در نتیجه، اطلاعات استثنا در سیستم ثبت لاگ ذخیره می‌شود
            logger.LogError(
                "Error Message: {exceptionMessage}, Time of occurrence {time}",
                exceptionMessage, DateTime.UtcNow);

            (string Detail, string Title, int StatusCode) details = exception switch
            {
                InternalServerException => (
                    exception.Message,
                    exception.GetType().Name,
                    StatusCodes.Status500InternalServerError
                ),

                ValidationException => (
                    exception.Message,
                    exception.GetType().Name,
                    StatusCodes.Status400BadRequest
                ),
                BadRequestException => (
                   exception.Message,
                   exception.GetType().Name,
                   StatusCodes.Status400BadRequest
               ),
                NotFoundException => (
                   exception.Message,
                   exception.GetType().Name,
                   StatusCodes.Status404NotFound
               ),
                _ => (exception.Message,
                   exception.GetType().Name,
                    StatusCodes.Status500InternalServerError
                    )
            };

            var problemDetails = new ProblemDetails
            {
                Title = details.Title,
                Detail = details.Detail,
                Status = details.StatusCode,
                Instance = context.Request.Path
            };
            // Return false to continue with the default behavior
            // - or - return true to signal that this exception is handled
            problemDetails.Extensions.Add("traceId",context.TraceIdentifier);
            if (exception is FluentValidation.ValidationException validationException)
            {
                problemDetails.Extensions.Add(
                    "errors",
                    validationException.Errors);
            }
            context.Response.StatusCode = details.StatusCode;

            await context.Response.WriteAsJsonAsync(
                problemDetails,
                cancellationToken);

            return true;
        }
    }
}

