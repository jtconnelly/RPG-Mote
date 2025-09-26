using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text.Json;
using RPGMote;

class Program
{
    static void Main(string[] args)
    {
        var current_directory = Directory.GetCurrentDirectory();
        var arguments = ParseArguments(args);
        string data_dir = current_directory + "/data";
        string plugin_dir = current_directory + "/plugins";
        string output_dir = current_directory + "/out";
        string? lib_path = null;

        // Check if 'help' argument is passed and output help string
        // If Zero arguments are passed in then output help string
        if (arguments.ContainsKey("--help"))
        {
            PrintHelp();
            return; // Exit the application
        }
        // Example: Retrieve the value of a named argument
        if (arguments.TryGetValue("--data", out string? data_str))
        {
            data_dir = data_str;
        }
        // Example: Retrieve the value of a named argument
        if (arguments.TryGetValue("--plugins", out string? plugin_str))
        {
            plugin_dir = plugin_str;
        }
        if (arguments.TryGetValue("--library", out string? lib_str))
        {
            lib_path = lib_str;
        }
        // Example: Retrieve the value of a named argument
        if (arguments.TryGetValue("--output", out string? out_str))
        {
            output_dir = out_str;
        }

        Console.WriteLine($"Data Directory: {data_dir}");
        if (lib_path != null)
        {
            Console.WriteLine($"Library Path: {lib_path}");
        }
        else
        {
            Console.WriteLine($"Plugin Directory: {plugin_dir}");
        }
        Console.WriteLine($"Output Directory: {output_dir}");


        List<RPGMote.LanguageInterface> langs;
        // Begin Loading
        if (lib_path != null)
        {
            langs = DllLoader.LoadSingleLib(lib_path);
        }
        else
        {
            langs = DllLoader.LoadLibs(plugin_dir);
        }

        var files = Directory.GetFiles(data_dir);
        foreach (var file in files)
        {
            // Begin Parsing
            JsonDocument json = JsonDocument.Parse(File.ReadAllText(file));
            foreach (RPGMote.LanguageInterface plugin in langs)
            {
                // Begin calls to generate
                if (!plugin.Generate(json, output_dir))
                {
                    Console.WriteLine($"Failed to generate {plugin.Language} class for {file}");
                }
                else
                {
                    Console.WriteLine($"Successfully generated {plugin.Language} class for {file} in {output_dir}");
                }
                Console.WriteLine();
            }
        }

        Console.WriteLine("------------------------------");
        Console.WriteLine("Finished Generating! Exiting...");
        Console.WriteLine("------------------------------");
    }

    static Dictionary<string, string?> ParseArguments(string[] args)
    {
        var arguments = new Dictionary<string, string?>();

        foreach (var arg in args)
        {
            // Split the argument by '=' to handle key/value pairs
            string[] parts = arg.Split('=');

            // Check if the argument is in the format "key=value"
            if (parts.Length == 2)
            {
                arguments[parts[0]] = parts[1];
            }
            // If not, assume it's just a named argument without a value
            else
            {
                arguments[arg] = null;
            }
        }

        return arguments;
    }

    static void PrintHelp()
    {
        Console.WriteLine("Usage: ClassParser [options]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --help             Display this help message");
        Console.WriteLine("  --data=<directory>    Specify directory containing the json files to parse, <current_directory>/data by default");
        Console.WriteLine("  --plugins=<directory>    Specify directory containing the Language Dlls that we will generate from, <current_directory>/plugins by default");
        Console.WriteLine("  --library=<file-path>    Specify single library to load in and work on. This will take precedence over the plugins argument if given");
        Console.WriteLine("  --output=<directory>  Specify directory where the generated files will be outputted to, <current_directory>/out by default");
    }
}
