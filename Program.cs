using System;
using System.Media;

namespace EscaperoomGame
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            //TODO: sounds deactivated for now until i can figure out how to make it work on other computers
            var walkSound1 = new SoundPlayer
            {
                SoundLocation = @"E:\SAE\Escaperoom\EscaperoomGame\sounds\walk1.wav"
            };
            var walkSound2 = new SoundPlayer
            {
                SoundLocation = @"E:\SAE\Escaperoom\EscaperoomGame\sounds\walk2.wav"
            };
            var walkSound3 = new SoundPlayer
            {
                SoundLocation = @"E:\SAE\Escaperoom\EscaperoomGame\sounds\walk3.wav"
            };
            var wallSound = new SoundPlayer
            {
                SoundLocation = @"E:\SAE\Escaperoom\EscaperoomGame\sounds\sound2.wav"
            };
            var keySound = new SoundPlayer
            {
                SoundLocation = @"E:\SAE\Escaperoom\EscaperoomGame\sounds\sound3.wav"
            };
            
            bool levelClear = false; //game finished if set to true
            bool keyCollected = false;  //key counts as collected if set to true
            bool showPosition = true;
            
            //room dimentions
            int roomX = 10; 
            int roomY = 10;

            string roomStringX, roomStringY;
            
            //coordinaten des players
            int posX = 0, posY = 0; 
            
            //key coordinaten 
            int keyX = 0, keyY = 0;
            
            //tür
            int doorY = 0;

            string playerIcon = "*";
            const string keyIcon = "@";
            const string doorIcon = "|";
            
            //intro text
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("~ Welcome to my escape room! ~\n\nThe rules are simple:\nFind the");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(" key ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("and go through the door.\n\nMove your character with");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(" W, A, S and D");
            Console.ForegroundColor = ConsoleColor.White;
            
            while (true)
            {
                Console.WriteLine("\nSpecify the size of the room.\nMake sure that it's between 5 and 20 fields big\n");

                //user declares height and width of the room
                //reprompts input if the room is too small
                do
                {
                    //tells the player if the room is too small by checking the size
                    if (roomX < 5 || roomY < 5)
                    {
                        Console.WriteLine("The room is too small..");
                    }
                    else if (roomX > 20 || roomY > 20)
                    {
                        Console.WriteLine("The room is too big..");
                    }
                    
                    //writes the height to a string and checks if it's a valid number
                    do
                    {
                        Console.Write("\nHeight: ");
                        //roomX = Convert.ToInt32(Console.ReadLine());
                        roomStringX = Console.ReadLine();
                        if (!IsAllDigits(roomStringX))
                        {
                            Console.WriteLine("\nInvalid number..\n");
                        }
                    } while (!IsAllDigits(roomStringX));

                    //parses (converts) the height into an integer (number)
                    roomX = int.Parse(roomStringX);

                    //same as height
                    do
                    {
                        Console.Write("Width: ");
                        //roomY = Convert.ToInt32(Console.ReadLine());
                        roomStringY = Console.ReadLine();
                        if (!IsAllDigits(roomStringY))
                        {
                            Console.WriteLine("\nInvalid number..\n");
                        }
                    } while (!IsAllDigits(roomStringY));

                    roomY = int.Parse(roomStringY);


                } while (roomX < 5 || roomY < 5 || roomX > 20 || roomY > 20);
            
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
                while (!levelClear)
                {
                    DrawRoom(roomX, roomY); //draw the room
                    
                    //Boundaries
                    //if the player steps where a wall is, it gets set back
                    if (posX < 1)
                    {
                        posX = 1;
                        //wallSound.Play();
                    }
                    else if (posY < 1)
                    {
                        posY = 1;
                        //wallSound.Play();
                    }
                    else if (posY > roomY-2)
                    {
                        posY = roomY-2;
                        //wallSound.Play();
                    }
                    else if (posX > roomX-2)
                    {
                        posX = roomX-2;
                        //wallSound.Play();
                    }

                    //shows coordinates below the room in gray
                    ShowCoordinates(posX, posY);
                    
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
                    Console.Write(playerIcon); //draw player char
                    
                    if (showPosition)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(" <-- Your character");
                    }
                
                    //Draw key if it's not collected yet
                    if (!keyCollected)
                    {
                        Console.SetCursorPosition(keyX, keyY);  //places cursor for key
                        Console.ForegroundColor = ConsoleColor.Yellow; //key color
                        Console.Write(keyIcon); //writes key
                        
                        if (showPosition)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(" <-- Collect the key");
                            showPosition = false;
                        }
                    }
                
                    //if player and key are on the same field, "keyCollected" set to true
                    if (posX == keyX && posY == keyY)
                    {
                        keyCollected = true;
                        
                        //player icon is now the key icon
                        playerIcon = keyIcon;
                        //keySound.Play();
                        keyY = 0;
                        keyX = 0;
                    }
                    
                    //Key press stored to variable "arrow"
                    var arrow = Console.ReadKey().Key;
                    
                    //TODO: make the controls the arrow keys instead of WASD
                    //switch statement for player movement
                    switch (arrow)
                    {
                        case ConsoleKey.W:
                            posY--;
                            //walkSound1.Play();
                            break;
                        case ConsoleKey.A:
                            posX--;
                            //walkSound2.Play();
                            break;
                        case ConsoleKey.S:
                            posY++;
                            //walkSound3.Play();
                            break;
                        case ConsoleKey.D:
                            posX++;
                            //walkSound2.Play();
                            break;
                    }
                    
                    //if player has the key and is at the door "levelClear" is set to true
                    //gameplay loop ends
                    if (posX == roomX-1 && posY == doorY && keyCollected)
                    {
                        levelClear = true;
                    }
                }
                
                //game ends
                Console.Clear();
                
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("~ Congrats! You've finished the game! ~");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\nPress any key to play again\nor press ");
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write(" Q ");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("to quit the game.");
                
                //variable for next input
                var continueInput = Console.ReadKey().Key;
                
                //if the player presses "Q" the game will close
                if (continueInput == ConsoleKey.Q)
                {
                    //Environment.Exit(0);
                    break;
                }

                //resets variables for game restart
                ResetVars();
            }

            void ResetVars()
            {
                levelClear = false;
                keyCollected = false;
                showPosition = true;
                playerIcon = "*";
                Console.CursorVisible = true;
            }
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

        static bool IsAllDigits(string s)
        {
            foreach (char c in s)
            {
                if (!char.IsDigit(c))
                    return false;
            }

            return true;
        }

        static void ShowCoordinates(int posX, int posY)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"\nplayer at X: {posX}, Y: {posY}"); 
        }
    }
}