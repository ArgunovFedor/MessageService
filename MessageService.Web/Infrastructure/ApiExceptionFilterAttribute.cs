using System;
using System.Collections.Generic;
using System.Net;
using Aeb.DigitalPlatform.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace MessageService.Web.Infrastructure;

public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
{
    private readonly Dictionary<Type, Action<ExceptionContext>> _handlers;
    private readonly ILogger<ApiExceptionFilterAttribute> _logger;

    public ApiExceptionFilterAttribute(ILogger<ApiExceptionFilterAttribute> logger)
    {
        _logger = logger;
        _handlers = new Dictionary<Type, Action<ExceptionContext>>
        {
            { typeof(ServiceException), HandleServiceException },
            { typeof(ValidationException), HandleValidationException },
            { typeof(Exception), HandleException }
            // Register Exception Handlers here
        };
    }

    public override void OnException(ExceptionContext context)
    {
        context.ExceptionHandled = true;
        _logger.LogError(context.Exception, "{ExceptionMessage}", context.Exception.ToString());
        foreach (var definition in _handlers)
        {
            if (definition.Key == context.Exception.GetType())
            {
                definition.Value.Invoke(context);
                return;
            }
        }

        HandleException(context);
    }

    private void HandleServiceException(ExceptionContext context)
    {
        var serviceException = (ServiceException) context.Exception;
        context.Result = serviceException.ErrorCode switch
        {
            "BAD_REQUEST" => GetObjectResultFromServiceException(serviceException, HttpStatusCode.BadRequest),
            "NOT_FOUND" => GetObjectResultFromServiceException(serviceException, HttpStatusCode.NotFound),
            _ => GetObjectResultFromServiceException(serviceException, HttpStatusCode.InternalServerError)
        };
    }

    private static ObjectResult GetObjectResultFromServiceException(ServiceException exception, HttpStatusCode statusCode)
    {
        var apiResult = new ApiResult
        {
            ErrorCode = exception.ErrorCode,
            ErrorMessage = exception.Message,
            ErrorDisplay = exception.ErrorDisplay
        };
        return new ObjectResult(apiResult) { StatusCode =  (int)statusCode};
    }

    private void HandleValidationException(ExceptionContext context)
    {
        var validationException = (ValidationException) context.Exception;
        context.Result = new ObjectResult(
            new ValidationExceptionApiResult(validationException.Failures,
                "VALIDATION", validationException.Message, "Не все поля заполнены корректно"))
        {
            StatusCode = (int) HttpStatusCode.BadRequest
        };
    }

    private void HandleException(ExceptionContext context) =>
        context.Result =
            new ObjectResult(new ApiResult("UNKNOWN", context.Exception.Message))
            {
                StatusCode = (int) HttpStatusCode.InternalServerError
            };
}