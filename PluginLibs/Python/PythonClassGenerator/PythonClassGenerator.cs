using System.Text.Json;

namespace RPGMote;

public class PythonClassGenerator : LanguageInterface
{
    public PythonClassGenerator()
    {
        Language = "Python";
    }
    public string Language { get; set; }
    public bool Generate(JsonDocument parsedJson, string output_dir)
    {
        return true;
    }
}
