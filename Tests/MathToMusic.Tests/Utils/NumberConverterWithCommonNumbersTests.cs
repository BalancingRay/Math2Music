using NUnit.Framework;
using MathToMusic.Utils;
using MathToMusic.Models;

namespace MathToMusic.Tests.Utils
{
    [TestFixture]
    public class NumberConverterWithCommonNumbersTests
    {
        // Test conversions using very long binary strings inspired by mathematical constants
        // These test strings are much longer than max uint64 (19 digits), supporting the requirement for very long sequences

        [Test]
        public void Convert_VeryLongBinaryFromPiDigits_ToHex()
        {
            // Using PI digits as source: 31415926535897932384... converted to a long binary string
            // This binary string is 80 bits long (much longer than 64-bit limit)
            string longBinary = "11010000111100101100101111110011001011100010110001101000111100111001110110";
            
            string result = NumberConverter.Convert(longBinary, NumberFormats.Bin, NumberFormats.Hex);
            
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Length, Is.GreaterThan(0));
            Assert.That(result, Does.Match("^[0-9A-F]+$")); // Valid hex string
        }

        [Test]
        public void Convert_VeryLongBinaryFromEulerDigits_ToOctal()
        {
            // Using E digits as source: 27182818284590452353... converted to binary
            // This binary string is 72 bits long
            string longBinary = "110010111110000110101101001000101011001001011100010110101110100101100001";
            
            string result = NumberConverter.Convert(longBinary, NumberFormats.Bin, NumberFormats.Oct);
            
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Length, Is.GreaterThan(0));
            Assert.That(result, Does.Match("^[0-7]+$")); // Valid octal string
        }

        [Test]
        public void Convert_VeryLongBinaryFromGoldenRatio_ToQuaternary()
        {
            // Using Golden Ratio digits as source: 16180339887... converted to binary
            // This binary string is 68 bits long
            string longBinary = "1111110000001110100011100111001100110100100101011011010010100010001101";
            
            string result = NumberConverter.Convert(longBinary, NumberFormats.Bin, NumberFormats.Qad);
            
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Length, Is.GreaterThan(0));
            Assert.That(result, Does.Match("^[0-3]+$")); // Valid quaternary string
        }

        [Test]
        public void Convert_BidirectionalConversion_VeryLongString()
        {
            // Test bidirectional conversion with a very long binary string (100+ bits)
            string originalBinary = "1010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010";
            
            // Convert Binary -> Hex -> Binary
            string hex = NumberConverter.Convert(originalBinary, NumberFormats.Bin, NumberFormats.Hex);
            string backToBinary = NumberConverter.Convert(hex, NumberFormats.Hex, NumberFormats.Bin);
            
            Assert.That(backToBinary, Is.EqualTo(originalBinary));
        }

        [Test]
        public void Convert_BidirectionalConversion_OctalToQuaternary()
        {
            // Test conversion between two non-binary formats using very long strings
            string longOctal = "1234567012345670123456701234567012345670123456701234567012345670";
            
            // Convert Octal -> Binary -> Quaternary
            string binary = NumberConverter.Convert(longOctal, NumberFormats.Oct, NumberFormats.Bin);
            string quaternary = NumberConverter.Convert(binary, NumberFormats.Bin, NumberFormats.Qad);
            
            // Convert back: Quaternary -> Binary -> Octal
            string binaryBack = NumberConverter.Convert(quaternary, NumberFormats.Qad, NumberFormats.Bin);
            string octalBack = NumberConverter.Convert(binaryBack, NumberFormats.Bin, NumberFormats.Oct);
            
            Assert.That(octalBack, Is.EqualTo(longOctal));
        }

        [Test]
        public void Convert_ExtremeLongBinary_ToAllFormats()
        {
            // Test with extremely long binary string (200+ bits) inspired by PI1000 precision
            string extremeLongBinary = "11010001111001011001011111100110010111000101100011010001111001110011101101101100101110011100010000111001110100011010000111100101100101111110011001011100010110001101000111100111001110110110110010111001110001000011100111010001";
            
            // Test conversion to all supported formats
            string hex = NumberConverter.Convert(extremeLongBinary, NumberFormats.Bin, NumberFormats.Hex);
            string oct = NumberConverter.Convert(extremeLongBinary, NumberFormats.Bin, NumberFormats.Oct);
            string qad = NumberConverter.Convert(extremeLongBinary, NumberFormats.Bin, NumberFormats.Qad);
            
            Assert.That(hex, Is.Not.Null.And.Not.Empty);
            Assert.That(oct, Is.Not.Null.And.Not.Empty);
            Assert.That(qad, Is.Not.Null.And.Not.Empty);
            
            // Verify all results are valid format strings
            Assert.That(hex, Does.Match("^[0-9A-F]+$"));
            Assert.That(oct, Does.Match("^[0-7]+$"));
            Assert.That(qad, Does.Match("^[0-3]+$"));
        }

        [Test]
        public void Convert_PatternFromCommonNumbers_MaintainsAccuracy()
        {
            // Test using a pattern derived from SQRT2 digits: 14142135623...
            // Convert to binary equivalent for testing
            string testBinary = "1101110110111000101100111100111110000001011010100011";
            
            // Test round-trip conversion through different formats
            string hex = NumberConverter.Convert(testBinary, NumberFormats.Bin, NumberFormats.Hex);
            string oct = NumberConverter.Convert(hex, NumberFormats.Hex, NumberFormats.Oct);
            string qad = NumberConverter.Convert(oct, NumberFormats.Oct, NumberFormats.Qad);
            string finalBinary = NumberConverter.Convert(qad, NumberFormats.Qad, NumberFormats.Bin);
            
            Assert.That(finalBinary, Is.EqualTo(testBinary));
        }

        [Test]
        public void Convert_MaximumLengthBinary_DoesNotOverflow()
        {
            // Test with a very long binary string that would exceed any integer type
            // This simulates processing mathematical constants with thousands of digits
            var binaryBuilder = new System.Text.StringBuilder();
            for (int i = 0; i < 1000; i++)
            {
                binaryBuilder.Append(i % 2); // Alternating 0,1 pattern
            }
            string maxLengthBinary = binaryBuilder.ToString();
            
            // Should not throw overflow exceptions
            Assert.DoesNotThrow(() => NumberConverter.Convert(maxLengthBinary, NumberFormats.Bin, NumberFormats.Hex));
            Assert.DoesNotThrow(() => NumberConverter.Convert(maxLengthBinary, NumberFormats.Bin, NumberFormats.Oct));
            Assert.DoesNotThrow(() => NumberConverter.Convert(maxLengthBinary, NumberFormats.Bin, NumberFormats.Qad));
        }

        [Test]
        public void Convert_DecimalFormat_NowSupported()
        {
            // Verify that decimal format is now supported
            string binaryInput = "1010101010101010101010";
            
            Assert.DoesNotThrow(() => 
                NumberConverter.Convert("123456789012345", NumberFormats.Dec, NumberFormats.Bin));
            Assert.DoesNotThrow(() => 
                NumberConverter.Convert(binaryInput, NumberFormats.Bin, NumberFormats.Dec));
                
            // Test actual conversions work correctly
            string decResult = NumberConverter.Convert(binaryInput, NumberFormats.Bin, NumberFormats.Dec);
            string binResult = NumberConverter.Convert(decResult, NumberFormats.Dec, NumberFormats.Bin);
            Assert.That(binResult, Is.EqualTo(binaryInput));
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