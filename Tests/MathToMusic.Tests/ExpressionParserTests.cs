using NUnit.Framework;
using System;

namespace MathToMusic.Tests
{
    [TestFixture]
    public class ExpressionParserTests
    {
        [Test]
        public void ParseExpression_SimplePolyphonic_ReturnsCorrectParts()
        {
            // Arrange
            string expression = "123+456";

            // Act
            string[] result = ExpressionParser.ParseExpression(expression);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Length.EqualTo(2));
            Assert.That(result[0], Is.EqualTo("123"));
            Assert.That(result[1], Is.EqualTo("456"));
        }

        [Test]
        public void ParseExpression_WithSpaces_TrimsWhitespace()
        {
            // Arrange
            string expression = "  123  +  456  ";

            // Act
            string[] result = ExpressionParser.ParseExpression(expression);

            // Assert
            Assert.That(result, Has.Length.EqualTo(2));
            Assert.That(result[0], Is.EqualTo("123"));
            Assert.That(result[1], Is.EqualTo("456"));
        }

        [Test]
        public void ParseExpression_HexFormat_ReturnsCorrectParts()
        {
            // Arrange
            string expression = "ABC+DEF";

            // Act
            string[] result = ExpressionParser.ParseExpression(expression);

            // Assert
            Assert.That(result, Has.Length.EqualTo(2));
            Assert.That(result[0], Is.EqualTo("ABC"));
            Assert.That(result[1], Is.EqualTo("DEF"));
        }

        [Test]
        public void ParseExpression_MultipleTerms_ReturnsAllParts()
        {
            // Arrange
            string expression = "12+34+56";

            // Act
            string[] result = ExpressionParser.ParseExpression(expression);

            // Assert
            Assert.That(result, Has.Length.EqualTo(3));
            Assert.That(result[0], Is.EqualTo("12"));
            Assert.That(result[1], Is.EqualTo("34"));
            Assert.That(result[2], Is.EqualTo("56"));
        }

        [Test]
        public void ParseExpression_NoPlus_ReturnsSinglePart()
        {
            // Arrange
            string expression = "12345";

            // Act
            string[] result = ExpressionParser.ParseExpression(expression);

            // Assert
            Assert.That(result, Has.Length.EqualTo(1));
            Assert.That(result[0], Is.EqualTo("12345"));
        }

        [Test]
        public void ParseExpression_EmptyInput_ReturnsEmptyArray()
        {
            // Arrange
            string expression = "";

            // Act
            string[] result = ExpressionParser.ParseExpression(expression);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Length.EqualTo(0));
        }

        [Test]
        public void ParseExpression_NullInput_ReturnsEmptyArray()
        {
            // Arrange
            string expression = null;

            // Act
            string[] result = ExpressionParser.ParseExpression(expression);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Length.EqualTo(0));
        }

        [Test]
        public void IsPolyphonic_WithPlusOperator_ReturnsTrue()
        {
            // Act & Assert
            Assert.That(ExpressionParser.IsPolyphonic("123+456"), Is.True);
            Assert.That(ExpressionParser.IsPolyphonic("A+B"), Is.True);
            Assert.That(ExpressionParser.IsPolyphonic("1+2+3"), Is.True);
        }

        [Test]
        public void IsPolyphonic_WithoutPlusOperator_ReturnsFalse()
        {
            // Act & Assert
            Assert.That(ExpressionParser.IsPolyphonic("123"), Is.False);
            Assert.That(ExpressionParser.IsPolyphonic("ABC"), Is.False);
            Assert.That(ExpressionParser.IsPolyphonic(""), Is.False);
            Assert.That(ExpressionParser.IsPolyphonic(null), Is.False);
        }
    }
}