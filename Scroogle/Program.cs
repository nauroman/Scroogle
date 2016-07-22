using System;
using System.Collections.Generic;
using System.Linq;

namespace Flashunity.Scroogle
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            SetBoard();
        }

        /// <summary>Set all board parameters and return all possible words and total score</summary>
        static void SetBoard()
        {
            int minWordLength = 4;
            string lettersString = "yoxrbaved";

            // yox
            // rba
            // ved

            string dictionaryString = "i,need,this,job,give,me,a,chance,and,you,wont,regret,bred,yore,byre,abed,oread,bore,orby,robed,broad,byroad,robe,bored,derby,bade,aero,read,orbed,verb,aery,bead,bread,very,road";

            // board letters
            char[] letters;
            // multipliers for each board cell
            int[] multipliers = new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1 };

            // all possible words dictionary
            Dictionary<string, bool> dictionary;
            // letters multipliers for some letters, for other letters multiplier is 1
            Dictionary<char, int> lettersMultipliers = new Dictionary<char, int> { { 'y', 3 }, { 'o', 2 }, { 'k', 2 } };

            // length 0 item is used for all words length more than 7
            Dictionary<int, int> wordLenghtScores = new Dictionary<int, int> { { 2, 1 }, { 3, 1 }, { 4, 1 }, { 5, 2 }, { 6, 3 }, { 7, 5 }, { 0, 11 } };

            // fill all possible words dictionary from a string
            var arr = dictionaryString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            dictionary = arr.ToDictionary(item => item, item => true);

            // fill board from string
            letters = lettersString.ToCharArray();

            // getting all words for 3x3 board
            int totalScore;
            var allWords = Scroggle.SetBoard(3, minWordLength, letters, multipliers, dictionary, lettersMultipliers, wordLenghtScores, out totalScore);

            Console.WriteLine("allWords.Length: " + allWords.Length);
            Console.WriteLine("score: " + totalScore);
        }
    }
}
