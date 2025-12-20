using System.Collections;
using System.Collections.Generic;

namespace Lox;

public enum TokenType
{
    LParen,
    RParen,
    LBrace,
    RBrace,
    Comma,
    Dot,
    Minus,
    Plus,
    SemiColon,
    Slash,
    Star,

    Bang,
    BangEqual,
    Equal,
    EqualEqual,
    Less,
    LessEqual,

    Identifier,
    String,
    Number,

    And,
    Class,
    Else,
    False,
    Fun,
    For,
    If,
    Nil,
    Or,
    Print,
    Return,
    Super,
    This,
    True,
    Var,
    While,

    Eof,
}

public readonly record struct Token(
    TokenType Type,
    int Start,
    int Length,
    object? Literal,
    int Line);

public class Scanner(string source)
{
    public const int MaxErrors = 100;

    private readonly string _source = source;
    private readonly List<Token> _tokens = [];
    private readonly List<ScannerError> _errors = [];
    private int _start = 0; // start of the current lexeme
    private int _current = 0; // position of the head
    private int _line = 1;

    public Result<List<Token>> ScanTokens()
    {
        while (true)
        {
            var err = TryScanNextToken(out var token);

            if (err is not null)
            {
                this._errors.Add(err.Value);
                if (this._errors.Count >= Scanner.MaxErrors) break;
            }
            else this._tokens.Add(token);

            if (token.Type == TokenType.Eof) break;
        }

        return this._errors.Count > 0
            ? Result.Failure<List<Token>>(new ScannerErrorList(this._errors))
            : Result.Success(this._tokens);
    }

    private ScannerError? TryScanNextToken(out Token token)
    {
        if (!this.IsEof())
        {
            token = this.ExtractToken(TokenType.Eof);
            return null;
        }

        char c = this.Advance();
        switch (c)
        {
            case '(': token = this.ExtractToken(TokenType.LParen); break;
            case ')': token = this.ExtractToken(TokenType.RParen); break;
            case '{': token = this.ExtractToken(TokenType.LBrace); break;
            case '}': token = this.ExtractToken(TokenType.RBrace); break;
            case ',': token = this.ExtractToken(TokenType.Comma); break;
            case '.': token = this.ExtractToken(TokenType.Dot); break;
            case '-': token = this.ExtractToken(TokenType.Minus); break;
            case '+': token = this.ExtractToken(TokenType.Plus); break;
            case ';': token = this.ExtractToken(TokenType.SemiColon); break;
            case '*': token = this.ExtractToken(TokenType.Star); break;
            default:
                token = default;
                return new ScannerError(this._line, ScannerErrorCode.UnexpectedChar, c);
        }

        return null;
    }

    private Token ExtractToken(TokenType type, object? literal = null)
    {
        int length = this._current - this._start;
        return new Token(type, this._start, length, literal, this._line);
    }

    private char Peek()
    {
        if (this.IsEof())
        {
            return '\0';
        }
        else
        {
            return this._source[this._current];
        }
    }

    private char Advance()
    {
        char output = this.Peek();
        if (!this.IsEof())
        {
            this._current++;
        }
        return output;
    }

    private bool IsEof()
    {
        return this._start >= this._source.Length;
    }
};

public enum ScannerErrorCode
{
    UnexpectedChar,
    UnterminatedString,
    InvalidNumber
}

public readonly record struct ScannerError : IError
{
    private readonly int _line;
    private readonly ScannerErrorCode _code;
    private readonly char _data;

    public ScannerError(int line, ScannerErrorCode code, char data = '\0')
    {
        _line = line;
        _code = code;
        _data = data;
    }

    public int Line
    {
        get { return _line; }
    }

    public string What
    {
        get
        {
            switch (_code)
            {
                case ScannerErrorCode.UnexpectedChar:
                    return "Unexpected character: " + _data;
                case ScannerErrorCode.UnterminatedString:
                    return "Unterminated string";
                case ScannerErrorCode.InvalidNumber:
                    return "Invalid number";
                default:
                    return "Unknown Scanner Error";
            }
        }
    }
}

// the lexing might return multiple errors
// i would hate for the compiler to stop at the first error
//
// ScannerError is a record struct so the List should truly be contiguous
public class ScannerErrorList : IError, IReadOnlyList<ScannerError>
{
    private readonly List<ScannerError> _errors;

    public ScannerErrorList(List<ScannerError> errors) => _errors = errors;

    // IError implementation
    public string What => $"{_errors.Count} errors occurred.";
    public int Line => _errors.Count > 0 ? _errors[0].Line : 0;

    // IReadOnlyList implementation
    public int Count => _errors.Count;
    public ScannerError this[int index] => _errors[index];

    public IEnumerator<ScannerError> GetEnumerator() => _errors.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator(); // legacy enumerator
}
