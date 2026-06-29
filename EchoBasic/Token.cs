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
        RightParenthesis
    }

    public class Token(TokenType type)
    {
        public TokenType Type { get; } = type;
        public double Value { get; } = 0.0;

        public Token(TokenType type, double value) : this(type)
        {
            Value = value;
        }

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
            return $"{Type}, {Value}";
        }
    }
}