using System.Collections;
using System.IO;
using System.Reflection;

class DllLoader
{
    public static List<RPGMote.LanguageInterface> LoadSingleLib(string path)
    {
        var ans = new List<RPGMote.LanguageInterface>();
        try
        {
            // Load the assembly from the specified path
            Assembly dll = Assembly.LoadFile(path);

            foreach (Type type in dll.GetExportedTypes())
            {
                Type? myInterface = type.GetInterface("RPGMote.LanguageInterface");
                if (myInterface != null)
                {
                    RPGMote.LanguageInterface? asIFace = Activator.CreateInstance(type) as RPGMote.LanguageInterface;
                    if (asIFace != null)
                    {
                        ans.Add(asIFace);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading or using DLL: {ex.Message}");
        }
        return ans;
    }
    public static List<RPGMote.LanguageInterface> LoadLibs(string path)
    {
        List<RPGMote.LanguageInterface> ans = new List<RPGMote.LanguageInterface>();
        string[] dir = Directory.GetFiles(path);
        foreach (string file in dir)
        {
            try
            {
                // Load the assembly from the specified path
                Assembly dll = Assembly.LoadFile(file);

                foreach (Type type in dll.GetExportedTypes())
                {
                    Type? myInterface = type.GetInterface("RPGMote.LanguageInterface");
                    if (myInterface != null)
                    {
                        RPGMote.LanguageInterface? asIFace = Activator.CreateInstance(type) as RPGMote.LanguageInterface;
                        if (asIFace != null)
                        {
                            ans.Add(asIFace);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading or using DLL: {ex.Message}");
            }
        }
        return ans;
    }
}