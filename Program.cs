using System;
using System.Collections.Generic;
// TODO: if the size are 2 different numbers, the game crashes for some odd reason??????
namespace EscaperoomGame
{
    internal class Program
    {
        private static bool _keyCollected; //key counts as collected if set to true
        private static bool _showPosition; //
        private static bool _debugMode;

        //room dimentions
        private static int _roomX = 10;
        private static int _roomY = 10;

        // minimum and maximum room dimentions
        private const int RoomMinWidth = 5;
        private const int RoomMinHeight = 5;
        private const int RoomMaxWidth = 20;
        private const int RoomMaxHeight = 20;
        
        // player coordinates
        private static int _playerX = 2, _playerY = 2; 
            
        //key coordinates
        private static int _keyPosX, _keyPosY;
            
        //door position
        private static int _doorPosX;
        private static int _doorPosY;

        //object icons
        private static string _playerIcon = "*";
        private static string _doorIcon = "\u2506";   // ┆

        //array
        private static int[,] _map;
        private static readonly Random Rnd = new Random();
        
        private enum EMapTiles
        {
            Free = -1,
            Wall,
            Door
        }

        private static char[] _mapTileCharacters = new char[]
        {
            ' ',
            '\u2503', // ┃
            '\u2506' // ┆
        };

        public static void Main(string[] args)
        {
            // shows text where the items are
            _showPosition = true;
            
            IntroText();
            
            GameplayLoop();
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
                    roomStringY = Console.ReadLine();
                    if (!IsAllDigits(roomStringY))
                    {
                        Console.WriteLine("\nInvalid number..\n");
                    }
                } while (!IsAllDigits(roomStringY));

                //parses (converts) the height into an integer (number)
                _roomY = int.Parse(roomStringY);

                //same as height
                do
                {
                    Console.Write("Width: ");
                    roomStringX = Console.ReadLine();
                    if (!IsAllDigits(roomStringX))
                    {
                        Console.WriteLine("\nInvalid number..\n");
                    }
                } while (!IsAllDigits(roomStringX));

                _roomX = int.Parse(roomStringX);


            } while (_roomX < 5 || _roomY < 5 || _roomX > 20 || _roomY > 20);
        }
        
        //function to draw the room using x and y for size
        private static void GameplayLoop()
        {
            Console.Clear();
            
            RoomSizeInput();
            
            //creates map array with the size _roomX and _roomY
            _map = new int[_roomX, _roomY];
            
            //hides the blinking cursor
            Console.CursorVisible = false;
            
            //algorithm to fill the map array
            FillMapLogic();
            
            //PlacePlayerAndKeys();
            
            while(true)
            {
                Console.Clear();
                
                //draws map
                PrintMap();

                int prevPlayerX = _playerX;
                int prevPlayerY = _playerY;

                //user controls
                ConsoleKeyInfo keyInput = Console.ReadKey();
                switch (keyInput.Key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        _playerY--;
                        break;
                    
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        _playerY++;
                        break;
                    
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                        _playerX--;
                        break;
                    
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                        _playerX++;
                        break;
                    
                    case ConsoleKey.X:
                        _debugMode = !_debugMode;
                        break;
                        
                    case ConsoleKey.Escape:
                        Environment.Exit(0);
                        break;
                }

                if (_playerX >= 0 && _playerX < _roomX && _playerY >= 0 && _playerY < _roomY)
                {
                    //only if this condition is false, the player will move normally
                    if (_map[_playerX, _playerY] == (int)EMapTiles.Wall)
                    {
                        ResetPlayer(prevPlayerX, prevPlayerY);
                    }
                    else if (_map[_playerX, _playerY] == (int)EMapTiles.Door) //player has key and enters the door
                    {
                        if (_keyCollected)
                        {
                            if (EndScreen() == ConsoleKey.Q) //player quits the game if Q is pressed
                            {
                                Environment.Exit(0);
                            }
                            else //game will restart
                            {
                                ResetVars();
                                GameplayLoop();
                            }
                        }
                        else
                        {
                            ResetPlayer(prevPlayerX, prevPlayerY);
                        }
                    }

                    if (_playerX == _keyPosX && _playerY == _keyPosY) //player is on the key position
                    {
                        CollectKey();
                    }
                }
                else // out of bounds
                {
                    ResetPlayer(prevPlayerX, prevPlayerY);
                }
            }
        }

        private static void FillMapLogic()
        {
            for (int y = 0; y < _roomY; y++) //draw x vertical lines
            {
                for (int x = 0; x < _roomX; x++) //draw y horizontal chars
                {
                    if (x == _doorPosX && y == _doorPosY)
                    {
                        _map[x, y] = (int)EMapTiles.Door;
                    }
                    if (x == 0 || y == 0 || x == _roomX - 1 || y == _roomY - 1)
                    {
                        _map[x, y] = (int)EMapTiles.Wall;
                    }
                    else
                    {
                        _map[x, y] = (int)EMapTiles.Free;
                    }
                }
            }
            
            PlacePlayerAndKeys();
        }

        private static void PrintMap()
        {
            Console.ForegroundColor = ConsoleColor.White;
            
            for (int y = 0; y < _roomX; y++)
            {
                for (int x = 0; x < _roomY; x++)
                {
                    #region wall variations

                    if (x == 0 && y == 0) // top left corner
                    {
                        Console.Write("\u250f"); // ┏
                    }
                    else if (y == 0 && x == _roomX - 1) // top right corner
                    {
                        Console.Write("\u2513"); // ┓
                    }
                    else if (y == _roomY - 1 && x == 0) // bottom left corner
                    {
                        Console.Write("\u2517"); // ┗
                    }
                    else if (y == _roomY - 1 && x == _roomX - 1) // bottom right corner
                    {
                        Console.Write("\u251b"); // ┛
                    }
                    else if (x == _doorPosX && y == _doorPosY) // door
                    {
                        if (_keyCollected)
                        {
                            Console.Write(" ");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            //Console.Write(_mapTileCharacters[(int)EMapTiles.door]);
                            Console.Write(_doorIcon);
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                    else if(y == _roomY - 1 || y == 0) // top and bottom walls
                    {
                        Console.Write("\u2501"); // ━
                    }
                    else if (x == 0 || x == _roomX - 1) // left and right walls
                    {
                        Console.Write("\u2503"); // ┃
                    }
                    
                    #endregion
                    
                    else if (x == _keyPosX && y == _keyPosY)
                    {
                        Console.Write("K");
                    }
                    else if (x == _playerX && y == _playerY) // draws player
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write(_playerIcon);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else // draws the rest of the enum, like free spaces
                    {
                        Console.Write(_mapTileCharacters[_map[x, y] + 1]);
                    }

                    if (y == 0 && x != _roomX - 1 || y == _roomY - 1 && x != _roomX - 1)
                    {
                        Console.Write('\u2501');
                    }
                    else
                    {
                        Console.Write(' '); // space after every character
                    }
                    
                }
                
                Console.WriteLine();
            }

            if (_debugMode)
            {
                ShowCoordinates(_playerX, _playerY, "player");
                ShowCoordinates(_keyPosX, _keyPosY, "key");
                ShowCoordinates(_doorPosX, _doorPosY, "door");
            }
            
            // shows the player- and key icon in the room in the beginning of the game
            if (_showPosition)
            {
                StartDescription(_playerX, _playerY, "player"); // player
                StartDescription(_keyPosX, _keyPosY, "key"); // key
            }
        }
        
        private static void PlacePlayerAndKeys()
        {
            int doorSide = Rnd.Next(1, 4);
            int doorX = 0;
            int doorY = 0;
            
            switch (doorSide)
            {
                case 1:
                    doorX = Rnd.Next(1, _roomX - 2);
                    doorY = 0;
                    _doorIcon = "\u2505"; // ┅
                    break;
                case 2:
                    doorX = _roomX - 1;
                    doorY = Rnd.Next(1, _roomY - 2);
                    _doorIcon = "\u2506"; // ┆
                    break;
                case 3:
                    doorX = Rnd.Next(1, _roomX - 2);
                    doorY = _roomY - 1;
                    _doorIcon = "\u2505"; // ┅
                    break;
                case 4:
                    doorX = 0;
                    doorY = Rnd.Next(1, _roomY - 2);
                    _doorIcon = "\u2506"; // ┆
                    break;
            }

            _map[doorX, doorY] = (int)EMapTiles.Door;
            _doorPosX = doorX;
            _doorPosY = doorY;
            
            // randomly places the player and the key
            _keyPosX = Rnd.Next(1, _roomX - 1);
            _keyPosY = Rnd.Next(1, _roomY - 1);

            _playerX = Rnd.Next(1, _roomX-1);
            while (_playerY == _keyPosY)
            {
                _playerY = Rnd.Next(1, _roomY-1);
            }
        }

        private static void ResetPlayer(int prevPlayerX, int prevPlayerY)
        {
            _playerX = prevPlayerX;
            _playerY = prevPlayerY;
        }

        private static void StartDescription(int posX, int posY, string item)
        {
            //places the cursor where the item is and draws the text
            Console.SetCursorPosition(posX*2+2, posY);
            Console.Write($"<-- {item}");
            _showPosition = false;
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

        private static void ShowCoordinates(int posX, int posY, string item)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"\n{item} at X: {posX}, Y: {posY}"); 
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

        private static void CollectKey()
        {
            _keyCollected = true;
            _keyPosX = 0;
            _keyPosY = 0;
            _map[_keyPosX, _keyPosY] = (int)EMapTiles.Free;
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
            _keyCollected = false;
            _showPosition = true;
            _playerIcon = "*";
            Console.CursorVisible = true;
            Console.Clear();
            ResetTextColor();
        }
    }
}