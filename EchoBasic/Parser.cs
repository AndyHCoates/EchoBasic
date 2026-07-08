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
                    while (parseStart + parseLength < input.Length && char.IsDigit(input[parseStart + parseLength]))
                    {
                        parseLength++;
                    }

                    var numberString = input.Substring(parseStart, parseLength);
                    if (!tokens.Any())
                    {
                        tokens.Add(new LineNumberToken(Int32.Parse(numberString)));
                        break;
                    }

                    tokens.Add(new NumberToken(int.Parse(numberString)));
                    parseStart += parseLength;
                }
                else if (char.IsLetter(currentChar) || currentChar == '$')
                {
                    var parseLength = 1;
                    while (parseStart + parseLength < input.Length && (char.IsLetter(input[parseStart + parseLength]) || input[parseStart + parseLength] == '$'))
                    {
                        parseLength++;
                    }

                    var statementString = input.Substring(parseStart, parseLength);
                    if (new KeywordToken(statementString).IsValid())
                    {
                        tokens.Add(new KeywordToken(statementString));
                    }
                    else if (statementString.Contains('$'))
                    {
                        if (statementString.EndsWith('$'))
                        {
                            tokens.Add(new StringIdentifierToken(statementString));
                        }
                        else
                        {
                            throw new BasicException(ErrorCode.SyntaxError, "Invalid Numeric Variable");
                        }
                    }
                    else
                    {
                        tokens.Add(new NumericIdentifierToken(statementString));
                    }
                    parseStart += parseLength;
                }
                else if (currentChar == '"')
                {
                    var parseLength = 1;
                    while (parseStart + parseLength < input.Length && input[parseStart + parseLength] != '"')
                    {
                        parseLength++;
                    }
                    if (parseStart + parseLength >= input.Length)
                    {
                        throw new Exception("Unterminated string literal");
                    }
                    var stringLiteral = input.Substring(parseStart + 1, parseLength - 1);
                    tokens.Add(new StringLiteralToken(stringLiteral));
                    parseStart += parseLength + 1; // Skip the closing quote
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
                else if (currentChar == '=')
                {
                    tokens.Add(new OperatorToken(TokenType.Assignment));
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
