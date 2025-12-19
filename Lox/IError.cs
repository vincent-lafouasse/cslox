namespace Lox;

public interface IError
{
    int Line { get; }
    string What { get; }
}
