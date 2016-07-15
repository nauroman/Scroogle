using NUnit.Framework;
using System;
using Flashunity.Scroogle;
using System.Collections.Generic;
using System.Linq;


namespace MonoTests.Flashunity.Scroogle
{

    // the class name should end with "Test" and start with the name of the class
    // you are testing, e.g. CollectionBaseTest
    [TestFixture]
    public class ScroogleTest// : AssertionHelper
    {
        int minWordLength = 4;
        string lettersString = "yoxrbaved";

        static string dictionaryString = "i,need,this,job,give,me,a,chance,and,you,wont,regret,bred,yore,byre,abed,oread,bore,orby,robed,broad,byroad,robe,bored,derby,bade,aero,read,orbed,verb,aery,bead,bread,very,road";

        char[] letters;
        int[] multipliers = new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        Dictionary<string, bool> dictionary;
        //        Dictionary<char, int> lettersMultipliers = new Dictionary<char, int> { { 'q', 3 }, { 'z', 2 }, { 'k', 2 } };
        Dictionary<char, int> lettersMultipliers = new Dictionary<char, int> { { 'y', 3 }, { 'o', 2 }, { 'k', 2 } };
        Dictionary<int, int> wordLenghtScores = new Dictionary<int, int> { { 2, 1 }, { 3, 1 }, { 4, 1 }, { 5, 2 }, { 6, 3 }, { 7, 5 }, { 0, 11 } };

        public ScroogleTest()
        {
            var arr = dictionaryString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            //dictionary = arr.ToDictionary(item => item, item => item);
            dictionary = arr.ToDictionary(item => item, item => true);
            letters = lettersString.ToCharArray();
        }

        // this method is run before each [Test] method is called. You can put
        // variable initialization, etc. here that is common to each test.
        // Just leave the method empty if you don't need to use it.
        // The name of the method does not matter; the attribute does.
        [SetUp]
        public void GetReady()
        {
        }

        // this method is run after each Test* method is called. You can put
        // clean-up code, etc. here.  Whatever needs to be done after each test.
        // Just leave the method empty if you don't need to use it.
        // The name of the method does not matter; the attribute does.
        [TearDown]
        public void Clean() { }

        [Test]
        public void WrongBoardWidth()
        {
            int totalScore;
            try
            {
                Scroggle.SetBoard(0, minWordLength, letters, multipliers, dictionary, lettersMultipliers, wordLenghtScores, out totalScore);
            }
            catch (Exception e)
            {
                Assert.True(e.Message.IndexOf("Should be more than 1 and less then letters amount") == 0, "board width is 0");
            }
        }

        [Test]
        public void WrongMinWordLength()
        {
            int totalScore;
            try
            {
                Scroggle.SetBoard(3, 1, letters, multipliers, dictionary, lettersMultipliers, wordLenghtScores, out totalScore);
            }
            catch (Exception e)
            {
                Assert.True(e.Message.IndexOf("Should be more than 1") == 0, "minWordLength is 1");
            }
        }

        [Test]
        public void WrongLetters()
        {
            int totalScore;
            try
            {
                Scroggle.SetBoard(2, 3, new char[] { 'a', 'b' }, new int[] { 1, 1 }, dictionary, lettersMultipliers, wordLenghtScores, out totalScore);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Assert.True(e.Message.IndexOf("Should be minWordLength letters or more") == 0, "2 letters");
            }
        }

        [Test]
        public void WrongMultipliers()
        {
            int totalScore;
            try
            {
                Scroggle.SetBoard(3, minWordLength, letters, new int[] { 1, 1, 1 }, dictionary, lettersMultipliers, wordLenghtScores, out totalScore);
            }
            catch (Exception e)
            {
                Assert.True(e.Message.IndexOf("Should be equal to letters length") == 0, "3 multipliers");
            }
        }

        [Test]
        public void WrongDictionary()
        {
            int totalScore;
            try
            {
                //    Scroggle.SetBoard(3, minWordLength, letters, multipliers, new Dictionary<string, string> { }, lettersMultipliers, wordLenghtScores, out totalScore);
                Scroggle.SetBoard(3, minWordLength, letters, multipliers, new Dictionary<string, bool> { }, lettersMultipliers, wordLenghtScores, out totalScore);
            }
            catch (Exception e)
            {
                Assert.True(e.Message.IndexOf("Length must be more than 0") == 0, "dictionary length is 0");
            }
        }

        [Test]
        public void GetCoordsByIndex()
        {
            int totalScore;
            Scroggle.SetBoard(3, minWordLength, letters, multipliers, dictionary, lettersMultipliers, wordLenghtScores, out totalScore);

            int x;
            int y;
            Scroggle.GetCoordsByIndex(0, out x, out y);
            Assert.AreEqual(0, x, "x=0");
            Assert.AreEqual(0, y, "y=0");

            Scroggle.GetCoordsByIndex(3, out x, out y);
            Assert.AreEqual(0, x, "x=0");
            Assert.AreEqual(1, y, "y=1");

            Scroggle.GetCoordsByIndex(4, out x, out y);
            Assert.AreEqual(1, x, "x=1");
            Assert.AreEqual(1, y, "y=1");

            Scroggle.GetCoordsByIndex(8, out x, out y);
            Assert.AreEqual(2, x, "x=2");
            Assert.AreEqual(2, y, "y=2");

            Assert.IsFalse(Scroggle.GetCoordsByIndex(-1, out x, out y), "index=-1");
            Assert.IsFalse(Scroggle.GetCoordsByIndex(letters.Length, out x, out y), "index=" + letters.Length);
        }

        [Test]
        public void GetIndexByCoords()
        {
            int totalScore;
            Scroggle.SetBoard(3, minWordLength, letters, multipliers, dictionary, lettersMultipliers, wordLenghtScores, out totalScore);

            Assert.AreEqual(0, Scroggle.GetIndexByCoords(0, 0), "0,0");
            Assert.AreEqual(3, Scroggle.GetIndexByCoords(0, 1), "0,1");
            Assert.AreEqual(8, Scroggle.GetIndexByCoords(2, 2), "2,2");
            Assert.AreEqual(-1, Scroggle.GetIndexByCoords(-1, 0), "-1,0");
            Assert.AreEqual(-1, Scroggle.GetIndexByCoords(0, -1), "0,-1");
            Assert.AreEqual(-1, Scroggle.GetIndexByCoords(3, 0), "3,0");
            Assert.AreEqual(-1, Scroggle.GetIndexByCoords(0, 3), "0,3");
            Assert.AreEqual(-1, Scroggle.GetIndexByCoords(3, 3), ",3");
        }

        [Test]
        public void GetIndicesScore()
        {
            int totalScore;
            Scroggle.SetBoard(3, minWordLength, letters, multipliers, dictionary, lettersMultipliers, wordLenghtScores, out totalScore);

            try
            {
                Scroggle.GetIndicesScore(new int[0]);
            }
            catch (Exception e)
            {
                Assert.True(e.Message.IndexOf("Should be more than 1 and less or equal letters amount") == 0, "length is 0");
            }

            try
            {
                Scroggle.GetIndicesScore(new int[letters.Length + 1]);
            }
            catch (Exception e)
            {
                Assert.True(e.Message.IndexOf("Should be more than 1 and less or equal letters amount") == 0, "length is: " + (letters.Length + 1));
            }

            Assert.AreEqual(0, Scroggle.GetIndicesScore(new int[] { 0, 1 }), "0,1");


            Assert.AreEqual(6, Scroggle.GetIndicesScore(new int[] { 0, 1, 2, 3 }), "0,1,2,3");
        }

        [Test]
        public void AllWords2x2()
        {
            int totalScore;
            var allWords = Scroggle.SetBoard(2, 2, new char[] { 'a', 'b', 'c', 'd' }, new int[] { 1, 1, 1, 1 },
                                             new Dictionary<string, bool> { { "ab", true } }, new Dictionary<char, int> { { 'a', 1 } },
                                             wordLenghtScores, out totalScore);
            Assert.AreEqual(1, totalScore, "totalScore=1");
            Assert.AreEqual(1, allWords.Length, "allWords=1");
            Assert.AreEqual("ab", allWords[0], "allWords[0]");
        }

        [Test]
        public void AllWords2x3()
        {
            int totalScore;
            var allWords = Scroggle.SetBoard(2, 2, new char[] { 'a', 'b', 'c', 'd', 'e', 'f' }, new int[] { 1, 1, 1, 1, 1, 1 },
                                             new Dictionary<string, bool> { { "ab", true } }, new Dictionary<char, int> { { 'a', 1 } },
                                             wordLenghtScores, out totalScore);
            Assert.AreEqual(1, totalScore, "totalScore=1");
            Assert.AreEqual(1, allWords.Length, "allWords=1");

            allWords = Scroggle.SetBoard(2, 2, new char[] { 'a', 'b', 'c', 'd', 'e', 'f' }, new int[] { 1, 1, 1, 1, 1, 1 },
                                 new Dictionary<string, bool> { { "ab", true } }, new Dictionary<char, int> { { 'a', 1 } },
                                 wordLenghtScores, out totalScore);
            Assert.AreEqual(1, totalScore, "totalScore=1");
            Assert.AreEqual(1, allWords.Length, "allWords=1");

            allWords = Scroggle.SetBoard(2, 2, new char[] { 'a', 'b', 'c', 'd', 'e', 'f' }, new int[] { 1, 1, 1, 1, 1, 1 },
                     new Dictionary<string, bool> { { "ab", true }, { "bc", true } }, new Dictionary<char, int> { { 'a', 1 } },
                     wordLenghtScores, out totalScore);
            Assert.AreEqual(2, totalScore, "totalScore=2");
            Assert.AreEqual(2, allWords.Length, "allWords=2");
            Assert.AreEqual("ab", allWords[0], "allWords[0]");
            Assert.AreEqual("bc", allWords[1], "allWords[1]");

            allWords = Scroggle.SetBoard(2, 2, new char[] { 'a', 'b', 'c', 'd', 'e', 'f' }, new int[] { 1, 1, 1, 1, 1, 1 },
         new Dictionary<string, bool> { { "ab", true }, { "bc", true }, { "abcdef", true } }, new Dictionary<char, int> { { 'a', 1 } },
         wordLenghtScores, out totalScore);
            Assert.AreEqual(5, totalScore, "totalScore=3");
            Assert.AreEqual(3, allWords.Length, "allWords=3");

            allWords = Scroggle.SetBoard(2, 2, new char[] { 'a', 'b', 'c', 'd', 'e', 'f' }, new int[] { 2, 1, 1, 1, 1, 1 },
new Dictionary<string, bool> { { "ab", true }, { "bc", true }, { "abcdef", true } }, new Dictionary<char, int> { { 'a', 2 } },
wordLenghtScores, out totalScore);
            Assert.AreEqual(17, totalScore, "totalScore=17");
            Assert.AreEqual(3, allWords.Length, "allWords=3");

            allWords = Scroggle.SetBoard(2, 2, new char[] { 'a', 'b', 'c', 'd', 'e', 'f' }, new int[] { 2, 2, 1, 1, 1, 1 },
new Dictionary<string, bool> { { "ab", true } }, new Dictionary<char, int> { { 'a', 2 }, { 'b', 3 } },
wordLenghtScores, out totalScore);
            Assert.AreEqual(24, totalScore, "totalScore=24");
            Assert.AreEqual(1, allWords.Length, "allWords=3");

        }

        [Test]
        public void AllWords3x3()
        {
            int totalScore;
            var allWords = Scroggle.SetBoard(3, minWordLength, letters, multipliers, dictionary, lettersMultipliers, wordLenghtScores, out totalScore);
            Assert.AreEqual(81, totalScore, "totalScore=81");
            Assert.AreEqual(23, allWords.Length, "allWords=23");
        }

    }
}