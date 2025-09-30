using System.Text.Json;

namespace RPGMote;

public class CsClassGenerator : LanguageInterface
{
    internal Dictionary<String, String> valueConversions = new Dictionary<String, String>
    {
        {"int8","sbyte" },
        {"int16","short" },
        {"int32","int" },
        {"int64","long" },
        {"uint8","byte" },
        {"uint16","ushort" },
        {"uint32","uint" },
        {"uint64","ulong" },
        {"vec", "List"},
        {"map", "Dictionary"},
        {"str", "string"}
     };

    internal string commentCharacter = "//";
    public CsClassGenerator()
    {
        Language = "Cs";
    }
    public string Language { get; set; }

    internal bool GenerateFile(string output_dir, string className, List<String> includes, Dictionary<String, String> members, Dictionary<String, RPGMote.Method> methods)
    {
        string output = "";
        Directory.CreateDirectory(output_dir + "/cs");
        string file_path = output_dir + "/cs/" + className + ".cs";

        foreach (var include in includes)
        {
            output += $"using RPGTemplate.{include};\n";
        }
        if (includes.Count != 0)
        {
            output += "\n";
        }

        output += "namespace RPGTemplate\n{\n";
        output += "public class "+className+"\n{\n";

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
                    if (!key.Contains("u") && returnVal.Contains("u" + key))
                    {
                        continue;
                    }
                    returnVal = returnVal.Replace(key, valueConversions[key]);

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
                        if (!key.Contains("u") && argVal.Contains("u" + key))
                        {
                            continue;
                        }
                        argVal = argVal.Replace(key, valueConversions[key]);
                    }

                }
                paramString += $"{argVal} {arg.Key}";
                if (!arg.Equals(meth.Value.paramList.Last()))
                {
                    paramString += ", ";
                }
            }
            output += $"public {returnVal} {meth.Key}({paramString});\n\n";
        }
        // End methods

        // Begin members
        foreach (var mem in members)
        {
            var type = mem.Value;
            foreach (var key in valueConversions.Keys)
            {
                if (type.Contains(key))
                {
                    if (!key.Contains("u") && type.Contains("u" + key))
                    {
                        continue;
                    }
                    type = type.Replace(key, valueConversions[key]);
                }

            }
            output += $"{type} {mem.Key}" + " { get; set; }\n\n";
        }

        // End members        

        output += "}\n"; // Class
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

        return GenerateFile(output_dir, name, includes, members, methodList);
    }
}
