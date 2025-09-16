using MathToMusic.Models;
using System.Numerics;

namespace MathToMusic.Utils
{
    public static class NumberConverter
    {
        /// <summary>
        /// Convert a number string from one format to another
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

            // First convert to decimal as intermediate format
            long decimalValue = ConvertToDecimal(input, fromFormat);
            
            // Then convert from decimal to target format
            return ConvertFromDecimal(decimalValue, toFormat);
        }

        /// <summary>
        /// Convert from any supported format to decimal
        /// </summary>
        /// <param name="input">Input number string</param>
        /// <param name="fromFormat">Source number format</param>
        /// <returns>Decimal value</returns>
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
                NumberFormats.Qad => ConvertQuaternaryToDecimal(input),
                _ => throw new ArgumentException($"Unsupported format: {fromFormat}")
            };
        }

        /// <summary>
        /// Convert from decimal to any supported format
        /// </summary>
        /// <param name="decimalValue">Decimal value to convert</param>
        /// <param name="toFormat">Target number format</param>
        /// <returns>Converted number string</returns>
        public static string ConvertFromDecimal(long decimalValue, NumberFormats toFormat)
        {
            return toFormat switch
            {
                NumberFormats.Bin => System.Convert.ToString(decimalValue, 2),
                NumberFormats.Oct => System.Convert.ToString(decimalValue, 8),
                NumberFormats.Dec => decimalValue.ToString(),
                NumberFormats.Hex => System.Convert.ToString(decimalValue, 16).ToUpper(),
                NumberFormats.Qad => ConvertDecimalToQuaternary(decimalValue),
                _ => throw new ArgumentException($"Unsupported format: {toFormat}")
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
        /// Convert quaternary string to decimal
        /// </summary>
        /// <param name="quaternaryInput">Quaternary input string</param>
        /// <returns>Decimal value</returns>
        private static long ConvertQuaternaryToDecimal(string quaternaryInput)
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
        /// Convert decimal to quaternary string
        /// </summary>
        /// <param name="decimalValue">Decimal value to convert</param>
        /// <returns>Quaternary string</returns>
        private static string ConvertDecimalToQuaternary(long decimalValue)
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
    }
}