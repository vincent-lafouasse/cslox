using System;

namespace Lox;

public readonly struct Result<T>
{
    // yes this isn't truly a sum type, i'm annoyed as well
    private readonly T _value;
    private readonly IError? _error; // determines if Ok or Err

    public Result(T value)
    {
        _value = value;
        _error = null;
    }

    public Result(IError error)
    {
        _value = default!; // the ! means shut up
        _error = error;
    }

    public bool IsOk => _error == null;
    public bool IsErr => _error != null;

    // checked accessors: return option
    public T? Ok() => IsOk ? _value : default;
    public IError? Err() => _error;

    // the UB special: you get bits, make sure they mean something
    // you may receive {0} so don't fuck around
    public T IntoOk() => _value;
    public IError IntoErr() => _error!; // the ! means shut up

    // i hate this but this has value ig
    public T Unwrap() => IsOk ? _value : throw new BadAccessException("Unwrap on error value");

    public class BadAccessException : Exception
    {
        public BadAccessException(string message) : base(message) { }
    }
}
