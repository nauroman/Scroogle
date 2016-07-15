# Scroogle
Scroogle game code

**Usage:**

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Flashunity.Scroogle

    class MainClass
    {
        static int minWordLength = 4;
        static string lettersString = "yoxrbaved";

        // yox
        // rba
        // ved

        static string dictionaryString = "i,need,this,job,give,me,a,chance,and,you,wont,regret,bred,yore,byre,abed,oread,bore,orby,robed,broad,byroad,robe,bored,derby,bade,aero,read,orbed,verb,aery,bead,bread,very,road";

        static char[] letters;
        static int[] multipliers = new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        static Dictionary<string, string> dictionary;
        static Dictionary<char, int> lettersMultipliers = new Dictionary<char, int> { { 'y', 3 }, { 'o', 2 }, { 'k', 2 } };
        static Dictionary<int, int> wordLenghtScores = new Dictionary<int, int> { { 2, 1 }, { 3, 1 }, { 4, 1 }, { 5, 2 }, { 6, 3 }, { 7, 5 }, { 0, 11 } };

        public static void Main(string[] args)
        {
            var arr = dictionaryString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            dictionary = arr.ToDictionary(item => item, item => item);
            letters = lettersString.ToCharArray();

            int totalScore;
            var allWords = Scroggle.SetBoard(3, minWordLength, letters, multipliers, dictionary, lettersMultipliers, wordLenghtScores, out totalScore);

            Console.WriteLine("allWords.Length: " + allWords.Length);
            Console.WriteLine("score: " + totalScore);
        }
    }

