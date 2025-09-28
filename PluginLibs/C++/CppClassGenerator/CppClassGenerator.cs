using System.Collections.Specialized;
using System.Dynamic;
using System.Net.Sockets;
using System.Text.Json;

namespace RPGMote;

public class CppClassGenerator : LanguageInterface
{
    internal Dictionary<String, String> valueConversions = new Dictionary<String, String>
    {
        {"int8","int8_t" },
        {"int16","int16_t" },
        {"int32","int32_t" },
        {"int64","int64_t" },
        {"vec", "std::vector"},
        {"map", "std::map"},
        {"str", "std::string"}
     };

    internal Dictionary<String, String> includeMap = new Dictionary<String, String>
    {
        {"std::vector", "vector"},
        {"std::string", "string"},
        {"std::map", "map"}
    };

    internal string commentCharacter = "//";
    public CppClassGenerator()
    {
        Language = "C++";
    }
    public string Language { get; set; }

    internal bool GenerateHeader(string output_dir, string className, List<String> includes, Dictionary<String, String> members, Dictionary<String, RPGMote.Method> methods)
    {
        HashSet<String> convertedIncludes = new HashSet<String>();
        string output = "";
        Directory.CreateDirectory(output_dir + "/cpp");
        string file_path = output_dir + "/cpp/" + className + ".h";
        string includeString = "";
        foreach (string include in includes)
        {
            includeString += $"#include \"{include}.h\"\n";
        }

        output += "namespace RPGTemplate\n{\n";
        output += "class "+className+"\n{\npublic:\n";
        output += className + "() = default;\n"; // Default Ctor
        output += "~" + className + "() = default;\n\n"; // Default Dtor

        // Begin methods
        foreach (var meth in methods)
        {
            if (meth.Value.comment != null && meth.Value.comment != "")
            {
                output += commentCharacter + meth.Value.comment + "\n";
            }
            var returnVal = meth.Value.returnVal;
            foreach (var key in valueConversions.Keys)
            {
                if (returnVal.Contains(key))
                {
                    returnVal = returnVal.Replace(key, valueConversions[key]);
                    if (includeMap.ContainsKey(valueConversions[key]) && !convertedIncludes.Contains(valueConversions[key]))
                    {
                        includeString += $"#include <{includeMap[valueConversions[key]]}>\n";
                        convertedIncludes.Add(valueConversions[key]);
                    }
                }
                if (includeMap.ContainsKey(key) && !convertedIncludes.Contains(key))
                {
                    includeString += $"#include <{includeMap[key]}>\n";
                        convertedIncludes.Add(key);
                }
            }

            string paramString = "";
            foreach (var arg in meth.Value.paramList)
            {
                var argVal = arg.Value;
                foreach (var key in valueConversions.Keys)
                {
                    if (argVal.Contains(key))
                    {
                        argVal = argVal.Replace(key, valueConversions[key]);
                        if (includeMap.ContainsKey(valueConversions[key]) && !convertedIncludes.Contains(valueConversions[key]))
                        {
                            includeString += $"#include <{includeMap[valueConversions[key]]}>\n";
                            convertedIncludes.Add(valueConversions[key]);
                        }
                    }
                    if (includeMap.ContainsKey(key) && !convertedIncludes.Contains(key))
                    {
                        includeString += $"#include <{includeMap[key]}>\n";
                        convertedIncludes.Add(key);
                    }
                }
                paramString += $"const {argVal}& {arg.Key}";
                if (!arg.Equals(meth.Value.paramList.Last()))
                {
                    paramString += ", ";
                }
            }
            output += $"{returnVal} {meth.Key}({paramString});\n\n";
        }
        // End methods

        // Begin members
        string getSets = "";
        string memberVars = "";

        foreach (var member in members)
        {
            var memberType = member.Value;
            foreach (var key in valueConversions.Keys)
            {
                if (memberType.Contains(key))
                {
                    memberType = memberType.Replace(key, valueConversions[key]);
                    if (includeMap.ContainsKey(valueConversions[key]) && !convertedIncludes.Contains(valueConversions[key]))
                    {
                        includeString += $"#include <{includeMap[valueConversions[key]]}>\n";
                        convertedIncludes.Add(valueConversions[key]);
                    }
                }
                if (includeMap.ContainsKey(key) && !convertedIncludes.Contains(key))
                {
                    includeString += $"#include <{includeMap[key]}>\n";
                    convertedIncludes.Add(key);
                }
            }
            getSets += $"const {memberType}& get{char.ToUpper(member.Key[0]) + member.Key.Substring(1)}() const;\n";
            getSets += $"void set{char.ToUpper(member.Key[0]) + member.Key.Substring(1)}(const {memberType}& val);\n";
            memberVars += $"{memberType} {member.Key};\n";
        }
        output += getSets + "\nprivate:\n" + memberVars;

        // End members        
        includeString += "#include <cstdint>\n\n";
        output = includeString + output;

        output += "};\n"; // Class
        output += "}\n"; // Namespace
        File.WriteAllText(file_path, output);
        return true;
    }
    public bool Generate(JsonDocument parsedJson, string output_dir)
    {
        JsonElement rootElem = parsedJson.RootElement;
        string? name = rootElem.GetProperty("name").GetString();
        if (name == null)
        {
            Console.WriteLine("Failed to find name");
            return false;
        }

        JsonElement includeElement;
        List<String> includes = new List<String>(); ;
        if (rootElem.TryGetProperty("include", out includeElement))
        {
            foreach (JsonElement header in includeElement.EnumerateArray())
            {
                string? value = header.GetString();
                if (value != null)
                {
                    includes.Add(value);
                }
            }
        }

        JsonElement memberElement = rootElem.GetProperty("members");
        Dictionary<String, String> members = new Dictionary<String, String>();
        foreach (JsonProperty memberProp in memberElement.EnumerateObject())
        {
            string? typeAsString = memberProp.Value.GetString();
            if (typeAsString != null)
            {
                members.Add(memberProp.Name, typeAsString);
            }
        }

        JsonElement methodElement = rootElem.GetProperty("methods");
        Dictionary<String, RPGMote.Method> methodList = new Dictionary<String, RPGMote.Method>();
        foreach (JsonProperty methodProp in methodElement.EnumerateObject())
        {
            string funcName = methodProp.Name;
            JsonElement funcInfo = methodProp.Value;
            JsonElement returnProp, paramProp, descriptionProp;
            RPGMote.Method methodInfo = new Method();
            if (funcInfo.TryGetProperty("returns", out returnProp))
            {
                string? ret = returnProp.GetString();
                if (ret != null)
                {
                    methodInfo.returnVal = ret;
                }
            }
            else
            {
                Console.WriteLine($"Error parsing {funcName}: returns property required.");
                continue;
            }

            if (funcInfo.TryGetProperty("params", out paramProp))
            {
                Dictionary<String, String> paramArgs = new Dictionary<String, String>();
                foreach (JsonProperty prop in paramProp.EnumerateObject())
                {
                    string? propType = prop.Value.GetString();
                    if (propType != null)
                    {
                        paramArgs.Add(prop.Name, propType);
                    }
                }
                methodInfo.paramList = paramArgs;
            }

            if (funcInfo.TryGetProperty("description", out descriptionProp))
            {
                string? desc = descriptionProp.GetString();
                if (desc != null)
                {
                    methodInfo.comment = desc;
                }
            }
            methodList.Add(funcName, methodInfo);
        }

        return GenerateHeader(output_dir, name, includes, members, methodList);
    }
}
