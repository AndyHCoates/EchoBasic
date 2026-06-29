using System;
using System.Collections.Generic;
using System.Text;

namespace EchoBasic
{

    public enum TokenType
    {
        Number,
        Plus,
        Minus,
        Multiply,
        Divide,
        LeftParenthesis,
        RightParenthesis,
        Keyword,
    }

    public class Token(TokenType type)
    {
        public TokenType Type { get; } = type;

        public int GetPrecedence()
        {
            return Type switch
            {
                TokenType.LeftParenthesis => 100,
                TokenType.Multiply or TokenType.Divide => 20,
                TokenType.Plus or TokenType.Minus => 10,
                _ => 0
            };
        }

        public override string ToString()
        {
            return $"{Type}";
        }
    }

    public sealed class NumberToken(double value) : Token(TokenType.Number)
    {
        public double Value { get; } = value;

        public override string ToString()
        {
            return $"{Type}, {Value}";
        }
    }

    public sealed class KeywordToken(string text) : Token(TokenType.Keyword)
    {
        public string Text { get; } = text;

        public override string ToString()
        {
            return $"{Type}, {Text}";
        }
    }
}