using System;
using System.Collections.Generic;
using System.Linq;

namespace MathToMusic
{
    /// <summary>
    /// Parser for polyphonic expressions using + operator (e.g., "abc+def")
    /// </summary>
    public static class ExpressionParser
    {
        /// <summary>
        /// Parse an expression with + operator into individual parts
        /// </summary>
        /// <param name="expression">Expression like "abc+def" or "123+456"</param>
        /// <returns>Array of individual numeric sequences</returns>
        public static string[] ParseExpression(string expression)
        {
            if (string.IsNullOrEmpty(expression))
                return Array.Empty<string>();

            // Split on + operator and trim whitespace
            var parts = expression.Split('+', StringSplitOptions.RemoveEmptyEntries)
                                 .Select(part => part.Trim())
                                 .Where(part => !string.IsNullOrEmpty(part))
                                 .ToArray();

            return parts;
        }

        /// <summary>
        /// Check if expression contains polyphonic notation (+ operator)
        /// </summary>
        /// <param name="expression">Expression to check</param>
        /// <returns>True if expression contains + operator</returns>
        public static bool IsPolyphonic(string expression)
        {
            return !string.IsNullOrEmpty(expression) && expression.Contains('+');
        }
    }
}