using MathToMusic.Models;

namespace MathToMusic.Contracts
{
    public interface ITonesProcessor
    {
        IList<Sequiention> Process(string numericSequention, NumberFormats outputFormat, NumberFormats inputFormat);
    }
}
