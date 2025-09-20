# Math2Music Operator Language Specification

## Overview

The Math2Music Operator Language transforms mathematical expressions into musical compositions by providing intuitive operators that combine, modify, and synchronize numeric sequences into sound patterns. This specification defines the core operators, their behaviors, and implementation guidelines for the Math2Music system.

## Core Operators

### `+` (Harmonic Addition) - Simultaneous Playback
**Syntax:** `sequence1+sequence2`  
**Behavior:** Plays both sequences simultaneously as harmonic combinations  
**Example:** `123+456` → produces three simultaneous tone pairs: (1,4), (2,5), (3,6)  
**Musical Effect:** Creates chord progressions and harmonic textures  

### `*` (Frequency Modulation Up) - Pitch Multiplication  
**Syntax:** `base_sequence*modulator_sequence`  
**Behavior:** Multiplies base sequence frequencies by corresponding modulator values  
**Example:** `123*245` → tone `1` frequency × 2, tone `2` frequency × 4, tone `7` frequency × 5  
**Musical Effect:** Creates upward pitch shifts and harmonic overtones  

### `/` (Frequency Division Down) - Pitch Division with Filtering
**Syntax:** `base_sequence/divisor_sequence`  
**Behavior:** Divides base frequencies by divisor values, filters out negative/zero results  
**Example:** `987/321` → tone `9` frequency ÷ 3, tone `8` frequency ÷ 2, tone `7` frequency ÷ 1  
**Musical Effect:** Creates lower register variations while maintaining musical coherence  

### `&` (Temporal Synchronization) - Time-Domain Mixing
**Syntax:** `(expression1)&(expression2)&...`  
**Behavior:** Synchronizes multiple expressions over time, extending shorter sequences to match longer ones  
**Example:** `(18+29)&(333+444+555)` → synchronizes two harmonic groups temporally  
**Musical Effect:** Creates complex polyrhythmic textures and evolving soundscapes  

## Expression Syntax Rules

### Operator Precedence (Highest to Lowest)
1. Parentheses `()`
2. Multiplication `*` and Division `/` (left-to-right)  
3. Addition `+` (left-to-right)
4. Synchronization `&` (left-to-right)

### Sequence Format Support
- **Decimal:** `123`, `456.789`
- **Binary:** `101`, `1010` (when input format = BIN)
- **Hexadecimal:** `ABC`, `1F2E` (when input format = HEX)
- **Octal:** `777`, `123` (when input format = OCT)

### Parentheses Usage
- Group sub-expressions: `(123+456)*789`
- Define synchronization blocks: `(abc+def)&(ghi+jkl)`
- Nest operations: `((12*34)+56)&(78/90)`

## Implementation Examples

### Simple Harmonic Combination
```
Input: 135+246
Output: Three simultaneous chord tones:
- Time 0: Tones (1,2) played together
- Time 1: Tones (3,4) played together  
- Time 2: Tones (5,6) played together
```

### Frequency Modulation Chain  
```
Input: 123*456/789
Processing:
1. 123*456 → frequency modulation up
2. Result/789 → frequency division down
3. Final: Complex harmonic transformation
```

### Complex Synchronization
```
Input: (12+34)&(567*890)&(ABC/DEF)
Output: Three synchronized tracks:
- Track 1: Harmonic combination of 12+34
- Track 2: Frequency modulated 567*890  
- Track 3: Frequency divided ABC/DEF
- All tracks time-aligned and played together
```

## Technical Specifications

### Frequency Mapping
- Digit `0` → Base frequency × 0 (silence/rest)
- Digit `1` → Base frequency × 1  
- Digit `2` → Base frequency × 2
- ...
- Digit `F` → Base frequency × 15 (hex)

### Base Parameters
- **Base Frequency:** 180 Hz (configurable)
- **Base Duration:** 300ms per digit (configurable)  
- **Frequency Range:** 20 Hz - 20,000 Hz (audible range)

### Sequence Alignment Rules
For sequences of different lengths:
- **Harmonic (`+`):** Shorter sequence loops to match longer
- **Modulation (`*`, `/`):** Shorter sequence extends with last value
- **Synchronization (`&`):** All expressions normalized to longest duration

## Error Handling

### Invalid Operations
- **Division by Zero:** Skip tone or use silence
- **Frequency Overflow:** Clamp to maximum audible frequency (20kHz)
- **Frequency Underflow:** Clamp to minimum audible frequency (20Hz)
- **Invalid Characters:** Ignore non-numeric characters based on input format

### Malformed Expressions
- **Mismatched Parentheses:** Return parse error with position
- **Empty Sequences:** Treat as silence
- **Invalid Operators:** Return syntax error with suggested corrections

## Future Extensions

### Planned Operators
- **`^`** (Octave Transposition): `abc^2` → transpose up one octave
- **`%`** (Rhythmic Modulation): `abc%4` → apply 4/4 time signature  
- **`~`** (Tremolo/Vibrato): `abc~def` → apply amplitude/frequency modulation
- **`-`** (Rest/Subtraction): `abc-def` → insert rests or subtract frequencies

### Advanced Features
- **Conditional Logic:** `condition?true_sequence:false_sequence`
- **Range Filtering:** `sequence<440>880` → only frequencies between 440-880Hz
- **Loop Constructs:** `abc{n}` → repeat sequence n times
- **Mathematical Constants:** `π`, `e`, `φ` as predefined sequences

## Implementation Guidelines

### Parser Requirements
1. Support operator precedence and parentheses
2. Handle multiple number format inputs (BIN, OCT, DEC, HEX)
3. Provide clear error messages with position information
4. Support nested expressions of arbitrary depth

### Processor Architecture  
1. **Expression Parser** → Abstract Syntax Tree
2. **Sequence Generator** → Convert AST nodes to tone sequences
3. **Track Combiner** → Merge multiple sequences per operator rules
4. **Audio Renderer** → Output final mixed audio

### Performance Considerations
- **Memory:** Efficient handling of very long numeric sequences
- **Processing:** Parallel processing for multiple tracks
- **Real-time:** Support for real-time expression evaluation
- **Caching:** Cache converted sequences for repeated sub-expressions

## Testing Strategy

### Unit Tests
- Individual operator functionality
- Edge cases (empty sequences, overflow, underflow)
- Number format conversion accuracy
- Expression parsing correctness

### Integration Tests  
- Complex nested expressions
- Multi-track synchronization
- Performance with long sequences
- Audio output verification

### User Acceptance Tests
- Mathematical constants (π, e, fibonacci) → musical output
- Data sonification scenarios
- Educational mathematics examples
- Algorithmic composition use cases

---

*This specification defines version 1.0 of the Math2Music Operator Language. Future versions may extend or modify these operators based on user feedback and use case analysis.*