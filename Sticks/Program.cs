using System;
using System.Linq;

namespace Sticks
{
    struct Move
    {
        public int row;
        public int number;
        
        public Move(int row, int number)
        {
            this.row = row;
            this.number = number;
        }
    }

    class Program
    {
        static void Main()
        {
            GameLoop();
        }

        static int PromptForInt(string message, int min, int max)
        {
            string response;
            int parsed;
            do
            {
                Console.WriteLine(message + " (" + min + "-" + max + ")");
                response = Console.ReadLine();
            } while (!int.TryParse(response, out parsed) || parsed < min || parsed > max);

            return parsed;
        }

        static bool PromptForYesOrNo(string message)
        {
            string response;
            do
            {
                Console.WriteLine(message + " (y/n)");
                response = Console.ReadLine();
            } while (response.Length != 1 || !"yn".Contains(response));

            return response.Equals("y");
        }

        static int[] BuildBoard(int rows)
        {
            int[] board = new int[rows];
            for (int i = 0; i < rows; i++)
            {
                board[i] = (2 * i + 1);
            }

            return board;
        }

        static void PrintBoard(int[] board)
        {
            for (int i = 0; i < board.Length; i++)
            {
                int stickCount = board[i];
                Console.Write(i + "  ");
                for (int j = 0; j < ((2 * board.Length - 1 - stickCount) + 1) / 2; j++)
                {
                    Console.Write(" ");
                }

                for (int j = 0; j < stickCount; j++)
                {
                    Console.Write("|");
                }
                Console.WriteLine();
            }
        }

        static int GetLineToRemoveFrom(int[] board)
        {
            int row = PromptForInt("Please enter row for sticks to be removed.", 0, board.Length - 1);
            while (board[row] < 1)
            {
                row = PromptForInt("Please pick a row with sticks in it.", 0, board.Length - 1);
            }

            return row;
        }

        static int CheckWinner(int sumSticks, int turn)
        {
            if (sumSticks <= 1)
            {
                return ((turn + sumSticks) % 2) + 1;
            }

            return 0;
        }

        static void GameLoop()
        {
            do
            {
                Console.Clear();
                bool multiplayer = PromptForYesOrNo("Play multiplayer?");

                int rows = PromptForInt("Please enter number of rows for sticks.", 2, 9);
                int[] numberArray = BuildBoard(rows);

                int turn = 0;
                int sumSticks = numberArray.Sum();
                int winner;

                while ((winner = CheckWinner(sumSticks, turn)) == 0)
                {
                    Console.Clear();
                    PrintBoard(numberArray);

                    if (multiplayer || turn % 2 == 0)
                    {
                        if (multiplayer)
                        {
                            Console.WriteLine("Player {0}'s turn.", (turn % 2) + 1);
                        }

                        int changeRow = GetLineToRemoveFrom(numberArray);
                        int num = PromptForInt("Please enter number of sticks to be removed.", 1, numberArray[changeRow]);

                        numberArray[changeRow] -= num;
                        sumSticks -= num;
                    }
                    else
                    {
                        Move AiResponse = NewSmartAi(numberArray);
                        numberArray[AiResponse.row] -= AiResponse.number;
                        sumSticks -= AiResponse.number;
                    }

                    turn++;
                }

                Console.Clear();
                PrintBoard(numberArray);

                string winnerText;

                if (multiplayer)
                {
                    winnerText = "Player " + winner;
                }
                else
                {
                    winnerText = winner == 1 ? "Player" : "AI";
                }

                Console.WriteLine("Game Over! {0} wins! Congratulations on such an incredible achievement!", winnerText);

            } while (PromptForYesOrNo("Play again?"));
        }

        static Move NewSmartAi(int[] board)
        {
            int[] rankedIndices = new int[board.Length];

            for (int i = 0; i < rankedIndices.Length; i++)
            {
                rankedIndices[i] = i;
            }

            rankedIndices = rankedIndices
                .OrderByDescending(t => board[t])
                .ToArray();

            int[] tempBoard = new int[board.Length];
            board.CopyTo(tempBoard, 0);

            for (int i = board[rankedIndices[0]]; i > 0; i--)
            {
                for (int j = 0; j < rankedIndices.Length && board[rankedIndices[j]] >= i; j++)
                {
                    tempBoard[rankedIndices[j]] -= i;

                    int OrAllResult = OrAll(tempBoard);
                    int XOrAllResult = XOrAll(tempBoard);

                    if ((OrAllResult == 1 && XOrAllResult != 0) || (OrAllResult != 1 && XOrAllResult == 0))
                    {
                        return new Move(rankedIndices[j], i);
                    }

                    tempBoard[rankedIndices[j]] += i;
                }
            }

            return new Move(rankedIndices[0], 1); ;
        }

        static int XOrAll(int[] arr)
        {
            if (arr.Length < 1)
            {
                return 0;
            }

            int ret = arr[0];

            for(int i = 1; i < arr.Length; i++)
            {
                ret ^= arr[i];
            }

            return ret;
        }

        static int OrAll(int[] arr)
        {
            if (arr.Length < 1)
            {
                return 0;
            }

            int ret = arr[0];

            for (int i = 1; i < arr.Length; i++)
            {
                ret |= arr[i];
            }

            return ret;
        }
    }
}
