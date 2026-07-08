using System;
using System.Collections.Generic;

namespace EchoBasic
{
    public static class Runtime
    {
        public static int Add(int a, int b) => a + b;
        public static int Subtract(int a, int b) => a - b;
        public static int Multiply(int a, int b) => a * b;
        public static int Divide(int a, int b)
        {
            if (b == 0)
            {
                throw new BasicException(ErrorCode.DivisionByZero, "Cannot divide by zero.");
            }

            return a / b;
        }

        public static int RunLine(List<Token> opTokens)
        {
            var firstToken = opTokens[0];
            if (firstToken.Type == TokenType.NumericIdentifier)
            {
                ImpliedNumericAssignment(opTokens.Skip(1).ToList(), ((NumericIdentifierToken) firstToken).Name);
            }
            else if (firstToken.Type == TokenType.StringIdentifier)
            {
                var variableName = ((StringIdentifierToken)firstToken).Name;
                ImpliedStringAssignment(opTokens.Skip(1).ToList(), variableName);
            }
            else if (firstToken.Type == TokenType.Keyword)
            {
                var keywordToken = (KeywordToken)firstToken;
                switch (keywordToken.Text.ToUpper())
                {
                    case "LET":
                        if (opTokens[1].Type == TokenType.NumericIdentifier)
                        {
                            var variableName = ((NumericIdentifierToken) opTokens[1]).Name;
                            ImpliedNumericAssignment(opTokens.Skip(2).ToList(), variableName);
                        }
                        else if (opTokens[1].Type == TokenType.StringIdentifier)
                        {
                            var variableName = ((StringIdentifierToken)opTokens[1]).Name;
                            ImpliedStringAssignment(opTokens.Skip(2).ToList(), variableName);
                        }
                        break;
                    case "PRINT":
                        if (opTokens[1].Type == TokenType.StringLiteral)
                        {
                            var text = ((StringLiteralToken)opTokens[1]).Value;
                            Console.WriteLine(text);
                        }
                        else if (opTokens[1].Type == TokenType.StringIdentifier)
                        {
                            var text = Storage.GetString(((StringIdentifierToken)opTokens[1]).Name);
                            Console.WriteLine(text);
                        }
                        else
                        {
                            var shuntQueue = ShuntingYard(opTokens.Skip(1).ToList());
                            var value = EvaluatePostFix(shuntQueue);
                            Console.WriteLine(value);
                        }
                        break;
                    case "GOTO":
                        var gotoShuntQueue = ShuntingYard(opTokens.Skip(1).ToList());
                        var gotoValue = EvaluatePostFix(gotoShuntQueue);
                        return gotoValue;
                        break;
                    default:
                        throw new NotImplementedException($"Unknown keyword: {keywordToken.Text}");
                }
            }
            else if (firstToken.Type == TokenType.LineNumber)
            {
                var lineNum = ((LineNumberToken)firstToken).LineNumber;
                var restOfLine =
                    string.Join(" ",
                        opTokens.Skip(1).Select(t => t.ToString())); // Reconstruct the rest of the line for storage
                Storage.AddLine(lineNum, restOfLine);
            }
            else
            {
                throw new NotImplementedException("Syntax error");
            }

            return 0;
        }
        
        private static void ImpliedNumericAssignment(List<Token> opTokens, string variableName)
        {
            if (opTokens.Count == 0)
            {
                throw new BasicException(ErrorCode.SyntaxError, "Expected an expression after assignment.");
            }
            var secondToken = opTokens[0];
            if (secondToken.Type != TokenType.Assignment)
            {
                throw new BasicException(ErrorCode.SyntaxError, "Expected an =");
            }

            var shuntQueue = ShuntingYard(opTokens.Skip(1).ToList());
            var value = EvaluatePostFix(shuntQueue);
            Storage.AddNumeric(variableName, value);
        }
        
        private static void ImpliedStringAssignment(List<Token> opTokens, string variableName)
        {
            if (opTokens.Count == 0) throw new BasicException(ErrorCode.SyntaxError, "No string found");

            var secondToken = opTokens[0];
            if (secondToken.Type != TokenType.Assignment) throw new BasicException(ErrorCode.SyntaxError, "Expected an =");

            if (opTokens.Count < 2 || opTokens[1].Type != TokenType.StringLiteral) throw new BasicException(ErrorCode.SyntaxError, "Expected a string after the =");

            var stringLiteralToken = (StringLiteralToken)opTokens[1];
            Storage.AddString(variableName, stringLiteralToken.Value);
        }

        public static Queue<Token> ShuntingYard(List<Token> tokens)
        {
            var operatorStack = new Stack<OperatorToken>();
            var outputQueue = new Queue<Token>();
            var parenCount = 0;

            foreach (var t in tokens)
            {
                switch (t.Type)
                {
                    case TokenType.Number:

                    case TokenType.NumericIdentifier:

                    case TokenType.StringIdentifier:
                        outputQueue.Enqueue(t);
                        break;

                    case TokenType.LeftParenthesis:
                        operatorStack.Push((OperatorToken)t);
                        parenCount++;
                        break;

                    case TokenType.RightParenthesis:
                        PopUntilLeftParen(operatorStack, outputQueue, ref parenCount);
                        break;

                    default:
                        PushOperator(operatorStack, outputQueue, (OperatorToken)t);
                        break;
                }
            }

            if (parenCount > 0)
            {
                throw new BasicException(ErrorCode.Parenthesis, "Mismatched parentheses.");
            }

            while (operatorStack.Any())
            {
                outputQueue.Enqueue(operatorStack.Pop());
            }

            return outputQueue;
        }

        private static void PopUntilLeftParen(Stack<OperatorToken> operatorStack, Queue<Token> outputQueue, ref int parenCount)
        {
            if (parenCount == 0)
            {
                throw new BasicException(ErrorCode.Parenthesis, "Mismatched parentheses.");
            }

            while (operatorStack.Any() && operatorStack.Peek().Type != TokenType.LeftParenthesis)
            {
                outputQueue.Enqueue(operatorStack.Pop());
            }

            if (!operatorStack.Any() || operatorStack.Peek().Type != TokenType.LeftParenthesis)
            {
                throw new BasicException(ErrorCode.Parenthesis, "Mismatched parentheses.");
            }

            operatorStack.Pop(); // Discard the left parenthesis
            parenCount--;
        }

        private static void PushOperator(Stack<OperatorToken> operatorStack, Queue<Token> outputQueue, OperatorToken op)
        {
            while (operatorStack.Any() && operatorStack.Peek().Type != TokenType.LeftParenthesis && operatorStack.Peek().GetPrecedence() >= op.GetPrecedence())
            {
                outputQueue.Enqueue(operatorStack.Pop());
            }

            operatorStack.Push(op);
        }
        /// <summary>
        /// Evaluates a postfix expression represented as a queue of tokens. Important: The queue will be modified during evaluation, so it should not be reused after calling this method.
        /// </summary>
        /// <param name="postfixQueue"></param>
        /// <returns></returns>
        public static int EvaluatePostFix(Queue<Token> postfixQueue)
        {
            Stack<NumberToken> operandStack = new();

            while (postfixQueue.Any())
            {
                var t = postfixQueue.Dequeue();
                if (t.Type == TokenType.Number)
                {
                    operandStack.Push((NumberToken)t);
                }
                else if (t.Type == TokenType.NumericIdentifier)
                {
                    var variableName = ((NumericIdentifierToken)t).Name;
                    if (Storage.HasNumeric(variableName))
                    {
                        var value = Storage.GetNumeric(variableName); // Assuming Storage has a GetNumeric method
                        operandStack.Push(new NumberToken(value));
                    }
                    else
                    {
                        throw new KeyNotFoundException($"Numeric variable '{variableName}' not found.");
                    }
                }
                else
                {
                    var first = operandStack.Pop();
                    var second = operandStack.Pop();
                    var result = 0;
                    switch (t.Type)
                    {
                        case TokenType.Plus:
                            result = Add(second.Value, first.Value);
                            break;
                        case TokenType.Minus:
                            result = Subtract(second.Value, first.Value);
                            break;
                        case TokenType.Multiply:
                            result = Multiply(second.Value, first.Value);
                            break;
                        case TokenType.Divide:
                            result = Divide(second.Value, first.Value);
                            break;
                        default:
                            throw new ArgumentException($"Unknown operator: {t.Type}");
                    }

                    operandStack.Push(new NumberToken(result));
                }
            }

            return operandStack.Pop().Value;
        }

        public static void Run(int startLine = 0)
        {
            var lineNumber = Storage.GetNextLine(startLine);

            while (lineNumber != 0)
            {
                var line = Storage.GetLine(lineNumber);
                var tokens = Parser.Tokenise(line); 
                int nextLine;
                try
                {
                    nextLine = RunLine(tokens);
                }
                catch (BasicException ex) when (ex.LineNumber == 0)
                {
                    throw (new BasicException(ex.Code, ex.Message, lineNumber));
                }
                if (nextLine == 0)
                {
                    lineNumber++;
                }
                else
                {
                    lineNumber = nextLine;
                }

                lineNumber = Storage.GetNextLine(lineNumber);
            }
        }

        //public static double Calculate(string input)
        //{
        //    var tokens = Parser.Tokenise(input);
        //    var postfixQueue = ShuntingYard(tokens);
        //    var result = EvaluatePostFix(postfixQueue);

        //    return result;
        //}
    }
}
