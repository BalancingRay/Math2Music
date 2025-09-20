using NUnit.Framework;
using MathToMusic;
using MathToMusic.Contracts;
using MathToMusic.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MathToMusic.Tests
{
    [TestFixture]
    public class PolyphonicDemonstrationTests
    {
        [Test]
        public void Demonstration_PolyphonicProcessing_ShowsCompleteWorkflow()
        {
            // This test demonstrates the complete polyphonic processing workflow
            // from input parsing through harmonic combination
            
            // Arrange
            var processor = new MultiTrackProcessor();
            
            // Test Case 1: Simple polyphonic - 123+456 -> (1,4), (2,5), (3,6)
            Console.WriteLine("=== Test Case 1: 123+456 ===");
            var result1 = processor.Process("123+456", NumberFormats.Dec, NumberFormats.Dec);
            PrintSequenceInfo(result1[0], "123+456");
            
            // Test Case 2: Different lengths - 12+3456 -> (1,3), (2,4), (5), (6)
            Console.WriteLine("\n=== Test Case 2: 12+3456 ===");
            var result2 = processor.Process("12+3456", NumberFormats.Dec, NumberFormats.Dec);
            PrintSequenceInfo(result2[0], "12+3456");
            
            // Test Case 3: Hex format - ABC+DEF -> (A,D), (B,E), (C,F)
            Console.WriteLine("\n=== Test Case 3: ABC+DEF (Hex) ===");
            var result3 = processor.Process("ABC+DEF", NumberFormats.Hex, NumberFormats.Hex);
            PrintSequenceInfo(result3[0], "ABC+DEF");
            
            // Test Case 4: Three tracks - 1+2+3 -> (1,2,3)
            Console.WriteLine("\n=== Test Case 4: 1+2+3 ===");
            var result4 = processor.Process("1+2+3", NumberFormats.Dec, NumberFormats.Dec);
            PrintSequenceInfo(result4[0], "1+2+3");
            
            // All tests should have valid results
            Assert.That(result1[0].Tones.Count, Is.EqualTo(3));
            Assert.That(result2[0].Tones.Count, Is.EqualTo(4));
            Assert.That(result3[0].Tones.Count, Is.EqualTo(3));
            Assert.That(result4[0].Tones.Count, Is.EqualTo(1));
            
            Console.WriteLine("\n=== All polyphonic processing tests completed successfully! ===");
        }

        private void PrintSequenceInfo(Sequiention sequence, string originalInput)
        {
            Console.WriteLine($"Input: {originalInput}");
            Console.WriteLine($"Title: {sequence.Title}");
            Console.WriteLine($"Total Duration: {sequence.TotalDuration.TotalMilliseconds}ms");
            Console.WriteLine($"Number of Chords: {sequence.Tones.Count}");
            
            for (int i = 0; i < sequence.Tones.Count; i++)
            {
                var tone = sequence.Tones[i];
                var frequencies = string.Join(", ", tone.ObertonFrequencies.Select(f => $"{f:F0}Hz"));
                Console.WriteLine($"  Chord {i + 1}: [{frequencies}] - Duration: {tone.Duration.TotalMilliseconds}ms");
            }
        }

        [Test] 
        public void Demonstration_ExpressionParser_ShowsParsing()
        {
            // Demonstrate expression parsing
            Console.WriteLine("=== Expression Parser Demonstration ===");
            
            var testCases = new[]
            {
                "123+456",
                "ABC+DEF", 
                "12+34+56",
                "1+2+3+4+5",
                "123",
                ""
            };

            foreach (var testCase in testCases)
            {
                var parts = ExpressionParser.ParseExpression(testCase);
                var isPolyphonic = ExpressionParser.IsPolyphonic(testCase);
                Console.WriteLine($"Input: '{testCase}' -> Parts: [{string.Join(", ", parts)}], Polyphonic: {isPolyphonic}");
            }
            
            Assert.Pass("Expression parser demonstration completed");
        }

        [Test]
        public void Demonstration_FrequencyMapping_ShowsToneGeneration()
        {
            // Demonstrate how characters map to frequencies
            Console.WriteLine("=== Frequency Mapping Demonstration ===");
            
            var processor = new SingleTrackProcessor();
            var characters = "0123456789ABCDEF";
            
            Console.WriteLine("Character -> Frequency mapping (base 180Hz):");
            for (int i = 0; i < characters.Length; i++)
            {
                var charStr = characters[i].ToString();
                var result = ((ITonesProcessor)processor).Process(charStr, NumberFormats.Dec, NumberFormats.Dec);
                if (result[0].Tones.Count > 0)
                {
                    var frequency = result[0].Tones[0].ObertonFrequencies[0];
                    Console.WriteLine($"  '{characters[i]}' -> {frequency:F0}Hz (multiplier: {frequency / 180:F0})");
                }
            }
            
            Assert.Pass("Frequency mapping demonstration completed");
        }
    }
}