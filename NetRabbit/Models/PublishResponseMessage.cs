using System;

namespace NetRabbit.Models;

public readonly record struct PublishResponseMessage
{
    public bool IsSuccess { get; }
    public Exception? Exception { get; }

    private static readonly PublishResponseMessage SuccessInstance = new(isSuccess: true);
    
    private PublishResponseMessage(bool isSuccess)
    {
        IsSuccess = isSuccess;
        Exception = null;
    }

    private PublishResponseMessage(Exception exception)
    {
        Exception = exception;
        IsSuccess = false;
    }

    public static PublishResponseMessage Success()
    {
        return SuccessInstance;
    }

    public static PublishResponseMessage FromException(Exception exception)
    {
        return new(exception);
    }

    public void EnsureSuccess()
    {
        if (Exception == null)
            return;
        throw Exception;
    }
}