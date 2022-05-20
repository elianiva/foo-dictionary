-- This file contains list of all statements used to setup the database
-- This file exists because the default SQLite CLI is whack, you can't type Japanese in the prompt.
-- and also, you can't do multiline stuff.

CREATE TABLE Dictionary (
  Id INTEGER PRIMARY KEY AUTOINCREMENT,
  EnglishText TEXT,
  JapaneseText TEXT
);

CREATE VIRTUAL TABLE DictionaryFTS 
USING fts4(
  JapaneseText, 
  content='Dictionary',
  tokenize=icu ja_JP
);

CREATE TRIGGER Dictionary_Update_FTS
AFTER INSERT ON Dictionary
BEGIN
  INSERT INTO DictionaryFTS(rowid, JapaneseText)
  VALUES (new.rowid, new.JapaneseText);
END;

INSERT INTO Dictionary (EnglishText, JapaneseText)
VALUES ("I woke up in the morning", "朝に起きた"),
       ("I was singing that song", "その歌を歌っていた"),
       ("The food was good", "食べ物はうまかった"),
       ("What time is it now?", "今何時？"),
       ("This is Japanese", "これは日本語です"),
       ("That car went too fast", "あの車が速すぎた"),
       ("It smells like spring", "春の匂いがする"),
       ("It's very cold outside", "外には寒いよ"),
       ("Today is saturday", "今日は土曜日です"),
       ("Please tell me", "教えてくれ"),
       ("Sorry I don't understand Japanese", "日本語分からなくてごめん"),
       ("This is not an apple", "これはりんごじゃないよ"),
       ("This book is mine", "この本は私のものです"),
       ("I usually sleep around this time", "こんな時ごろに寝る"),
       ("Your Japanese is good", "日本語上手ですね"),
       ("I like cats", "私は猫が好き"),
       ("My cat is white", "私の猫が白です");
