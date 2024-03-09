using System.Net;

namespace BillSplit.Domain.ResultHandling;

public sealed class ResultError
{
    public ResultError(string message, HttpStatusCode statusCode, params string[] reasons)
    {
        Message = message;
        StatusCode = statusCode;
        
        if (reasons.Length == 0)
        {
            reasons = new[] { message };
        }
        
        Reasons = reasons;
    }

    public string Message { get; }
    public HttpStatusCode StatusCode { get; }
    public string[] Reasons { get; }
}