using System.IO;
using System.Text.Json;

namespace Scool.DataSeeds
{
    public class DataSeedBase
    {
        public static string GetJsonDataFilePath(string fileName)
        {
            string basePath = System.IO.Path.GetDirectoryName( 
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6);
            string path = Path.Combine(
                basePath,
                "DataSeeds",
                fileName);

            return path;
        }

        public static TResult ParseDataFromJsonFile<TResult>(string path)
        {
            string jsonText = File.ReadAllText(path);
            TResult data = JsonSerializer.Deserialize<TResult>(jsonText);
            return data;
        }
    }
}