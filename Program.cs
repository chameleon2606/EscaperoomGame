using System;
using System.Media;

namespace EscaperoomGame
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var walkSound = new SoundPlayer
            {
                SoundLocation = @"E:\SAE\Escaperoom\EscaperoomGame\sounds\sound1.wav"
            };
            var wallSound = new SoundPlayer
            {
                SoundLocation = @"E:\SAE\Escaperoom\EscaperoomGame\sounds\sound2.wav"
            };
            var keySound = new SoundPlayer
            {
                SoundLocation = @"E:\SAE\Escaperoom\EscaperoomGame\sounds\sound3.wav"
            };
            
            bool LevelClear = false; //game finished if set to true
            bool keyCollected = false;  //key counts as collected if set to true
            
            //room dimentions
            int roomX = 10; 
            int roomY = 10;
            
            //coordinaten des players
            int posX = 0, posY = 0; 
            
            //key coordinaten 
            int keyX = 0, keyY = 0;
            
            //tür
            int doorX = 0, doorY = 0;

            string playerIcon = "*";
            const string keyIcon = "@";
            const string doorIcon = "|";
            
            //intro text
            Console.Clear();
            Console.WriteLine("~ Welcome to my escape room! ~\nThe rules are simple:\nFind the key and go through the door!"); 
            Console.WriteLine("\nSpecify the size of the room in fields\nMake sure that the width and height are at least 5 tiles!\n");

            //user declares height and width of the room
            //reprompts input if the room is too small
            do
            {
                if (roomX < 5 || roomY < 5)
                {
                    Console.WriteLine("The room is too small..");
                }
                Console.Write("\nHeight: ");
                roomX = Convert.ToInt32(Console.ReadLine());    //convert string input to int
                Console.Write("Width: ");
                roomY = Convert.ToInt32(Console.ReadLine());    //convert string input to int
            } while (roomX < 5 || roomY < 5);
            
            //hides the cursor
            Console.CursorVisible = false;

            //places player and keys at random places
            Random rnd = new Random();
            
            //generates random coordinates for player, keys and the door
            posX = rnd.Next(1, roomX - 1);
            posY = rnd.Next(1, roomY - 1);
            
            keyX = rnd.Next(1, roomX - 1);
            keyY = rnd.Next(1, roomY - 1);
            
            doorY = rnd.Next(1,roomY-1);
            
            //if the key and player spawned on the same position, recalculate the key coordinate
            while (posX == keyX && posY == keyY)
            {
                keyX = rnd.Next(1, roomX - 1);
                keyY = rnd.Next(1, roomY - 1);
            }
            
            //loops and refreshes the game room as long as the variable "LevelClear" is false
            while (!LevelClear)
            {
                DrawRoom(roomX, roomY); //draw the room
                
                //Boundaries
                //if the player steps where a wall is, it gets set back
                if (posX < 1)
                {
                    posX = 1;
                    wallSound.Play();
                }
                if (posY < 1)
                {
                    posY = 1;
                    wallSound.Play();
                }
                if (posY > roomY-2)
                {
                    posY = roomY-2;
                    wallSound.Play();
                }
                if (posX > roomX-2)
                {
                    posX = roomX-2;
                    wallSound.Play();
                }

                //shows key and door coordinates below the room in gray
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"\nplayer at X: {posX}, Y: {posY}"); 
                Console.WriteLine($"\ndoor at X: {doorX}, Y: {doorY}");
                
                //place cursor and draw door
                Console.SetCursorPosition(roomX-1, doorY);
                //door is green if "keyCollected" is set to true
                if (keyCollected)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                Console.Write(doorIcon);
                
                //places cursor and is ready to draw the player char
                Console.SetCursorPosition(posX, posY);
                //changes color when key is collected
                if (!keyCollected)  
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                Console.Write(playerIcon); //Player char
                
                //Draw key if it's not collected yet
                if (!keyCollected)
                {
                    Console.SetCursorPosition(keyX, keyY);  //places cursor for key
                    Console.ForegroundColor = ConsoleColor.Yellow; //key color
                    Console.Write(keyIcon); //writes key
                }
                
                //if player and key are on the same field, "keyCollected" set to true
                if (posX == keyX && posY == keyY)
                {
                    keyCollected = true;
                    //player icon is now the key icon
                    playerIcon = keyIcon;
                    keySound.Play();
                }
                
                //Key press stored to variable "arrow"
                var arrow = Console.ReadKey().Key;
                
                //switch statement for player movement
                switch (arrow)
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
                
                //if player has the key and is at the door "levelClear" is set to true
                //gameplay loop ends
                if (posX == roomX-1 && posY == doorY && keyCollected)
                {
                    LevelClear = true;
                }
            }
            
            //game ends
            Console.Clear();
            Console.WriteLine("Congrats! You've finished the game!");
            Console.ReadKey();
        }

        static void DrawRoom(int x, int y)  //function to draw the room using x and y for size
        {
            Console.Clear();    //refreshes console each move

            Console.ForegroundColor = ConsoleColor.DarkBlue;
            
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