-- This file contains queries that you can run after populating the database with data.
-- This file exists because the default SQLite CLI is whack, you can't type Japanese in the prompt.

SELECT
    Dictionary.EnglishText,
    JapaneseTextSnippet
FROM Dictionary
JOIN ( 
    SELECT 
        snippet(DictionaryFTS, '[', ']') AS JapaneseTextSnippet, 
        rowid 
    FROM DictionaryFTS
    WHERE DictionaryFTS MATCH "です"
) AS ResultFTS
    ON Dictionary.Id = ResultFTS.rowid;
