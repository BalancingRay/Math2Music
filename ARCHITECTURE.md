# Math2Music Architecture Documentation

## System Architecture Overview

```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   Console UI    │───▶│  Tone Processor  │───▶│  Audio Output   │
│   (Program.cs)  │    │(SingleTrackProc.)│    │  (BeepOutput)   │
└─────────────────┘    └──────────────────┘    └─────────────────┘
         │                        │                        
         ▼                        ▼                        
┌─────────────────┐    ┌──────────────────┐                
│  Input Parsing  │    │   Data Models    │                
│ (NumberFormats) │    │ (Tone, Sequence) │                
└─────────────────┘    └──────────────────┘                
         │                                                 
         ▼                                                 
┌─────────────────┐                                        
│ Math Constants  │                                        
│(CommonNumbers)  │                                        
└─────────────────┘                                        
```

## Class Relationship Diagram

```
ITonesProcessor ◄───────── SingleTrackProcessor
     │                           │
     │                           │ uses
     ▼                           ▼
ITonesOutput   ◄─────────── BeepOutput
     │                           │
     │ defines                   │ implements
     ▼                           ▼
Sequiention ────contains───► List<Tone>
```

## Data Flow Process

### 1. Input Processing
```
User Input → Format Detection → Number Validation → Constant Lookup
                                                  ↓
                                            Original/Substituted Number
```

### 2. Tone Generation
```
Numeric String → Character Processing → Frequency Mapping → Tone Creation
     │                                                            │
     ▼                                                            ▼
Format Conversion                                           Duration Assignment
(Binary grouping)                                          (300ms per tone)
```

### 3. Audio Output
```
List<Tone> → Frequency Check → Audio Generation
                │                     │
                ▼                     ▼
           0Hz = Silence        Console.Beep(freq, duration)
```

## Key Design Patterns

### 1. Strategy Pattern
- `ITonesProcessor` interface allows for different processing strategies
- Currently only `SingleTrackProcessor` implemented
- Easy to extend with new conversion algorithms

### 2. Factory Pattern (Implicit)
- `Program.cs` creates processor and output instances
- Could be extended with explicit factory for dynamic type selection

### 3. Command Pattern (Implicit) 
- Each numeric input acts as a command to generate specific tone sequences
- Input format acts as command modifier

## Processing Algorithms

### Direct Mapping (Same Input/Output Format)
```csharp
for each character in input:
    if toneMap.contains(character):
        frequency = baseToneHz * toneMap[character]
        add Tone(frequency, baseDuration) to track
```

### Binary Conversion Algorithm
```csharp
binGroupSize = log2(outputFormat)
for i from (length-1) to 0 step binGroupSize:
    convertedValue = 0
    for j from 0 to binGroupSize-1:
        if input[i-j] == '1':
            convertedValue += 2^j
    
    convertedString = convertedValue.toString(outputFormat)
    frequency = baseToneHz * toneMap[convertedString[0]]
    add Tone(frequency, baseDuration) to track
```

## Memory and Performance Characteristics

### Memory Usage
- **Input Storage**: O(n) where n = input string length
- **Tone Generation**: O(n) for tone list storage  
- **Constant Lookup**: O(1) hashtable access
- **Total**: Linear memory growth with input size

### Time Complexity
- **Direct Mapping**: O(n) - single pass through input
- **Binary Conversion**: O(n) - still linear but with grouping overhead
- **Audio Output**: O(n) - sequential playback

### Audio Timing
- **Per Tone Duration**: 300ms fixed
- **Total Playback**: (number of digits) × 300ms
- **Example**: 100-digit PI = 30 seconds playback

## Extension Points

### 1. New Processors
```csharp
public class HarmonicProcessor : ITonesProcessor
{
    // Generate multiple overtones per digit
    // Support chord generation
    // Musical scale mapping
}
```

### 2. New Output Methods
```csharp
public class WaveFileOutput : ITonesOutput 
{
    // Generate WAV files
    // Support for complex waveforms
    // Cross-platform compatibility
}

public class MIDIOutput : ITonesOutput
{
    // Generate MIDI sequences
    // Support for instruments
    // Musical notation export
}
```

### 3. Enhanced Models
```csharp
public class Tone 
{
    // Current: single frequency + duration
    // Future: multiple harmonics, envelopes, effects
    public double[] Harmonics { get; set; }
    public EnvelopeSettings Envelope { get; set; }
    public EffectSettings Effects { get; set; }
}
```

## Configuration Architecture

### Current Hardcoded Values
```csharp
baseToneHz = 180;                    // Base frequency
baseDurationMilliseconds = 300;      // Tone duration  
toneMap = { '0':0, '1':1, ... };    // Frequency multipliers
```

### Potential Configuration System
```csharp
public class AudioSettings 
{
    public double BaseFrequency { get; set; } = 180;
    public int ToneDuration { get; set; } = 300;
    public Dictionary<char, int> FrequencyMap { get; set; }
    public bool UseHarmonics { get; set; } = false;
}
```

## Error Handling Strategy

### Current State
- Basic null/empty input validation
- Platform exception on non-Windows systems
- No graceful degradation for audio failures

### Recommended Improvements
```csharp
public interface ITonesOutput 
{
    bool CanPlayAudio { get; }
    AudioResult Send(IList<Sequiention> input);
}

public class AudioResult 
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public Exception Exception { get; set; }
}
```

## Testing Strategy

### Unit Testing Targets
1. **SingleTrackProcessor.Process()** - Core conversion logic
2. **Tone creation** - Frequency and duration calculations  
3. **Format conversions** - Binary to other base conversions
4. **CommonNumbers lookup** - Constant substitution
5. **Input validation** - Error handling paths

### Integration Testing
1. **End-to-end processing** - Input → Tones → Output
2. **Format compatibility** - All number format combinations
3. **Mathematical constants** - Verify correct constant values
4. **Cross-platform behavior** - Audio output variations

### Performance Testing
1. **Large input handling** - 1000+ digit numbers
2. **Memory usage patterns** - Long-running sessions
3. **Audio timing accuracy** - Playback duration verification