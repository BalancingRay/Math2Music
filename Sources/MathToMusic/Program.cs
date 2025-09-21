// See https://aka.ms/new-console-template for more information
using MathToMusic;
using MathToMusic.Contracts;
using MathToMusic.Extensions;
using MathToMusic.Models;
using MathToMusic.Outputs;
using MathToMusic.Processors;
using MathToMusic.Utils;


char[] decimalDelimiter = new char[] { ',', '.' };
Console.WriteLine("Hello! It's Math To Music converter v1.");
Console.WriteLine();

// Session Configuration
double baseTone = 180; // Default base tone
int baseDuration = 300; // Default base duration

Console.WriteLine("=== SESSION CONFIGURATION ===");
Console.WriteLine($"Current sound settings - Base Tone: {baseTone}Hz, Duration: {baseDuration}ms");
Console.Write("Would you like to configure sound settings? (y/N): ");
string? configChoice = Console.ReadLine();

if (!string.IsNullOrEmpty(configChoice) && configChoice.ToLower().StartsWith("y"))
{
    Console.Write($"Enter base tone frequency in Hz (current: {baseTone}): ");
    string? toneInput = Console.ReadLine();
    if (!string.IsNullOrEmpty(toneInput) && double.TryParse(toneInput, out double newTone) && newTone > 0)
    {
        baseTone = newTone;
    }

    Console.Write($"Enter base duration in milliseconds (current: {baseDuration}): ");
    string? durationInput = Console.ReadLine();
    if (!string.IsNullOrEmpty(durationInput) && int.TryParse(durationInput, out int newDuration) && newDuration > 0)
    {
        baseDuration = newDuration;
    }

    Console.WriteLine($"Sound settings updated - Base Tone: {baseTone}Hz, Duration: {baseDuration}ms");
}

Console.WriteLine();
Console.WriteLine("=== AVAILABLE INPUT OPTIONS ===");
Console.WriteLine("1. Custom numeric sequences (e.g., 123456, ABCDEF for HEX)");
Console.WriteLine("2. Mathematical constants:");

// Show available constants from CommonNumbers
var constantKeys = CommonNumbers.Collection.Keys.Take(10).ToList();
foreach (var key in constantKeys)
{
    var preview = CommonNumbers.Collection[key];
    var shortPreview = preview.Length > 30 ? preview.Substring(0, 30) + "..." : preview;
    Console.WriteLine($"   - {key} ({shortPreview})");
}

Console.WriteLine("3. Operators: Use '+' to combine sequences for polyphonic music");
Console.WriteLine("   Example: '123+456' creates harmony between two sequences");
Console.WriteLine();

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

    Console.WriteLine("Do you prefere to use reach sound processor: Y / N (skip) ?");
    string yes = "y";
    var userInput = Console.ReadLine();
    ITonesProcessor singleProcessor = yes.Equals(userInput, StringComparison.OrdinalIgnoreCase) 
        ? new ReachSingleTrackProcessor(baseDuration, baseTone) 
        : new SingleTrackProcessor(baseDuration, baseTone);
    // Choose processor based on whether input contains + operator for polyphonic processing
    ITonesProcessor processor = ExpressionParser.IsPolyphonic(input)
        ? new MultiTrackProcessor(singleProcessor)
        : singleProcessor;
    ITonesFileOutput output = new WavFileOutput().OpenFile();
    foreach (var outputFormat in outFormats)
    {
        var song = processor.Process(input, outputFormat, inputFormat);
        if (song.Count > 0 && song[0].TotalDuration > TimeSpan.Zero)
        {
            output.Send(song);
        }

        Console.WriteLine($"Result: ");
        //Console.WriteLine(resultBuilder.ToString());
    }
}


