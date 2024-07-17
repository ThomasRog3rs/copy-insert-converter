using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

class Program
{
    static void Main()
    {
        string inputFilePath = "input.txt"; // Path to your input file
        string outputFilePath = "output.sql"; // Path to your output file

        // Read the input file
        string[] lines = File.ReadAllLines(inputFilePath);

        // Parse the COPY statement and the table name
        string copyStatement = lines[0];
        string tableName = ParseTableName(copyStatement);
        string columnList = ParseColumnList(copyStatement);

        // Build the INSERT statement
        StringBuilder insertStatement = new StringBuilder();
        insertStatement.AppendLine($"INSERT INTO {tableName} {columnList} VALUES");

        // Process each data line and add it to the INSERT statement
        List<string> valuesList = new List<string>();
        for (int i = 1; i < lines.Length; i++)
        {
            if (lines[i] == "\\.") break; // End of data
            string[] values = lines[i].Split('\t');
            valuesList.Add($"('{string.Join("', '", values)}')");
        }

        insertStatement.AppendLine(string.Join(",\n", valuesList) + ";");

        // Write the INSERT statement to the output file
        File.WriteAllText(outputFilePath, insertStatement.ToString());

        Console.WriteLine("Conversion completed. Check the output file.");
    }

    static string ParseTableName(string copyStatement)
    {
        // Example: COPY public.marina_types (key, value, icon_key) FROM stdin;
        int startIndex = "COPY ".Length;
        int endIndex = copyStatement.IndexOf('(', startIndex) - 1;
        return copyStatement.Substring(startIndex, endIndex - startIndex + 1).Trim();
    }

    static string ParseColumnList(string copyStatement)
    {
        // Example: COPY public.marina_types (key, value, icon_key) FROM stdin;
        int startIndex = copyStatement.IndexOf('(');
        int endIndex = copyStatement.IndexOf(')') + 1;
        return copyStatement.Substring(startIndex, endIndex - startIndex);
    }
}