using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
namespace HtmlSerializer
{
    internal class HtmlHelper
    {
        public string[] DoubleTag { get; set; }
        public string[] SingleTag { get; set; }

        private readonly static HtmlHelper _Instance = new HtmlHelper();
        public static HtmlHelper Instance => _Instance;
        private HtmlHelper()
        {
            try
            {
                DoubleTag = LoadJson("json\\HtmlTags.json");
                SingleTag = LoadJson("json\\HtmlVoidTags.json");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing HtmlHelper: {ex.Message}");
            }
        }

        private string[] LoadJson(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"The file {path} does not exist.");
            }

            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<string[]>(json);
        }

    }
}
