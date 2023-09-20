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
        private static int _posX = 2, _posY = 2; 
            
        //key coordinates
        private static int _keyX, _keyY;
            
        //door position
        private static int _doorY;

        //object icons
        private static string _playerIcon = "*";
        private const string KeyIcon = "@";
        private const string DoorIcon = "\u2551";   // ║

        private static int[,] map;
        private static Random rnd = new Random();
        private static int prevPos;

        private enum EMapTiles
        {
            free = -1,
            wall,
            box
        }

        private static char[] mapTileCharacters = new char[]
        {
            ' ',
            '#',
            '@'
        };

        public static void Main(string[] args)
        {
            //intro text
            IntroText();
            RoomSizeInput();
            map = new int[_roomX, _roomY];
            
            DrawRoom();

            /*while (true)
            {
                Console.WriteLine("\nSpecify the size of the room.\nMake sure that it's between 5 and 20 fields big\n");

                //user declares height and width of the room
                //reprompts input if the room is too small

                RoomSizeInput();

                map = new int[_roomX, _roomY];

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
                    DrawRoom(); //draw the room

                    //Boundaries
                    //if the player steps where a wall is, it gets set back
                    //if (_posX < 2)  //left wall
                    //{
                    //    _posX = 1;
                    //    Console.Beep(90, 150);
                    //}
                    //else if (_posY < 1) //top wall
                    //{
                    //    _posY = 1;
                    //    Console.Beep(90, 150);
                    //}
                    //else if (_posY > _roomY-2)  //bottom wall
                    //{
                    //    _posY = _roomY-2;
                    //    Console.Beep(90, 150);
                    //}
                    //else if (_posX > _roomX-2)  //right wall
                    //{
                    //    _posX = _roomX-2;
                    //    Console.Beep(90, 150);
                    //}
                    //else
                    //{
                    //    //Console.Beep(300, 150);
                    //}

                    //shows coordinates below the room in gray
                    ShowCoordinates();

                    //place cursor and draw door
                    //DrawDoor();

                    //places cursor and is ready to draw the player char
                    //DrawPlayer();

                    // shows where the player is at the start of the game
                    //if (_showPosition)
                    //{
                    //    Console.ForegroundColor = ConsoleColor.White;
                    //    Console.Write(" <-- Your character");
                    //}

                    //Draw key if it's not collected yet
                    //if (!_keyCollected)
                    //{
                    //    DrawKey();
                    //
                    //    if (_showPosition)
                    //    {
                    //        Console.ForegroundColor = ConsoleColor.White;
                    //        Console.Write(" <-- Collect the key");
                    //        _showPosition = false;
                    //    }
                    //}

                    //if player and key are on the same field, "keyCollected" set to true
                    //if (_posX == _keyX && _posY == _keyY)
                    //{
                    //    _keyCollected = true;
                    //
                    //    //player icon is now the key icon
                    //    _playerIcon = KeyIcon;
                    //
                    //    //beeping sound when key is collected
                    //    Console.Beep(1500, 150);
                    //
                    //    _keyY = 0;
                    //    _keyX = 0;
                    //}

                    //PlayerControls();

                    //if player has the key and is at the door "levelClear" is set to true
                    //gameplay loop ends
                    //if (_posX == _roomX-1 && _posY == _doorY && _keyCollected)
                    //{
                    //    _levelClear = true;
                    //}
                }

                //game ends
                //if the player presses "Q" the game will close
                //if (EndScreen() == ConsoleKey.Q)
                //{
                //    //Environment.Exit(0);
                //    break;
                //}

                //resets variables for game restart
                ResetVars();
            }*/
        }

        private static void RoomSizeInput()
        {
            do
            {
                var roomStringX = "";
                var roomStringY = "";
                
                //tells the player if the room is too small by checking the size
                if (_roomX < RoomMinWidth || _roomY < RoomMinHeight)
                {
                    Console.WriteLine("The room is too small..");
                }
                else if (_roomX > RoomMaxWidth || _roomY > RoomMaxHeight)
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
                _roomX = int.Parse(roomStringX);

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

                _roomY = int.Parse(roomStringY);


            } while (_roomX < 5 || _roomY < 5 || _roomX > 20 || _roomY > 20);
        }
        
        private static void DrawRoom()  //function to draw the room using x and y for size
        {
            Console.CursorVisible = false;
            Console.Clear();    //refreshes console each move

            Console.ForegroundColor = ConsoleColor.DarkBlue;    //wall color
            
            //algorithm to draw the room
            for (int y = 0; y < _roomY; y++) //draw x vertical lines
            {
                for (int x = 0; x < _roomX; x++) //draw y horizontal chars
                {
                    if (y == 0 || x == 0 || y == _roomY - 1 || x == _roomX - 1)
                    {
                        map[x, y] = (int)EMapTiles.wall;
                    }
                    else
                    {
                        map[x, y] = (int)EMapTiles.free;
                    }
                    
                    //if (y == 0) //top walls
                    //{
                    //    if (x == 0)
                    //    {
                    //        Console.Write("\u250f\u2501");    // ┏━
                    //    }
                    //    else if (x == _roomX-1)
                    //    {
                    //        Console.Write("\u2501\u2513");    // ━┓
                    //    }
                    //    else
                    //    {
                    //        Console.Write("\u2501\u2501");    // ━━
                    //    }
                    //}
                    //else if (y == _roomY - 1) //bottom walls
                    //{
                    //    if (x == 0)
                    //    {
                    //        Console.Write("\u2517\u2501");    // ┗━
                    //    }
                    //    else if (x == _roomX - 1)
                    //    {
                    //        Console.Write("\u2501\u251b");    // ━┛
                    //    }
                    //    else
                    //    {
                    //        Console.Write("\u2501\u2501");    // ━━
                    //    }
                    //}

                    //else if (x == 0)    //left wall
                    //{
                    //    Console.Write("\u2503 ");    // ┃
                    //}
                    //else if (x == _roomX - 1)    //right wall
                    //{
                    //    Console.Write(" \u2503");   // ┃
                    //}
                    //
                    //else //floor char
                    //{
                    //    Console.Write("  ");
                    //}
                }
            }
            
            int boxX = rnd.Next(1, _roomX - 1);
            int boxY = rnd.Next(1, _roomY - 1);

            map[boxX, boxY] = (int)EMapTiles.box;
            
            ResetTextColor();
            bool isRunning = true;
            while (isRunning)
            {
                Console.Clear();
                PrintMap();

                int prevPlayerX = _posX;
                int prevPlayerY = _posY;

                ConsoleKeyInfo keyInput = Console.ReadKey();
                switch (keyInput.Key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        _posY--;
                        break;
                    
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        _posY++;
                        break;
                    
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                        _posX--;
                        break;
                    
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                        _posX++;
                        break;
                        
                    case ConsoleKey.Escape:
                        Environment.Exit(0);
                        break;
                }

                if (_posX >= 0 && _posX < _roomX && _posY >= 0 && _posY < _roomY)
                {
                    if (map[_posX, _posY] == (int)EMapTiles.wall)
                    {
                        _posX = prevPlayerX;
                        _posY = prevPlayerY;
                    }
                }
                else
                {
                    _posX = prevPlayerX;
                    _posY = prevPlayerY;
                }
            }
        }

        private static void PrintMap()
        {
            for (int y = 0; y < _roomX; y++)
            {
                for (int x = 0; x < _roomY; x++)
                {
                    if (x == _posX && y == _posY)
                    {
                        Console.Write(_playerIcon + " ");
                    }
                    else
                    {
                        int mapValue = map[x, y];
                        Console.Write(mapTileCharacters[mapValue + 1] + " ");
                    }
                }
                Console.WriteLine();
            }
        }

        private static void DrawDoor()
        {
            Console.SetCursorPosition(_roomX-1, _doorY);
            //door is green if "keyCollected" is set to true
            if (_keyCollected)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            Console.Write(DoorIcon);
            ResetTextColor();
        }

        private static void DrawPlayer()
        {
            Console.SetCursorPosition(_posX, _posY);
            //changes color when key is collected
            if (!_keyCollected)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            Console.Write(_playerIcon); //draw player char
            ResetTextColor();
        }

        private static void DrawKey()
        {
            Console.SetCursorPosition(_keyX, _keyY);  //places cursor for key
            Console.ForegroundColor = ConsoleColor.Yellow; //key color
            Console.Write(KeyIcon); //writes key
            ResetTextColor();
        }

        private static void PlayerControls()
        {
            //Key press stored to variable "arrow"
            var arrow = Console.ReadKey(true).Key;
            
            //switch statement for player movement
            switch (arrow)
            {
                case ConsoleKey.W:
                case ConsoleKey.UpArrow:    
                    _posY--;
                    break;
                case ConsoleKey.A:
                case ConsoleKey.LeftArrow:    
                    _posX--;
                    break;
                case ConsoleKey.S:
                case ConsoleKey.DownArrow:    
                    _posY++;
                    break;
                case ConsoleKey.D:
                case ConsoleKey.RightArrow:    
                    _posX++;
                    break;
                case ConsoleKey.Escape:
                    Environment.Exit(0);
                    break;
            }
        }

        private static bool IsAllDigits(string s)
        {
            foreach (char c in s)
            {
                if (!char.IsDigit(c))
                    return false;
            }

            return true;
        }

        private static void ShowCoordinates()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"\nplayer at X: {_posX}, Y: {_posY}"); 
            ResetTextColor();
        }

        private static void IntroText()
        {
            Console.Clear();
            //string introText = "\u2554\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2557\n\u2551 \t\t\t    \u2551\n\u255f welcome to my escaperoom! \u2562\n\u2551\t\t\t    \u2551\n\u2551\t\t\t    \u2551\n\u255f   The rules are simple:   \u2562\n\u2551\t\t\t    \u2551\n\u255f   Find the key and go     \u2562\n\u2551   through the door.       \u2551\n\u2551\t\t\t    \u2551\n\u255f Move your character with  \u2562\n\u2551\t  W A S D\t    \u2551\n\u2551\t\t\t    \u2551\n\u255a\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u2550\u255d";
            //Console.Write(introText);
            
            ResetTextColor();
            Console.Write("~ Welcome to my escape room! ~\n\nThe rules are simple:\nFind the");
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(" key ");
            
            ResetTextColor();
            Console.Write("and go through the door.\n\nMove your character with");
            
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(" W, A, S and D");
            
            ResetTextColor();
            Console.Write(" or the ");

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("arrow keys");
            
            ResetTextColor();
        }

        private static ConsoleKey EndScreen()
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

        private static void ResetTextColor()
        {
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void ResetVars()
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
}