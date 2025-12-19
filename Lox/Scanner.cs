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
    string Lexeme,
    object? Literal,
    int Line);

public class Scanner(string source)
{
    private readonly string _source = source;
    private readonly List<Token> _tokens = [];
    private int _start = 0; // start of the current lexeme
    private int _current = 0; // position of the head
    private int _line = 1;

    public List<Token> ScanTokens()
    {
        while (!this.IsEof())
        {
            // new lexeme
            this._start = this._current;
            this.ScanNextToken();
        }

        this.AddToken(TokenType.Eof);
        return this._tokens;
    }

    private void ScanNextToken()
    {
        char c = this.Advance();
        switch (c)
        {
            case '(': this.AddToken(TokenType.LParen); break;
            case ')': this.AddToken(TokenType.RParen); break;
            case '{': this.AddToken(TokenType.LBrace); break;
            case '}': this.AddToken(TokenType.RBrace); break;
            case ',': this.AddToken(TokenType.Comma); break;
            case '.': this.AddToken(TokenType.Dot); break;
            case '-': this.AddToken(TokenType.Minus); break;
            case '+': this.AddToken(TokenType.Plus); break;
            case ';': this.AddToken(TokenType.SemiColon); break;
            case '*': this.AddToken(TokenType.Star); break;
            default:
                Lox.Error(this._line, $"Unexpected token: {c}");
                break;
        }
    }

    private void AddToken(TokenType type, object? literal = null)
    {
        int length = this._current - this._start;
        string lexeme = this._source.Substring(this._start, length);
        this._tokens.Add(new Token(type, lexeme, literal, this._line));
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
