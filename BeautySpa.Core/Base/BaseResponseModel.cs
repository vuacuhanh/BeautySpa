﻿using Microsoft.AspNetCore.Http;

namespace BeautySpa.Core.Base
{
    public class BaseResponseModel<T>
    {
        public T? Data { get; set; }
        public object? AdditionalData { get; set; }
        public string? Message { get; set; }
        public int StatusCode { get; set; }
        public string Code { get; set; }
        public bool IsSuccess => StatusCode >= 200 && StatusCode < 300 && Data != null;
        public BaseResponseModel(int statusCode, string code, T? data, object? additionalData = null, string? message = null)
        {
            this.StatusCode = statusCode;
            this.Code = code;
            this.Data = data;
            this.AdditionalData = additionalData;
            this.Message = message;
        }

        public BaseResponseModel(int statusCode, string code, string? message)
        {
            this.StatusCode = statusCode;
            this.Code = code;
            this.Message = message;
        }

        public static BaseResponseModel<T> OkResponseModel(T data, object? additionalData = null, string code = ResponseCodeConstants.SUCCESS)
        {
            return new BaseResponseModel<T>(StatusCodes.Status200OK, code, data, additionalData);
        }

        public static BaseResponseModel<T> NotFoundResponseModel(T? data, object? additionalData = null, string code = ResponseCodeConstants.NOT_FOUND)
        {
            return new BaseResponseModel<T>(StatusCodes.Status404NotFound, code, data, additionalData);
        }

        public static BaseResponseModel<T> BadRequestResponseModel(T? data, object? additionalData = null, string code = ResponseCodeConstants.FAILED)
        {
            return new BaseResponseModel<T>(StatusCodes.Status400BadRequest, code, data, additionalData);
        }

        public static BaseResponseModel<T> InternalErrorResponseModel(T? data, object? additionalData = null, string code = ResponseCodeConstants.FAILED)
        {
            return new BaseResponseModel<T>(StatusCodes.Status500InternalServerError, code, data, additionalData);
        }

        public static object? BadRequestResponseModel(object value, string message)
        {
            throw new NotImplementedException();
        }

        public static BaseResponseModel<T> Success(T? data, object? additionalData = null, string code = ResponseCodeConstants.SUCCESS)
        {
            return new BaseResponseModel<T>(StatusCodes.Status200OK, code, data, additionalData);
        }

        public static BaseResponseModel<T> Error(int statusCode, string message, string code = ResponseCodeConstants.FAILED)
        {
            return new BaseResponseModel<T>(statusCode, code, default, null, message);
        }
        public static class ResponseCodeConstants
        {
            public const string SUCCESS = "SUCCESS";
            public const string FAILED = "FAILED";
            public const string NOT_FOUND = "NOT_FOUND";
        }
    }

    public class BaseResponseModel : BaseResponseModel<object>
    {
        public BaseResponseModel(int statusCode, string code, object? data, object? additionalData = null, string? message = null) : base(statusCode, code, data, additionalData, message)
        {
        }
        public BaseResponseModel(int statusCode, string code, string? message) : base(statusCode, code, message)
        {
        }
    }
}
