using System;
using System.Collections.Generic;

namespace Flashunity.Scroogle
{
    public static class Scroggle
    {

        static int minWordLength;
        // yox
        // rba
        // ved
        private static Cell[] board;
        private static int boardWidth;
        private static int boardHeight;

        private static Dictionary<string, string> dictionary;
        private static Dictionary<char, int> lettersMultipliers;
        private static Dictionary<int, int> wordLenghtScores;

        /// <summary>
        /// Set all board parameters and return all words and total score     
        /// </summary>
        public static string[] SetBoard(int boardWidth, int minWordLength, char[] letters, int[] multipliers, Dictionary<string, string> dictionary, Dictionary<char, int> lettersMultipliers, Dictionary<int, int> wordLenghtScores, out int totalScore)
        {
            totalScore = 0;

            if (boardWidth <= 0 || boardWidth > letters.Length || letters.Length % boardWidth != 0)
            {
                throw new ArgumentOutOfRangeException("boardWidth", boardWidth, "Should be more than 0 and less then letters amount");
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

        static string[] GetAllWords(ref int totalScore)
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


        /// <summary>
        /// return total score for selected board indices
        /// </summary>
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

        static int GetCellScore(Cell cell)
        {
            int score;
            if (!lettersMultipliers.TryGetValue(cell.letter, out score)) score = 1;

            score = score * cell.multiplier;

            return score;
        }

        static void FillBoard(char[] letters, int[] multipliers)
        {
            board = new Cell[letters.Length];

            for (int i = 0; i < letters.Length; i++)
                board[i] = new Cell(letters[i], i, multipliers[i]);

            UpdateCellsNeigbors();
        }

        static void UpdateCellsNeigbors()
        {
            for (int i = 0; i < board.Length; i++)
                UpdateCellNeigbors(board[i]);
        }

        static void UpdateCellNeigbors(Cell cell)
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
                            cell.AddNeighbor(board[index]);
                    }
                }
        }

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

        internal static int GetIndexByCoords(int x, int y)
        {
            if (x < 0 || x >= boardWidth || y < 0 || y >= boardHeight)
            {
                return -1;
            }

            return boardHeight * y + x;
        }

    }

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

        public void AddNeighbor(Cell cell)
        {
            neighbors.Add(cell);
        }

    }

}

