using System.Text.Json;

namespace RPGMote;

public class CppClassGenerator : LanguageInterface
{
    public CppClassGenerator()
    {
        Language = "C++";
    }
    public string Language { get; set; }
    public bool Generate(JsonDocument parsedJson, string output_dir)
    {
        return true;
    }
}
