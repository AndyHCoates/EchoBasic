using System;
using System.Collections.Generic;
using System.Text;

namespace EchoBasic
{
    public static class Parser
    {
        public static List<Token> Tokenise(string input, bool immediate = false)
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
                    if (!tokens.Any())
                    {
                        tokens.Add(new LineNumberToken(Int32.Parse(numberString)));
                        break;
                    }

                    tokens.Add(new NumberToken(double.Parse(numberString)));
                    parseStart += parseLength;
                }
                if (char.IsLetter(currentChar))
                {
                    var parseLength = 1;
                    while (parseStart + parseLength < input.Length && (char.IsLetter(input[parseStart + parseLength])))
                    {
                        parseLength++;
                    }

                    var statementString = input.Substring(parseStart, parseLength);
                    if (new KeywordToken(statementString).IsValid())
                    {
                        tokens.Add(new KeywordToken(statementString));
                    }
                    else
                    {
                        tokens.Add(new IdentifierToken(statementString));
                    }
                    parseStart += parseLength;
                }
                else if (currentChar == '+')
                {
                    tokens.Add(new OperatorToken(TokenType.Plus));
                    parseStart++;
                }
                else if (currentChar == '-')
                {
                    tokens.Add(new OperatorToken(TokenType.Minus));
                    parseStart++;
                }
                else if (currentChar == '*')
                {
                    tokens.Add(new OperatorToken(TokenType.Multiply));
                    parseStart++;
                }
                else if (currentChar == '/')
                {
                    tokens.Add(new OperatorToken(TokenType.Divide));
                    parseStart++;
                }
                else if (currentChar == '(')
                {
                    tokens.Add(new OperatorToken(TokenType.LeftParenthesis));
                    parseStart++;
                }
                else if (currentChar == ')')
                {
                    tokens.Add(new OperatorToken(TokenType.RightParenthesis));
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
