using System;
using System.IO;
using Microsoft.Data.Sqlite;
using SQLitePCL;

// adjust this path to where your sqlite3.so is stored
string sqliteLibPath = Path.GetFullPath("./External/sqlite/.libs/libsqlite3.so");

SQLite3Provider_dynamic_cdecl.Setup("sqlite3", new NativeLibraryAdapter(sqliteLibPath));
SQLitePCL.raw.SetProvider(new SQLite3Provider_dynamic_cdecl());

string databasePath = Path.GetFullPath("./Data/dictionary.db");
using var connection = new SqliteConnection($"Data Source={databasePath}");

// open the database connection
connection.Open();

// close the database connection when we're done
Console.CancelKeyPress += delegate
{
    Console.WriteLine("Closing database connection...");
    connection.Close();
    Environment.Exit(0);
};
// close the database connection when we quit the app using Ctrl+C
Console.CancelKeyPress += delegate
{
    connection.Close();
    Environment.Exit(0);
};

// ask the user for a word to look up until they quit the app
while (true)
{
    Console.Write("== Enter a word (ctrl+c to cancel): ");
    var word = Console.ReadLine();

    // ask again if the word is empty
    if (string.IsNullOrEmpty(word))
    {
        Console.WriteLine("Word can't be empty!");
        continue;
    }

    // create a command to search the database and bind the user input
    // to the `@word` parameter
    var command = connection.CreateCommand();
    command.CommandText = $@"
    SELECT
        Dictionary.EnglishText,
        JapaneseTextSnippet
    FROM Dictionary
    JOIN ( 
        SELECT 
            snippet(DictionaryFTS, '[', ']') AS JapaneseTextSnippet, 
            rowid 
        FROM DictionaryFTS
        WHERE DictionaryFTS MATCH @word
    ) AS ResultFTS
        ON Dictionary.Id = ResultFTS.rowid;
    ";
    command.Parameters.Add(new SqliteParameter("@word", word));

    using var reader = command.ExecuteReader();

    // prints a message when no result was found
    if (!reader.HasRows)
    {
        Console.WriteLine("No result was found!");
        continue;
    }

    // print the results in a list
    while (reader.Read())
    {
        var englishText = reader.GetString(0);
        var japaneseTextSnippet = reader.GetString(1).Replace("[", "[1;31m").Replace("]", "[0m");
        Console.WriteLine($"- {englishText} => {japaneseTextSnippet}");
    }
}
