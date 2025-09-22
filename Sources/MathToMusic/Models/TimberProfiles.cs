namespace MathToMusic.Models
{
    /// <summary>
    /// Static collection of predefined timber profiles for natural instruments and synthetic sounds
    /// </summary>
    public static class TimberProfiles
    {
        /// <summary>
        /// Dictionary of predefined timber profiles. Each profile contains coefficients for fundamental + overtones.
        /// Index 0 = fundamental frequency coefficient, Index 1+ = overtone coefficients
        /// </summary>
        public static readonly IReadOnlyDictionary<string, float[]> Profiles = new Dictionary<string, float[]>
        {
            // Natural Instruments
            ["Piano"] = new float[] { 1.0f, 0.8f, 0.6f, 0.4f, 0.3f, 0.2f },
            ["Guitar"] = new float[] { 1.0f, 0.7f, 0.5f, 0.3f, 0.2f },
            ["Violin"] = new float[] { 1.0f, 0.9f, 0.7f, 0.5f, 0.4f, 0.3f, 0.2f },
            ["Flute"] = new float[] { 1.0f, 0.3f, 0.1f, 0.05f },
            ["Trumpet"] = new float[] { 1.0f, 0.8f, 0.6f, 0.4f, 0.2f, 0.1f },
            ["Organ"] = new float[] { 1.0f, 0.9f, 0.8f, 0.7f, 0.6f, 0.5f, 0.4f },
            ["Clarinet"] = new float[] { 1.0f, 0.2f, 0.6f, 0.3f, 0.5f }, // Emphasizes odd harmonics
            ["Saxophone"] = new float[] { 1.0f, 0.7f, 0.8f, 0.5f, 0.6f, 0.3f },
            ["Cello"] = new float[] { 1.0f, 0.8f, 0.6f, 0.5f, 0.4f, 0.3f },
            ["Oboe"] = new float[] { 1.0f, 0.9f, 0.8f, 0.6f, 0.7f, 0.4f },

            // Synthetic Sounds
            ["Sawtooth"] = new float[] { 1.0f, 0.5f, 0.33f, 0.25f, 0.2f, 0.17f, 0.14f }, // 1/n pattern
            ["Square"] = new float[] { 1.0f, 0.0f, 0.33f, 0.0f, 0.2f, 0.0f, 0.14f }, // Odd harmonics only
            ["Triangle"] = new float[] { 1.0f, 0.0f, 0.11f, 0.0f, 0.04f, 0.0f, 0.02f }, // Odd harmonics, 1/n² pattern
            ["Pulse"] = new float[] { 1.0f, 0.8f, 0.6f, 0.8f, 0.4f, 0.6f, 0.2f },
            ["Bright"] = new float[] { 0.8f, 1.0f, 0.9f, 0.7f, 0.8f, 0.5f, 0.6f }, // Emphasizes higher overtones
            ["Warm"] = new float[] { 1.0f, 0.6f, 0.3f, 0.1f, 0.05f }, // Emphasizes fundamental
            ["Metallic"] = new float[] { 1.0f, 0.3f, 0.8f, 0.2f, 0.9f, 0.4f, 0.7f }, // Inharmonic overtones simulation
            ["Bell"] = new float[] { 1.0f, 0.2f, 0.4f, 0.1f, 0.6f, 0.3f, 0.5f },
            ["Pad"] = new float[] { 1.0f, 0.9f, 0.7f, 0.8f, 0.6f, 0.7f, 0.5f }, // Rich harmonics
            ["Sine"] = new float[] { 1.0f }, // Pure sine wave (fundamental only)
            
            // Special Effects
            ["Hollow"] = new float[] { 0.5f, 0.0f, 0.8f, 0.0f, 0.6f, 0.0f, 0.4f }, // Missing even harmonics
            ["Nasal"] = new float[] { 1.0f, 0.3f, 0.9f, 0.4f, 0.8f, 0.5f }, // Emphasis on 3rd and 5th harmonics
            ["Growl"] = new float[] { 1.0f, 0.9f, 1.2f, 0.8f, 1.1f, 0.7f, 1.0f }, // Boosted odd harmonics
            ["Ethereal"] = new float[] { 0.7f, 1.0f, 0.4f, 0.8f, 0.3f, 0.6f, 0.2f }, // Higher harmonics prominent
        };

        /// <summary>
        /// Get all available timber profile names
        /// </summary>
        public static IEnumerable<string> GetAvailableProfiles()
        {
            return Profiles.Keys.OrderBy(k => k);
        }

        /// <summary>
        /// Check if a timber profile exists
        /// </summary>
        public static bool HasProfile(string profileName)
        {
            return Profiles.ContainsKey(profileName);
        }

        /// <summary>
        /// Get a timber profile by name, returns null if not found
        /// </summary>
        public static float[]? GetProfile(string profileName)
        {
            return Profiles.TryGetValue(profileName, out var profile) ? profile : null;
        }
    }
}