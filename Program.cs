using System;

namespace EscaperoomGame
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            bool LevelClear = false; //game finished if set to true
            bool keyCollected = false;
            int posX = 0, posY = 0; //coordinaten des cursors (player)
            int keyX = 0, keyY = 0;
            int doorX = 0, doorY = 0;

            string playerIcon = "*";
            const string keyIcon = "@";
            const string doorIcon = "|";
            
            Console.Clear();
            Console.WriteLine("~ Welcome to my escape room! ~\nThe rules are simple:\nFind the key and go through the door!"); //intro
            Console.WriteLine("Specify the size of the room in fields\n");
            
            Console.Write("\nHeight: ");
            var x = Convert.ToInt32(Console.ReadLine());    //convert string input to int
            Console.Write("Width: ");
            var y = Convert.ToInt32(Console.ReadLine());    //convert string input to int
            
            Console.CursorVisible = false;  //hides the cursor

            Random rnd = new Random();  //places player and keys at random places
            
            posX = rnd.Next(1, x - 1);
            posY = rnd.Next(1, y - 1);
            
            keyX = rnd.Next(1, x - 1);
            keyY = rnd.Next(1, y - 1);
            
            doorY = rnd.Next(1,y-1);
            
            while (posX == keyX && posY == keyY)
            {
                keyX = rnd.Next(1, x - 1);
                keyY = rnd.Next(1, y - 1);
            }

            while (!LevelClear)
            {
                DrawRoom(x, y); //draw the room
                
                //  Boundaries
                if (posX <= 1)
                {
                    posX = 1;
                }
                if (posY <= 1)
                {
                    posY = 1;
                }
                if (posY >= y-2)
                {
                    posY = y-2;
                }
                if (posX >= x-2)
                {
                    posX = x-2;
                }

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"\nplayer at X: {posX}, Y: {posY}"); //shows coordinates below the room
                Console.WriteLine($"\ndoor at X: {doorX}, Y: {doorY}");    //shows door coordiantes
                
                Console.SetCursorPosition(x-1, doorY);  //place cursor and draw door
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(doorIcon);
                
                Console.SetCursorPosition(posX, posY);  //places cursor and draws player
                if (!keyCollected)  //changes color when key is collected
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                Console.Write(playerIcon); //Player char
                
                if (!keyCollected)  //Disables key icon when collected
                {
                    Console.SetCursorPosition(keyX, keyY);  //places cursor and draws key
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(keyIcon); //Key char
                }
                
                if (posX == keyX && posY == keyY)
                {
                    keyCollected = true;
                    playerIcon = "@";
                }
                
                var arrow = Console.ReadKey().Key; //Key press stored to variable "arrow"
                
                switch (arrow)  //player movement
                {
                    case ConsoleKey.W:
                        posY--;
                        break;
                    case ConsoleKey.A:
                        posX--;
                        break;
                    case ConsoleKey.S:
                        posY++;
                        break;
                    case ConsoleKey.D:
                        posX++;
                        break;
                }
                
                if (posX == x-1 && posY == doorY && keyCollected) //door entered with key
                {
                    LevelClear = true;
                }
            }
            
            Console.Clear();
            Console.WriteLine("Congrats! You've finished the game!");
            Console.ReadKey();
        }

        static void DrawRoom(int x, int y)  //function to draw the room using x and y for size
        {
            Console.Clear();    //refreshes console each move

            Console.ForegroundColor = ConsoleColor.DarkRed;
            
            //algorithm 
            for (int i = 0; i < y; i++) //draw x vertical lines
            {
                for (int j = 0; j < x; j++) //draw y horizontal chars
                {
                    if (i == 0 || i == y-1) //top and bottom walls
                    {
                        Console.Write("_");
                    }

                    else if (j == 0 || j == x-1)    //side walls
                    {
                        Console.Write("|");
                    }
                    
                    else //floor char
                    {
                        Console.Write(" ");
                    }
                }
                Console.Write("\n"); //line break
            }
        }
    }
}