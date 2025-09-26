using System.Text.Json;

namespace RPGMote
{
    public interface LanguageInterface
    {
        bool Generate(JsonDocument parsedJson, string output_dir);
    }
}