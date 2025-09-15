using NUnit.Framework;
using MathToMusic.Utils;
using MathToMusic.Models;

namespace MathToMusic.Tests.Utils
{
    [TestFixture]
    public class NumberConverterWithCommonNumbersTests
    {
        // Test conversions using data patterns inspired by CommonNumbers.cs
        // Testing with various mathematical constants and their digit patterns

        [TestCase("314159", NumberFormats.Dec, NumberFormats.Bin, ExpectedResult = "1001100101100101111")]
        [TestCase("314159", NumberFormats.Dec, NumberFormats.Oct, ExpectedResult = "1145457")]
        [TestCase("314159", NumberFormats.Dec, NumberFormats.Hex, ExpectedResult = "4CB2F")]
        public string Convert_PiDigits_ToOtherFormats(string input, NumberFormats from, NumberFormats to)
        {
            return NumberConverter.Convert(input, from, to);
        }

        [TestCase("161803", NumberFormats.Dec, NumberFormats.Bin, ExpectedResult = "100111100000001011")]
        [TestCase("161803", NumberFormats.Dec, NumberFormats.Oct, ExpectedResult = "474013")]
        [TestCase("161803", NumberFormats.Dec, NumberFormats.Hex, ExpectedResult = "2780B")]
        public string Convert_GoldenRatioDigits_ToOtherFormats(string input, NumberFormats from, NumberFormats to)
        {
            return NumberConverter.Convert(input, from, to);
        }

        [TestCase("271828", NumberFormats.Dec, NumberFormats.Bin, ExpectedResult = "1000010010111010100")]
        [TestCase("271828", NumberFormats.Dec, NumberFormats.Oct, ExpectedResult = "1022724")]
        [TestCase("271828", NumberFormats.Dec, NumberFormats.Hex, ExpectedResult = "425D4")]
        public string Convert_EulerDigits_ToOtherFormats(string input, NumberFormats from, NumberFormats to)
        {
            return NumberConverter.Convert(input, from, to);
        }

        // Test reverse conversions (back to decimal)
        [TestCase("1001100101100101111", NumberFormats.Bin, NumberFormats.Dec, ExpectedResult = "314159")]
        [TestCase("1145457", NumberFormats.Oct, NumberFormats.Dec, ExpectedResult = "314159")]
        [TestCase("4CB2F", NumberFormats.Hex, NumberFormats.Dec, ExpectedResult = "314159")]
        public string Convert_FromOtherFormats_BackToDecimal(string input, NumberFormats from, NumberFormats to)
        {
            return NumberConverter.Convert(input, from, to);
        }

        // Test conversions between non-decimal formats
        [TestCase("FF", NumberFormats.Hex, NumberFormats.Bin, ExpectedResult = "11111111")]
        [TestCase("FF", NumberFormats.Hex, NumberFormats.Oct, ExpectedResult = "377")]
        [TestCase("11111111", NumberFormats.Bin, NumberFormats.Oct, ExpectedResult = "377")]
        [TestCase("377", NumberFormats.Oct, NumberFormats.Bin, ExpectedResult = "11111111")]
        [TestCase("377", NumberFormats.Oct, NumberFormats.Hex, ExpectedResult = "FF")]
        [TestCase("11111111", NumberFormats.Bin, NumberFormats.Hex, ExpectedResult = "FF")]
        public string Convert_BetweenNonDecimalFormats_ReturnsCorrect(string input, NumberFormats from, NumberFormats to)
        {
            return NumberConverter.Convert(input, from, to);
        }

        // Test with square root of 2 digits (141421)
        [TestCase("141421", NumberFormats.Dec, NumberFormats.Bin, ExpectedResult = "100010100001101101")]
        [TestCase("141421", NumberFormats.Dec, NumberFormats.Oct, ExpectedResult = "424155")]
        [TestCase("141421", NumberFormats.Dec, NumberFormats.Hex, ExpectedResult = "2286D")]
        public string Convert_Sqrt2Digits_ToOtherFormats(string input, NumberFormats from, NumberFormats to)
        {
            return NumberConverter.Convert(input, from, to);
        }

        // Test edge cases with single digits from mathematical constants
        [TestCase("3", NumberFormats.Dec, NumberFormats.Bin, ExpectedResult = "11")]
        [TestCase("1", NumberFormats.Dec, NumberFormats.Oct, ExpectedResult = "1")]
        [TestCase("4", NumberFormats.Dec, NumberFormats.Hex, ExpectedResult = "4")]
        [TestCase("5", NumberFormats.Dec, NumberFormats.Qad, ExpectedResult = "11")]
        public string Convert_SingleDigitsFromConstants_ReturnsCorrect(string input, NumberFormats from, NumberFormats to)
        {
            return NumberConverter.Convert(input, from, to);
        }

        // Test larger numbers from PI1000 (first few digits)
        [TestCase("31415926535", NumberFormats.Dec, NumberFormats.Hex, ExpectedResult = "75088FF07")]
        [TestCase("31415926535", NumberFormats.Dec, NumberFormats.Oct, ExpectedResult = "352042177407")]
        public string Convert_LargerConstantDigits_ReturnsCorrect(string input, NumberFormats from, NumberFormats to)
        {
            return NumberConverter.Convert(input, from, to);
        }
    }
}