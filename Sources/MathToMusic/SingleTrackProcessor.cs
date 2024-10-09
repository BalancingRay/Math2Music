using MathToMusic.Contracts;
using MathToMusic.Models;

namespace MathToMusic
{
    public class SingleTrackProcessor : ITonesProcessor
    {
        double baseToneHz = 180;
        int baseDurationMilliseconds = 300;

        Dictionary<char, int> toneMap = new Dictionary<char, int>()
        {
            { '0', 0 },
            { '1', 1 },
            { '2', 2 },
            { '3', 3 },
            { '4', 4 },
            { '5', 5 },
            { '6', 6 },
            { '7', 7 },
            { '8', 8 },
            { '9', 9 },
            { 'A', 10},
            { 'B', 11},
            { 'C', 12},
            { 'D', 13},
            { 'E', 14},
            { 'F', 15},
        };

        IList<Sequiention> ITonesProcessor.Process(string numericSequention, NumberFormats outputFormat, NumberFormats inputFormat)
        {
            var track = new List<Tone>();
            if (inputFormat == outputFormat)
            {
                for (var i = 0; i < numericSequention.Length; i++)
                {
                    if (toneMap.TryGetValue(numericSequention[i], out var tone))
                    {
                        track.Add(new Tone(baseToneHz * tone, baseDurationMilliseconds));
                    }

                    //if (zeroNoteConverter.TryGetValue(input[i], out var convertedNote))
                    //{
                    //    resultBuilder.Append(convertedNote);
                    //    resultBuilder.Append(GetSize(delimiterPosition, i));
                    //    resultBuilder.Append(' ');
                    //}
                }
            }
            else if (inputFormat is NumberFormats.Bin
                && outputFormat is not NumberFormats.Dec)
            {
                //var convertedBuilder = new StringBuilder();
                int binGroupSize = 1;
                int outputIndex = (int)outputFormat;
                while (outputIndex > 2)
                {
                    outputIndex /= 2;
                    binGroupSize += 1;
                }
                string format = "0";
                if (outputFormat is NumberFormats.Hex)
                    format = "X";

                int delimiterPosition = -1;// input.IndexOfAny(decimalDelimiter);
                if (delimiterPosition < 0)
                    delimiterPosition = numericSequention.Length;
                for (var i = delimiterPosition - 1; i >= 0; i -= binGroupSize)
                {
                    int convertedValue = 0;
                    for (int j = 0; j < binGroupSize; j++)
                    {
                        if (i - j >= 0 && numericSequention[i - j].Equals('1'))
                            convertedValue += (int)Math.Pow(2, j);
                    }
                    //convertedBuilder.Insert(0, convertedValue.ToString(format));
                    string convertedString = convertedValue.ToString(format);
                    if (toneMap.TryGetValue(convertedString[0], out var convertedTone))
                    {
                        track.Add(new Tone(baseToneHz * convertedTone, baseDurationMilliseconds));
                        //    resultBuilder.Insert(0, "4 ");
                        //    resultBuilder.Insert(0, convertedNote);
                    }
                }
            }

            var song = new List<Sequiention>()
        {
            new Sequiention()
            {
                Tones = track,
                TotalDuration = TimeSpan.FromMilliseconds(track.Sum(t=> t.Duration.TotalMilliseconds)),
                Title = "Single"
            }
        };
            return song;
        }
    }
}
