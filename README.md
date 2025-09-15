# Math2Music - Convert Numbers to Musical Tones

## Overview

Math2Music is a C# console application that converts numeric sequences into musical sounds by mapping digits to overtones. The core concept is to transform mathematical representations (like 1,2,3,4) into corresponding frequencies (like 100Hz, 200Hz, 300Hz, 400Hz), creating an audible representation of numerical data.

## Core Concept

The application implements a simple but powerful mathematical concept:
- Each digit (0-F) is mapped to a frequency multiplier
- Base frequency is 180 Hz 
- Each digit creates a tone at `baseFrequency × digitValue`
- For example: digit '4' generates a tone at 180Hz × 4 = 720Hz
- Duration of each tone is 300 milliseconds by default

## Project Structure

```
Sources/MathToMusic/
├── Program.cs                    # Main application entry point and user interface
├── MathToMusic.csproj           # Project configuration file
├── Contracts/                   # Interface definitions
│   ├── ITonesProcessor.cs       # Interface for tone processing logic
│   └── ITonesOutput.cs         # Interface for audio output + core data models
├── Models/                      # Data models and enums
│   └── NumberFormats.cs        # Enum defining supported number formats
├── Inputs/                      # Output implementations  
│   ├── BeepOutput.cs           # Console.Beep audio output implementation
│   └── SomeOutput.cs           # Musical note mapping (unused in current flow)
├── Utils/                       # Utility classes
│   └── CommonNumbers.cs        # Predefined mathematical constants
└── SingleTrackProcessor.cs     # Main tone processing implementation
```

## Core Classes and Components

### 1. Program.cs - Main Application Loop
**Purpose**: Entry point and user interaction handler

**Key Features**:
- Interactive console interface
- Support for multiple number formats (Binary, Quaternary, Octal, Decimal, Hexadecimal)
- Input format detection and conversion
- Output format selection
- Integration with predefined mathematical constants

**Key Functions**:
- `Main()` - Infinite loop handling user input and format selection
- `GetSize()` - Helper function for delimiter sizing (currently unused)

### 2. SingleTrackProcessor.cs - Core Processing Logic
**Purpose**: Converts numeric sequences to musical tones

**Key Properties**:
- `baseToneHz`: Base frequency (180 Hz)
- `baseDurationMilliseconds`: Duration per tone (300ms)
- `toneMap`: Dictionary mapping characters (0-F) to numeric multipliers

**Key Methods**:
- `Process()` - Main conversion method that handles:
  - Direct digit-to-tone conversion when input/output formats match
  - Binary-to-other format conversion with grouping logic

**Conversion Logic**:
1. **Same Format**: Each digit directly maps to a frequency
2. **Binary Conversion**: Groups binary digits based on target format, converts to target base, then maps to frequencies

### 3. Data Models (ITonesOutput.cs)

#### Tone (struct)
**Purpose**: Represents a single musical tone
- `Duration`: How long the tone plays
- `ObertonFrequencies`: Array of frequencies (supports harmonics, though currently single-frequency)

#### Sequiention (class)  
**Purpose**: Represents a sequence of tones (a musical phrase)
- `TotalDuration`: Total playback time
- `Tones`: List of individual tones
- `Title`: Descriptive name

### 4. Interfaces

#### ITonesProcessor
**Purpose**: Defines contract for tone processing implementations
- `Process()` - Converts numeric string to musical sequences

#### ITonesOutput  
**Purpose**: Defines contract for audio output implementations
- `Send()` - Plays the generated musical sequences

### 5. Output Implementations

#### BeepOutput.cs
**Purpose**: Produces audio using Console.Beep (Windows-specific)
- Handles frequency of 0 as silence (Thread.Sleep)
- Uses Console.Beep for actual tone generation
- **Note**: Limited to Windows platform

#### SomeOutput.cs (Unused)
**Purpose**: Contains musical note mappings for potential future implementations
- Maps digits to standard musical notation
- Contains filters for different number formats
- Currently not integrated into main application flow

### 6. Models and Utilities

#### NumberFormats (enum)
Supported numeric systems:
- `Bin = 2` (Binary)
- `Qad = 4` (Quaternary) 
- `Oct = 8` (Octal)
- `Dec = 10` (Decimal)
- `Hex = 16` (Hexadecimal)

#### CommonNumbers.cs
**Purpose**: Provides predefined mathematical constants for easy access
- PI (100 and 1000 digit precision)
- Golden Ratio (FI100)
- Euler's number (E100, E1000)
- Square roots (SQRT2, SQRT3)

## Usage Examples

### Basic Usage
1. Run the application: `dotnet run`
2. Enter a number (e.g., "1234")
3. Optionally specify output formats (e.g., "2,8,16" for binary, octal, hex)
4. Listen to the generated tones

### Mathematical Constants
Enter predefined constants by name:
- "PI100" - Plays first 100 digits of π
- "FI100" - Plays golden ratio
- "E100" - Plays Euler's number

### Format Conversion
1. Enter format number to change input format (e.g., "2" for binary)
2. Enter binary number (e.g., "1010")
3. Specify output formats for different interpretations

## Frequency Mapping

The application uses a simple linear mapping:

| Digit | Multiplier | Frequency (Hz) |
|-------|------------|----------------|
| 0     | 0          | 0 (Silence)    |
| 1     | 1          | 180            |
| 2     | 2          | 360            |
| 3     | 3          | 540            |
| 4     | 4          | 720            |
| 5     | 5          | 900            |
| 6     | 6          | 1080           |
| 7     | 7          | 1260           |
| 8     | 8          | 1440           |
| 9     | 9          | 1620           |
| A     | 10         | 1800           |
| B     | 11         | 1980           |
| C     | 12         | 2160           |
| D     | 13         | 2340           |
| E     | 14         | 2520           |
| F     | 15         | 2700           |

## Technical Notes

### Platform Compatibility
- **Windows**: Full functionality with Console.Beep
- **Linux/macOS**: Builds successfully but audio output may not work due to Console.Beep limitations

### Performance Considerations
- Each tone plays for 300ms, so longer numbers result in longer playback times
- Memory usage scales with input length
- Binary conversion involves mathematical calculations that may impact performance for very large inputs

### Current Limitations
1. Audio output is Windows-specific (Console.Beep)
2. Single-threaded playback (sequential tone generation)
3. Fixed tone duration and base frequency
4. Limited to single-frequency tones (no harmonics despite infrastructure support)

## Extensibility

The application is designed with extensibility in mind:

1. **New Output Methods**: Implement `ITonesOutput` for different audio systems
2. **Processing Algorithms**: Implement `ITonesProcessor` for alternative conversion logic  
3. **Harmonic Support**: `Tone` struct supports multiple frequencies for richer sounds
4. **Mathematical Constants**: Easy to add new constants to `CommonNumbers`

## Building and Running

```bash
# Clone the repository
git clone https://github.com/BalancingRay/Math2Music.git

# Navigate to project directory
cd Math2Music/Sources/MathToMusic

# Build the project
dotnet build

# Run the application  
dotnet run
```

## Dependencies

- .NET 8.0
- No external NuGet packages required
- Uses built-in Console.Beep for audio output

## Future Enhancement Ideas

1. **Cross-platform audio**: Integrate with audio libraries like NAudio or FMOD
2. **Configurable parameters**: Allow users to adjust base frequency, duration, etc.
3. **Harmonic generation**: Use the existing overtone infrastructure for richer sounds
4. **Musical scales**: Map numbers to specific musical scales instead of linear frequency
5. **File output**: Generate WAV/MP3 files instead of real-time playback
6. **Visualization**: Add graphical representation of the frequency patterns
7. **MIDI output**: Generate MIDI files for use with external music software