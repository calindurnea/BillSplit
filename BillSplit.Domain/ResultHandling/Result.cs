using System.Net;

namespace BillSplit.Domain.ResultHandling;

public record Result
{
    public static IResult<TValue> Failure<TValue>(string message, HttpStatusCode statusCode, params string[] reasons)
    {
        return Failure<TValue>(new ResultError(message, statusCode, reasons));
    }

    public static IResult<TValue> Failure<TValue, TOther>(IResult<TOther> error)
    {
        var failedResult = error as IFailureResult<TOther>;
        ArgumentNullException.ThrowIfNull(failedResult);

        return new _failure<TValue>(failedResult.ResultError);
    }

    public static IResult<TValue> Failure<TValue>(ResultError error)
    {
        return new _failure<TValue>(error);
    }

    public static IResult<TValue> Success<TValue>(TValue result)
    {
        ArgumentNullException.ThrowIfNull(result);
        return new _success<TValue>(result);
    }

    public interface ISuccessResult<TValue> : IResult<TValue>
    {
        TValue Result { get; }
    }

    public interface IFailureResult<TValue> : IResult<TValue>
    {
        ResultError ResultError { get; }
    }

    private sealed record _failure<TValue>(ResultError Error) : IFailureResult<TValue>
    {
        ResultError IFailureResult<TValue>.ResultError => Error;
    }

    private sealed record _success<TValue>(TValue Result) : ISuccessResult<TValue>
    {
        TValue ISuccessResult<TValue>.Result => Result;
    }
}