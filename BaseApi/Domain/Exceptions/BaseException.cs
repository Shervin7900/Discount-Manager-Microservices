using System;

namespace BaseApi.Domain.Exceptions;

public abstract class BaseException : Exception
{
    protected BaseException(string message) : base(message) { }
}

public class NotFoundException : BaseException
{
    public NotFoundException(string name, object key) 
        : base($"Entity \"{name}\" ({key}) was not found.") { }
}

public class ValidationException : BaseException
{
    public ValidationException(string message) : base(message) { }
}

public class UnauthorizedException : BaseException
{
    public UnauthorizedException(string message) : base(message) { }
}
