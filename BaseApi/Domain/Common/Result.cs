using System.Collections.Generic;

namespace BaseApi.Domain.Common;

public class Result
{
    public bool IsSuccess { get; protected set; }
    public string Message { get; protected set; } = string.Empty;
    public List<string> Errors { get; protected set; } = new();

    public static Result Success(string message = "") => new() { IsSuccess = true, Message = message };
    public static Result Failure(string error) => new() { IsSuccess = false, Errors = new List<string> { error } };
    public static Result Failure(List<string> errors) => new() { IsSuccess = false, Errors = errors };
}

public class Result<T> : Result
{
    public T? Data { get; protected set; }

    public static Result<T> Success(T data, string message = "") => new() { IsSuccess = true, Data = data, Message = message };
    public new static Result<T> Failure(string error) => new() { IsSuccess = false, Errors = new List<string> { error } };
    public new static Result<T> Failure(List<string> errors) => new() { IsSuccess = false, Errors = errors };
}
