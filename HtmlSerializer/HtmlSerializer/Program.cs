
using HtmlSerializer;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using static System.Net.Mime.MediaTypeNames;



static async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
}
static List<string> func2(string input)
{
    string noOtherSpaces = Regex.Replace(input, @"[^\S ]+", "");
    string cleanString = Regex.Replace(noOtherSpaces, @" {2,}", " ");
    List<string> htmlLines = new Regex("<(.*?)>").Split(cleanString).Where(s => s.Length > 0).ToList();
    List<string> listWithoutEmptyLines = new List<string>();
    foreach (var line in htmlLines)
        if (line != " ")
            listWithoutEmptyLines.Add(line);
    return listWithoutEmptyLines;
}
var html = await Load("https://chani-k.co.il/sherlok-game/");
List<string> listObject=func2(html);

HtmlElement root = new HtmlElement();
root = HtmlTreeBuilder.BuildTree(listObject);
Selector s=Selector.CreateQuery("body#home div.copyR");
HashSet<HtmlElement> h = new HashSet<HtmlElement>();
h = root.ElementsFitSelector(root, s, h);







