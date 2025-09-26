using System.Text.Json;

namespace RPGMote
{
    public interface LanguageInterface
    {
        string Language{ get; set; }
        bool Generate(JsonDocument parsedJson, string output_dir);
    }
}