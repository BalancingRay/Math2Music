# Math2Music - Convert Numbers to Musical Tones

## Overview

Math2Music is a C# console application that converts numeric sequences into rich musical sounds by mapping digits to frequencies with advanced audio processing capabilities. The application now supports both monophonic and polyphonic music generation, WAV file output, configurable sound settings, timber profiles, and multiple audio processing modes including multi-octave harmonics.

## Core Concept

The application implements a sophisticated mathematical-to-musical mapping system:
- Each digit (0-F) is mapped to a frequency multiplier 
- Configurable base frequency (default: 180 Hz) and duration (default: 300ms)
- Each digit creates a tone at `baseFrequency × digitValue`
- For example: digit '4' generates a tone at 180Hz × 4 = 720Hz
- Supports polyphonic music using `+` operator (e.g., `123+456` creates harmonic chords)
- Multiple output formats: Console beeps, WAV files with stereo positioning
- Rich timber profiles simulate natural instruments (Piano, Guitar, Violin) and synthetic sounds
- Multi-octave processing creates harmonically rich compositions

## Project Structure

```
Sources/MathToMusic/
├── Program.cs                     # Main application entry point with rich configuration UI
├── ExpressionParser.cs           # Polyphonic expression parser (123+456 notation)
├── MathToMusic.csproj            # Project configuration file
├── Contracts/                    # Interface definitions
│   ├── ITonesProcessor.cs        # Interface for tone processing logic  
│   ├── ISequenceProcessor.cs     # Interface for sequence processing
│   ├── ITonesOutput.cs          # Interface for audio output + core data models
│   └── ITonesFileOutput.cs      # Interface for file-based outputs
├── Models/                       # Data models and enums
│   ├── NumberFormats.cs         # Enum: Bin, Qad, Oct, Dec, Hex, Base32
│   └── TimberProfiles.cs        # 18+ predefined timber profiles (Piano, Guitar, etc.)
├── Processors/                   # Audio processing implementations
│   ├── SingleTrackProcessor.cs  # Basic monophonic processing
│   ├── ReachSingleTrackProcessor.cs # Multi-octave harmonic processing
│   ├── MultiTrackProcessor.cs   # Polyphonic music processing
│   ├── TimberSequenceProcessor.cs # Timber profile application
│   └── HarmonicCombiner.cs      # Harmonic combination logic
├── Outputs/                      # Audio output implementations
│   ├── BeepOutput.cs            # Console.Beep audio output
│   ├── WavFileOutput.cs         # Professional WAV file generation
│   ├── OpenFileOutput.cs        # Auto-open generated files
│   ├── OpenFileLocationOutput.cs # Auto-open file locations
│   └── TestMelodyOutput.cs      # Testing and demonstration output
├── Extensions/                   # Fluent API extensions
│   └── FileOutputExtensions.cs  # Chainable file output operations
├── Demo/                         # Demonstration code
│   └── FluentApiDemo.cs         # WAV output fluent API examples
└── Utils/                        # Utility classes
    ├── CommonNumbers.cs         # Mathematical constants (PI, E, Golden Ratio)
    └── NumberConverter.cs       # Base conversion utilities
```

## Core Classes and Components

### 1. Program.cs - Enhanced Application Interface
**Purpose**: Rich interactive console interface with comprehensive configuration options

**Key Features**:
- **Session Configuration**: Configurable base tone frequency and duration
- **Timber Profile Selection**: Choose from 18+ predefined instrument profiles
- **Multiple Number Formats**: Binary, Quaternary, Octal, Decimal, Hexadecimal, Base32
- **Polyphonic Processing**: Support for `+` operator notation (e.g., `123+456`)
- **Processor Selection**: Choose between simple or ReachSingleTrackProcessor
- **WAV File Output**: Professional audio file generation with auto-open capability
- **Mathematical Constants**: Easy access to predefined constants (PI, E, Golden Ratio)

### 2. Audio Processors - Multi-tier Processing Architecture

#### SingleTrackProcessor.cs - Basic Monophonic Processing
**Purpose**: Converts numeric sequences to single-track musical tones

**Key Properties**:
- `baseToneHz`: Configurable base frequency (default: 180 Hz)
- `baseDurationMilliseconds`: Configurable duration per tone (default: 300ms)
- `toneMap`: Dictionary mapping characters (0-F) to frequency multipliers

#### ReachSingleTrackProcessor.cs - Multi-Octave Harmonic Processing  
**Purpose**: Creates rich harmonic content by generating multiple octave groups

**Key Features**:
- **Multiple Octave Groups**: Low, MidLow, MidHigh, High frequency ranges
- **Harmonic Richness**: Distributes digits across different octave ranges
- **Synchronized Timing**: All octave groups maintain consistent duration
- **Advanced Filtering**: Smart digit distribution based on value ranges

#### MultiTrackProcessor.cs - Polyphonic Music Processing
**Purpose**: Handles polyphonic expressions with `+` operator for harmonic music

**Key Features**:
- **Expression Parsing**: Automatically detects and parses `123+456` notation
- **Stereo Positioning**: Places different sequences across stereo field
- **Harmonic Combination**: Synchronizes multiple sequences for chord creation
- **Dynamic Scaling**: Prevents clipping when mixing multiple sequences

#### TimberSequenceProcessor.cs - Timber Profile Application
**Purpose**: Applies instrument-specific harmonic profiles to generated sequences

**Key Features**:
- **Overtone Generation**: Creates realistic instrument sounds using harmonic series
- **18+ Predefined Profiles**: Piano, Guitar, Violin, Flute, Trumpet, Saxophone, etc.
- **Synthetic Sounds**: Sawtooth, Square, Triangle, Sine wave profiles
- **Custom Coefficients**: User-definable overtone amplitude ratios

### 3. Audio Output System - Professional File and Console Output

#### WavFileOutput.cs - Professional WAV File Generation
**Purpose**: High-quality stereo WAV file generation with advanced audio processing

**Key Features**:
- **CD Quality Audio**: 44.1kHz, 16-bit, stereo output
- **Stereo Positioning**: Automatic stereo field distribution for polyphonic sequences
- **Dynamic Range Management**: Intelligent normalization and amplitude scaling
- **Frequency-Based Amplitude**: Low frequencies receive appropriate boost for audibility
- **Anti-Clipping Protection**: Sophisticated multi-sequence mixing without distortion
- **Results Directory**: Organized output in `Results/` folder with timestamped filenames

#### Fluent API Extensions (FileOutputExtensions.cs)
**Purpose**: Chainable post-processing operations for WAV files

**Fluent Methods**:
- `.OpenFile()` - Auto-opens generated WAV file in default application
- `.OpenFileLocation()` - Opens Windows Explorer at file location
- **Chainable**: `new WavFileOutput().OpenFileLocation().OpenFile()`
- **Cross-Platform**: Graceful fallback on non-Windows systems

#### BeepOutput.cs - Console Audio Output
**Purpose**: Immediate console-based audio feedback using system beeps
- Real-time playback during processing
- Windows-specific Console.Beep API
- Useful for quick testing and immediate feedback

### 4. Data Models and Core Structures

#### Tone (struct) - Enhanced Audio Representation  
**Purpose**: Represents a single musical tone with rich harmonic content
- `Duration`: Precise timing control via TimeSpan
- `ObertonFrequencies`: Array supporting multiple harmonics and overtones
- `BaseTone`: Convenient access to fundamental frequency

#### Sequiention (class) - Musical Sequence Container
**Purpose**: Represents a complete musical sequence with metadata
- `TotalDuration`: Total playback duration for synchronization
- `Tones`: List of individual tone objects
- `Title`: Descriptive name for identification
- `Timber`: Optional timber profile coefficients for harmonic shaping

### 5. Enhanced Models and Utilities

#### NumberFormats (enum) - Extended Format Support
Supported numeric systems:
- `Bin = 2` (Binary)  
- `Qad = 4` (Quaternary)
- `Oct = 8` (Octal)
- `Dec = 10` (Decimal)
- `Hex = 16` (Hexadecimal)
- `Base32 = 32` (Base32) - **NEW**: Extended support for Base32 encoding

#### TimberProfiles.cs - Comprehensive Instrument Library
**Purpose**: Provides predefined timber profiles for realistic instrument simulation

**Natural Instruments** (9 profiles):
- Piano, Guitar, Violin, Flute, Trumpet, Organ, Clarinet, Saxophone, Cello, Oboe

**Synthetic Sounds** (7 profiles):
- Sawtooth, Square, Triangle, Pulse, Bright, Warm, Metallic, Bell, Pad, Sine

**Special Effects** (4 profiles):
- Hollow, Nasal, Growl, Ethereal

**Key Methods**:
- `GetAvailableProfiles()` - List all available timber profiles
- `HasProfile(name)` - Check if profile exists
- `GetProfile(name)` - Retrieve coefficients for specific profile

#### ExpressionParser.cs - Polyphonic Notation Parser
**Purpose**: Parses mathematical expressions with musical operators

**Key Features**:
- **Plus Operator**: Parses `123+456+789` into individual sequences
- **Polyphonic Detection**: `IsPolyphonic()` identifies multi-track expressions  
- **Clean Parsing**: Handles whitespace and empty segments gracefully
- **Array Output**: Returns string array of individual numeric sequences

#### CommonNumbers.cs - Mathematical Constants Library
**Purpose**: Provides easy access to important mathematical constants

**Available Constants**:
- **PI variations**: PI100, PI1000 (π with 100 and 1000 digit precision)
- **Golden Ratio**: FI100 (φ with 100 digit precision)  
- **Euler's Number**: E100, E1000 (e with 100 and 1000 digit precision)
- **Square Roots**: SQRT2, SQRT3 (√2 and √3)
- **Additional**: Various other mathematical constants in the collection

## Usage Examples

### Session Configuration and Setup
1. **Launch Application**: `dotnet run`
2. **Configure Settings**: Optionally customize base frequency and duration
3. **Select Timber Profile**: Choose from Piano, Guitar, Violin, or 15+ other profiles
4. **Choose Processor**: Basic SingleTrack or advanced ReachSingleTrack with harmonics

### Basic Monophonic Usage
1. Enter a number (e.g., "1234")
2. Specify output formats (e.g., "2,8,16" for binary, octal, hex)  
3. Generated WAV files automatically saved in Results/ folder
4. Files auto-open in default application (Windows)

### Polyphonic Music Creation
1. Use `+` operator to combine sequences: `123+456`
2. Create complex harmonies: `ABC+DEF+789` 
3. Different sequences positioned across stereo field
4. All sequences synchronized for harmonic chord progression

### Advanced Multi-Octave Processing  
1. Select "ReachSingleTrackProcessor" when prompted
2. Single input generates multiple octave groups (Low, MidLow, MidHigh, High)
3. Rich harmonic content with frequency distribution across ranges
4. Creates orchestral-depth sound from simple numeric input

### Mathematical Constants Usage
Enter predefined constants by name:
- **"PI100"** - Plays first 100 digits of π (approximately 30 seconds)
- **"PI1000"** - Extended 1000-digit π sequence 
- **"FI100"** - Golden ratio φ with 100 digits
- **"E100"** - Euler's number e with 100 digits
- **"E1000"** - Extended 1000-digit e sequence
- **"SQRT2"**, **"SQRT3"** - Square root constants

### Advanced Format Conversion and Base32 Support
1. **Set Input Format**: Enter format number (2=Binary, 8=Octal, 10=Decimal, 16=Hex, 32=Base32)
2. **Enter Number**: Input number in selected format (e.g., "1010" for binary)
3. **Select Output Formats**: Specify multiple formats for comparison (e.g., "2,8,10,16,32")
4. **Base32 Support**: Full support for Base32 encoding with characters 0-9, A-V

### Timber Profile Application
1. **Select Profile**: Choose from 18+ available profiles during configuration
2. **Natural Instruments**: Piano, Guitar, Violin, Flute create realistic instrument sounds
3. **Synthetic Waveforms**: Sawtooth, Square, Triangle for electronic music
4. **Special Effects**: Hollow, Nasal, Growl, Ethereal for unique textures

### WAV File Output and Fluent API  
```csharp
// Basic WAV output
var output = new WavFileOutput();

// With auto-open functionality (fluent API)
var output = new WavFileOutput().OpenFileLocation().OpenFile();
```
- Professional CD-quality stereo WAV files (44.1kHz, 16-bit)
- Automatic file organization in Results/ directory
- Cross-platform compatible with graceful Windows-specific feature fallback

## Frequency Mapping

The application uses a configurable linear frequency mapping system:

| Digit/Character | Multiplier | Default Frequency (Hz)* |
|-----------------|------------|-------------------------|
| 0               | 0          | 0 (Silence)            |
| 1               | 1          | 180                    |
| 2               | 2          | 360                    |
| 3               | 3          | 540                    |
| 4               | 4          | 720                    |
| 5               | 5          | 900                    |
| 6               | 6          | 1080                   |
| 7               | 7          | 1260                   |
| 8               | 8          | 1440                   |
| 9               | 9          | 1620                   |
| A               | 10         | 1800                   |
| B               | 11         | 1980                   |
| C               | 12         | 2160                   |
| D               | 13         | 2340                   |
| E               | 14         | 2520                   |
| F               | 15         | 2700                   |
| G               | 16         | 2880                   |
| H               | 17         | 3060                   |
| I               | 18         | 3240                   |
| J               | 19         | 3420                   |
| K               | 20         | 3600                   |
| L               | 21         | 3780                   |
| M               | 22         | 3960                   |
| N               | 23         | 4140                   |
| O               | 24         | 4320                   |
| P               | 25         | 4500                   |
| Q               | 26         | 4680                   |
| R               | 27         | 4860                   |
| S               | 28         | 5040                   |
| T               | 29         | 5220                   |
| U               | 30         | 5400                   |
| V               | 31         | 5580                   |

*Default frequencies shown with 180Hz base frequency. Both base frequency and duration are user-configurable.*

**Frequency Calculation**: `frequency = baseFrequency × digitValue`
**Example**: With base frequency 180Hz, digit '4' produces 180 × 4 = 720Hz

## Technical Notes

### Platform Compatibility
- **Windows**: Full functionality with Console.Beep + WAV file generation
- **Linux/macOS**: Complete WAV file generation, limited console beep capability
- **Cross-Platform**: WAV output works identically across all platforms
- **Windows-Specific Features**: Auto-open files and file locations (graceful fallback on other platforms)

### Performance Characteristics  
- **Memory Usage**: Linear O(n) scaling with input length
- **Audio Processing**: Real-time generation with professional quality output
- **File I/O**: Efficient WAV file generation with configurable sample rates
- **Timing Precision**: Accurate duration control via TimeSpan objects
- **Polyphonic Processing**: Intelligent stereo field distribution and anti-clipping normalization

### Audio Quality Features
- **Professional Output**: CD-quality 44.1kHz, 16-bit stereo WAV files
- **Dynamic Range Management**: Automatic normalization prevents clipping in polyphonic mixes
- **Frequency-Based Amplitude**: Lower frequencies receive appropriate boost for audibility
- **Stereo Positioning**: Polyphonic sequences distributed across stereo field for clarity
- **Timber Processing**: Realistic instrument simulation through harmonic coefficient application

### Advanced Processing Capabilities  
- **Multi-Octave Generation**: ReachSingleTrackProcessor creates harmonically rich compositions
- **Polyphonic Synchronization**: Multiple sequences precisely time-aligned for harmonic music
- **Configurable Parameters**: User control over base frequency, duration, and timber profiles
- **Expression Parsing**: Robust parsing of complex polyphonic expressions with + notation
- **Format Flexibility**: Seamless conversion between all supported numeric formats including Base32

### Current Capabilities (Previously Limitations)
1. ✅ **Multi-Platform Audio**: WAV file output works across all platforms
2. ✅ **Configurable Parameters**: User-adjustable tone duration and base frequency  
3. ✅ **Rich Harmonic Content**: Full timber profile support with overtone generation
4. ✅ **Polyphonic Processing**: Multi-track harmonic music generation
5. ✅ **Professional Output**: High-quality stereo WAV file generation

## Extensibility

The application features a sophisticated, extensible architecture with multiple extension points:

### 1. Audio Output Extensions
- **`ITonesOutput`**: Implement for new audio output methods (MIDI, MP3, etc.)
- **`ITonesFileOutput`**: Extend for new file-based outputs with return path capability
- **Fluent API Pattern**: Easily chainable post-processing operations via extension methods

### 2. Audio Processing Extensions  
- **`ITonesProcessor`**: Create new conversion algorithms and processing logic
- **`ISequenceProcessor`**: Implement custom sequence processing pipelines
- **Timber Profiles**: Add new instrument profiles to `TimberProfiles.cs`
- **Expression Parsing**: Extend `ExpressionParser` for new musical operators

### 3. Advanced Processing Features
- **Multi-Octave Processing**: `ReachSingleTrackProcessor` demonstrates sophisticated harmonic generation
- **Polyphonic Composition**: `MultiTrackProcessor` shows multi-track synchronization
- **Harmonic Infrastructure**: Full overtone support in `Tone` struct enables rich harmonic content
- **Mathematical Constants**: Easily extend `CommonNumbers` collection

### 4. Cross-Platform Compatibility
- **Platform-Specific Features**: Graceful fallback for Windows-specific functionality
- **File System Integration**: Configurable output directories and file management
- **Audio Format Support**: Professional WAV generation with configurable parameters

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

- **.NET 8.0**: Modern C# features and performance optimizations
- **No External NuGet Packages**: Self-contained with built-in .NET capabilities
- **Built-in Audio APIs**: Uses Console.Beep for immediate feedback (Windows)
- **Native File I/O**: Professional WAV file generation using standard .NET libraries
- **Cross-Platform Compatibility**: Runs on Windows, Linux, and macOS

## Future Enhancement Ideas

### Completed Features ✅
1. ~~Cross-platform audio~~ → **WAV file generation implemented**
2. ~~Configurable parameters~~ → **Base frequency and duration now user-configurable**
3. ~~Harmonic generation~~ → **Full timber profile system with 18+ instruments**
4. ~~File output~~ → **Professional WAV file generation with fluent API**

### Potential Future Enhancements
1. **MIDI Output**: Generate MIDI files for use with external music software and DAWs
2. **Additional Audio Formats**: MP3, FLAC, OGG export capabilities
3. **Advanced Musical Features**: 
   - Musical scale mapping (Pentatonic, Chromatic, etc.)
   - Time signature support and rhythmic patterns
   - Chord progression analysis
4. **Visualization Components**:
   - Real-time frequency spectrum display
   - Waveform visualization
   - Musical notation export
5. **Advanced Expression Language**:
   - Mathematical operators beyond `+` (-, *, /, ^)
   - Conditional logic and looping constructs  
   - Variable assignment and function definitions
6. **Performance Optimizations**:
   - Multi-threaded audio generation
   - GPU-accelerated processing for large datasets
   - Streaming audio for very long sequences
7. **Educational Features**:
   - Interactive tutorials for mathematical concepts
   - Integration with educational platforms
   - Real-time parameter adjustment during playback