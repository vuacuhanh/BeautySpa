using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace BeautySpa.Core.Base
{
    public class CoreException : Exception
    {
        public CoreException(string code, string message = "", int statusCode = StatusCodes.Status500InternalServerError)
            : base(message)
        {
            Code = code;
            StatusCode = statusCode;
        }

        public string Code { get; }

        public int StatusCode { get; set; }

        [JsonExtensionData]
        public Dictionary<string, object> AdditionalData { get; set; } = new(); 
    }

    public class BadRequestException : ErrorException
    {
        public BadRequestException(string errorCode, string? message = null) 
            : base(400, errorCode, message)
        {
        }

        public BadRequestException(ICollection<KeyValuePair<string, ICollection<string>>> errors)
            : base(400, new ErrorDetail
            {
                ErrorCode = "bad_request",
                ErrorMessage = errors
            })
        {
        }
    }

    public class ErrorException : Exception
    {
        public int StatusCode { get; }

        public ErrorDetail ErrorDetail { get; }

        public ErrorException(int statusCode, string errorCode, string? message = null)
        {
            StatusCode = statusCode;
            ErrorDetail = new ErrorDetail
            {
                ErrorCode = errorCode,
                ErrorMessage = message ?? string.Empty
            };
        }

        public ErrorException(int statusCode, ErrorDetail errorDetail)
        {
            StatusCode = statusCode;
            ErrorDetail = errorDetail;
        }
    }

    public class ErrorDetail
    {
        [JsonPropertyName("errorCode")]
        public required string ErrorCode { get; set; }

        [JsonPropertyName("errorMessage")]
        public required object ErrorMessage { get; set; }
    }

    public static class ErrorCode
    {
        public const string BadRequest = "Bad Request";
        public const string UnAuthenticated = "Un-Authenticate";
        public const string UnAuthorized = "Forbidden";
        public const string NotFound = "Not Found";
        public const string Unknown = "Oops! Something went wrong, please try again later.";
        public const string NotUnique = "The resource is already, please try another.";
        public const string TokenExpired = "The Token is already expired.";
        public const string TokenInvalid = "The Token is invalid.";
        public const string Validated = "Validated.";
        public const string Conflicted = "Conflicted.";
        public const string InvalidInput = "Invalid input!";
        public const string InternalServerError = "Internal Server Error";
        public const string Duplicate = "Duplicate";
        public const string Failed = "Failed";
    }

    public static class ResponseCodeConstants
    {
        public const string NOT_FOUND = "Not found!";
        public const string SUCCESS = "Success!";
        public const string FAILED = "Failed!";
        public const string EXISTED = "Existed!";
        public const string DUPLICATE = "Duplicate!";
        public const string INTERNAL_SERVER_ERROR = "INTERNAL_SERVER_ERROR";
        public const string INVALID_INPUT = "Invalid input!";
        public const string UNAUTHORIZED = "Unauthorized!";
        public const string BADREQUEST = "Bad request!";
    }
}
