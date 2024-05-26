namespace MessageService.Web.Infrastructure;

public class ApiResult
{
    public bool Result { get; set; }
    public string ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
    public string ErrorDisplay { get; set; }
    public string DebugMessage { get; set; }

    public ApiResult()
    {
        Result = true;
    }

    public ApiResult(string errorCode, string errorMessage, string errorDisplay = null, string debugMessage = null)
    {
        Result = false;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
        ErrorDisplay = errorDisplay ?? errorMessage;
        DebugMessage = debugMessage;
    }
}

public class ApiResult<T> : ApiResult
{
    public T Data { get; set; }

    public ApiResult()
    {
        Result = true;
    }

    public ApiResult(string errorCode, string errorMessage, string errorDisplay = null, string debugMessage = null) : base(errorCode, errorMessage, errorDisplay, debugMessage)
    {
    }

    public ApiResult(T data)
    {
        Data = data;
    }
}