using System;
using System.Collections.Generic;
using System.IO;

public class Program
{
    static void Main(string[] args)
    {
        string currentDirectory = Directory.GetCurrentDirectory();

        string directoryPath = Path.Combine(currentDirectory, "repo-factory");

        List<string> listOfDir = new List<string>();

        
    }

    public void ShowDir(string dir) 
    {
        if (Directory.Exists(dir))
        {
            Console.WriteLine("Contents of directory:");

            string[] files = Directory.GetFiles(dir);
            for (int i = 0; i < files.Length; i++)
            {
                string fileName = Path.GetFileName(files[i]);
                listOfDir.Add(fileName);

                Console.WriteLine($"{listOfDir.Count} {listOfDir[i]}");
            }

            string[] directories = Directory.GetDirectories(directoryPath);
            for (int i = 0; i < directories.Length; i++)
            {
                string dirName = Path.GetFileName(directories[i]);
                listOfDir.Add(dirName);

                Console.WriteLine($"{listOfDir.Count} {listOfDir[listOfDir.Count - 1]}");
            }
        }
        else
        {
            Console.WriteLine("Directory does not exist.");
        }
    }
}
