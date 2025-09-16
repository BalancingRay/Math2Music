using MathToMusic.Models;

namespace MathToMusic.Utils
{
    public static class NumberConverter
    {
        /// <summary>
        /// Convert a number string from one format to another (supports very long strings)
        /// Currently supports only binary-based formats: Bin, Qad, Oct, Hex
        /// Uses binary as intermediate format for conversions between different bases
        /// </summary>
        /// <param name="input">Input number string</param>
        /// <param name="fromFormat">Source number format</param>
        /// <param name="toFormat">Target number format</param>
        /// <returns>Converted number string</returns>
        public static string Convert(string input, NumberFormats fromFormat, NumberFormats toFormat)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            if (fromFormat == toFormat)
                return input;

            // Validate supported formats
            ValidateSupportedFormat(fromFormat);
            ValidateSupportedFormat(toFormat);

            // Use binary as intermediate format
            string binaryString = fromFormat == NumberFormats.Bin ? input : ConvertToBinary(input, fromFormat);
            
            if (toFormat == NumberFormats.Bin)
                return binaryString;
            
            return ConvertFromBinary(binaryString, toFormat);
        }

        /// <summary>
        /// Validate that the format is supported by the string-based conversion
        /// </summary>
        /// <param name="format">Number format to validate</param>
        /// <exception cref="ArgumentException">Thrown if format is not supported</exception>
        private static void ValidateSupportedFormat(NumberFormats format)
        {
            if (format == NumberFormats.Dec)
            {
                throw new ArgumentException("Decimal format is not supported in current implementation. Use binary-based formats (Bin, Qad, Oct, Hex).");
            }
        }

        /// <summary>
        /// Convert from any supported format to binary string (supports very long strings)
        /// </summary>
        /// <param name="input">Input number string</param>
        /// <param name="fromFormat">Source number format</param>
        /// <returns>Binary string</returns>
        private static string ConvertToBinary(string input, NumberFormats fromFormat)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return fromFormat switch
            {
                NumberFormats.Qad => ConvertQuaternaryToBinary(input),
                NumberFormats.Oct => ConvertOctalToBinary(input),
                NumberFormats.Hex => ConvertHexToBinary(input),
                _ => throw new ArgumentException($"Unsupported conversion from format: {fromFormat}")
            };
        }

        /// <summary>
        /// Convert from binary string to any supported format (supports very long strings)
        /// </summary>
        /// <param name="binaryInput">Binary input string</param>
        /// <param name="toFormat">Target format</param>
        /// <returns>Converted number string</returns>
        private static string ConvertFromBinary(string binaryInput, NumberFormats toFormat)
        {
            if (string.IsNullOrEmpty(binaryInput))
                return string.Empty;

            return toFormat switch
            {
                NumberFormats.Qad => ConvertBinaryToQuaternary(binaryInput),
                NumberFormats.Oct => ConvertBinaryToOctal(binaryInput),
                NumberFormats.Hex => ConvertBinaryToHex(binaryInput),
                _ => throw new ArgumentException($"Unsupported conversion to format: {toFormat}")
            };
        }

        /// <summary>
        /// Convert binary string to target format using grouping (as used in original SingleTrackProcessor)
        /// </summary>
        /// <param name="binaryInput">Binary input string</param>
        /// <param name="targetFormat">Target format</param>
        /// <returns>List of converted character values</returns>
        public static List<char> ConvertBinaryWithGrouping(string binaryInput, NumberFormats targetFormat)
        {
            if (string.IsNullOrEmpty(binaryInput))
                return new List<char>();

            var result = new List<char>();
            int groupSize = GetGroupSizeForFormat(targetFormat);
            string format = targetFormat == NumberFormats.Hex ? "X" : "0";

            // Process from right to left in groups (exactly like original algorithm)
            for (var i = binaryInput.Length - 1; i >= 0; i -= groupSize)
            {
                int convertedValue = 0;
                for (int j = 0; j < groupSize; j++)
                {
                    if (i - j >= 0 && binaryInput[i - j].Equals('1'))
                        convertedValue += (int)Math.Pow(2, j);
                }
                
                string convertedString = convertedValue.ToString(format);
                if (convertedString.Length > 0)
                {
                    // Add at end (not insert at beginning) to match original behavior
                    result.Add(convertedString[0]);
                }
            }

            return result;
        }

        /// <summary>
        /// Get the binary group size needed for conversion to the target format
        /// </summary>
        /// <param name="format">Target format</param>
        /// <returns>Group size in bits</returns>
        private static int GetGroupSizeForFormat(NumberFormats format)
        {
            int groupSize = 1;
            int formatValue = (int)format;
            while (formatValue > 2)
            {
                formatValue /= 2;
                groupSize += 1;
            }
            return groupSize;
        }

        /// <summary>
        /// Convert binary string to quaternary string (supports very long strings)
        /// </summary>
        /// <param name="binaryInput">Binary input string</param>
        /// <returns>Quaternary string</returns>
        private static string ConvertBinaryToQuaternary(string binaryInput)
        {
            if (string.IsNullOrEmpty(binaryInput))
                return string.Empty;

            var result = new System.Text.StringBuilder();
            
            // Process from right to left in groups of 2 bits (since 2^2 = 4 for quaternary)
            for (var i = binaryInput.Length - 1; i >= 0; i -= 2)
            {
                int value = 0;
                // Process up to 2 bits
                for (int j = 0; j < 2 && (i - j) >= 0; j++)
                {
                    if (binaryInput[i - j] == '1')
                        value += (int)Math.Pow(2, j);
                }
                result.Insert(0, value.ToString());
            }
            
            return result.ToString();
        }

        /// <summary>
        /// Convert binary string to octal string (supports very long strings) 
        /// </summary>
        /// <param name="binaryInput">Binary input string</param>
        /// <returns>Octal string</returns>
        private static string ConvertBinaryToOctal(string binaryInput)
        {
            if (string.IsNullOrEmpty(binaryInput))
                return string.Empty;

            var result = new System.Text.StringBuilder();
            
            // Process from right to left in groups of 3 bits (since 2^3 = 8 for octal)
            for (var i = binaryInput.Length - 1; i >= 0; i -= 3)
            {
                int value = 0;
                // Process up to 3 bits
                for (int j = 0; j < 3 && (i - j) >= 0; j++)
                {
                    if (binaryInput[i - j] == '1')
                        value += (int)Math.Pow(2, j);
                }
                result.Insert(0, value.ToString());
            }
            
            return result.ToString();
        }

        /// <summary>
        /// Convert binary string to hexadecimal string (supports very long strings)
        /// </summary>
        /// <param name="binaryInput">Binary input string</param>
        /// <returns>Hexadecimal string</returns>
        private static string ConvertBinaryToHex(string binaryInput)
        {
            if (string.IsNullOrEmpty(binaryInput))
                return string.Empty;

            var result = new System.Text.StringBuilder();
            
            // Process from right to left in groups of 4 bits (since 2^4 = 16 for hex)
            for (var i = binaryInput.Length - 1; i >= 0; i -= 4)
            {
                int value = 0;
                // Process up to 4 bits
                for (int j = 0; j < 4 && (i - j) >= 0; j++)
                {
                    if (binaryInput[i - j] == '1')
                        value += (int)Math.Pow(2, j);
                }
                result.Insert(0, value.ToString("X"));
            }
            
            return result.ToString();
        }

        /// <summary>
        /// Convert quaternary string to binary string (supports very long strings)
        /// </summary>
        /// <param name="quaternaryInput">Quaternary input string</param>
        /// <returns>Binary string</returns>
        private static string ConvertQuaternaryToBinary(string quaternaryInput)
        {
            if (string.IsNullOrEmpty(quaternaryInput))
                return string.Empty;

            var result = new System.Text.StringBuilder();
            
            foreach (char digit in quaternaryInput)
            {
                int digitValue = digit - '0';
                if (digitValue < 0 || digitValue > 3)
                    throw new ArgumentException($"Invalid quaternary digit: {digit}");
                
                // Convert each quaternary digit to 2 binary digits
                string binaryDigits = System.Convert.ToString(digitValue, 2).PadLeft(2, '0');
                result.Append(binaryDigits);
            }
            
            // Remove leading zeros but keep at least one digit
            string binaryResult = result.ToString().TrimStart('0');
            return string.IsNullOrEmpty(binaryResult) ? "0" : binaryResult;
        }

        /// <summary>
        /// Convert octal string to binary string (supports very long strings)
        /// </summary>
        /// <param name="octalInput">Octal input string</param>
        /// <returns>Binary string</returns>
        private static string ConvertOctalToBinary(string octalInput)
        {
            if (string.IsNullOrEmpty(octalInput))
                return string.Empty;

            var result = new System.Text.StringBuilder();
            
            foreach (char digit in octalInput)
            {
                int digitValue = digit - '0';
                if (digitValue < 0 || digitValue > 7)
                    throw new ArgumentException($"Invalid octal digit: {digit}");
                
                // Convert each octal digit to 3 binary digits
                string binaryDigits = System.Convert.ToString(digitValue, 2).PadLeft(3, '0');
                result.Append(binaryDigits);
            }
            
            // Remove leading zeros but keep at least one digit
            string binaryResult = result.ToString().TrimStart('0');
            return string.IsNullOrEmpty(binaryResult) ? "0" : binaryResult;
        }

        /// <summary>
        /// Convert hexadecimal string to binary string (supports very long strings)
        /// </summary>
        /// <param name="hexInput">Hexadecimal input string</param>
        /// <returns>Binary string</returns>
        private static string ConvertHexToBinary(string hexInput)
        {
            if (string.IsNullOrEmpty(hexInput))
                return string.Empty;

            var result = new System.Text.StringBuilder();
            
            foreach (char digit in hexInput)
            {
                int digitValue;
                if (char.IsDigit(digit))
                {
                    digitValue = digit - '0';
                }
                else if (char.ToUpper(digit) >= 'A' && char.ToUpper(digit) <= 'F')
                {
                    digitValue = char.ToUpper(digit) - 'A' + 10;
                }
                else
                {
                    throw new ArgumentException($"Invalid hexadecimal digit: {digit}");
                }
                
                // Convert each hex digit to 4 binary digits
                string binaryDigits = System.Convert.ToString(digitValue, 2).PadLeft(4, '0');
                result.Append(binaryDigits);
            }
            
            // Remove leading zeros but keep at least one digit
            string binaryResult = result.ToString().TrimStart('0');
            return string.IsNullOrEmpty(binaryResult) ? "0" : binaryResult;
        }

        /// <summary>
        /// Convert from any supported format to decimal (LEGACY - for backward compatibility only)
        /// For new code, use Convert method with binary as intermediate format
        /// </summary>
        /// <param name="input">Input number string</param>
        /// <param name="fromFormat">Source number format</param>
        /// <returns>Decimal value</returns>
        [Obsolete("This method is kept for backward compatibility. Use Convert method for new code.")]
        public static long ConvertToDecimal(string input, NumberFormats fromFormat)
        {
            if (string.IsNullOrEmpty(input))
                return 0;

            return fromFormat switch
            {
                NumberFormats.Bin => System.Convert.ToInt64(input, 2),
                NumberFormats.Oct => System.Convert.ToInt64(input, 8),
                NumberFormats.Dec => System.Convert.ToInt64(input, 10),
                NumberFormats.Hex => System.Convert.ToInt64(input, 16),
                NumberFormats.Qad => ConvertQuaternaryToDecimalLegacy(input),
                _ => throw new ArgumentException($"Unsupported format: {fromFormat}")
            };
        }

        /// <summary>
        /// Convert from decimal to any supported format (LEGACY - for backward compatibility only)
        /// For new code, use Convert method with binary as intermediate format
        /// </summary>
        /// <param name="decimalValue">Decimal value to convert</param>
        /// <param name="toFormat">Target number format</param>
        /// <returns>Converted number string</returns>
        [Obsolete("This method is kept for backward compatibility. Use Convert method for new code.")]
        public static string ConvertFromDecimal(long decimalValue, NumberFormats toFormat)
        {
            return toFormat switch
            {
                NumberFormats.Bin => System.Convert.ToString(decimalValue, 2),
                NumberFormats.Oct => System.Convert.ToString(decimalValue, 8),
                NumberFormats.Dec => decimalValue.ToString(),
                NumberFormats.Hex => System.Convert.ToString(decimalValue, 16).ToUpper(),
                NumberFormats.Qad => ConvertDecimalToQuaternaryLegacy(decimalValue),
                _ => throw new ArgumentException($"Unsupported format: {toFormat}")
            };
        }

        /// <summary>
        /// Convert quaternary string to decimal (LEGACY - for backward compatibility only)
        /// </summary>
        /// <param name="quaternaryInput">Quaternary input string</param>
        /// <returns>Decimal value</returns>
        private static long ConvertQuaternaryToDecimalLegacy(string quaternaryInput)
        {
            long result = 0;
            long baseValue = 1;

            for (int i = quaternaryInput.Length - 1; i >= 0; i--)
            {
                int digit = quaternaryInput[i] - '0';
                if (digit < 0 || digit > 3)
                    throw new ArgumentException($"Invalid quaternary digit: {quaternaryInput[i]}");
                
                result += digit * baseValue;
                baseValue *= 4;
            }

            return result;
        }

        /// <summary>
        /// Convert decimal to quaternary string (LEGACY - for backward compatibility only)
        /// </summary>
        /// <param name="decimalValue">Decimal value to convert</param>
        /// <returns>Quaternary string</returns>
        private static string ConvertDecimalToQuaternaryLegacy(long decimalValue)
        {
            if (decimalValue == 0)
                return "0";

            var result = new System.Text.StringBuilder();
            while (decimalValue > 0)
            {
                result.Insert(0, (decimalValue % 4).ToString());
                decimalValue /= 4;
            }

            return result.ToString();
        }
    }
}