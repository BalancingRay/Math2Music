using NUnit.Framework;
using MathToMusic.Utils;
using MathToMusic.Models;

namespace MathToMusic.Tests.Utils
{
    [TestFixture]
    public class NumberConverterTests
    {
        [Test]
        public void Convert_SameFormat_ReturnsOriginal()
        {
            // Arrange
            string input = "101";
            
            // Act
            string result = NumberConverter.Convert(input, NumberFormats.Bin, NumberFormats.Bin);
            
            // Assert
            Assert.That(result, Is.EqualTo(input));
        }

        [TestCase("", ExpectedResult = "")]
        [TestCase(null, ExpectedResult = "")]
        public string Convert_EmptyOrNullInput_ReturnsEmpty(string input)
        {
            return NumberConverter.Convert(input, NumberFormats.Dec, NumberFormats.Bin);
        }

        [TestCase("1010", NumberFormats.Bin, NumberFormats.Dec, ExpectedResult = "10")]
        [TestCase("1010", NumberFormats.Bin, NumberFormats.Oct, ExpectedResult = "12")]
        [TestCase("1010", NumberFormats.Bin, NumberFormats.Hex, ExpectedResult = "A")]
        [TestCase("12", NumberFormats.Oct, NumberFormats.Bin, ExpectedResult = "1010")]
        [TestCase("12", NumberFormats.Oct, NumberFormats.Dec, ExpectedResult = "10")]
        [TestCase("12", NumberFormats.Oct, NumberFormats.Hex, ExpectedResult = "A")]
        [TestCase("A", NumberFormats.Hex, NumberFormats.Bin, ExpectedResult = "1010")]
        [TestCase("A", NumberFormats.Hex, NumberFormats.Oct, ExpectedResult = "12")]
        [TestCase("A", NumberFormats.Hex, NumberFormats.Dec, ExpectedResult = "10")]
        [TestCase("10", NumberFormats.Dec, NumberFormats.Bin, ExpectedResult = "1010")]
        [TestCase("10", NumberFormats.Dec, NumberFormats.Oct, ExpectedResult = "12")]
        [TestCase("10", NumberFormats.Dec, NumberFormats.Hex, ExpectedResult = "A")]
        public string Convert_BetweenFormats_ReturnsCorrectResult(string input, NumberFormats from, NumberFormats to)
        {
            return NumberConverter.Convert(input, from, to);
        }

        [TestCase("11", NumberFormats.Qad, NumberFormats.Dec, ExpectedResult = "5")] // 1*4^1 + 1*4^0 = 5
        [TestCase("5", NumberFormats.Dec, NumberFormats.Qad, ExpectedResult = "11")]
        public string Convert_QuaternaryFormat_ReturnsCorrectResult(string input, NumberFormats from, NumberFormats to)
        {
            return NumberConverter.Convert(input, from, to);
        }

        [TestCase("101", NumberFormats.Bin, ExpectedResult = 5)]
        [TestCase("12", NumberFormats.Oct, ExpectedResult = 10)]
        [TestCase("FF", NumberFormats.Hex, ExpectedResult = 255)]
        [TestCase("100", NumberFormats.Dec, ExpectedResult = 100)]
        public long ConvertToDecimal_VariousFormats_ReturnsCorrectDecimal(string input, NumberFormats format)
        {
            return NumberConverter.ConvertToDecimal(input, format);
        }

        [TestCase(10, NumberFormats.Bin, ExpectedResult = "1010")]
        [TestCase(10, NumberFormats.Oct, ExpectedResult = "12")]
        [TestCase(10, NumberFormats.Hex, ExpectedResult = "A")]
        [TestCase(10, NumberFormats.Dec, ExpectedResult = "10")]
        public string ConvertFromDecimal_VariousFormats_ReturnsCorrectResult(long value, NumberFormats format)
        {
            return NumberConverter.ConvertFromDecimal(value, format);
        }

        [Test]
        public void ConvertBinaryWithGrouping_ToOctal_ReturnsCorrectChars()
        {
            // Arrange - binary 101011 should convert to octal 53, but processed right-to-left
            string binaryInput = "101011";
            
            // Act
            var result = NumberConverter.ConvertBinaryWithGrouping(binaryInput, NumberFormats.Oct);
            
            // Assert - processes 011 (3) first, then 101 (5)
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0], Is.EqualTo('3'));
            Assert.That(result[1], Is.EqualTo('5'));
        }

        [Test]
        public void ConvertBinaryWithGrouping_ToHex_ReturnsCorrectChars()
        {
            // Arrange - binary 10101111 should convert to hex AF, but processed right-to-left
            string binaryInput = "10101111";
            
            // Act
            var result = NumberConverter.ConvertBinaryWithGrouping(binaryInput, NumberFormats.Hex);
            
            // Assert - processes 1111 (F) first, then 1010 (A)
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0], Is.EqualTo('F'));
            Assert.That(result[1], Is.EqualTo('A'));
        }

        [Test]
        public void ConvertBinaryWithGrouping_EmptyInput_ReturnsEmptyList()
        {
            // Act
            var result = NumberConverter.ConvertBinaryWithGrouping("", NumberFormats.Hex);
            
            // Assert
            Assert.That(result, Is.Empty);
        }

        // Test cases using data patterns similar to CommonNumbers.cs
        [TestCase("11111111", NumberFormats.Bin, NumberFormats.Hex, ExpectedResult = "FF")]
        [TestCase("11111111", NumberFormats.Bin, NumberFormats.Oct, ExpectedResult = "377")]
        [TestCase("10101010", NumberFormats.Bin, NumberFormats.Hex, ExpectedResult = "AA")]
        [TestCase("10101010", NumberFormats.Bin, NumberFormats.Oct, ExpectedResult = "252")]
        public string Convert_BinaryPatterns_ReturnsCorrectResult(string input, NumberFormats from, NumberFormats to)
        {
            return NumberConverter.Convert(input, from, to);
        }

        // Tests for new large number conversion methods
        
        [Test]
        public void ConvertLargeBinaryToDecimal_SmallNumber_ReturnsCorrect()
        {
            // Arrange - test with small numbers first to verify correctness
            string binaryInput = "1010"; // 10 in decimal
            
            // Act
            string result = NumberConverter.ConvertLargeBinaryToDecimal(binaryInput);
            
            // Assert
            Assert.That(result, Is.EqualTo("10"));
        }

        [Test]
        public void ConvertLargeDecimalToBinary_SmallNumber_ReturnsCorrect()
        {
            // Arrange - test with small numbers first to verify correctness
            string decimalInput = "10";
            
            // Act
            string result = NumberConverter.ConvertLargeDecimalToBinary(decimalInput);
            
            // Assert
            Assert.That(result, Is.EqualTo("1010"));
        }

        [Test]
        public void ConvertLargeBinaryToDecimal_LargeNumber_ReturnsCorrect()
        {
            // Arrange - test with a number larger than uint64 max (18446744073709551615)
            string binaryInput = "11111111111111111111111111111111111111111111111111111111111111111"; // 65 bits, all 1s
            
            // Act
            string result = NumberConverter.ConvertLargeBinaryToDecimal(binaryInput);
            
            // Assert - Should be 2^65 - 1 = 36893488147419103231
            Assert.That(result, Is.EqualTo("36893488147419103231"));
        }

        [Test]
        public void ConvertLargeDecimalToBinary_LargeNumber_ReturnsCorrect()
        {
            // Arrange - test with a number larger than uint64 max
            string decimalInput = "36893488147419103231"; // 2^65 - 1
            
            // Act
            string result = NumberConverter.ConvertLargeDecimalToBinary(decimalInput);
            
            // Assert - Should be 65 bits, all 1s
            Assert.That(result, Is.EqualTo("11111111111111111111111111111111111111111111111111111111111111111"));
        }

        [Test]
        public void ConvertLarge_DecToBin_SmallNumber_ReturnsCorrect()
        {
            // Arrange
            string input = "255";
            
            // Act
            string result = NumberConverter.ConvertLarge(input, NumberFormats.Dec, NumberFormats.Bin);
            
            // Assert
            Assert.That(result, Is.EqualTo("11111111"));
        }

        [Test]
        public void ConvertLarge_BinToDec_SmallNumber_ReturnsCorrect()
        {
            // Arrange
            string input = "11111111";
            
            // Act
            string result = NumberConverter.ConvertLarge(input, NumberFormats.Bin, NumberFormats.Dec);
            
            // Assert
            Assert.That(result, Is.EqualTo("255"));
        }

        [Test]
        public void ConvertLarge_SameFormat_ReturnsOriginal()
        {
            // Arrange
            string input = "12345";
            
            // Act
            string result = NumberConverter.ConvertLarge(input, NumberFormats.Dec, NumberFormats.Dec);
            
            // Assert
            Assert.That(result, Is.EqualTo(input));
        }

        [Test]
        public void ConvertLarge_EmptyInput_ReturnsEmpty()
        {
            // Act
            string result = NumberConverter.ConvertLarge("", NumberFormats.Dec, NumberFormats.Bin);
            
            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void ConvertLarge_UnsupportedFormat_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                NumberConverter.ConvertLarge("123", NumberFormats.Dec, NumberFormats.Hex));
        }

        [TestCase("", ExpectedResult = "0")]
        [TestCase("0", ExpectedResult = "0")]
        [TestCase("1", ExpectedResult = "1")]
        [TestCase("101", ExpectedResult = "5")]
        [TestCase("1111", ExpectedResult = "15")]
        public string ConvertLargeBinaryToDecimal_EdgeCases_ReturnsCorrect(string input)
        {
            return NumberConverter.ConvertLargeBinaryToDecimal(input);
        }

        [TestCase("", ExpectedResult = "0")]
        [TestCase("0", ExpectedResult = "0")]
        [TestCase("1", ExpectedResult = "1")]
        [TestCase("5", ExpectedResult = "101")]
        [TestCase("15", ExpectedResult = "1111")]
        public string ConvertLargeDecimalToBinary_EdgeCases_ReturnsCorrect(string input)
        {
            return NumberConverter.ConvertLargeDecimalToBinary(input);
        }

        [Test]
        public void ConvertLargeBinaryToDecimal_WithSpaces_ReturnsCorrect()
        {
            // Arrange - test with spaces in input (should be ignored)
            string input = "1010 1010";
            
            // Act
            string result = NumberConverter.ConvertLargeBinaryToDecimal(input);
            
            // Assert - Should treat as 10101010 (170 in decimal)
            Assert.That(result, Is.EqualTo("170"));
        }

        [Test]
        public void ConvertLargeDecimalToBinary_WithSpaces_ReturnsCorrect()
        {
            // Arrange - test with spaces in input (should be ignored)
            string input = "1 7 0";
            
            // Act
            string result = NumberConverter.ConvertLargeDecimalToBinary(input);
            
            // Assert - Should treat as 170 
            Assert.That(result, Is.EqualTo("10101010"));
        }

        [Test]
        public void ConvertLargeBinaryToDecimal_InvalidChar_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                NumberConverter.ConvertLargeBinaryToDecimal("102")); // '2' is invalid in binary
        }

        [Test]
        public void ConvertLargeDecimalToBinary_InvalidChar_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                NumberConverter.ConvertLargeDecimalToBinary("12a3")); // 'a' is invalid in decimal
        }
    }
}