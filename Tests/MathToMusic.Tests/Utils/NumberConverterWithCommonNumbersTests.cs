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
        public void Convert_DecimalFormat_ThrowsExpectedException()
        {
            // Verify that decimal format is not supported as per requirements
            string binaryInput = "1010101010101010101010";
            
            Assert.Throws<ArgumentException>(() => 
                NumberConverter.Convert("123456789012345", NumberFormats.Dec, NumberFormats.Bin));
            Assert.Throws<ArgumentException>(() => 
                NumberConverter.Convert(binaryInput, NumberFormats.Bin, NumberFormats.Dec));
        }
    }
}