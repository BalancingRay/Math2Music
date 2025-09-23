namespace MathToMusic.Outputs
{
    internal class SomeOutput
    {
        Dictionary<char, string> baseNoteConverter = new Dictionary<char, string>()
        {
            { '0',"CC" },
            { '1',"C" },
            { '2',"G" },
            { '3',"c" },
            { '4',"e" },
            { '5',"g" },
            { '6',"b-" },
            { '7',"c'" },
            { '8',"d'" },
            { '9',"e'" },
            { 'A',"f'" }, // not accurate note
            { 'B',"g'" },
            { 'C',"a'" }, // not accurate note
            { 'D',"b-'" },
            { 'E',"b'" }, // not accurate note
            { 'F',"c''" }
        };
        Dictionary<char, string> zeroNoteConverter = new Dictionary<char, string>()
        {
            { '.',"r" },
            { '0',"r" },
            { '1',"CC" },
            { '2',"C" },
            { '3',"G" },
            { '4',"c" },
            { '5',"e" },
            { '6',"g" },
            { '7',"b-" },
            { '8',"c'" },
            { '9',"d'" },
            { 'A',"e'" },
            { 'B',"f'" }, // not accurate note
            { 'C',"g'" },
            { 'D',"a'" }, // not accurate note
            { 'E',"b'-" },
            { 'F',"b'n" } // not accurate note
        };

        string decFilter = "0123456789.,";
        string binFilter = "01.,";
    }
}
