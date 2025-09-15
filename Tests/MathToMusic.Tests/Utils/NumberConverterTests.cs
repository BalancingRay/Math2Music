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
    }
}