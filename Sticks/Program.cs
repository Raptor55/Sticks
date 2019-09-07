using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sticks
{
    struct winlose
    {
        public winlose(int a, int b)
        {
            wins = a;
            losses = b;
        }
        public int wins;
        public int losses;
    }

    struct symmetry
    {
        public symmetry(int a)
        {
            occurences = 0;
            indexes = new List<int>();
        }
        public int occurences;
        public List<int> indexes;
    }

    class Program
    {
        static void Main(string[] args)
        {
            gameLoop();
        }

        static int promptForInt(string message, int min, int max)
        {
            string response = "";
            int parsed = 0;
            do
            {
                Console.WriteLine(message + " (" + min + "-" + max + ")");
                response = Console.ReadLine();
            } while (!int.TryParse(response, out parsed) || parsed < min || parsed > max);

            return parsed;
        }

        static bool promptForYesOrNo(string message)
        {
            string response = "";
            do
            {
                Console.WriteLine(message + " (y/n)");
                response = Console.ReadLine();
            } while (response.Length != 1 || !"yn".Contains(response));

            return response.Equals("y");
        }

        static int[] buildBoard(int rows)
        {
            int[] board = new int[rows];
            for (int i = 0; i < rows; i++)
            {
                board[i] = (2 * i + 1);
            }

            return board;
        }

        static void printBoard(int[] board)
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

        static int getLineToRemoveFrom(int[] board)
        {
            int row = promptForInt("Please enter row for sticks to be removed.", 0, board.Length - 1);
            while (board[row] < 1)
            {
                row = promptForInt("Please pick a row with sticks in it.", 0, board.Length - 1);
            }

            return row;
        }

        static int checkWinner(int sumSticks, int turn)
        {
            if (sumSticks <= 1)
            {
                return ((turn + sumSticks) % 2) + 1;
            }

            return 0;
        }

        static void gameLoop()
        {
            do
            {
                Console.Clear();
                bool multiplayer = promptForYesOrNo("Play multiplayer?");

                int rows = promptForInt("Please enter number of rows for sticks.", 2, 9);
                int[] numberArray = buildBoard(rows);

                int turn = 0;
                int sumSticks = numberArray.Sum();
                int winner = 0;

                while ((winner = checkWinner(sumSticks, turn)) == 0)
                {
                    Console.Clear();
                    printBoard(numberArray);

                    if (multiplayer || turn % 2 == 0)
                    {
                        if (multiplayer)
                        {
                            Console.WriteLine("Player {0}'s turn.", (turn % 2) + 1);
                        }

                        int changeRow = getLineToRemoveFrom(numberArray);
                        int num = promptForInt("Please enter number of sticks to be removed.", 1, numberArray[changeRow]);

                        numberArray[changeRow] -= num;
                        sumSticks -= num;
                    }
                    else
                    {
                        winlose AiResponse = SmartAi(numberArray, sumSticks);
                        numberArray[AiResponse.wins] -= AiResponse.losses;
                        sumSticks -= AiResponse.losses;
                    }

                    turn++;
                }

                Console.Clear();
                printBoard(numberArray);

                string winnerText = "";

                if (multiplayer)
                {
                    winnerText = "Player " + winner;
                }
                else
                {
                    winnerText = winner == 1 ? "Player" : "AI";
                }

                Console.WriteLine("Game Over! {0} wins!", winnerText);

            } while (promptForYesOrNo("Play again?"));
        }

        static winlose SmartAi(int[] numberArray, int sum)
        {
            int numRows = numberRows(numberArray);
            int num = 0;
            int row = 0;
            int sumSticks = 0;

            for (int i = 0; i < numberArray.Length; i++)
            {
                sumSticks += numberArray[i];
            }

            for (int i = 0; i < numberArray.Length; i++)
            {
                if (numberArray[i] == 0)
                {
                    continue;
                }

                if (numberArray[i] == 1)
                {
                    num++;
                }

                else
                {
                    row = i;
                }
            }
            if (num == numRows && num % 2 == 0)
            {
                for (int i = 0; i < numberArray.Length; i++)
                {
                    if (numberArray[i] > 0)
                    {
                        return new winlose(i, numberArray[i]);
                    }
                }
                    return new winlose(row, numberArray[row]);
            }
            
            if (num == numRows - 1)
            {
                if ((numRows) % 2 == 0)
                {
                    return new winlose(row, numberArray[row]);
                }

                else 
                {
                    return new winlose(row, numberArray[row] - 1);
                }
            }

            else if (numRows % 2 == 0)
            { 
                symmetry[] pairArray = new symmetry[2*numberArray.Length + 1];
                
                for(int i = 0; i < numberArray.Length; i++)
                {
                    pairArray[numberArray[i]] = new symmetry(1);
                }
                
                for (int i = 0; i < numberArray.Length; i++)
                {
                    pairArray[numberArray[i]].occurences++;
                    pairArray[numberArray[i]].indexes.Add(i);
                }

                int count = 0;
                int firstfucked = 0;
                int secondfucked = 0;
                
                for (int i = 1; i < pairArray.Length; i ++)
                {
                    if (pairArray[i].occurences % 2 == 1)
                    {
                        count++;
                        if (count == 1)
                        {
                            for (int j = 0; j < numberArray.Length; j++)
                            {
                                if (numberArray[j] == i)
                                {
                                    firstfucked = j;
                                    break;
                                }
                            }
                        }
                        
                        else if (count == 2)
                        {
                            for (int j = 0; j < numberArray.Length; j++)
                            {
                                if (numberArray[j] == i)
                                {
                                    secondfucked = j;
                                    break;
                                }
                            }
                        }
                    }
                    
                    if (count > 2)
                    {
                        break;
                    }
                }
                
                if (count == 2)
                {
                    if (numberArray[firstfucked] > numberArray[secondfucked])
                    {
                        return new winlose(firstfucked, numberArray[firstfucked]-numberArray[secondfucked]);
                    }
                    
                    else
                    {
                        return new winlose(secondfucked, numberArray[secondfucked] - numberArray[firstfucked]);
                    }
                }
            }
            
            if (numRows % 2 == 1)
            {
                symmetry[] pairArray = new symmetry[2*numberArray.Length + 1];

                for (int i = 0; i < pairArray.Length; i++)
                {
                    pairArray[i] = new symmetry(1);
                }

                    for (int i = 0; i < numberArray.Length; i++)
                    {
                        pairArray[numberArray[i]].occurences++;
                        pairArray[numberArray[i]].indexes.Add(i);
                    }

                int count = 0;
                int firstfucked = 0;
                
                for (int i = 1; i < pairArray.Length; i ++)
                {
                    if (pairArray[i].occurences % 2 == 1)
                    {
                        count++;
                        if (count == 1)
                        {
                            for (int j = 0; j < numberArray.Length; j++)
                            {
                                if (numberArray[j] == i)
                                {
                                    firstfucked = j;
                                }
                            }   
                        }
                    }
                    
                    if (count > 1)
                    {
                        break;
                    }
                }
                
                if (count == 1)
                {
                    return new winlose(firstfucked, numberArray[firstfucked]);
                }
            }

            if (numRows == 3 || numRows == 4)
            {
                if (numRows == 3)
                {
                    int [] indexes = new int [3];
                    int count = 0;
                    
                    for (int i = 0; i < numberArray.Length; i++)
                    {
                        if (numberArray[i] > 0)
                        {
                            if (count < 3)
                            {
                                indexes[count] = i;
                                count++;
                            }
                        }
                    }

                    int gcd = 0;
                    int temp = 0;

                    gcd = GCD(numberArray[indexes[0]], numberArray[indexes[1]]);
                    temp = Math.Abs(numberArray[indexes[0]] / gcd - numberArray[indexes[1]] / gcd);
                    if (temp == 1)
                    {
                        if (numberArray[indexes[0]]/gcd % 2 == 1)
                        {
                            if ((numberArray[indexes[2]] - numberArray[indexes[1]]) > gcd)
                            {
                                return new winlose (indexes[2], (numberArray[indexes[2]] - (numberArray[indexes[1]] - gcd)));
                            }
                        }
                        else
                        {
                            if (numberArray[indexes[2]] > (numberArray[indexes[0]]-gcd))
                            {
                                return new winlose (indexes[2], numberArray[indexes[2]]-(numberArray[indexes[0]]-gcd));
                            }
                        }
                    }

                    gcd = GCD(numberArray[indexes[1]], numberArray[indexes[2]]);
                    temp = Math.Abs(numberArray[indexes[1]] / gcd - numberArray[indexes[2]] / gcd);
                    if (temp == 1)
                    {
                        if (numberArray[indexes[1]]/gcd % 2 == 1)
                        {
                            if ((numberArray[indexes[0]] - numberArray[indexes[2]]) > gcd)
                            {
                                return new winlose (indexes[0], (numberArray[indexes[0]] - (numberArray[indexes[2]] - gcd)));
                            }
                        }
                        else
                        {
                            if (numberArray[indexes[0]] > (numberArray[indexes[1]]-gcd))
                            {
                                return new winlose (indexes[0], numberArray[indexes[0]]-(numberArray[indexes[1]]-gcd));
                            }
                        }
                    }

                    gcd = GCD(numberArray[indexes[2]], numberArray[indexes[0]]);
                    temp = Math.Abs(numberArray[indexes[2]] / gcd - numberArray[indexes[0]] / gcd);
                    if (temp == 1)
                    {
                        if (numberArray[indexes[2]]/gcd % 2 == 1)
                        {
                            if ((numberArray[indexes[1]] - numberArray[indexes[0]]) > gcd)
                            {
                                return new winlose (indexes[1], (numberArray[indexes[1]] - (numberArray[indexes[0]] - gcd)));
                            }
                        }
                        else
                        {
                            if (numberArray[indexes[1]] > (numberArray[indexes[2]]-gcd))
                            {
                                return new winlose (indexes[1], numberArray[indexes[1]]-(numberArray[indexes[2]]-gcd));
                            }
                        }
                    }

                    //second time for adding shit in middle instead of on ends

                    gcd = GCD(numberArray[indexes[0]], numberArray[indexes[1]]);
                    temp = Math.Abs(numberArray[indexes[1]] / gcd - numberArray[indexes[0]] / gcd);
                    if (temp == 2)
                    {
                        if (numberArray[indexes[1]] / gcd % 2 == 1)
                        {
                            if ((numberArray[indexes[2]] - numberArray[indexes[0]]) >= gcd)
                            {
                                return new winlose(indexes[2], (numberArray[indexes[2]] - numberArray[indexes[0]] - gcd));
                            }
                        }
                    }

                    gcd = GCD(numberArray[indexes[1]], numberArray[indexes[2]]);
                    temp = Math.Abs(numberArray[indexes[1]] / gcd - numberArray[indexes[2]] / gcd);
                    if (temp == 2)
                    {
                        if (numberArray[indexes[1]] / gcd % 2 == 1)
                        {
                            if ((numberArray[indexes[0]] - numberArray[indexes[2]]) >= gcd)
                            {
                                return new winlose(indexes[0], (numberArray[indexes[0]] - numberArray[indexes[2]] - gcd));
                            }
                        }
                    }

                    gcd = GCD(numberArray[indexes[2]], numberArray[indexes[0]]);
                    temp = Math.Abs((numberArray[indexes[2]] / gcd) - (numberArray[indexes[0]] / gcd));
                    if (temp == 2)
                    {
                        if (numberArray[indexes[2]] / gcd % 2 == 1)
                        {
                            if ((numberArray[indexes[1]] - numberArray[indexes[0]]) >= gcd)
                            {
                                return new winlose(indexes[1], (numberArray[indexes[1]] - numberArray[indexes[0]] - gcd));
                            }
                        }
                    }
                }
                else
                {
                    int [] indexes = new int [4];
                    int count = 0;
                    for (int i =0; i < numberArray.Length; i++)
                    {
                        if (numberArray[i] > 0)
                        {
                            if (count < 4)
                            {
                                indexes[count] = i;
                                count++;
                            }
                        }
                    }

                    for (int i = 0; i < 4; i++)
                    {
                        if (i == 0)
                        {
                            int gcd = GCD(GCD(numberArray[indexes[1]], numberArray[indexes[2]]), numberArray[indexes[3]]);
                        
                            int [] temp = new int[] {numberArray[indexes[1]]/gcd, numberArray[indexes[2]]/gcd, numberArray[indexes[3]]/gcd};
                            Array.Sort(temp);
                            if (temp[2] - temp[0] == 2 && temp[1] - temp[0] == 1)
                            {
                                return new winlose(indexes[i], numberArray[indexes[i]]);
                            }
                        }
                        else if (i == 1)
                        {
                            int gcd = GCD(GCD(numberArray[indexes[2]], numberArray[indexes[3]]), numberArray[indexes[0]]);
                        
                            int [] temp = new int[] {numberArray[indexes[2]]/gcd, numberArray[indexes[3]]/gcd, numberArray[indexes[0]]/gcd};
                            Array.Sort(temp);
                            if (temp[2] - temp[0] == 2 && temp[1] - temp[0] == 1)
                            {
                                return new winlose(indexes[i], numberArray[indexes[i]]);
                            }
                        }
                        else if (i == 2)
                        {
                            int gcd = GCD(GCD(numberArray[indexes[3]], numberArray[indexes[0]]), numberArray[indexes[1]]);
                        
                            int [] temp = new int[] {numberArray[indexes[3]]/gcd, numberArray[indexes[0]]/gcd, numberArray[indexes[1]]/gcd};
                            Array.Sort(temp);
                            if (temp[2] - temp[0] == 2 && temp[1] - temp[0] == 1)
                            {
                                return new winlose(indexes[i], numberArray[indexes[i]]);
                            }
                        }
                        else
                        {
                            int gcd = GCD(GCD(numberArray[indexes[0]], numberArray[indexes[1]]), numberArray[indexes[2]]);
                        
                            int [] temp = new int[] {numberArray[indexes[0]]/gcd, numberArray[indexes[1]]/gcd, numberArray[indexes[2]]/gcd};
                            Array.Sort(temp);
                            if (temp[2] - temp[0] == 2 && temp[1] - temp[0] == 1)
                            {
                                return new winlose(indexes[i], numberArray[indexes[i]]);
                            }
                        }

                    }
                }
            }

            if(true)
            {
                int [] difs = new int [numberArray.Length];
                int bestI = 0;

                for (int i = 0; i < numberArray.Length; i++)
                {
                    difs[i] = 2*i + 1 - numberArray[i];
                }
                
                int [] dif1 = new int [difs.Length];
                Array.Copy(difs, dif1, difs.Length);
                int max = dif1[0];

                int count = -1;
                for (int i = 0; i < difs.Length; i++)
                {
                    if (difs[i] == max)
                    {
                        if (count == -1)
                        {
                            bestI = i;
                        }
                        count++;

                    }
                }
                
                if (count > 0)
                {
                    int temp = 0;
                    for (int i = 0; i < difs.Length; i++)
                    {
                        if (difs[i] == max)
                        {
                            if (numberArray[i] > temp)
                            {
                                temp = numberArray[i];
                                bestI = i;
                            }
                        }
                    }
                }

                if (sumSticks % 2 == 0)
                {
                    return new winlose(bestI, 2);
                }
                else
                {
                    return new winlose(bestI, 1);
                }
            }
        }

        static int numberRows(int[] numberArray)
        {
            int num = 0;
            for (int i = 0; i < numberArray.Length; i++)
            {
                if (numberArray[i] > 0)
                {
                    num++;
                }
            }
            return num;
        }

        static int GCD(int a, int b)
        {
            int Remainder;
    
            while( b != 0 )
            {
                Remainder = a % b;
                a = b;
                b = Remainder;
            }
      
            return a;
        }
    }
}