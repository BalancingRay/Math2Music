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

        // Tests for new large number conversion functionality using CommonNumbers data
        
        [Test]
        public void ConvertLarge_PI100Digits_DecToBinAndBack_RoundTrip()
        {
            // Arrange - extract numeric part of PI100 (remove decimal point)
            string piDigits = CommonNumbers.Collection["PI100"].Replace(".", "").Replace(" ", "");
            piDigits = piDigits.Substring(0, Math.Min(piDigits.Length, 50)); // Take first 50 digits for test
            
            // Act - convert to binary and back to decimal
            string binaryResult = NumberConverter.ConvertLarge(piDigits, NumberFormats.Dec, NumberFormats.Bin);
            string decimalResult = NumberConverter.ConvertLarge(binaryResult, NumberFormats.Bin, NumberFormats.Dec);
            
            // Assert - should get back original number
            Assert.That(decimalResult, Is.EqualTo(piDigits));
        }

        [Test]
        public void ConvertLarge_E100Digits_DecToBinAndBack_RoundTrip()
        {
            // Arrange - extract numeric part of E100 (remove decimal point)
            string eDigits = CommonNumbers.Collection["E100"].Replace(".", "").Replace(" ", "");
            eDigits = eDigits.Substring(0, Math.Min(eDigits.Length, 50)); // Take first 50 digits for test
            
            // Act - convert to binary and back to decimal
            string binaryResult = NumberConverter.ConvertLarge(eDigits, NumberFormats.Dec, NumberFormats.Bin);
            string decimalResult = NumberConverter.ConvertLarge(binaryResult, NumberFormats.Bin, NumberFormats.Dec);
            
            // Assert - should get back original number
            Assert.That(decimalResult, Is.EqualTo(eDigits));
        }

        [Test]
        public void ConvertLarge_FI100Digits_DecToBinAndBack_RoundTrip()
        {
            // Arrange - extract numeric part of FI100 (golden ratio, remove decimal point)
            string fiDigits = CommonNumbers.Collection["FI100"].Replace(".", "").Replace(" ", "");
            fiDigits = fiDigits.Substring(0, Math.Min(fiDigits.Length, 50)); // Take first 50 digits for test
            
            // Act - convert to binary and back to decimal
            string binaryResult = NumberConverter.ConvertLarge(fiDigits, NumberFormats.Dec, NumberFormats.Bin);
            string decimalResult = NumberConverter.ConvertLarge(binaryResult, NumberFormats.Bin, NumberFormats.Dec);
            
            // Assert - should get back original number
            Assert.That(decimalResult, Is.EqualTo(fiDigits));
        }

        [Test]
        public void ConvertLarge_VeryLargePI1000_FirstDigits_RoundTrip()
        {
            // Arrange - use first part of PI1000 for a very large number test
            string pi1000 = CommonNumbers.Collection["PI1000"].Replace(".", "").Replace(" ", "");
            string firstPart = pi1000.Substring(0, Math.Min(pi1000.Length, 100)); // Take first 100 digits
            
            // Act - convert to binary and back to decimal
            string binaryResult = NumberConverter.ConvertLarge(firstPart, NumberFormats.Dec, NumberFormats.Bin);
            string decimalResult = NumberConverter.ConvertLarge(binaryResult, NumberFormats.Bin, NumberFormats.Dec);
            
            // Assert - should get back original number
            Assert.That(decimalResult, Is.EqualTo(firstPart));
        }

        [Test]
        public void ConvertLarge_VeryLargeE1000_FirstDigits_RoundTrip()
        {
            // Arrange - use first part of E1000 for a very large number test
            string e1000 = CommonNumbers.Collection["E1000"].Replace(".", "").Replace(" ", "");
            string firstPart = e1000.Substring(0, Math.Min(e1000.Length, 100)); // Take first 100 digits
            
            // Act - convert to binary and back to decimal
            string binaryResult = NumberConverter.ConvertLarge(firstPart, NumberFormats.Dec, NumberFormats.Bin);
            string decimalResult = NumberConverter.ConvertLarge(binaryResult, NumberFormats.Bin, NumberFormats.Dec);
            
            // Assert - should get back original number
            Assert.That(decimalResult, Is.EqualTo(firstPart));
        }

        // Performance test for very long arguments
        [Test]
        public void ConvertLarge_PerformanceTest_VeryLongNumber()
        {
            // Arrange - create a very long decimal number (500 digits)
            string longNumber = string.Concat(Enumerable.Repeat("1234567890", 50));
            
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            // Act - convert to binary and back
            string binaryResult = NumberConverter.ConvertLarge(longNumber, NumberFormats.Dec, NumberFormats.Bin);
            string decimalResult = NumberConverter.ConvertLarge(binaryResult, NumberFormats.Bin, NumberFormats.Dec);
            
            stopwatch.Stop();
            
            // Assert - should complete within reasonable time and maintain accuracy
            Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(5000), "Conversion should complete within 5 seconds");
            Assert.That(decimalResult, Is.EqualTo(longNumber), "Round trip should maintain accuracy");
        }

        // Test with specific examples that are easy to verify manually
        [TestCase("255", ExpectedResult = "11111111")]
        [TestCase("1024", ExpectedResult = "10000000000")]
        [TestCase("65535", ExpectedResult = "1111111111111111")]
        public string ConvertLarge_DecimalToBinary_KnownValues(string input)
        {
            return NumberConverter.ConvertLarge(input, NumberFormats.Dec, NumberFormats.Bin);
        }

        [TestCase("11111111", ExpectedResult = "255")]
        [TestCase("10000000000", ExpectedResult = "1024")]
        [TestCase("1111111111111111", ExpectedResult = "65535")]
        public string ConvertLarge_BinaryToDecimal_KnownValues(string input)
        {
            return NumberConverter.ConvertLarge(input, NumberFormats.Bin, NumberFormats.Dec);
        }

        // Test numbers larger than uint64 max (18446744073709551615)
        [Test]
        public void ConvertLarge_NumberLargerThanUint64Max_Works()
        {
            // Arrange - number larger than uint64 max
            string largeNumber = "18446744073709551616"; // uint64 max + 1
            
            // Act
            string binaryResult = NumberConverter.ConvertLarge(largeNumber, NumberFormats.Dec, NumberFormats.Bin);
            string decimalResult = NumberConverter.ConvertLarge(binaryResult, NumberFormats.Bin, NumberFormats.Dec);
            
            // Assert
            Assert.That(decimalResult, Is.EqualTo(largeNumber));
            Assert.That(binaryResult, Is.EqualTo("10000000000000000000000000000000000000000000000000000000000000000"));
        }
    }
}