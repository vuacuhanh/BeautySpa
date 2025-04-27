using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using BeautySpa.Core.Base;
using System.Text.Json;
using System.Threading.Tasks;

namespace BeautySpa.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (TaskCanceledException)
            {
                // Nếu request bị hủy (ví dụ client đóng kết nối), không cần log lỗi
                _logger.LogWarning("Request was cancelled by the client.");
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred.");

            if (!context.Response.HasStarted)
            {
                var response = CreateErrorResponse(ex);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = response.StatusCode;

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };

                var jsonResponse = JsonSerializer.Serialize(response, options);
                await context.Response.WriteAsync(jsonResponse);
            }
        }

        private BaseResponseModel<object> CreateErrorResponse(Exception ex)
        {
            if (ex is ErrorException errorEx)
            {
                return new BaseResponseModel<object>(
                    errorEx.StatusCode,
                    errorEx.ErrorDetail.ErrorCode,
                    null,
                    null,
                    errorEx.ErrorDetail.ErrorMessage?.ToString()
                );
            }
            else if (ex is CoreException coreEx)
            {
                return new BaseResponseModel<object>(
                    coreEx.StatusCode,
                    coreEx.Code,
                    null,
                    coreEx.AdditionalData,
                    coreEx.Message
                );
            }
            else
            {
                return new BaseResponseModel<object>(
                    StatusCodes.Status500InternalServerError,
                    ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                    null,
                    null,
                    "An unexpected error occurred."
                );
            }
        }
    }
}
