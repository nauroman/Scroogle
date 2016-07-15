using System;
using System.Collections.Generic;

namespace Flashunity.Scroogle
{
    public static class Scroggle
    {

        /// <summary>Mininum length of a word. Should be more than 1</summary>
        static int minWordLength;
        // yox
        // rba
        // ved
        /// <summary>Square game board. Each Cell is a letter, multiplier, cell index and neigbors cells</summary>
        private static Cell[] board;

        /// <summary>Game board width. Should be more then 1</summary>
        private static int boardWidth;

        /// <summary>Game board height. Should be more then 1</summary>
        private static int boardHeight;

        /// <summary>Words dictionary. It runs fast for ContainsKey, according that article: http://cc.davelozinski.com/c-sharp/fastest-collection-for-string-lookups</summary>
        private static Dictionary<string, bool> dictionary;

        /// <summary>Multiplier for each letter</summary>
        private static Dictionary<char, int> lettersMultipliers;

        /// <summary>Score for word length. For default (max) length use key 0</summary>
        private static Dictionary<int, int> wordLenghtScores;

        /// <summary>Set all board parameters and return all possible words and total score</summary>
        public static string[] SetBoard(int boardWidth, int minWordLength, char[] letters, int[] multipliers, Dictionary<string, bool> dictionary, Dictionary<char, int> lettersMultipliers, Dictionary<int, int> wordLenghtScores, out int totalScore)
        {
            totalScore = 0;

            if (boardWidth <= 1 || boardWidth > letters.Length || letters.Length % boardWidth != 0)
            {
                throw new ArgumentOutOfRangeException("boardWidth", boardWidth, "Should be more than 1 and less then letters amount");
                return new string[0];
            }

            if (minWordLength <= 1)
            {
                throw new ArgumentOutOfRangeException("minWordLength", minWordLength, "Should be more than 1");
                return new string[0];
            }

            if (letters.Length < minWordLength)
            {
                throw new ArgumentOutOfRangeException("letters", letters, "Should be minWordLength letters or more");
                return new string[0];
            }

            if (multipliers.Length != letters.Length)
            {
                throw new ArgumentOutOfRangeException("multipliers", multipliers, "Should be equal to letters length");
                return new string[0];
            }

            if (dictionary.Count <= 0)
            {
                throw new ArgumentOutOfRangeException("dictionary", dictionary, "Length must be more than 0");
                return new string[0];
            }

            Scroggle.boardWidth = boardWidth;
            Scroggle.boardHeight = letters.Length / boardWidth;
            Scroggle.minWordLength = minWordLength;
            FillBoard(letters, multipliers);
            Scroggle.dictionary = dictionary;
            Scroggle.lettersMultipliers = lettersMultipliers;
            Scroggle.wordLenghtScores = wordLenghtScores;

            return GetAllWords(ref totalScore);
        }


        /// <summary>Get all words and total score</summary>
        public static string[] GetAllWords(ref int totalScore)
        {
            totalScore = 0;
            var words = new List<string>();

            for (int i = 0; i < board.Length; i++)
            {
                var usedCells = new List<Cell>();
                var word = "";
                GetNextCell(board[i], word, words, ref totalScore, usedCells);
            }

            return words.ToArray();
        }

        /// <summary>Recursive function to collect all possible words for the cell</summary>
        static void GetNextCell(Cell cell, string word, List<string> words, ref int totalScore, List<Cell> usedCells)
        {
            if (!usedCells.Contains(cell))
            {
                usedCells.Add(cell);
                word += cell.letter;

                if (word.Length >= minWordLength)
                {
                    if (dictionary.ContainsKey(word))
                    {
                        words.Add(word);
                        totalScore += GetCellsScore(usedCells.ToArray());
                    }
                }
            }

            for (int i = 0; i < cell.neighbors.Count; i++)
            {
                var neighbor = cell.neighbors[i];

                if (!usedCells.Contains(neighbor))
                {
                    //                    thisCellUsedCells.Add(neighbor);
                    GetNextCell(neighbor, word, words, ref totalScore, usedCells);
                }
            }

            word = word.Remove(word.Length - 1);

            usedCells.Remove(cell);
        }


        /// <summary>Return total score for selected board indices</summary>
        public static int GetIndicesScore(int[] indices)
        {
            if (indices.Length <= 1 || indices.Length > board.Length)
            {
                throw new ArgumentOutOfRangeException("indices", indices, "Should be more than 1 and less or equal letters amount");
                return 0;
            }

            var cells = new Cell[indices.Length];

            for (int i = 0; i < indices.Length; i++)
                cells[i] = board[indices[i]];

            return GetCellsScore(cells);
        }

        /// <summary>Returns score for cells using cells multiplier and letters multiplier</summary>
        static int GetCellsScore(Cell[] cells)
        {
            if (cells.Length < minWordLength) return 0;

            int score;

            if (!wordLenghtScores.TryGetValue(cells.Length, out score))
            {
                score = wordLenghtScores[0];
            }

            for (int i = 0; i < cells.Length; i++)
                score = score * GetCellScore(cells[i]);

            return score;
        }

        /// <summary>Returns score for a cell using the cell multiplier and the letter multiplier</summary>
        static int GetCellScore(Cell cell)
        {
            int score;
            if (!lettersMultipliers.TryGetValue(cell.letter, out score)) score = 1;

            score = score * cell.multiplier;

            return score;
        }

        /// <summary>Convert letters and multipliers to Cells and add cells to the board</summary>
        static void FillBoard(char[] letters, int[] multipliers)
        {
            board = new Cell[letters.Length];

            for (int i = 0; i < letters.Length; i++)
                board[i] = new Cell(letters[i], i, multipliers[i]);

            // add neighbors for each cell
            UpdateCellsNeighbors();
        }

        /// <summary>add neighbors for each cell</summary>
        static void UpdateCellsNeighbors()
        {
            for (int i = 0; i < board.Length; i++)
                UpdateCellNeighbors(board[i]);
        }

        /// <summary>Update cell neighbors - cells from left, right, top and bottom</summary>
        static void UpdateCellNeighbors(Cell cell)
        {
            int cellX;
            int cellY;

            GetCoordsByIndex(cell.index, out cellX, out cellY);

            for (int x = cellX - 1; x <= cellX + 1; x++)
                for (int y = cellY - 1; y <= cellY + 1; y++)
                {
                    if (x != cellX || y != cellY)
                    {
                        int index = GetIndexByCoords(x, y);
                        if (index != -1)
                            cell.neighbors.Add(board[index]);
                    }
                }
        }

        /// <summary>Returns coord by single dimension board index</summary>
        internal static bool GetCoordsByIndex(int index, out int x, out int y)
        {
            if (index < 0 || index >= board.Length)
            {
                x = 0;
                y = 0;
                return false;
            }

            y = index / boardWidth;
            x = index - y * boardWidth;
            return true;
        }

        /// <summary>Returns single dimension board index by coord</summary>
        internal static int GetIndexByCoords(int x, int y)
        {
            if (x < 0 || x >= boardWidth || y < 0 || y >= boardHeight)
            {
                return -1;
            }

            return boardWidth * y + x;
        }

    }

    /// <summary>Game board cell. Each Cell is a letter, multiplier, cell index and neigbors cells</summary>
    class Cell
    {
        public readonly char letter;
        public readonly int multiplier;
        public readonly int index;
        public List<Cell> neighbors;

        public Cell(char letter, int index, int multiplier)
        {
            this.letter = letter;
            this.multiplier = multiplier;
            this.index = index;
            neighbors = new List<Cell>();
        }

    }

}

