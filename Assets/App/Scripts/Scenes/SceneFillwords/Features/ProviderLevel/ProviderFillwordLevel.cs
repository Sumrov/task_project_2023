using App.Scripts.Scenes.SceneFillwords.Features.FillwordModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace App.Scripts.Scenes.SceneFillwords.Features.ProviderLevel
{
    public class ProviderFillwordLevel : IProviderFillwordLevel
    {
        public GridFillWords LoadModel(int index)
        {
            string[] levelsTxt = File.ReadAllLines("Assets/App/Resources/Fillwords/pack_0.txt");
            string[] wordsTxt = File.ReadAllLines("Assets/App/Resources/Fillwords/words_list.txt");

            var strings = levelsTxt[index - 1].Split(' ');
            List<Word> words = new();

            for (int i = 0; i < strings.Length; i++)
            {
                var element = strings[i];
                
                if (i % 2 == 0)
                {
                    var text = wordsTxt[int.Parse(element)];
                    var word = new Word(text);
                    words.Add(word);
                }
                else
                {
                    var coordinates = element.Split(';');
                    for(var ii = 0; ii < coordinates.Length; ii++)
                        words.Last().CharCoordinates[ii] = int.Parse(coordinates[ii]);
                }
            }

            var countChar = words.SelectMany(o => o.CharCoordinates).Max() + 1;
            var gridSide = (int)Math.Sqrt(countChar);
            var grid = new GridFillWords(new(gridSide, gridSide));
            foreach (var word in words) 
                for (var i = 0; i < word.CharCoordinates.Length; i++)
                    grid.Set(word.CharCoordinates[i] / gridSide, word.CharCoordinates[i] % gridSide, new (word.Text[i]));

            return grid;

            //я думаю скорей всего метод надо как-то декомпозировать если вы могли бы дать совет мне было бы очень интересно
            //возможно мне стоило получше подумать над названиями переменных
            //так же я не совсем уверен стоило ли использовать tryparse в данном случае 
            //насколько я знаю указывать пути к файлам таким хард кодом не очень хорошая практика, тоже бы послушал ваше мнение на этот счёт
        }

        private class Word
        {
            public string Text;
            public int[] CharCoordinates;

            public Word(string text)
            {
                Text = text;
                CharCoordinates = new int[text.Length];
            }
        }
    }
}