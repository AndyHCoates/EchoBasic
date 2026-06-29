namespace EchoBasic.Tests
{
    [TestFixture]
    public class CalculatorTests
    {
        private Calculator _calc;

        [SetUp]
        public void Setup()
        {
            _calc = new Calculator();
        }

        [Test]
        public void AddTwoNumbers()
        {
            var result = _calc.Add(2, 3);
            Assert.That(result, Is.EqualTo(5));
        }

        [Test]
        public void SubtractTwoNumbers()
        {
            var result = _calc.Subtract(5, 2);
            Assert.That(result, Is.EqualTo(3));
        }

        [Test]
        public void MultiplyTwoNumbers()
        {
            var result = _calc.Multiply(4, 5);
            Assert.That(result, Is.EqualTo(20));
        }

        [Test]
        public void DivideTwoNumbers()
        {
            var result = _calc.Divide(10, 2);
            Assert.That(result, Is.EqualTo(5));
        }

        [Test]
        public void DivideByZero()
        {
            Assert.Throws<DivideByZeroException>(() => _calc.Divide(10, 0));
        }

        [Test]
        public void SimpleCalculation()
        {
            var result = _calc.Calculate("3 + 4");
            Assert.That(result, Is.EqualTo(7));
        }
        [Test]
        public void SimpleCalculationWithMultiplication()
        {
            var result = _calc.Calculate("3 * 4");
            Assert.That(result, Is.EqualTo(12));
        }

        [Test]
        public void SimpleCalculationWithDivision()
        {
            var result = _calc.Calculate("10 / 2");
            Assert.That(result, Is.EqualTo(5));
        }

        [Test]
        public void CalculationWithSubtractionNoSpaces()
        {
            var result = _calc.Calculate("-1-1");
            Assert.That(result, Is.EqualTo(-2));
        }

        [Test]
        public void CalculationWithPrecedenceNoParenthesesAddSub()
        {
            var result = _calc.Calculate("2 - 5 + 4");
            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void CalculationWithPrecedenceNoParenthesesMultiply()
        {
            var result = _calc.Calculate("2 + 5 * 4");
            Assert.That(result, Is.EqualTo(22));
        }
        [Test]
        public void CalculationWithPrecedenceNoParenthesesDivide()
        {
            var result = _calc.Calculate("10 / 2 + 3");
            Assert.That(result, Is.EqualTo(8));
        }

        [Test]
        public void CalculationWithParentheses()
        {
            var result = _calc.Calculate("(3 + 4) * 2");
            Assert.That(result, Is.EqualTo(14));
        }

        [Test]
        public void CalculationWithNestedParentheses()
        {
            var result = _calc.Calculate("((3 + 4) * 2) / 7");
            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public void CalculationWithComplexExpression()
        {
            var result = _calc.Calculate("5 + 3 * (10 - 6) / 2");
            Assert.That(result, Is.EqualTo(11));
        }

        [Test]
        public void CalculationWithFloatingPointNumbers()
        {
            var result = _calc.Calculate("3.14 + 2.71 * 2");
            Assert.That(result, Is.EqualTo(8.56));
        }

        [Test]
        public void CalculationWithNegativeNumbersAndParentheses()
        {
            var result = _calc.Calculate("-5 * (2 + -3)");
            Assert.That(result, Is.EqualTo(5));
        }

        [Test]
        public void CalculationWithDivisionAndFloatingPointResult()
        {
            var result = _calc.Calculate("7 / 2");
            Assert.That(result, Is.EqualTo(3.5));
        }
    }

    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void TokeniseSimpleExpression()
        {
            var input = "3.14 + 4";
            var tokens = Parser.Tokenise(input);
            Assert.That(tokens.Count, Is.EqualTo(3));
            Assert.That(tokens[0].Type, Is.EqualTo(TokenType.Number));
            Assert.That(((NumberToken)tokens[0]).Value, Is.EqualTo(3.14));
            Assert.That(tokens[1].Type, Is.EqualTo(TokenType.Plus));
            Assert.That(tokens[2].Type, Is.EqualTo(TokenType.Number));
            Assert.That(((NumberToken)tokens[2]).Value, Is.EqualTo(4));
        }

        [Test]
        public void TokeniseComplexExpression()
        {
            var input = "(3 + 4) * 2";
            var tokens = Parser.Tokenise(input);
            Assert.That(tokens.Count, Is.EqualTo(7));
            Assert.That(tokens[0].Type, Is.EqualTo(TokenType.LeftParenthesis));
            Assert.That(tokens[1].Type, Is.EqualTo(TokenType.Number));
            Assert.That(((NumberToken)tokens[1]).Value, Is.EqualTo(3));
            Assert.That(tokens[2].Type, Is.EqualTo(TokenType.Plus));
            Assert.That(tokens[3].Type, Is.EqualTo(TokenType.Number));
            Assert.That(((NumberToken)tokens[3]).Value, Is.EqualTo(4));
            Assert.That(tokens[4].Type, Is.EqualTo(TokenType.RightParenthesis));
            Assert.That(tokens[5].Type, Is.EqualTo(TokenType.Multiply));
            Assert.That(tokens[6].Type, Is.EqualTo(TokenType.Number));
            Assert.That(((NumberToken)tokens[6]).Value, Is.EqualTo(2));
        }

        [Test]
        public void TokeniseUnaryMinus()
        {
            var input = "-5 + 10";
            var tokens = Parser.Tokenise(input);
            Assert.That(tokens.Count, Is.EqualTo(3));
            Assert.That(tokens[0].Type, Is.EqualTo(TokenType.Number));
            Assert.That(((NumberToken)tokens[0]).Value, Is.EqualTo(-5));
            Assert.That(tokens[1].Type, Is.EqualTo(TokenType.Plus));
            Assert.That(tokens[2].Type, Is.EqualTo(TokenType.Number));
            Assert.That(((NumberToken)tokens[2]).Value, Is.EqualTo(10));
        }

        [Test]
        public void TokeniseUnaryMinusAfterLeftParenthesis()
        {
            var input = "1+(-2*3)";
            var tokens = Parser.Tokenise(input);
            Assert.That(tokens.Count, Is.EqualTo(7));
            Assert.That(tokens[0].Type, Is.EqualTo(TokenType.Number));
            Assert.That(((NumberToken)tokens[0]).Value, Is.EqualTo(1));
            Assert.That(tokens[1].Type, Is.EqualTo(TokenType.Plus));
            Assert.That(tokens[2].Type, Is.EqualTo(TokenType.LeftParenthesis));
            Assert.That(tokens[3].Type, Is.EqualTo(TokenType.Number));
            Assert.That(((NumberToken)tokens[3]).Value, Is.EqualTo(-2));
            Assert.That(tokens[4].Type, Is.EqualTo(TokenType.Multiply));
            Assert.That(tokens[5].Type, Is.EqualTo(TokenType.Number));
            Assert.That(((NumberToken)tokens[5]).Value, Is.EqualTo(3));
            Assert.That(tokens[6].Type, Is.EqualTo(TokenType.RightParenthesis));
        }

        [Test]
        public void TokeniseSubtractionNoSpaces()
        {
            var input = "-1-1";
            var tokens = Parser.Tokenise(input);
            Assert.That(tokens.Count, Is.EqualTo(3));
            Assert.That(tokens[0].Type, Is.EqualTo(TokenType.Number));
            Assert.That(((NumberToken)tokens[0]).Value, Is.EqualTo(-1));
            Assert.That(tokens[1].Type, Is.EqualTo(TokenType.Minus));
            Assert.That(tokens[2].Type, Is.EqualTo(TokenType.Number));
            Assert.That(((NumberToken)tokens[2]).Value, Is.EqualTo(1));
        }
    }

    [TestFixture]
    public class TokenTests
    {
        [Test]
        public void TokenPrecedence()
        {
            var plusToken = new Token(TokenType.Plus);
            var multiplyToken = new Token(TokenType.Multiply);
            var leftParenthesisToken = new Token(TokenType.LeftParenthesis);
            Assert.That(multiplyToken.GetPrecedence(), Is.GreaterThan(plusToken.GetPrecedence()));
            Assert.That(leftParenthesisToken.GetPrecedence(), Is.GreaterThan(multiplyToken.GetPrecedence()));
        }
    }
}
