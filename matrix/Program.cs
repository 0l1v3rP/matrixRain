using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace matrix
{

    
    internal class matrix
    {
       

        /// <param name="u">direction, default=down, if written direction will be up </param>
        /// <param name="color">character color: b:blue c:cyan g:green m:magenta p:pink r:red y:yellow z:purple default=green</param>
        /// <param name="characters">0 is for alphabetical set of characters, 1 is for numerical and 2 is for alphanumerical, defaul=2</param>
        /// <param name="delay">delay of a program in milisecond, default==100</param>
        /// <param name="help"></param>
        static void Main( bool u = false, string color = "m", int characters = 2, int delay = 100)
        {
            int Drop_num = Console.WindowWidth;

            int[] dropLength = new int[Drop_num];
            char[][] rain = new char[Drop_num][];

            bool[] isReady = new bool[Drop_num];
            int[][] position = new int[Drop_num][];
            int[] dropPosition = new int[Drop_num];
            bool[][] printWhite = new bool[Drop_num][];
            

            int length = Console.WindowHeight ;

            int[] directions = ConfigureDirection(u);
            
            int x_plus = directions[0];
            int y_plus = directions[1]; 

            Random random = new Random();
            
            int[] ints = InitiatePool(characters);

            int from = ints[0];
            int to = ints[1];

            StringBuilder sb = new StringBuilder();

            InitiateRain(position, dropLength, random, u,rain,length, dropPosition, printWhite );
            string _color =  GetColor(color);

            //int consoleHeigth = Console.WindowHeight;
            //int consoleWidth = Console.WindowWidth;

            while(true)
            {
                Step(position, isReady, dropLength, random, rain, length,x_plus, y_plus, from, to, dropPosition, printWhite);
                PrintRain(rain, printWhite, _color,sb);
                
                //CheckAndResetWindowSize(consoleHeigth, consoleWidth);
                Thread.Sleep(delay);
                
                if (Console.KeyAvailable)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
            }

        }


        static string GetColor(string color)
        {
            switch (color)
            {
                case "b":
                    return "\x1b[34m"; // blue
                case "c":
                    return "\x1b[36m"; // cyan
                case "g":
                    return "\x1b[32m"; // green
                case "m":
                    return "\x1b[35m"; // magenta
                case "p":
                    return "\x1b[95m"; // pink
                case "r":
                    return "\x1b[31m"; // red
                case "y":
                    return "\x1b[33m"; // yellow
                case "z":
                    return "\x1b[95m"; // purple
                default:
                    return "\x1b[32m"; // green
            }
        }

        //static void CheckAndResetWindowSize(int height, int width)
        //{
        //    if (Console.WindowHeight != height|| Console.WindowWidth != width )
        //    {
        //        Console.SetWindowSize(width, height);
        //    }
        //}

        static void InitiateRain(int[][] position,  int[] dropLength, Random random, bool direction,char[][] rain, int length, int[] dropPosition, bool[][] printInWhite)
        {

            for (int i = 0; i < rain.Length; i++)
            {
                dropPosition[i] = 0;
                dropLength[i] = random.Next(2, length);
                
                

                position[i] = ConfigureStartingPosition(direction, i);
                rain[i] = new char[length];
                printInWhite[i] = new bool[length];
                for (int j = 0; j <  length; j++)
                {
                    rain[i][j] = ' ';
                    printInWhite[i][j] = false;
                }    
            }

        }

        static void Step(int[][] position, bool[] isReady, int[] dropLength, Random random, char[][] rain, int length, int x_plus, int y_plus,
            int from, int to, int[] dropPosition, bool[][] printInWhite)
        {
            
            int actXp;
            int actYp;
            int actDl;
            int end;

            for (int i = 0; i < position.Length; i++)
            {
                end = (x_plus + y_plus) == 1 ? length - 1 : 0;
                if (rain[i][end] != ' ')
                {
                    isReady[i] = false;
                    dropLength[i] = random.Next(2, length);

                    printInWhite[i][end] = false;

                    for (int j = 0; j < length; j++)
                    {

                        if (rain[i][end] != ' ')
                        {
                            rain[i][end] = GetRandomChar(random, from, to);
                        }
                        else
                        {
                            break;
                        }

                        end -= y_plus + x_plus;
                    }
                    rain[i][end + (x_plus + y_plus)] = ' ';

                }
                else
                {
                    if (random.Next(0, 33) == 0) isReady[i] = true; 

                }

                if (isReady[i] == true)
                {
                    actDl = dropLength[i] < dropPosition[i] ? dropLength[i] +1: dropPosition[i] + 1;
                    actXp = i;
                    actYp = position[i][1] - (actDl * y_plus);

                    if (dropLength[i] < dropPosition[i])
                    { 
                        rain[actXp][actYp] = ' ';
                        actXp += x_plus;
                        actYp += y_plus;
                        actDl--; 
                    }
                    for (int j = 0; j < actDl; j++)
                    {   
                        actXp += x_plus;
                        actYp += y_plus;
                        rain[actXp][actYp] = GetRandomChar(random, from, to);
                    }

                    printInWhite[actXp][actYp] = true;

                    if (actXp - x_plus >= 0 && actXp - x_plus < printInWhite.Length &&
                        actYp - y_plus >= 0 && actYp - y_plus < printInWhite[0].Length)
                    {
                        printInWhite[actXp - x_plus][actYp - y_plus] = false;
                    }                

                    dropPosition[i]++;
                    position[i][0] += x_plus;
                    position[i][1] += y_plus;

                    if (dropPosition[i] == length)
                    {
                        position[i][0] -= (x_plus * dropPosition[i]);
                        position[i][1] -= (y_plus * dropPosition[i]);
                        dropPosition[i] = 0;
                    }
                }

            }
            
        }

        static void PrintRain(char[][] rain, bool[][] lead, string color, StringBuilder sb)
        {

            Console.CursorVisible = false;
            Console.Clear();
            sb.Clear();
            for (int y = 0; y < rain[0].Length; y++)
            {
                for (int x = 0; x < rain.Length; x++)
                {
                    if (lead[x][y])
                    {
                        sb.Append("\x1b[37m"); 
                    }
                    else
                    {
                        sb.Append(color); 
                    }
                    sb.Append(rain[x][y]);
                }
                
            }

            Console.Write(sb.ToString());
        }


        

        static int[] InitiatePool(int characters)
        {
            int[] ints = new int[2];
            switch (characters)
            {
                case 0:
                    ints[0] = 65; ints[1] = 90; break;
                case 1:
                    ints[0] = 48; ints[1] = 57; break;
                default:
                    ints[0] = 48; ints[1] = 90; break;
            }
            return ints;
        }

        static char GetRandomChar(Random random, int from, int to)
        {           
                return (char)random.Next(from, to);            
        }

        static int[] ConfigureDirection(bool direction)
        {
            int[] directions = new int[2];
            switch (direction)
            {
                case true:
                    directions[0] = 0; directions[1] = -1; break;
                default:
                    directions[0] = 0; directions[1] = 1; break;
            }

            return directions;
        }



        static int[] ConfigureStartingPosition(bool direction, int i)
        {
            int[] ints = new int[2];
            switch (direction)
            {
                case true:
                    ints[0] = i;
                    ints[1] = Console.WindowHeight - 1;
                    break;
                default:
                    ints[0] = i;
                    ints[1] = 0;
                    break;
            }
            return ints;
        }
    }
}
