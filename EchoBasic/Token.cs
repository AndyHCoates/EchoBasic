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
        Assignment,
        Keyword,
        NumericIdentifier,
        StringIdentifier,
        LineNumber,
        StringLiteral
    }

    public class Token(TokenType type)
    {
        public TokenType Type { get; } = type;

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
        private string[] _validKeywords = ["PRINT", "LET", "GOTO"];

        public string Text { get; } = text;

        public override string ToString()
        {
            return $"{Type}, {Text}";
        }
        
        public bool IsValid()
        {
            return _validKeywords.Contains(Text.ToUpper());
        }
    }

    public sealed class OperatorToken(TokenType type) : Token(type)
    {
        public int GetPrecedence()
        {
            return Type switch
            {
                TokenType.LeftParenthesis => 100,
                TokenType.Multiply or TokenType.Divide => 20,
                TokenType.Plus or TokenType.Minus => 10,
                TokenType.Assignment => 1,
                _ => 0
            };
        }
    }
    
    public sealed class NumericIdentifierToken(string name) : Token(TokenType.NumericIdentifier)
    {
        public string Name { get; } = name;
        public override string ToString()
        {
            return $"{Type}, {Name}";
        }
    }

    public sealed class StringIdentifierToken(string name) : Token(TokenType.StringIdentifier)
    {
        public string Name { get; } = name;
        public override string ToString()
        {
            return $"{Type}, {Name}";
        }
    }

    public sealed class LineNumberToken(int lineNumber) : Token(TokenType.LineNumber)
    {
        public int LineNumber { get; } = lineNumber;
        public override string ToString()
        {
            return $"{Type}, {LineNumber}";
        }
    }
    
    public sealed class StringLiteralToken(string value) : Token(TokenType.StringLiteral)
    {
        public string Value { get; } = value;
        public override string ToString()
        {
            return $"{Type}, \"{Value}\"";
        }
    }
    
}