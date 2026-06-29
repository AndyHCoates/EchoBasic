namespace EchoBasic.Tests
{
    [TestFixture]
    public class RuntimeTests
    {
        private Runtime _calc;

        [SetUp]
        public void Setup()
        {
            _calc = new Runtime();
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
    }

    [TestFixture]
    public class ParserTests
    {
    }

    [TestFixture]
    public class TokenTests
    {
        [Test]
        public void TokenPrecedence()
        {
            var plusToken = new OperatorToken(TokenType.Plus);
            var multiplyToken = new OperatorToken(TokenType.Multiply);
            var leftParenthesisToken = new OperatorToken(TokenType.LeftParenthesis);
            Assert.That(multiplyToken.GetPrecedence(), Is.GreaterThan(plusToken.GetPrecedence()));
            Assert.That(leftParenthesisToken.GetPrecedence(), Is.GreaterThan(multiplyToken.GetPrecedence()));
        }
    }
}
