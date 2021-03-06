// Example.cs

using System;
using System.IO;
using System.Text;

/// Example opens the source files, calls the AStyleInterface methods
/// to format the files, and saves the reformatted source. The files
/// are in a test-data directory. The option mode=cs must be included
/// for C# files.
public class Example
{   /// Main function for this example.
    public static void Main(string[] args)
    {   // files to pass to AStyle
        string[] fileName =  { "ASBeautifier.cpp",
                               "ASFormatter.cpp",
                               "astyle.h"
                             };

        // options to pass to AStyle
        // mode=cs is required for C# files
        string options = "-A2tOP";

        // create an object
        AStyleInterface AStyle = new AStyleInterface();

        // get Artistic Style version
        // does not need to terminate on an error
        string version = AStyle.GetVersion();
        if (version != String.Empty)
            Console.WriteLine("Example C# - AStyle " + version);

        // process the files
        for (int i = 0; i < fileName.Length; i++)
        {   // get the text to format
            string filePath = GetTestDirectoryPath() + fileName[i];
            string textIn = GetText(filePath);

            // call the Artistic Style formatting function
            // does not need to terminate on an error
            string textOut = AStyle.FormatSource(textIn, options);
            // does not need to terminate on an error
            // an error message has been displayed by the error handler
            if (textOut == String.Empty)
            {   Console.WriteLine("Cannot format "  + filePath);
                continue;
            }

            // return the formatted text
            Console.WriteLine("Formatted " + fileName[i]);
            SetText(textOut, filePath);
        }

        return;
    }

    ///  Error message function for this example.
    private static void Error(string message)
    {   Console.WriteLine(message);
        Console.WriteLine("The program has terminated!");
        Environment.Exit(1);
    }

    /// Find the test directory path from the application program path.
    /// This may need to be changed for your directory structure.
    private static string GetTestDirectoryPath()
    {   string topDirectory = "astyledev";
        string appDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
        if (String.IsNullOrEmpty(appDirectory))
            Error("Cannot find application directory!");
        int indexTop = appDirectory.ToLower().IndexOf(topDirectory);
        if (indexTop == -1)
            Error("Cannot find top level folder!");
        string testPath = appDirectory.Substring(
                              0, indexTop + topDirectory.Length) + "/test-data/";
        return testPath;
    }

    ///  Get the text to be formatted.
    ///  Usually the text would be obtained from an edit control.
    private static string GetText(string filePath)
    {   // create input buffers
        const int readSize = 131072;     // 128 KB
        StringBuilder bufferIn = new StringBuilder(readSize);
        char[] fileIn = new char[readSize];

        // read file data
        try
        {   FileStream file = new FileStream(filePath, FileMode.Open);
            StreamReader streamIn = new StreamReader(file);
            // use ReadBlock to preserve the current line endings
            int charsIn = streamIn.ReadBlock(fileIn, 0, readSize);
            while (charsIn != 0)
            {   bufferIn.Append(fileIn, 0, charsIn);
                charsIn = streamIn.ReadBlock(fileIn, 0, readSize);
            }
            streamIn.Close();
        }
        catch (DirectoryNotFoundException e)
        {   Console.WriteLine(e.ToString());
            Error("Cannot find directory " + filePath);
        }
        catch (FileNotFoundException e)
        {   Console.WriteLine(e.ToString());
            Error("Cannot find file " + filePath);
        }
        catch (Exception e)
        {   Console.WriteLine(e.ToString());
            Error("Error reading file " + filePath);
        }

        return bufferIn.ToString();
    }

    ///  Return the formatted text.
    ///  Usually the text would be returned to an edit control.
    private static void SetText(string textOut, string filePath)
    {   // create a backup file
        string origfilePath = filePath + ".orig";
        File.Delete(origfilePath);                  // remove a pre-existing file
        FileInfo outFile = new FileInfo(filePath);
        outFile.MoveTo(origfilePath);

        // write the output file - same name as input
        try
        {   char[] bufferOut = textOut.ToCharArray();
            FileStream file = new FileStream(filePath, FileMode.Create);
            StreamWriter streamOut = new StreamWriter(file);
            streamOut.Write(bufferOut, 0, bufferOut.Length);
            streamOut.Close();
        }
        catch (Exception e)
        {   Console.WriteLine(e.ToString());
            Error("Error writing file " + filePath);
        }

        return;
    }

}   // class Example
