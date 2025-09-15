using MathToMusic.Models;

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
    }
}