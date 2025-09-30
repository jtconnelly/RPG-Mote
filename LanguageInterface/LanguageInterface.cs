using System.Text.Json;

namespace RPGMote
{
    public struct Method
    {
        public string returnVal;
        public Dictionary<String, String> paramList;
        public string comment;
    }
    public interface LanguageInterface
    {
        string Language { get; set; }
        bool Generate(JsonDocument parsedJson, string output_dir);
    }
}