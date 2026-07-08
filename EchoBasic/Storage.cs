using System;
using System.Collections.Generic;
using System.Text;

namespace EchoBasic
{
    public static class Storage
    {
        private static SortedList<int, string> _programLines = [];
        private static Dictionary<string, int> _numberVariables = [];
        private static Dictionary<string, string> _stringVariables = [];
        
        public static void AddLine(int number, string text)
        {
            if (number <= 0)
            {
                throw new BasicException(ErrorCode.LineNumber, "Line numbers must be positive.", number);
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                throw new BasicException(ErrorCode.EmptyLine, "Program line text cannot be empty.", number);
            }
            _programLines[number] = text;
        }

        public static void AddNumeric(string text, int number)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new BasicException(ErrorCode.SyntaxError, "Numeric variable name cannot be null or empty");
            }

            _numberVariables[text] = number;
        }
        
        public static void AddString(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new BasicException(ErrorCode.SyntaxError, "String variable name cannot be null or empty");
            }
            _stringVariables[name] = value;
        }

        public static string GetLine(int number)
        {
            if (!_programLines.TryGetValue(number, out string? line))
            {
                throw new KeyNotFoundException($"Line number {number} not found.");
            }

            return line;
        }

        public static int GetNumeric(string text)
        {
            if (!_numberVariables.TryGetValue(text, out int value))
            {
                throw new KeyNotFoundException($"Numeric variable '{text}' not found.");
            }

            return value;
        }
        
        public static string GetString(string name)
        {
            if (!_stringVariables.TryGetValue(name, out var value)) throw new KeyNotFoundException($"String variable '{name}' not found.");

            return value;
        }

        public static bool HasLine(int number)
        {
            return _programLines.ContainsKey(number);
        }

        public static bool HasNumeric(string text)
        {
            return _numberVariables.ContainsKey(text);
        }

        public static void Clear()
        {
            _programLines.Clear();
            _numberVariables.Clear();
            _stringVariables.Clear();
        }
        
        public static int GetLastLine()
        {
            if (_programLines.Count == 0)
            {
                return 0;
            }
            return _programLines.Keys[^1]; // Get the last key in the sorted list
        }

        public static int GetNextLine(int searchLine)
        {
            return _programLines.Keys.FirstOrDefault(lineNumber => lineNumber >= searchLine);
        }

        public static void ListProgram()
        {
            foreach (var line in _programLines) Console.WriteLine($"{line.Key} {line.Value}");
        }
    }
    
}
