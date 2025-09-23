using MathToMusic.Models;
using System.Numerics;

namespace MathToMusic.Utils
{
    public static class NumberConverter
    {
        /// <summary>
        /// Convert a number string from one format to another (supports very long strings)
        /// Supports all number formats: Bin, Qad, Oct, Dec, Hex
        /// Uses binary as intermediate format for conversions between non-decimal bases
        /// Uses direct conversion for decimal format conversions
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

            // Handle conversions involving decimal format using ConvertLarge method
            if (fromFormat == NumberFormats.Dec || toFormat == NumberFormats.Dec)
            {
                return ConvertWithDecimal(input, fromFormat, toFormat);
            }

            // For binary-based formats, use binary as intermediate format
            string binaryString = fromFormat == NumberFormats.Bin ? input : ConvertToBinary(input, fromFormat);
            
            if (toFormat == NumberFormats.Bin)
                return binaryString;
            
            return ConvertFromBinary(binaryString, toFormat);
        }

        /// <summary>
        /// Handle conversions involving decimal format
        /// </summary>
        /// <param name="input">Input number string</param>
        /// <param name="fromFormat">Source number format</param>
        /// <param name="toFormat">Target number format</param>
        /// <returns>Converted number string</returns>
        private static string ConvertWithDecimal(string input, NumberFormats fromFormat, NumberFormats toFormat)
        {
            // Direct Dec <-> Bin conversion using ConvertLarge
            if ((fromFormat == NumberFormats.Dec && toFormat == NumberFormats.Bin) ||
                (fromFormat == NumberFormats.Bin && toFormat == NumberFormats.Dec))
            {
                return ConvertLarge(input, fromFormat, toFormat);
            }

            // For Dec -> other formats: Dec -> Bin -> target format
            if (fromFormat == NumberFormats.Dec)
            {
                string binaryIntermediate = ConvertLarge(input, NumberFormats.Dec, NumberFormats.Bin);
                if (toFormat == NumberFormats.Bin)
                    return binaryIntermediate;
                return ConvertFromBinary(binaryIntermediate, toFormat);
            }

            // For other formats -> Dec: source format -> Bin -> Dec
            if (toFormat == NumberFormats.Dec)
            {
                string binaryIntermediate = fromFormat == NumberFormats.Bin ? input : ConvertToBinary(input, fromFormat);
                return ConvertLarge(binaryIntermediate, NumberFormats.Bin, NumberFormats.Dec);
            }

            // This should not be reached, but included for completeness
            throw new ArgumentException($"Unsupported conversion from {fromFormat} to {toFormat}");
        }

        /// <summary>
        /// Validate that the format is supported by the string-based conversion
        /// </summary>
        /// <param name="format">Number format to validate</param>
        /// <exception cref="ArgumentException">Thrown if format is not supported</exception>
        private static void ValidateSupportedFormat(NumberFormats format)
        {
            // All formats are now supported: Bin, Qad, Oct, Dec, Hex
            if (!Enum.IsDefined(typeof(NumberFormats), format))
            {
                throw new ArgumentException($"Unsupported number format: {format}");
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
                NumberFormats.Base32 => ConvertBase32ToBinary(input),
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
                NumberFormats.Base32 => ConvertBinaryToBase32(binaryInput),
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

            // Process from right to left in groups (exactly like original algorithm)
            for (var i = binaryInput.Length - 1; i >= 0; i -= groupSize)
            {
                int convertedValue = 0;
                for (int j = 0; j < groupSize; j++)
                {
                    if (i - j >= 0 && binaryInput[i - j].Equals('1'))
                        convertedValue += (int)Math.Pow(2, j);
                }
                
                // Convert value to appropriate character based on target format
                char convertedChar = GetCharacterForValue(convertedValue, targetFormat);
                result.Add(convertedChar);
            }

            return result;
        }

        /// <summary>
        /// Convert a numeric value to the appropriate character for the target format
        /// </summary>
        /// <param name="value">Numeric value to convert</param>
        /// <param name="format">Target number format</param>
        /// <returns>Character representation</returns>
        private static char GetCharacterForValue(int value, NumberFormats format)
        {
            return format switch
            {
                NumberFormats.Hex => value < 10 ? (char)('0' + value) : (char)('A' + value - 10),
                NumberFormats.Base32 => value < 10 ? (char)('0' + value) : (char)('A' + value - 10),
                _ => (char)('0' + value) // For Bin, Qad, Oct, Dec - just use numeric characters
            };
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
        /// Convert a very long binary string to decimal format using BigInteger
        /// </summary>
        /// <param name="binaryString">Binary string (can be very long, bigger than uint64)</param>
        /// <returns>Decimal string representation</returns>
        public static string ConvertLargeBinaryToDecimal(string binaryString)
        {
            if (string.IsNullOrEmpty(binaryString))
                return "0";

            // Remove any spaces or invalid characters
            var cleanBinary = binaryString.Replace(" ", "").Replace(".", "");
            
            // Validate binary string
            foreach (char c in cleanBinary)
            {
                if (c != '0' && c != '1')
                    throw new ArgumentException($"Invalid binary digit: {c}");
            }

            // Convert using BigInteger
            BigInteger result = 0;
            BigInteger power = 1;

            // Process from right to left
            for (int i = cleanBinary.Length - 1; i >= 0; i--)
            {
                if (cleanBinary[i] == '1')
                {
                    result += power;
                }
                power *= 2;
            }

            return result.ToString();
        }

        /// <summary>
        /// Convert a very long decimal string to binary format using BigInteger
        /// </summary>
        /// <param name="decimalString">Decimal string (can be very long, bigger than uint64)</param>
        /// <returns>Binary string representation</returns>
        public static string ConvertLargeDecimalToBinary(string decimalString)
        {
            if (string.IsNullOrEmpty(decimalString))
                return "0";

            // Remove any spaces and parse
            var cleanDecimal = decimalString.Replace(" ", "").Replace(".", "");
            
            if (!BigInteger.TryParse(cleanDecimal, out BigInteger value))
                throw new ArgumentException($"Invalid decimal string: {decimalString}");

            if (value == 0)
                return "0";

            // Convert to binary
            var result = new System.Text.StringBuilder();
            while (value > 0)
            {
                result.Insert(0, (value % 2).ToString());
                value /= 2;
            }

            return result.ToString();
        }

        /// <summary>
        /// Convert between very large number formats using BigInteger
        /// This method can handle numbers much larger than uint64
        /// </summary>
        /// <param name="input">Input number string</param>
        /// <param name="fromFormat">Source number format (currently supports Dec and Bin)</param>
        /// <param name="toFormat">Target number format (currently supports Dec and Bin)</param>
        /// <returns>Converted number string</returns>
        public static string ConvertLarge(string input, NumberFormats fromFormat, NumberFormats toFormat)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            if (fromFormat == toFormat)
                return input;

            // For now, implement only Dec <-> Bin conversion for large numbers
            // Other formats can be added as needed
            return (fromFormat, toFormat) switch
            {
                (NumberFormats.Dec, NumberFormats.Bin) => ConvertLargeDecimalToBinary(input),
                (NumberFormats.Bin, NumberFormats.Dec) => ConvertLargeBinaryToDecimal(input),
                _ => throw new ArgumentException($"Large number conversion from {fromFormat} to {toFormat} not implemented yet")
            };
        }

        /// <summary>
        /// Convert binary string to Base32 string (supports very long strings)
        /// Base32 uses characters 0-9 and A-V (32 characters total)
        /// </summary>
        /// <param name="binaryInput">Binary input string</param>
        /// <returns>Base32 string</returns>
        private static string ConvertBinaryToBase32(string binaryInput)
        {
            if (string.IsNullOrEmpty(binaryInput))
                return string.Empty;

            var result = new System.Text.StringBuilder();
            
            // Process from right to left in groups of 5 bits (since 2^5 = 32 for Base32)
            for (var i = binaryInput.Length - 1; i >= 0; i -= 5)
            {
                int value = 0;
                // Process up to 5 bits
                for (int j = 0; j < 5 && (i - j) >= 0; j++)
                {
                    if (binaryInput[i - j] == '1')
                        value += (int)Math.Pow(2, j);
                }
                
                // Convert value (0-31) to Base32 character
                char base32Char = value < 10 ? (char)('0' + value) : (char)('A' + value - 10);
                result.Insert(0, base32Char);
            }
            
            return result.ToString();
        }

        /// <summary>
        /// Convert Base32 string to binary string (supports very long strings)
        /// Base32 uses characters 0-9 and A-V (32 characters total)
        /// </summary>
        /// <param name="base32Input">Base32 input string</param>
        /// <returns>Binary string</returns>
        private static string ConvertBase32ToBinary(string base32Input)
        {
            if (string.IsNullOrEmpty(base32Input))
                return string.Empty;

            var result = new System.Text.StringBuilder();
            
            foreach (char digit in base32Input)
            {
                int digitValue;
                if (char.IsDigit(digit))
                {
                    digitValue = digit - '0';
                }
                else if (char.ToUpper(digit) >= 'A' && char.ToUpper(digit) <= 'V')
                {
                    digitValue = char.ToUpper(digit) - 'A' + 10;
                }
                else
                {
                    throw new ArgumentException($"Invalid Base32 digit: {digit}. Valid characters are 0-9 and A-V.");
                }
                
                // Convert each Base32 digit to 5 binary digits
                string binaryDigits = System.Convert.ToString(digitValue, 2).PadLeft(5, '0');
                result.Append(binaryDigits);
            }
            
            // Remove leading zeros but keep at least one digit
            string binaryResult = result.ToString().TrimStart('0');
            return string.IsNullOrEmpty(binaryResult) ? "0" : binaryResult;
        }
    }
}