using System;
using System.Collections.Generic;
using System.Text;

namespace EchoBasic
{
    public class Runtime
    {
        public double Add(double a, double b) => a + b;
        public double Subtract(double a, double b) => a - b;
        public double Multiply(double a, double b) => a * b;
        public double Divide(double a, double b)
        {
            if (b == 0)
            {
                throw new DivideByZeroException("Cannot divide by zero.");
            }

            return a / b;
        }

        public Queue<Token> ShuntingYard(List<Token> tokens)
        {
            var operatorStack = new Stack<OperatorToken>();
            var outputQueue = new Queue<Token>();
            var parenCount = 0;

            foreach (var t in tokens)
            {
                switch (t.Type)
                {
                    case TokenType.Number:
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
                throw new ArgumentException("Mismatched parentheses.");
            }

            while (operatorStack.Any() && operatorStack.Peek().Type != TokenType.LeftParenthesis)
            {
                outputQueue.Enqueue(operatorStack.Pop());
            }

            if (!operatorStack.Any() || operatorStack.Peek().Type != TokenType.LeftParenthesis)
            {
                throw new ArgumentException("Mismatched parentheses.");
            }

            operatorStack.Pop(); // Discard the left parenthesis
            parenCount--;
        }

        private static void PushOperator(Stack<OperatorToken> operatorStack, Queue<Token> outputQueue, OperatorToken op)
        {
            if (operatorStack.Any() && operatorStack.Peek().Type != TokenType.LeftParenthesis && operatorStack.Peek().GetPrecedence() >= op.GetPrecedence())
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
        public double EvaluatePostFix(Queue<Token> postfixQueue)
        {
            Stack<NumberToken> operandStack = new();

            while (postfixQueue.Any())
            {
                var t = postfixQueue.Dequeue();
                if (t.Type == TokenType.Number)
                {
                    operandStack.Push((NumberToken)t);
                }
                else
                {
                    var first = operandStack.Pop();
                    var second = operandStack.Pop();
                    var result = 0.0;
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

        public double Calculate(string input)
        {
            var tokens = Parser.Tokenise(input);
            var postfixQueue = ShuntingYard(tokens);
            var result = EvaluatePostFix(postfixQueue);

            return result;
        }
    }
}
