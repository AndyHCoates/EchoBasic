using System;
using System.Collections.Generic;
using System.Text;

namespace EchoBasic
{
    public static class Storage
    {
        private static Dictionary<int, string> _programLines = [];
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
    }
    
}
