using System;
using System.Collections;
using System.Collections.Generic;

namespace MessageService.Core.Infrastructure;

public class ServiceException : Exception
{
    public string ErrorCode { get; }
    public ICollection Errors { get; }
        
    public string ErrorDisplay { get; set; }

    public ServiceException(string errorCode, Exception innerException = null, ICollection errors = null) : base($"See message by errorCode = '{errorCode}'", innerException)
    {
        ErrorCode = errorCode;
        Errors = errors;
    }

    public ServiceException(string errorCode, string message, Exception innerException = null,
        ICollection errors = null) : base(message, innerException)
    {
        ErrorCode = errorCode;
        Errors = errors;
    }

    public const string UnknownErrorCode = "UNKNOWN";

}
