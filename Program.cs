using System;
using System.Collections.Generic;

namespace EscaperoomGame
{
    internal class Program
    {
        private static bool _keyCollected; //key counts as collected if set to true
        private static bool _showPosition;
        private static bool _debugMode;

        //room dimensions
        private static int _roomX = 10;
        private static int _roomY = 10;

        // minimum and maximum room dimensions
        private const int RoomMinWidth = 10;
        private const int RoomMinHeight = 10;
        private const int RoomMaxWidth = 30;
        private const int RoomMaxHeight = 30;

        // player coordinates
        private static int _playerX = 2, _playerY = 2;
        
        //enemy
        private static int enemyLength = 6;
        private static int[][] _enemyTrail;

        //key coordinates
        private static int _keyPosX, _keyPosY;

        //door position
        private static int _doorPosX;
        private static int _doorPosY;

        //object icons
        private static string _playerIcon = "^";
        private static string _doorIcon = "\u2506"; // ┆
        private const string KeyIcon = "ƫ";
        private const string EnemyIcon = "X";

        //array
        private static int[,] _map;
        private static readonly Random Rnd = new Random();
        

        private enum EMapTiles
        {
            Free = -1,
            Wall,
            Door,
            Forbidden
        }

        private static char[] _mapTileCharacters = new char[]
        {
            ' ', // free
            '#', // wall
            ' ' // door
        };

        public static void Main(string[] args)
        {
            //new game starts
            NewGame();
        }

        /// <summary>
        /// starts a new game and calls all the relevant functions
        /// </summary>
        private static void NewGame()
        {
            
            //initializes the _enemyTrail array to have 2 positions for each array
            _enemyTrail = new int[enemyLength][];
            for (int i = 0; i < _enemyTrail.Length; i++)
            {
                _enemyTrail[i] = new int[2];
            }
            
            //start text
            IntroText();
            
            //user declares the size of the room
            DynamicRoomSizeInput();
            
            _keyCollected = false;
            
            //algorithm to fill the map array
            FillMapLogic();
            
            // shows text where the items are
            _showPosition = true;
            
            GameplayLoop();
        }

        /// <summary>
        /// everything that happens after moving once
        /// </summary>
        private static void GameplayLoop()
        {
            bool playing = true;
            while (playing)
            {
                Console.Clear();

                //draws map
                PrintMap();

                //previous player position
                int prevPlayerX = _playerX;
                int prevPlayerY = _playerY;

                PlayerControls();
                
                ChasePlayer();

                ProcessMovement(ref prevPlayerX, ref prevPlayerY);
                
                UpdateEnemyTrailPosition();
                
                //checks if the player stepped on the deadly trail
                foreach (var enemy in _enemyTrail)
                {
                    if (_playerX == enemy[0] && _playerY == enemy[1])
                    {
                        playing = false;
                    }
                }
            }
            
            //goes here if the player loses
            EndScreen(false);
        }

        /// <summary>
        /// User declares how big the room is by pressing the arrow keys to make it bigger or smaller
        /// </summary>
        private static void DynamicRoomSizeInput()
        {
            Console.CursorVisible = false;
            bool inputting = true;
            do
            {
                PrintRawMap();

                ConsoleKeyInfo keyInput = Console.ReadKey(true);
                switch (keyInput.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (_roomY > RoomMinHeight)
                        {
                            _roomY--;
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if (_roomY < RoomMaxHeight)
                        {
                            _roomY++;
                        }
                        break;
                    case ConsoleKey.LeftArrow:
                        if (_roomX > RoomMinWidth)
                        {
                            _roomX--;
                        }
                        break;
                    case ConsoleKey.RightArrow:
                        if (_roomX < RoomMaxWidth)
                        {
                            _roomX++;
                        }
                        break;
                    case ConsoleKey.Enter:
                        inputting = false;
                        break;
                }
            } while (inputting);
            
            //creates map array with the size _roomX and _roomY
            _map = new int[_roomX, _roomY];
        }

        /// <summary>
        /// prints only the map with no doors or other icons
        /// </summary>
        private static void PrintRawMap()
        {
            Console.Clear();
            Console.Write("Press the ");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("arrow keys");
            Console.ResetColor();
            Console.Write(" arrow keys to adjust the size of the room\n");
            
            for (int y = 0; y < _roomY; y++)
            {
                for (int x = 0; x < _roomX; x++)
                {
                    #region wall variations

                    if (x == 0 && y == 0) // top left corner
                    {
                        Console.Write("\u250c"); // ┌
                    }
                    else if (y == 0 && x == _roomX - 1) // top right corner
                    {
                        Console.Write("\u2510"); // ┐
                    }
                    else if (y == _roomY - 1 && x == 0) // bottom left corner
                    {
                        Console.Write("\u2514"); // └
                    }
                    else if (y == _roomY - 1 && x == _roomX - 1) // bottom right corner
                    {
                        Console.Write("\u2518"); // ┘
                    }
                    else if (y == _roomY - 1 || y == 0) // top and bottom walls
                    {
                        Console.Write("\u2500"); // ─
                    }
                    else if (x == 0 || x == _roomX - 1) // left and right walls
                    {
                        Console.Write("\u2502"); // │
                    }

                    #endregion

                    else // draws the rest of the enum, like free spaces
                    {
                        Console.Write(' ');
                        Console.ResetColor();
                    }

                    if (y == 0 && x != _roomX - 1 ||
                        y == _roomY - 1 && x != _roomX - 1) //top/bottom wall draws 2 dashes
                    {
                        Console.Write('\u2500'); // ─
                    }
                    else
                    {
                        Console.Write(' '); // space after every character
                    }

                }

                Console.WriteLine();
            }
            Console.WriteLine("Press enter to play");
        }

        /// <summary>
        /// Puts all static elements into the room
        /// </summary>
        private static void FillMapLogic()
        {
            RandomElements(); // places door, key, player and enemy randomly

            for (int y = 0; y < _roomY; y++) //draw x vertical lines
            {
                for (int x = 0; x < _roomX; x++) //draw y horizontal chars
                {
                    if (x == _doorPosX && y == _doorPosY)
                    {
                        _map[x, y] = (int)EMapTiles.Door;
                    }
                    else if (x == 0 || y == 0 || x == _roomX - 1 || y == _roomY - 1)
                    {
                        _map[x, y] = (int)EMapTiles.Wall;
                    }
                    else
                    {
                        _map[x, y] = (int)EMapTiles.Free;
                    }
                }
            }
        }
        
        /// <summary>
        /// Initializes random elements like the key, the enemy position and the door
        /// </summary>
        private static void RandomElements()
        {
            //random door location
            //1 top wall, 2 right wall, 3 bottom wall, 4 left wall
            int doorSide = Rnd.Next(1, 5);
            int doorX = 0;
            int doorY = 0;

            //could maybe be optimized, but it works ㄟ( ▔, ▔ )ㄏ
            switch (doorSide)
            {
                case 1:
                    doorX = Rnd.Next(1, _roomX - 2);
                    doorY = 0;
                    _doorIcon = "\u2550"; // ═

                    _map[doorX, 1] = (int)EMapTiles.Forbidden;
                    break;
                case 2:
                    doorX = _roomX - 1;
                    doorY = Rnd.Next(1, _roomY - 2);
                    _doorIcon = "\u2551"; // ║

                    _map[doorX - 1, doorY] = (int)EMapTiles.Forbidden;
                    break;
                case 3:
                    doorX = Rnd.Next(1, _roomX - 2);
                    doorY = _roomY - 1;
                    _doorIcon = "\u2550"; // ═

                    _map[doorX, doorY - 1] = (int)EMapTiles.Forbidden;
                    break;
                case 4:
                    doorX = 0;
                    doorY = Rnd.Next(1, _roomY - 2);
                    _doorIcon = "\u2551"; // ║

                    _map[1, doorY] = (int)EMapTiles.Forbidden;
                    break;
            }

            //puts door coords in the map array
            _map[doorX, doorY] = (int)EMapTiles.Door;
            _doorPosX = doorX;
            _doorPosY = doorY;

            // randomly places the key inside the room
            do
            {
                _keyPosX = Rnd.Next(1, _roomX - 1);
                _keyPosY = Rnd.Next(1, _roomY - 1);
            } while (_map[_keyPosX,_keyPosY] == (int)EMapTiles.Forbidden);

            //randomly places the player on a different x position than the key, so the start text doesn't overlap
            do
            {
                _playerX = Rnd.Next(1, _roomX - 1);
                do
                {
                    _playerY = Rnd.Next(1, _roomY - 1);
                } while (_playerY == _keyPosY);
            } while (_map[_keyPosX,_keyPosY] == (int)EMapTiles.Forbidden);
            

            //places the enemy at a random position
            do
            {
                _enemyTrail[0][0] = Rnd.Next(1, _roomX - 1);
                do
                {
                    _enemyTrail[0][1] = Rnd.Next(1, _roomY - 1);
                } while (_enemyTrail[0][1] == _playerY || _enemyTrail[0][1] == _keyPosY);
            } while (_map[_playerX, _playerY] == (int)EMapTiles.Forbidden);
        }

        /// <summary>
        /// Prints the map
        /// </summary>
        private static void PrintMap()
        {
            Console.ResetColor();

            for (int y = 0; y < _roomY; y++)
            {
                for (int x = 0; x < _roomX; x++)
                {
                    #region wall variations

                    if (x == 0 && y == 0) // top left corner
                    {
                        Console.Write("\u250c"); // ┌
                    }
                    else if (y == 0 && x == _roomX - 1) // top right corner
                    {
                        Console.Write("\u2510"); // ┐
                    }
                    else if (y == _roomY - 1 && x == 0) // bottom left corner
                    {
                        Console.Write("\u2514"); // └
                    }
                    else if (y == _roomY - 1 && x == _roomX - 1) // bottom right corner
                    {
                        Console.Write("\u2518"); // ┘
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
                            Console.Write(_doorIcon);
                            Console.ResetColor();
                        }
                    }
                    else if (y == _roomY - 1 || y == 0) // top and bottom walls
                    {
                        Console.Write("\u2500"); // ─
                    }
                    else if (x == 0 || x == _roomX - 1) // left and right walls
                    {
                        Console.Write("\u2502"); // │
                    }

                    #endregion

                    else if (x == _keyPosX && y == _keyPosY) // draws key
                    {
                        DrawItem(ConsoleColor.Yellow, KeyIcon);
                    }
                    else if (x == _playerX && y == _playerY) // draws player
                    {
                        DrawItem(ConsoleColor.Cyan, _playerIcon);
                    }
                    else
                    {
                        bool enemyFound = false;
                        foreach (var enemy in _enemyTrail) //checks for all enemy trail positions
                        {
                            if (x == enemy[0] && y == enemy[1]) //draws the enemy
                            {
                                enemyFound = true;
                                DrawItem(ConsoleColor.Red, EnemyIcon);
                                Console.ResetColor();
                                break;
                            }
                        }

                        if (!enemyFound) //draw free space
                        {
                            Console.Write(_mapTileCharacters[_map[x, y] + 1]);
                        }
                    }

                    if (y == 0 && x != _roomX - 1 ||
                        y == _roomY - 1 && x != _roomX - 1) //top/bottom wall draws 2 dashes
                    {
                        Console.Write('\u2500'); // ─
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
                StartDescription(_enemyTrail[0][0], _enemyTrail[0][1], "enemy"); // enemy
            }
        }

        /// <summary>
        /// gets user input and moves player to the next position
        /// </summary>
        private static void PlayerControls()
        {
            ConsoleKeyInfo keyInput = Console.ReadKey(true);
            switch (keyInput.Key)
            {
                case ConsoleKey.UpArrow:
                case ConsoleKey.W:
                    _playerY--;
                    _playerIcon = "^";
                    break;

                case ConsoleKey.DownArrow:
                case ConsoleKey.S:
                    _playerY++;
                    _playerIcon = "V";
                    break;

                case ConsoleKey.LeftArrow:
                case ConsoleKey.A:
                    _playerX--;
                    _playerIcon = "<";
                    break;

                case ConsoleKey.RightArrow:
                case ConsoleKey.D:
                    _playerX++;
                    _playerIcon = ">";
                    break;

                case ConsoleKey.X:
                    _debugMode = !_debugMode;
                    break;

                case ConsoleKey.Escape:
                    Environment.Exit(0);
                    break;
            }
            
        }

        /// <summary>
        /// Checks if player is inside a wall, is collecting the key, goes through the door or is killed
        /// </summary>
        /// <param name="prevPlayerX">previous x coordinate of player</param>
        /// <param name="prevPlayerY">previous y coordinate of player</param>
        private static void ProcessMovement(ref int prevPlayerX, ref int prevPlayerY)
        {
            if (_playerX >= 0 && _playerX < _roomX && _playerY >= 0 && _playerY < _roomY)
            {
                //only if this condition is false, the player will move normally
                if (_map[_playerX, _playerY] == (int)EMapTiles.Wall)
                {
                    ResetPlayer(prevPlayerX, prevPlayerY);
                    Console.Beep(150, 10);
                }
                else if (_map[_playerX, _playerY] == (int)EMapTiles.Door)//player collides with the door
                {
                    if (_keyCollected)//player has key and enters the door
                    {
                        EndScreen(true); // player won
                    }
                    else //player cant go through the door
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

        /// <summary>
        /// puts player to previous position
        /// </summary>
        /// <param name="prevPlayerX">previous x coordinate</param>
        /// <param name="prevPlayerY">previous y coordinate</param>
        private static void ResetPlayer(int prevPlayerX, int prevPlayerY)
        {
            _playerX = prevPlayerX;
            _playerY = prevPlayerY;
        }

        /// <summary>
        /// arrows indicating where the important objects are
        /// </summary>
        /// <param name="posX">x coordinate of item</param>
        /// <param name="posY">y coordinate of item</param>
        /// <param name="item">item name</param>
        private static void StartDescription(int posX, int posY, string item)
        {
            //places the cursor where the item is and draws the text
            Console.SetCursorPosition(posX * 2 + 2, posY);
            Console.Write($"<-- {item}");
            _showPosition = false;
        }
        
        /// <summary>
        /// Shows the position of specified
        /// </summary>
        /// <param name="posX">x coordinate of item</param>
        /// <param name="posY">y coordinate of item</param>
        /// <param name="item">item name</param>
        private static void ShowCoordinates(int posX, int posY, string item)
        {
            //shows grey text of where the important items are
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"\n{item} at X: {posX}, Y: {posY}");
            Console.ResetColor();
        }

        /// <summary>
        /// welcome message with explanation of how the game works
        /// </summary>
        private static void IntroText()
        {
            Console.Clear();

            Console.ResetColor();
            Console.Write("~ Welcome to my escape room! ~\n\nThe rules are simple:\nFind the");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(" key ");

            Console.ResetColor();
            Console.Write("and go through the door.\nBut avoid the ");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("red trail");

            Console.ResetColor();
            Console.Write(" that follows you!\n\nMove your character with");

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(" W, A, S and D");

            Console.ResetColor();
            Console.Write(" or the ");

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("arrow keys");

            Console.ResetColor();
            Console.WriteLine("\n\nPress any key to continue with the room size input..");
            Console.ReadKey();

            Console.ResetColor();
        }

        /// <summary>
        /// Disables the key visual and plays a noise
        /// </summary>
        private static void CollectKey()
        {
            _keyCollected = true;
            _keyPosX = 0;
            _keyPosY = 0;
            _map[_keyPosX, _keyPosY] = (int)EMapTiles.Free;

            //sounds plays when picking up the key
            Console.Beep(1900, 25);
            Console.Beep(1900, 275);
        }

        /// <summary>
        /// Shows the end screen
        /// </summary>
        /// <param name="end">true = you won, false = you lost</param>
        private static void EndScreen(bool end)
        {
            Console.Clear();

            switch (end)
            {
                case true:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("~ Congrats! You've finished the game! ~");

                    Console.Beep(293, 200);
                    Console.Beep(369, 200);
                    Console.Beep(440, 200);
                    Console.Beep(587, 600);
                    break;
                case false:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Game Over!");
                    Console.ResetColor();

                    Console.WriteLine("You died because the enemy caught you!");
                    break;
            }

            Console.ResetColor();
            Console.Write("\nPress any key to play again\nor press ");

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(" Q ");

            Console.ResetColor();
            Console.Write("to quit the game.");
            
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.Q:
                    Environment.Exit(0);
                    break;
                default:
                    NewGame();
                    break;
            }
        }
        
        /// <summary>
        /// Faster method to draw an icon with a color
        /// </summary>
        /// <param name="color">color of the icon</param>
        /// <param name="icon">what to draw</param>
        private static void DrawItem(ConsoleColor color, string icon)
        {
            Console.ForegroundColor = color;
            Console.Write(icon);
            Console.ResetColor();
        }

        /// <summary>
        /// moves the enemy closer to the player by 1 field
        /// </summary>
        private static void ChasePlayer()
        {
            int x = _enemyTrail[0][0], y = _enemyTrail[0][1];
            
            //enemy will either move on the vertical or horizontal axis;
            switch (Rnd.Next(0,2))
            {
                case 0:

                    if (_playerX < _enemyTrail[0][0])
                    {
                        x--;
                    }
                    else if (_playerX > _enemyTrail[0][0])
                    {
                        x++;
                    }
                    else
                    {
                        if (_playerY < _enemyTrail[0][1])
                        {
                            y--;
                        }
                        else
                        {
                            y++;
                        }
                    }

                    break;

                case 1:

                    if (_playerY < _enemyTrail[0][1])
                    {
                        y--;
                    }
                    else if (_playerY > _enemyTrail[0][1])
                    {
                        y++;
                    }
                    else
                    {
                        if (_playerX < _enemyTrail[0][0])
                        {
                            x--;
                        }
                        else
                        {
                            x++;
                        }
                    }

                    break;
            }
            
            _enemyTrail[0][0] = x;
            _enemyTrail[0][1] = y;
        }

        /// <summary>
        /// moves all the arrays 1 position up towards the end and clears the last position
        /// </summary>
        private static void UpdateEnemyTrailPosition()
        {
            _map[_enemyTrail[_enemyTrail.Length-1][0], _enemyTrail[_enemyTrail.Length-1][1]] = (int)EMapTiles.Free;
            
            for (int i = _enemyTrail.Length; i >= 0; i--)
            {
                if (i < _enemyTrail.Length-1)
                {
                    _enemyTrail[i + 1][0] = _enemyTrail[i][0];
                    _enemyTrail[i + 1][1] = _enemyTrail[i][1];
                }
            }
        }
    }
}