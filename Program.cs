using System;
using System.Collections.Generic;

namespace EscaperoomGame
{
    internal class Program
    {
        private static bool _levelClear; //game finished if set to true
        private static bool _keyCollected;  //key counts as collected if set to true
        private static bool _showPosition = true;
        
        //room dimentions
        private static int _roomX = 10; 
        private static int _roomY = 10;

        // minimum and maximum room dimentions
        private const int RoomMinWidth = 5;
        private const int RoomMinHeight = 5;
        private const int RoomMaxWidth = 20;
        private const int RoomMaxHeight = 20;
            
        // player coordinates
        private static int _posX, _posY; 
            
        //key coordinates
        private static int _keyX, _keyY;
            
        //door
        private static int _doorY;

        private static string _playerIcon = "*";
        private const string KeyIcon = "@";
        private const string DoorIcon = "\u2551";   // ║
        
        public static void Main(string[] args)
        {
            //intro text
            IntroText();
            
            while (true)
            {
                Console.WriteLine("\nSpecify the size of the room.\nMake sure that it's between 5 and 20 fields big\n");

                //user declares height and width of the room
                //reprompts input if the room is too small
                var room = RoomSizeInput(_roomX, _roomY);
                _roomX = room[0];
                _roomY = room[1];
            
                //hides the cursor
                Console.CursorVisible = false;

                //places player and keys at random places
                Random rnd = new Random();
            
                //generates random coordinates for player, keys and the door
                _posX = rnd.Next(1, _roomX - 1);
                _posY = rnd.Next(1, _roomY - 1);
            
                _keyX = rnd.Next(1, _roomX - 1);
                _keyY = rnd.Next(1, _roomY - 1);
            
                _doorY = rnd.Next(1,_roomY-1);
            
                //if the key and player spawned on the same position, recalculate the key coordinate
                while (_posX == _keyX && _posY == _keyY)
                {
                    _keyX = rnd.Next(1, _roomX - 1);
                    _keyY = rnd.Next(1, _roomY - 1);
                }
            
                //loops and refreshes the game room as long as the variable "LevelClear" is false
                while (!_levelClear)
                {
                    DrawRoom(_roomX, _roomY); //draw the room
                    
                    //Boundaries
                    //if the player steps where a wall is, it gets set back
                    if (_posX < 1)
                    {
                        _posX = 1;
                        Console.Beep(90, 150);
                    }
                    else if (_posY < 1)
                    {
                        _posY = 1;
                        Console.Beep(90, 150);
                    }
                    else if (_posY > _roomY-2)
                    {
                        _posY = _roomY-2;
                        Console.Beep(90, 150);
                    }
                    else if (_posX > _roomX-2)
                    {
                        _posX = _roomX-2;
                        Console.Beep(90, 150);
                    }
                    else
                    {
                        //Console.Beep(300, 150);
                    }

                    //shows coordinates below the room in gray
                    ShowCoordinates(_posX, _posY);

                    //place cursor and draw door
                    DrawDoor(_roomX, _doorY, _keyCollected, DoorIcon);

                    //places cursor and is ready to draw the player char
                    DrawPlayer(_posX, _posY, _keyCollected, _playerIcon);
                    
                    // shows where the player is at the start of the game
                    if (_showPosition)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(" <-- Your character");
                    }
                    
                    //Draw key if it's not collected yet
                    if (!_keyCollected)
                    {
                        DrawKey(_keyX, _keyY, KeyIcon);
                        
                        if (_showPosition)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(" <-- Collect the key");
                            _showPosition = false;
                        }
                    }
                
                    //if player and key are on the same field, "keyCollected" set to true
                    if (_posX == _keyX && _posY == _keyY)
                    {
                        _keyCollected = true;
                        
                        //player icon is now the key icon
                        _playerIcon = KeyIcon;
                        
                        //beeping sound when key is collected
                        Console.Beep(1500, 150);
                        
                        _keyY = 0;
                        _keyX = 0;
                    }

                    var pos = PlayerControls(_posX, _posY);
                    _posX = pos[0];
                    _posY = pos[1];
                    
                    //if player has the key and is at the door "levelClear" is set to true
                    //gameplay loop ends
                    if (_posX == _roomX-1 && _posY == _doorY && _keyCollected)
                    {
                        _levelClear = true;
                    }
                }
                
                //game ends
                //if the player presses "Q" the game will close
                if (EndScreen() == ConsoleKey.Q)
                {
                    //Environment.Exit(0);
                    break;
                }

                //resets variables for game restart
                ResetVars();
            }

            void ResetVars()
            {
                _levelClear = false;
                _keyCollected = false;
                _showPosition = true;
                _playerIcon = "*";
                Console.CursorVisible = true;
                Console.Clear();
                ResetTextColor();
            }
        }

        static List<int> RoomSizeInput(int roomX, int roomY)
        {
            do
            {
                var roomStringX = "";
                var roomStringY = "";
                
                //tells the player if the room is too small by checking the size
                if (roomX < RoomMinWidth || roomY < RoomMinHeight)
                {
                    Console.WriteLine("The room is too small..");
                }
                else if (roomX > RoomMaxWidth || roomY > RoomMaxHeight)
                {
                    Console.WriteLine("The room is too big..");
                }
                    
                //writes the height to a string and checks if it's a valid number
                do
                {
                    Console.Write("\nHeight: ");
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
                    roomStringY = Console.ReadLine();
                    if (!IsAllDigits(roomStringY))
                    {
                        Console.WriteLine("\nInvalid number..\n");
                    }
                } while (!IsAllDigits(roomStringY));

                roomY = int.Parse(roomStringY);


            } while (roomX < 5 || roomY < 5 || roomX > 20 || roomY > 20);

            var room = new List<int>{roomX, roomY};
            return room;
        }
        
        static void DrawRoom(int x, int y)  //function to draw the room using x and y for size
        {
            Console.Clear();    //refreshes console each move

            Console.ForegroundColor = ConsoleColor.DarkBlue;    //wall color
            
            //algorithm 
            for (int i = 0; i < y; i++) //draw x vertical lines
            {
                for (int j = 0; j < x; j++) //draw y horizontal chars
                {
                    if (i == 0) //top walls
                    {
                        if (j == 0)
                        {
                            Console.Write("\u250f");    // ┏
                        }
                        else if (j == x-1)
                        {
                            Console.Write("\u2513");    // ┓
                        }
                        else
                        {
                            Console.Write("\u2501");    // ━
                        }
                    }
                    else if (i == y - 1) //bottom walls
                    {
                        if (j == 0)
                        {
                            Console.Write("\u2517");    // ┗
                        }
                        else if (j == x - 1)
                        {
                            Console.Write("\u251b");    // ┛
                        }
                        else
                        {
                            Console.Write("\u2501");    // ━
                        }
                    }

                    else if (j == 0 || j == x-1)    //side walls
                    {
                        Console.Write("\u2503");    // ┃
                    }
                    
                    else //floor char
                    {
                        Console.Write(" ");
                    }
                }
                Console.Write("\n"); //line break
            }
            ResetTextColor();
        }

        static void DrawDoor(int roomX, int doorY, bool keyCollected, string doorIcon)
        {
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
            ResetTextColor();
        }

        static void DrawPlayer(int posX, int posY, bool keyCollected, string playerIcon)
        {
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
            ResetTextColor();
        }

        static void DrawKey(int keyX, int keyY, string keyIcon)
        {
            Console.SetCursorPosition(keyX, keyY);  //places cursor for key
            Console.ForegroundColor = ConsoleColor.Yellow; //key color
            Console.Write(keyIcon); //writes key
            ResetTextColor();
        }

        static List<int> PlayerControls(int posX, int posY)
        {
            //Key press stored to variable "arrow"
            var arrow = Console.ReadKey(true).Key;
                    
            //TODO: make the controls the arrow keys instead of WASD
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

            var pos = new List<int> { posX, posY };
            return pos;
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
            ResetTextColor();
        }

        static void IntroText()
        {
            Console.Clear();
            ResetTextColor();
            Console.Write("~ Welcome to my escape room! ~\n\nThe rules are simple:\nFind the");
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(" key ");
            
            ResetTextColor();
            Console.Write("and go through the door.\n\nMove your character with");
            
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(" W, A, S and D");
            
            ResetTextColor();
        }

        static ConsoleKey EndScreen()
        {
            Console.Clear();
                
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("~ Congrats! You've finished the game! ~");
            
            ResetTextColor();
            Console.Write("\nPress any key to play again\nor press ");
            
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(" Q ");
            
            ResetTextColor();
            Console.Write("to quit the game.");
            
            //variable for next input
            var continueInput = Console.ReadKey(true).Key;

            return continueInput;
        }

        static void ResetTextColor()
        {
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}