namespace EchoBasic.Tests
{
    [TestFixture]
    public class RuntimeTests
    {

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void AddTwoNumbers()
        {
            var result = Runtime.Add(2, 3);
            Assert.That(result, Is.EqualTo(5));
        }

        [Test]
        public void SubtractTwoNumbers()
        {
            var result = Runtime.Subtract(5, 2);
            Assert.That(result, Is.EqualTo(3));
        }

        [Test]
        public void MultiplyTwoNumbers()
        {
            var result = Runtime.Multiply(4, 5);
            Assert.That(result, Is.EqualTo(20));
        }

        [Test]
        public void DivideTwoNumbers()
        {
            var result = Runtime.Divide(10, 2);
            Assert.That(result, Is.EqualTo(5));
        }

        [Test]
        public void DivideByZero()
        {
            Assert.Throws<DivideByZeroException>(() => Runtime.Divide(10, 0));
        }
    }

    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void TokeniseLeadingNumberReturnsLineNumberToken()
        {
            var tokens = Parser.Tokenise("10");
            Assert.That(tokens, Has.Count.EqualTo(1));
            Assert.That(tokens[0].Type, Is.EqualTo(TokenType.LineNumber));
            Assert.That(((LineNumberToken)tokens[0]).LineNumber, Is.EqualTo(10));
        }

        [Test]
        public void TokeniseAssignmentExpressionReturnsCorrectTokenSequence()
        {
            var tokens = Parser.Tokenise("x = 5 + 3");
            Assert.That(tokens, Has.Count.EqualTo(5));
            Assert.That(tokens[0].Type, Is.EqualTo(TokenType.Identifier));
            Assert.That(tokens[1].Type, Is.EqualTo(TokenType.Assignment));
            Assert.That(tokens[2].Type, Is.EqualTo(TokenType.Number));
            Assert.That(tokens[3].Type, Is.EqualTo(TokenType.Plus));
            Assert.That(tokens[4].Type, Is.EqualTo(TokenType.Number));
        }

        [Test]
        public void TokeniseValidKeywordReturnsKeywordToken()
        {
            var tokens = Parser.Tokenise("PRINT");
            Assert.That(tokens, Has.Count.EqualTo(1));
            Assert.That(tokens[0].Type, Is.EqualTo(TokenType.Keyword));
            Assert.That(((KeywordToken)tokens[0]).Text, Is.EqualTo("PRINT"));
        }

        [Test]
        public void TokeniseUnaryMinusParsedAsNegativeNumber()
        {
            var tokens = Parser.Tokenise("x = -5");
            Assert.That(tokens, Has.Count.EqualTo(3));
            Assert.That(tokens[2].Type, Is.EqualTo(TokenType.Number));
            Assert.That(((NumberToken)tokens[2]).Value, Is.EqualTo(-5));
        }

        [Test]
        public void TokeniseParenthesesReturnCorrectTokenTypes()
        {
            var tokens = Parser.Tokenise("x = (1 + 2)");
            Assert.That(tokens[2].Type, Is.EqualTo(TokenType.LeftParenthesis));
            Assert.That(tokens[6].Type, Is.EqualTo(TokenType.RightParenthesis));
        }

        [Test]
        public void TokeniseUnknownCharacterThrowsException()
        {
            Assert.Throws<Exception>(() => Parser.Tokenise("x @ 5"));
        }
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

        [Test]
        public void MinusHasSamePrecedenceAsPlus()
        {
            var plus = new OperatorToken(TokenType.Plus);
            var minus = new OperatorToken(TokenType.Minus);
            Assert.That(minus.GetPrecedence(), Is.EqualTo(plus.GetPrecedence()));
        }

        [Test]
        public void DivideHasSamePrecedenceAsMultiply()
        {
            var multiply = new OperatorToken(TokenType.Multiply);
            var divide = new OperatorToken(TokenType.Divide);
            Assert.That(divide.GetPrecedence(), Is.EqualTo(multiply.GetPrecedence()));
        }

        [Test]
        public void AssignmentHasLowerPrecedenceThanPlus()
        {
            var assignment = new OperatorToken(TokenType.Assignment);
            var plus = new OperatorToken(TokenType.Plus);
            Assert.That(assignment.GetPrecedence(), Is.LessThan(plus.GetPrecedence()));
        }

        [Test]
        public void KeywordTokenValidKeywordsIsValidReturnsTrue()
        {
            Assert.That(new KeywordToken("PRINT").IsValid(), Is.True);
            Assert.That(new KeywordToken("LET").IsValid(), Is.True);
        }

        [Test]
        public void KeywordTokenInvalidKeywordIsValidReturnsFalse()
        {
            Assert.That(new KeywordToken("GOTO").IsValid(), Is.False);
        }
    }

    [TestFixture]
    public class AssignmentTests
    {
        [Test]
        public void ImpliedAssignmentTest()
        {
            var tokens = Parser.Tokenise("X = 5", true);
            Runtime.RunLine(tokens);
            Assert.That(Storage.GetNumeric("X"), Is.EqualTo(5));
        }
        
        [Test]
        public void ImpliedAssignmentWithExpression()
        {
            var tokens = Parser.Tokenise("X = 5 + 3", true);
            Runtime.RunLine(tokens);
            Assert.That(Storage.GetNumeric("X"), Is.EqualTo(8));
        }

        [Test]
        public void LetAssignmentWithExpression()
        {
            var tokens = Parser.Tokenise("LET Y = 10 * 2", true);
            Runtime.RunLine(tokens);
            Assert.That(Storage.GetNumeric("Y"), Is.EqualTo(20));
        }

        [Test]
        public void LetAssignmentWithParentheses()
        {
            var tokens = Parser.Tokenise("LET Z = (5 + 3) * 2", true);
            Runtime.RunLine(tokens);
            Assert.That(Storage.GetNumeric("Z"), Is.EqualTo(16));
        }
        
        [Test]
        public void LineNumberStorageWithInvalidLineNumberThrowsException()
        {
            var tokens = Parser.Tokenise("0 LET X = 5", true);
            Assert.Throws<ArgumentOutOfRangeException>(() => Runtime.RunLine(tokens));
        }

        [Test]
        public void LineNumberStorageWithEmptyTextThrowsException()
        {
            var tokens = Parser.Tokenise("10 ", true);
            Assert.Throws<ArgumentException>(() => Runtime.RunLine(tokens));
        }
        
    }

    [TestFixture]
    public class PrintTests
    {
        private StringWriter _output;
        
        [SetUp]
        public void Setup()
        {
            _output = new StringWriter();
            Console.SetOut(_output);
        }

        [TearDown]
        public void TearDown()
        {
            _output.Dispose();
            Console.SetOut(Console.Out);
        }

        [Test]
        public void PrintNumeric()
        {
            var tokens = Parser.Tokenise("print 10", true);
            Runtime.RunLine(tokens);
            var result = _output.ToString().Trim();
            Assert.That(result, Is.EqualTo("10"));
        }

        [Test]
        public void PrintEquation()
        {
            var tokens = Parser.Tokenise("print 12 * 4", true);
            Runtime.RunLine(tokens);
            var result = _output.ToString().Trim();
            Assert.That(result, Is.EqualTo("48"));
        }

        [Test]
        public void PrintVariable()
        {
            Storage.AddNumeric("x", 16.4);
            var tokens = Parser.Tokenise("print x", true);
            Runtime.RunLine(tokens);
            var result = _output.ToString().Trim();
            Assert.That(result, Is.EqualTo("16.4"));
        }

        [Test]
        public void PrintExpressionWithVariable()
        {
            Storage.AddNumeric("x", 3);
            var tokens = Parser.Tokenise("print 8 + x", true);
            Runtime.RunLine(tokens);
            var result = _output.ToString().Trim();
            Assert.That(result, Is.EqualTo("11"));
        }
    }

    [TestFixture]
    public class StorageTests
    {
        [SetUp]
        public void SetUp()
        {
            Storage.Clear();
        }

        [Test]
        public void AddLineValidInputShouldAddLine()
        {
            Storage.AddLine(10, "PRINT 10");
        }

        [Test]
        public void AddLineNegativeLineNumberShouldThrowException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Storage.AddLine(-1, "PRINT 10"));
        }

        [Test]
        public void AddLineEmptyTextShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => Storage.AddLine(10, ""));
        }

        [Test]
        public void AddLineZeroLineNumberShouldThrowException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Storage.AddLine(0, "PRINT 10"));
        }

        [Test]
        public void AddLineWhitespaceOnlyTextShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => Storage.AddLine(10, "   "));
        }

        [Test]
        public void GetLineStoredLineReturnsCorrectText()
        {
            Storage.AddLine(10, "PRINT 10");
            Assert.That(Storage.GetLine(10), Is.EqualTo("PRINT 10"));
        }

        [Test]
        public void GetLineMissingLineThrowsKeyNotFoundException()
        {
            Assert.Throws<KeyNotFoundException>(() => Storage.GetLine(99));
        }

        [Test]
        public void AddNumericAndGetNumericRoundTrip()
        {
            Storage.AddNumeric("x", 3.14);
            Assert.That(Storage.GetNumeric("x"), Is.EqualTo(3.14));
        }

        [Test]
        public void GetNumericMissingVariableThrowsKeyNotFoundException()
        {
            Assert.Throws<KeyNotFoundException>(() => Storage.GetNumeric("missing"));
        }

        [Test]
        public void HasLineExistingLineReturnsTrue()
        {
            Storage.AddLine(10, "PRINT 10");
            Assert.That(Storage.HasLine(10), Is.True);
        }

        [Test]
        public void HasLineMissingLineReturnsFalse()
        {
            Assert.That(Storage.HasLine(99), Is.False);
        }

        [Test]
        public void HasNumericExistingVariableReturnsTrue()
        {
            Storage.AddNumeric("x", 1.0);
            Assert.That(Storage.HasNumeric("x"), Is.True);
        }

        [Test]
        public void HasNumericMissingVariableReturnsFalse()
        {
            Assert.That(Storage.HasNumeric("missing"), Is.False);
        }
    }

    [TestFixture]
    public class ShuntingYardTests
    {
        [Test]
        public void SingleNumberReturnsValue()
        {
            var tokens = new List<Token> { new NumberToken(42) };
            var queue = Runtime.ShuntingYard(tokens);
            Assert.That(Runtime.EvaluatePostFix(queue), Is.EqualTo(42));
        }

        [Test]
        public void SimpleAdditionReturnsCorrectResult()
        {
            // 2 + 3 = 5
            var tokens = new List<Token>
            {
                new NumberToken(2),
                new OperatorToken(TokenType.Plus),
                new NumberToken(3)
            };
            var queue = Runtime.ShuntingYard(tokens);
            Assert.That(Runtime.EvaluatePostFix(queue), Is.EqualTo(5));
        }

        [Test]
        public void MultiplicationPrecedenceAppliedBeforeAddition()
        {
            // 2 + 3 * 4 = 14, not 20
            var tokens = new List<Token>
            {
                new NumberToken(2),
                new OperatorToken(TokenType.Plus),
                new NumberToken(3),
                new OperatorToken(TokenType.Multiply),
                new NumberToken(4)
            };
            var queue = Runtime.ShuntingYard(tokens);
            Assert.That(Runtime.EvaluatePostFix(queue), Is.EqualTo(14));
        }

        [Test]
        public void MixedPrecedenceExpressionAllOperatorsAppliedCorrectly()
        {
            // 10 - 2 * 3 + 4 = 8
            var tokens = new List<Token>
            {
                new NumberToken(10),
                new OperatorToken(TokenType.Minus),
                new NumberToken(2),
                new OperatorToken(TokenType.Multiply),
                new NumberToken(3),
                new OperatorToken(TokenType.Plus),
                new NumberToken(4)
            };
            var queue = Runtime.ShuntingYard(tokens);
            Assert.That(Runtime.EvaluatePostFix(queue), Is.EqualTo(8));
        }

        [Test]
        public void ParenthesesOverrideOperatorPrecedence()
        {
            // (2 + 3) * 4 = 20, not 14
            var tokens = new List<Token>
            {
                new OperatorToken(TokenType.LeftParenthesis),
                new NumberToken(2),
                new OperatorToken(TokenType.Plus),
                new NumberToken(3),
                new OperatorToken(TokenType.RightParenthesis),
                new OperatorToken(TokenType.Multiply),
                new NumberToken(4)
            };
            var queue = Runtime.ShuntingYard(tokens);
            Assert.That(Runtime.EvaluatePostFix(queue), Is.EqualTo(20));
        }

        [Test]
        public void NestedParenthesesEvaluateCorrectly()
        {
            // (2 + (3 * 4)) = 14
            var tokens = new List<Token>
            {
                new OperatorToken(TokenType.LeftParenthesis),
                new NumberToken(2),
                new OperatorToken(TokenType.Plus),
                new OperatorToken(TokenType.LeftParenthesis),
                new NumberToken(3),
                new OperatorToken(TokenType.Multiply),
                new NumberToken(4),
                new OperatorToken(TokenType.RightParenthesis),
                new OperatorToken(TokenType.RightParenthesis)
            };
            var queue = Runtime.ShuntingYard(tokens);
            Assert.That(Runtime.EvaluatePostFix(queue), Is.EqualTo(14));
        }

        [Test]
        public void UnclosedParenthesisThrowsArgumentException()
        {
            var tokens = new List<Token>
            {
                new OperatorToken(TokenType.LeftParenthesis),
                new NumberToken(2),
                new OperatorToken(TokenType.Plus),
                new NumberToken(3)
            };
            Assert.Throws<ArgumentException>(() => Runtime.ShuntingYard(tokens));
        }

        [Test]
        public void ExtraClosingParenthesisThrowsArgumentException()
        {
            var tokens = new List<Token>
            {
                new NumberToken(2),
                new OperatorToken(TokenType.Plus),
                new NumberToken(3),
                new OperatorToken(TokenType.RightParenthesis)
            };
            Assert.Throws<ArgumentException>(() => Runtime.ShuntingYard(tokens));
        }

        [Test]
        public void DivisionByZeroInExpressionThrowsDivideByZeroException()
        {
            var tokens = new List<Token>
            {
                new NumberToken(5),
                new OperatorToken(TokenType.Divide),
                new NumberToken(0)
            };
            var queue = Runtime.ShuntingYard(tokens);
            Assert.Throws<DivideByZeroException>(() => Runtime.EvaluatePostFix(queue));
        }
    }
}
