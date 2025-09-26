using System.Text.Json;

namespace RPGMote;

public class CsClassGenerator : LanguageInterface
{
    public CsClassGenerator()
    {
        Language = "Cs";
    }
    public string Language { get; set; }
    public bool Generate(JsonDocument parsedJson, string output_dir)
    {
        return true;   
    }
}
