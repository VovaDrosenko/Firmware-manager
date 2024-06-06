using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

internal class Program
{
    string mainDirectory;
    string currentDirectory;
    List<string> listOfDirAndFiles;

    public Program()
    {
        mainDirectory = Path.Combine(Directory.GetCurrentDirectory(), "repo-factory");
        currentDirectory = mainDirectory;
        listOfDirAndFiles = new List<string>();
        UpdateDir();
    }

    static void Main(string[] args)
    {
        Program program = new Program();

        while (true)
        {
            Console.Clear();
            program.ShowDir();
            program.MoveToDir();
        }
    }

    public void UpdateDir()
    {
        string[] directories = Directory.GetDirectories(currentDirectory);
        foreach (string directory in directories)
        {
            string dirName = Path.GetFileName(directory);
            listOfDirAndFiles.Add($"[DIR] {dirName}");
        }

        string[] files = Directory.GetFiles(currentDirectory);
        foreach (string file in files)
        {
            string fileName = Path.GetFileName(file);
            listOfDirAndFiles.Add($"[FILE] {fileName}");
        }
    }

    public void ShowDir()
    {
        Console.WriteLine($"<--{Path.GetFileName(currentDirectory)}-->");
        for (int i = 0; i < listOfDirAndFiles.Count; i++)
        {
            Console.WriteLine($"{i + 1} - {listOfDirAndFiles[i]}");
        }
    }

    public void MoveToDir()
    {
        Console.WriteLine("\n(-1 back)");
        Console.WriteLine("Enter the number");
        if (int.TryParse(Console.ReadLine(), out int selectedDirIndex))
        {
            if (selectedDirIndex == -1)
            {
                MoveBack();
                return;
            }

            else if (selectedDirIndex >= 1 && selectedDirIndex <= listOfDirAndFiles.Count)
            {
                string selectedDirOrFile = listOfDirAndFiles[selectedDirIndex - 1];
                if (selectedDirOrFile.StartsWith("[DIR]"))
                {
                    string selectedDir = selectedDirOrFile.Substring("[DIR] ".Length);
                    currentDirectory = Path.Combine(currentDirectory, selectedDir);
                    listOfDirAndFiles.Clear();
                    UpdateDir();
                }
                else if (selectedDirOrFile.StartsWith("[FILE]"))
                {
                    string selectedFile = selectedDirOrFile.Substring("[FILE] ".Length);
                    ShowFileContent(selectedFile);
                }
            }
            else
            {
                Console.WriteLine("Invalid directory or file number.");
            }
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a number.");
        }
    }

    public void MoveBack()
    {
        if (currentDirectory == mainDirectory)
        {
            Console.WriteLine("Already in main directory.");
            return;
        }

        currentDirectory = Directory.GetParent(currentDirectory)?.FullName ?? currentDirectory;
        listOfDirAndFiles.Clear();
        UpdateDir();
    }

    public void ShowFileContent(string fileName)
    {
        Console.Clear();
        string filePath = Path.Combine(currentDirectory, fileName);
        if (File.Exists(filePath))
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath);
                Console.WriteLine($"<-{fileName}->");
                for (int i = 0; i < lines.Length; i++)
                {
                    Console.WriteLine($"{i + 1}\t{lines[i]}");
                }
                Console.WriteLine("\n(\"-1\" back)\n(\"2\" Edit file)");
                

                if (int.TryParse(Console.ReadLine(), out int option))
                {
                    switch (option)
                    {
                        case -1:
                            break;
                        case 2:
                            ClearLastLine(3);
                            EditFile(filePath, lines);
                            break;
                        default:
                            Console.WriteLine("Invalid option.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine($"File {fileName} does not exist.");
        }
    }

    public void EditFile(string filePath, string[] txt)
    {
        string[] lines = txt;
        List<Tuple<string, string>> editedStrings = new List<Tuple<string, string>>();
        bool editTime = true;
        Console.WriteLine("(\"-1\" back)\n(\"show\" show changes)\n(\"save\" save changes)");
        while (editTime)
        {
            Console.WriteLine("\nparameter: ");
            string param = Console.ReadLine();
            switch (param)
            {
                case "-1":
                    editTime = false;
                    break;
                case "save":
                    File.WriteAllLines(filePath, lines);
                    Console.Write("update other files?(Y/N):");
                    string resp = Console.ReadLine().ToUpperInvariant();
                    switch (resp)
                    {
                        case "Y":
                            UpdateOtherFiles(editedStrings);
                            break;
                        case "N":
                            editTime = false;
                            break;
                    }
                    break;
                case "show":
                    foreach (Tuple<string, string> line in editedStrings)
                    {
                        Console.WriteLine(line.Item1 + " " + line.Item2);
                    }
                    break;
                default:
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (lines[i].StartsWith(param))
                        {
                            Console.Write($"{i + 1}. {param} ");
                            string value = Console.ReadLine();

                            lines[i] = $"{param} {value}";
                            editedStrings.Add(new Tuple<string, string>(param, value));
                            break;
                        }
                    }
                    break;
            }
        }
    }


    public void UpdateOtherFiles(List<Tuple<string, string>> lines)
    {
        ShowDir();
        List<string> listOfFiles = new List<string>();
        var choice = Console.ReadLine();
        string[] indexString = choice.Split(' ');
        int[] indexes = Array.ConvertAll(indexString,int.Parse);

        for (int i = 0; i < indexes.Length; i++) //file
        {
            string selectedDirOrFile = listOfDirAndFiles[indexes[i] - 1];
            string selectedFile = selectedDirOrFile.Substring("[FILE] ".Length);
            string filePath = Path.Combine(currentDirectory, selectedFile);
            if (File.Exists(filePath))
            {
                try
                {
                    string[] fileLines = File.ReadAllLines(filePath);
                    for (int j = 0; j < lines.Count; j++) //param
                    {
                        for (int k = 0; k < fileLines.Length; k++) //text
                        {
                            if (fileLines[k].Contains(lines[j].Item1))
                            {
                                fileLines[k] = lines[j].Item1 + " " + lines[j].Item2;
                            }
                        }
                    }
                    File.WriteAllLines(filePath, fileLines);
                    Console.WriteLine($"File {selectedFile} updated successfully.");
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Error updating file {selectedFile}: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"File {selectedFile} does not exist.");
            }
        }
    }
    public void ClearLastLine(int del)
    {
        int currentLineCursor = Console.CursorTop;
        for(int i = 0; i <= del; i++)
        {
            Console.SetCursorPosition(0, currentLineCursor - i);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor - i);
        }
    }
}
