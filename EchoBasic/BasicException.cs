using System;
using System.Collections.Generic;
using System.Text;

namespace EchoBasic
{
    public class BasicException(ErrorCode code, string message, int lineNumber = 0) : Exception(message)
    {
        public ErrorCode Code { get; } = code;
        public int LineNumber { get; } = lineNumber;
    }

    public enum ErrorCode
    {
        SyntaxError,
        DivisionByZero,
        UndefinedVariable,
        TypeMismatch,
        LineNumber,
        EmptyLine,
        Parenthesis,
    }
}
