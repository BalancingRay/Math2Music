using MathToMusic.Contracts;
using MathToMusic.Models;

namespace MathToMusic.Processors
{
    /// <summary>
    /// Processor for handling polyphonic expressions with + operator
    /// Processes each part independently then combines them harmonically
    /// </summary>
    public class MultiTrackProcessor : ITonesProcessor
    {
        private readonly ITonesProcessor _singleTrackProcessor;

        public MultiTrackProcessor(ITonesProcessor singleTrackProcessor)
        {
            _singleTrackProcessor = singleTrackProcessor;
        }

        public MultiTrackProcessor()
        {
            _singleTrackProcessor = new SingleTrackProcessor();
        }

        /// <summary>
        /// Process polyphonic expression (e.g., "abc+def") into harmonic sequences
        /// </summary>
        /// <param name="numericSequention">Polyphonic expression with + operator</param>
        /// <param name="outputFormat">Target number format</param>
        /// <param name="inputFormat">Source number format</param>
        /// <returns>List containing single harmonically combined sequence</returns>
        public IList<Sequiention> Process(string numericSequention, NumberFormats outputFormat, NumberFormats inputFormat)
        {
            if (string.IsNullOrEmpty(numericSequention))
                return new List<Sequiention> { new Sequiention { Tones = new List<Tone>(), Title = "Empty", TotalDuration = TimeSpan.Zero } };

            // Parse the expression into individual parts
            string[] parts = ExpressionParser.ParseExpression(numericSequention);

            if (parts.Length == 0)
                return new List<Sequiention> { new Sequiention { Tones = new List<Tone>(), Title = "Empty", TotalDuration = TimeSpan.Zero } };

            // Process each part independently using SingleTrackProcessor through interface
            var sequences = new List<Sequiention>();
            foreach (string part in parts)
            {
                ITonesProcessor processor = _singleTrackProcessor;
                var partSequences = processor.Process(part, outputFormat, inputFormat);
                if (partSequences != null && partSequences.Count > 0)
                {
                    sequences.Add(partSequences[0]); // SingleTrackProcessor returns exactly one sequence
                }
            }
            return sequences;
        }
    }
}