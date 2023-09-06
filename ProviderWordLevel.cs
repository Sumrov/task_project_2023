using App.Scripts.Scenes.SceneWordSearch.Features.Level.Models.Level;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace App.Scripts.Scenes.SceneWordSearch.Features.Level.BuilderLevelModel.ProviderWordLevel
{
    public class ProviderWordLevel : IProviderWordLevel
    {
        public LevelInfo LoadLevelData(int levelIndex)
        {
            string folderPath = Path.Combine(Application.dataPath, "App/Resources/WordSearch/Levels");
            string[] filePaths = Directory.GetFiles(folderPath, "*.json");

            var jsonText = File.ReadAllText(filePaths[levelIndex - 1]);
            var wordData = JsonUtility.FromJson<WordData>(jsonText);
            LevelInfo levelInfo = new() { words = wordData.words };

            return levelInfo;
        }
        //я знаю такое мнение что использовать оператор var везде где можно не всегда правильно
        //потому что читаемость кода снижается и вот интересно было бы узнать действительно ли это так.
        //и опять же проверки входных данных нужно ли все оборачивать в try catch или нет не уверен.
        //я знаю что есть разные способы присвоить значения списка другому
        //не уверен правильно ли в данном случае делать это таким образом

        [System.Serializable]
        private class WordData
        {
            public List<string> words;
        }
    }
}