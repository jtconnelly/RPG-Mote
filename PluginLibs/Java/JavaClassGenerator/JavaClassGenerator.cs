using System.Text.Json;

namespace RPGMote;

public class JavaClassGenerator : LanguageInterface
{
    public JavaClassGenerator()
    {
        Language = "Java";
    }
    public string Language { get; set; }
    public bool Generate(JsonDocument parsedJson, string output_dir)
    {
        return true;
    }
}
