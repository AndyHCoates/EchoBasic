using System;
using System.Collections.Generic;
using System.Text;

namespace EchoBasic
{
    public static class Storage
    {
        private static SortedList<int, string> _programLines = [];
        private static Dictionary<string, double> _numberVariables = [];
        
        public static void AddLine(int number, string text)
        {
            if (number <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(number), "Line numbers must be positive.");
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Program line text cannot be null or empty.", nameof(text));
            }
            _programLines[number] = text;
        }

        public static void AddNumeric(string text, double number)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Numeric variable name cannot be null or empty.", nameof(text));
            }

            _numberVariables[text] = number;
        }

        public static string GetLine(int number)
        {
            if (!_programLines.TryGetValue(number, out string? line))
            {
                throw new KeyNotFoundException($"Line number {number} not found.");
            }

            return line;
        }

        public static double GetNumeric(string text)
        {
            if (!_numberVariables.TryGetValue(text, out double value))
            {
                throw new KeyNotFoundException($"Numeric variable '{text}' not found.");
            }

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
