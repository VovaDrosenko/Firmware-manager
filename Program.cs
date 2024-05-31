using System;
using System.Collections.Generic;
using System.IO;

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
                Console.WriteLine("\n1 - Return to directory view");
                Console.WriteLine("2 - Edit file");

                if (int.TryParse(Console.ReadLine(), out int option))
                {
                    switch (option)
                    {
                        case 1:
                            break;
                        case 2:
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

    public void EditFile(string filePath, string[] lines)
    {
        bool editTime = true;
        while (editTime)
        {
            Console.Write("Line number: ");

            if (int.TryParse(Console.ReadLine(), out int lineNumber))
            {
                if (lineNumber >= 1 && lineNumber <= lines.Length)
                {
                    string oldContent = lines[lineNumber - 1];
                    Console.WriteLine($"Old content {oldContent}");
                    Console.WriteLine($"Enter the new content for line {lineNumber}:");
                    string newContent = Console.ReadLine();

                    lines[lineNumber - 1] = newContent;

                    try
                    {
                        File.WriteAllLines(filePath, lines);

                        Console.WriteLine("Do you want to update other files? (Y/N)");
                        string response = Console.ReadLine().Trim().ToUpper();
                        if (response == "Y")
                        {
                            UpdateOtherFiles(filePath, oldContent, newContent);
                        }
                        else if (response != "N")
                        {
                            Console.WriteLine("Invalid response. Please enter Y or N.");
                        }
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine($"Error updating file: {ex.Message}");
                    }
                }
                else if (lineNumber == -1)
                {
                    editTime = false;
                }
                else
                {
                    Console.WriteLine("Invalid line number.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number.");
            }
        }
    }

    public void UpdateOtherFiles(string editedFilePath, string oldContent, string newContent)
    {
        for(int i = 0; i <listOfDirAndFiles.Count; i++)
        {
                string selectedDirOrFile = listOfDirAndFiles[i];
                if (selectedDirOrFile.StartsWith("[FILE]"))
                {
                    string selectedFile = selectedDirOrFile.Substring("[FILE] ".Length);
                    string filePath = Path.Combine(currentDirectory, selectedFile);
                    if (File.Exists(filePath))
                    {
                        try
                        {
                            string[] fileLines = File.ReadAllLines(filePath);
                            for (int j = 0; j < fileLines.Length; j++)
                            {
                                if (fileLines[j].Contains(oldContent))
                                {
                                    fileLines[j] = fileLines[j].Replace(oldContent, newContent);
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
                else
                {
                    Console.WriteLine($"Selected item is not a file: {selectedDirOrFile}");
                }
        }
    }
}
