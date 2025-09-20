// See https://aka.ms/new-console-template for more information
using MathToMusic;
using MathToMusic.Contracts;
using MathToMusic.Inputs;
using MathToMusic.Models;
using MathToMusic.Utils;


char[] decimalDelimiter = new char[] { ',', '.' };
Console.WriteLine("Hello! It's Math To Music converter v1.");
NumberFormats inputFormat = NumberFormats.Dec;
NumberFormats[] outFormats = new NumberFormats[] { NumberFormats.Dec };
while (true)
{
    Console.WriteLine($"Please send numeric input in {inputFormat} format. (or set numeric system. 2 for BIN)");
    string? input = Console.ReadLine();

    //Console.WriteLine("Please send numeric input in DEC format");
    //var input = Console.ReadLine();

    if (int.TryParse(input, out var inputFormatBase)
        && Enum.GetValues<NumberFormats>().Contains((NumberFormats)inputFormatBase))
    {
        inputFormat = (NumberFormats)inputFormatBase;
        Console.WriteLine($"Please send numeric input in {inputFormat} format");
        input = Console.ReadLine();
    }
    if (string.IsNullOrEmpty(input))
    {
        Console.WriteLine("Incorrect input");
        continue;
    }

    input = input.ToUpper();

    Console.WriteLine($"Set prefer numeric systems: 2,4,8,10,16 or skip. Current is {outFormats[0]}");
    string? preferFormatString = Console.ReadLine();
    if (string.IsNullOrEmpty(preferFormatString) is false)
    {
        string[] outFormatsValue = preferFormatString.Split(',');
        outFormats = preferFormatString
            .Split(',')
            .Where(format => int.TryParse(format, out var formatValue)
                && Enum.GetValues<NumberFormats>().Contains((NumberFormats)formatValue))
            .Select(format => (NumberFormats)int.Parse(format))
            .ToArray();
    }

    if (CommonNumbers.Collection.TryGetValue(input, out string commonNumber))
    {
        input = commonNumber;
    }

    // Ask user for processing preference
    Console.WriteLine("Choose processing method: 1 for single track (monophonic), 2 for multi track (polyphonic)");
    string? processingChoice = Console.ReadLine();
    ITonesProcessor processor = processingChoice == "2" ? new MultiTrackProcessor() : new SingleTrackProcessor();
    
    // Ask user for output preference
    Console.WriteLine("Choose output method: 1 for Beep (console), 2 for WAV file");
    string? outputChoice = Console.ReadLine();
    ITonesOutput output = outputChoice == "2" ? new WavFileOutput() : new BeepOutput();
    foreach (var outputFormat in outFormats)
    {
        var song = processor.Process(input, outputFormat, inputFormat);
        if (song.Count > 0 && song[0].TotalDuration > TimeSpan.Zero)
        {
            output.Send(song);
        }
        //if (false)
        //{
        //    Console.WriteLine($"Converted number ({outputFormat} by binGroup {binGroupSize}): {convertedBuilder}");
        //}
        //else
        //{
        //    Console.WriteLine($"Number converting from {inputFormat} to {outputFormat} not supported");
        //    continue;
        //}

        Console.WriteLine($"Result: ");
        //Console.WriteLine(resultBuilder.ToString());
    }
    
    // Ask if user wants to continue
    Console.WriteLine("Process another number? (y/n, or press Enter to exit)");
    string? continueChoice = Console.ReadLine();
    if (string.IsNullOrEmpty(continueChoice) || continueChoice.ToLower() != "y")
    {
        break;
    }
}


