using Microsoft.AspNetCore.Mvc;

namespace BillSplit.Domain.ResultHandling;

public static class ResultExtensions
{
    public static IActionResult HandleResult<TValue>(IResult<TValue> resultOutcome, IActionResult result)
    {
        return resultOutcome switch
        {
            Result.ISuccessResult<TValue> => result,
            Result.IFailureResult<TValue> failure => HandleFailedResult(failure),
            _ => throw new InvalidOperationException("Operation result did not match any expected type")
        };
    }

    public static IActionResult HandleResult<TValue>(IResult<TValue> resultOutcome,  Func<TValue, IActionResult> func)
    {
        return resultOutcome switch
        {
            Result.ISuccessResult<TValue> success => func(success.Result),
            Result.IFailureResult<TValue> failure => HandleFailedResult(failure),
            _ => throw new InvalidOperationException("Operation result did not match any expected type")
        };
    }
    
    private static ObjectResult HandleFailedResult<TValue>(Result.IFailureResult<TValue> failure)
    {
        var problemDetails = new ProblemDetails
        {
            Status = (int)failure.ResultError.StatusCode,
            Title = failure.ResultError.Message,
            Extensions = new Dictionary<string, object?>
            {
                {"reasons", failure.ResultError.Reasons}
            }
        };
        return new ObjectResult(problemDetails);
    }
}