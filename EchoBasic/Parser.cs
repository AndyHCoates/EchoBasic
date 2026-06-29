using System;
using System.Collections.Generic;
using System.Text;

namespace EchoBasic
{
    public static class Parser
    {
        public static List<Token> Tokenise(string input)
        {
            var tokens = new List<Token>();

            var parseStart = 0;

            while (parseStart < input.Length)
            {
                var currentChar = input[parseStart];
                var nextChar = input[parseStart + 1 < input.Length ? parseStart + 1 : parseStart];

                var isUnaryMinus = currentChar == '-' && char.IsDigit(nextChar)
                    && (tokens.Count == 0
                        || (tokens[^1].Type != TokenType.Number && tokens[^1].Type != TokenType.RightParenthesis));

                if (char.IsDigit(currentChar) || isUnaryMinus)
                {
                    var parseLength = 1;
                    while (parseStart + parseLength < input.Length && (char.IsDigit(input[parseStart + parseLength]) || input[parseStart + parseLength] == '.'))
                    {
                        parseLength++;
                    }

                    var numberString = input.Substring(parseStart, parseLength);
                    tokens.Add(new NumberToken(double.Parse(numberString)));
                    parseStart += parseLength;
                }
                else if (currentChar == '+')
                {
                    tokens.Add(new Token(TokenType.Plus));
                    parseStart++;
                }
                else if (currentChar == '-')
                {
                    tokens.Add(new Token(TokenType.Minus));
                    parseStart++;
                }
                else if (currentChar == '*')
                {
                    tokens.Add(new Token(TokenType.Multiply));
                    parseStart++;
                }
                else if (currentChar == '/')
                {
                    tokens.Add(new Token(TokenType.Divide));
                    parseStart++;
                }
                else if (currentChar == '(')
                {
                    tokens.Add(new Token(TokenType.LeftParenthesis));
                    parseStart++;
                }
                else if (currentChar == ')')
                {
                    tokens.Add(new Token(TokenType.RightParenthesis));
                    parseStart++;
                }
                else if (char.IsWhiteSpace(currentChar))
                {
                    parseStart++;
                }
                else
                {
                    throw new Exception($"Unknown character: {currentChar}");
                }
            }

            return tokens;
        }
    }
}
