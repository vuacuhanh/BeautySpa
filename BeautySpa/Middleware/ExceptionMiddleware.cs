using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using BeautySpa.Core.Base;
using System;
using System.Linq;
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
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            _logger.LogError($"Error occurred: {ex}");

            if (!context.Response.HasStarted)
            {
                BaseResponseModel<object> response;

                // Kiểm tra nếu là ErrorException
                if (ex is ErrorException errorException)
                {
                    response = new BaseResponseModel<object>(
                        errorException.StatusCode,
                        errorException.ErrorDetail.ErrorCode,
                        null,
                        errorException.ErrorDetail.ErrorMessage ?? "An error occurred."
                    );
                }
                else if (ex is CoreException coreException)
                {
                    // Trả về mã trạng thái và thông tin từ CoreException
                    response = new BaseResponseModel<object>(
                        coreException.StatusCode,
                        coreException.Code,
                        null,
                        coreException.Message
                    );

                    // Ghi thêm dữ liệu bổ sung nếu có
                    if (coreException.AdditionalData.Any())
                    {
                        response.AdditionalData = JsonSerializer.Serialize(coreException.AdditionalData);
                    }
                }
                else
                {
                    // Mặc định là lỗi 500
                    response = new BaseResponseModel<object>(
                        StatusCodes.Status500InternalServerError,
                        ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                        null,
                        "An unexpected error occurred."
                    );
                }

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = response.StatusCode;

                var jsonResponse = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(jsonResponse);
            }
        }


    }
}