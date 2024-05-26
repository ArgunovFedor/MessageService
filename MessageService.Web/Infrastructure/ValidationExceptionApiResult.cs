using System.Collections.Generic;

namespace MessageService.Web.Infrastructure;

public class ValidationExceptionApiResult : ApiResult
{
    public ValidationExceptionApiResult(IDictionary<string, string[]> modelState, string errorCode, 
        string errorMessage, string errorDisplay = null, string debugMessage = null) 
        : base(errorCode, errorMessage, errorDisplay, debugMessage) 
    {
        ModelState = modelState;
    }

    public IDictionary<string, string[]> ModelState { get; set; }
}
