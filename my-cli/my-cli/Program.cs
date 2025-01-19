using System;
using System.CommandLine;
using System.Globalization;
using System.Diagnostics;
using my_cli;
using System;
using System.Net.WebSockets;
using System.ComponentModel.Design;
using System.ComponentModel;

static void RemoveEmptyLines(string path)
{
    string[] lines = File.ReadAllLines(path);

    List<string> nonEmptyLines = new List<string>();
    foreach (string line in lines)
    {
        if (!string.IsNullOrWhiteSpace(line))
        {
            nonEmptyLines.Add(line);
        }
    }

    File.WriteAllLines(path, nonEmptyLines);

}//remove e mpty lines
static void AddFileToBundle(string currentPath, string bundlePath)
{
    File.AppendAllText(bundlePath, File.ReadAllText(currentPath));
}
static void AuthorOrNote(string username, string bundlePath)
{
    File.AppendAllText(bundlePath, "//" + username);
}

static List<string> SortByName(List<string> values)
{
    return values.OrderBy(path => Path.GetFileName(path)).ToList();
}
static void SortFilesByExtension(List<string> files)
{
    files.Sort((x, y) =>
    {
        string extensionX = System.IO.Path.GetExtension(x);
        string extensionY = System.IO.Path.GetExtension(y);
        return string.Compare(extensionX, extensionY, StringComparison.OrdinalIgnoreCase);
    });
}

static void GetInsideDirectory(string folderPath, List<string> files, List<string> languages)
{
    
    try
    {
        string[] myFiles = Directory.GetFiles(folderPath);
        string[] myFolders = Directory.GetDirectories(folderPath);
        if (myFiles.Length == 0 && myFolders.Length == 0)
            return;
        if (myFiles.Length != 0)
        {
            foreach (string file in myFiles)
            {
                if (languages.Contains(System.IO.Path.GetExtension(file)))
                    files.Add(file);
            }
        }
        if (myFolders.Length != 0)
        {
            foreach (string folder in myFolders)
            {

                if (folder != "bin" && folder != "Debug")
                { GetInsideDirectory(folder, files, languages); }
            }
        }
    }

    catch (ArgumentException ex)
    {
        Console.WriteLine($"Argument error: {ex.Message}");
    }
    catch (DirectoryNotFoundException ex)
    {
        Console.WriteLine($"Directory not found: {ex.Message}");
    }
    catch (UnauthorizedAccessException ex)
    {
        Console.WriteLine($"Access denied: {ex.Message}");
    }
    catch (PathTooLongException ex)
    {
        Console.WriteLine($"Path too long: {ex.Message}");
    }
    catch (IOException ex)
    {
        Console.WriteLine($"I/O error: {ex.Message}");
    }
}

static void PassAllFiles(string folderPath, string bundlePath, Options options)
{
    List<string> files = new List<string>();
    GetInsideDirectory(folderPath, files, options.Languages);


    if (options.Author != "")//check author in case
    {
        options.Author += "\n";
        AuthorOrNote(options.Author, bundlePath);
    }
    if (options.SortByExtension)
    { SortFilesByExtension(files); }
    else if (options.SortByABOrder)
    { files = SortByName(files); }
    

    foreach (string file in files)
    {
        if (options.RemoveEmptyLines)
            RemoveEmptyLines(file);
        if (options.Note)
            AuthorOrNote(Path.GetRelativePath(Directory.GetCurrentDirectory(), file) + "\n", bundlePath);

        AddFileToBundle(file, bundlePath);

    }
}

static void AddAllLanguages(Options options, Dictionary<string, string> fileExtentions)
{
    foreach (string language in fileExtentions.Values)
    {
        options.Languages.Add(language);
    }
}
static bool IsValidPath(string path)
{

    if (!Path.IsPathRooted(path))
    {
        return false; // Not a valid path structure
    }

    // Get the directory part of the path
    string directoryPath = Path.GetDirectoryName(path);

    // Check if the directory exists
    if (Directory.Exists(directoryPath))
    {
        return true; // Path structure is valid and directories exist
    }

    return false;
}
var bundleOptionOutput = new Option<string>("--output", "export the file in this name");
var bundleOptionLanguages = new Option<string[]>("--languages", "press the language you wants to include") { AllowMultipleArgumentsPerToken = true };
var bundleOptionSort = new Option<string>("--sort", "sort the order of the files");
var bundleOptionNote = new Option<bool>("--note", "remote with your path");
var bundleOptionAuthor = new Option<string>("--author", "add a comment with your name");
var bundleOptionRemoveEmptyLines = new Option<bool>("--rel", "Remove empty lines");

var rspCommand = new Command("create-rsp", "create a response file");

rspCommand.SetHandler(() =>
{
    string path = "", request = " bundle ", fileName = "";
    Console.WriteLine("Do you want to choose your path? enter y/n");
    char response = Console.ReadKey().KeyChar;
    Console.WriteLine(); // Move to the next line

    if (response == 'y')
    {
        Console.WriteLine("Enter your path including the file name:");
        path = Console.ReadLine();
        request += "-ot " + path + ".txt ";
        path += ".rsp";
    }
    else if (response == 'n')
    {
        Console.WriteLine("Enter the file name:");
        fileName = Console.ReadLine();
        path = Path.Combine(Environment.CurrentDirectory, fileName + ".rsp");
        request += "-ot " + Environment.CurrentDirectory;
        request += "\\" + fileName + ".txt";
        request += " ";

    }



    try
    {
        using (File.Create(path)) { } // Create the file

        Console.WriteLine("Do you want to remove empty lines? choose y/n");
        response = Console.ReadKey().KeyChar;
        Console.WriteLine();
        request += response == 'y' ? "-rel " : "";

        Console.WriteLine("Do you want a note? choose y/n");
        response = Console.ReadKey().KeyChar;
        Console.WriteLine();
        request += response == 'y' ? "-n " : "";

        Console.WriteLine("Do you want to add a remark with author? y/n");
        response = Console.ReadKey().KeyChar;
        Console.WriteLine();
        if (response == 'y')
        {
            request += "-a ";
            Console.WriteLine("Enter the remark:");
            request += Console.ReadLine() + " ";
        }

        Console.WriteLine("Enter your favorite languages (enter -1 to stop):");
        request += "-l ";
        string language;
        while ((language = Console.ReadLine()) != "-1")
        {
            request += language + " ";
        }

        Console.WriteLine("Do you want to sort in alphabetic order? enter y/n");
        response = Console.ReadKey().KeyChar;
        Console.WriteLine();
        if (response == 'y')
        {
            
            request += " -s sort-by-ab-order ";
        }
        else
        {
            Console.WriteLine("Do you want to sort by extension? enter y/n");
            response = Console.ReadKey().KeyChar;
            Console.WriteLine();
            if (response == 'y')
            {
                
                request += "-s sort-by-extension";
            }
        }
        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.WriteLine(request);
        }

        Console.WriteLine($"Request written to '{path}' successfully.");
        Console.WriteLine("request: " + request);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }



    
});

bundleOptionAuthor.AddAlias("-a");
bundleOptionLanguages.AddAlias("-l");
bundleOptionNote.AddAlias("-n");
bundleOptionOutput.AddAlias("-ot");
bundleOptionSort.AddAlias("-s");
bundleOptionRemoveEmptyLines.AddAlias("-rel");

bundleOptionLanguages.IsRequired = true;
var bundleCommand = new Command("bundle", "bundle code for one file");

bundleCommand.AddOption(bundleOptionOutput);
bundleCommand.AddOption(bundleOptionLanguages);
bundleCommand.AddOption(bundleOptionSort);
bundleCommand.AddOption(bundleOptionNote);
bundleCommand.AddOption(bundleOptionAuthor);
bundleCommand.AddOption(bundleOptionRemoveEmptyLines);

Dictionary<string, string> fileExtensions = new Dictionary<string, string>
        {
            { "C", ".c" },
            { "C++", ".cpp" },
            { "C#", ".cs" },
            { "Java", ".java" },
            { "Python", ".py" },
            { "Ruby", ".rb" },
            { "Go", ".go" },
            { "Swift", ".swift" },
            { "HTML", ".html" },
            { "CSS", ".css" },
            { "JavaScript", ".js" },
            { "TypeScript", ".ts" },
            { "PHP", ".php" },
            { "SQL", ".sql" },
            { "Database", ".db" },
            { "JSON", ".json" },
            { "XML", ".xml" },
            { "YAML", ".yaml" },
            { "INI", ".ini" },
            { "TOML", ".toml" },
            { "Shell", ".sh" },
            { "Batch", ".bat" },
            { "PowerShell", ".ps1" }
        };

bundleCommand.SetHandler((output, rel, note, author, languages, sort) =>
{
    Options options = new Options();

    if (string.IsNullOrEmpty(output))
    {
        Console.WriteLine("output option is not provided");
        return;
    }
    else
    {
        if(IsValidPath(output))
        options.Output = output;
        else
        {
            Console.WriteLine("Path is not valid");
            return;
        }
    }
    if (note)
    {
        //string currentDirectory = Directory.GetCurrentDirectory();
        //string projectName = Path.GetFileName(currentDirectory);

        //options.Note = currentDirectory + projectName;
        options.Note = true;
        
        
        
    }
    if (rel)
    {
        options.RemoveEmptyLines = true;
    }
    if (!string.IsNullOrEmpty(author))
    {
        options.Author = author;
    }
    if (languages != null && languages.Length > 0)
    {
        foreach (var language in languages)
        {
            

            if (fileExtensions.ContainsKey(language))
            {
                options.Languages.Add(fileExtensions.GetValueOrDefault(language));
                Console.WriteLine("language:" + options.Languages.First());
            }
            else if (language.CompareTo("all") == 0)
            {
                AddAllLanguages(options, fileExtensions);
            }
            else
            {
                Console.WriteLine("In valid language");
            }
        }
    }
    else
    {
        Console.WriteLine("you must enter language");
        return;
    }
    if (!string.IsNullOrEmpty(sort))
    {
        if(sort== "sort-by-extension")
        {
            options.SortByExtension = true;
        }
        else
        {
            if(sort== "Sort-by-ab-order")
            {
                options.SortByABOrder = true;
            }
        }
        
    }
    



    
    File.Create(output).Close();
    PassAllFiles(Environment.CurrentDirectory, output, options);

}, bundleOptionOutput, bundleOptionRemoveEmptyLines, bundleOptionNote, bundleOptionAuthor, bundleOptionLanguages, bundleOptionSort);
var rootCommand = new RootCommand("bundle command for file bundle cli");
rootCommand.AddCommand(bundleCommand);
rootCommand.AddCommand(rspCommand);
rootCommand.InvokeAsync(args);









