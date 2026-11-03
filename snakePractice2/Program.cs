using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Principal;

namespace snakePractice2
{
    using rockrlib;

    internal class Program
    {
        static bool _edgePortals = true;

        static bool _gameRunning = true;

        static int _gameSpeed = 3000;

        static (int x, int y) _playerPos;
        static int _frameTimer = 0;
        static System.ConsoleKey _input = System.ConsoleKey.D;
        static ConsoleKey _newInput = ConsoleKey.Enter;

        static List<(int x, int y)> _tailPos = new List<(int x, int y)>();
        static int _tailLength = 0;

        static bool _newApplePos = true;
        static (int x, int y) _applePos = (0, 0);

        static char _snakeSkinHead = '0';
        static char _snakeSkinTail = 'O';
        static char _appleSkin = 'A';
        static void Main(string[] args)
        {
            Console.CursorVisible = false;


            while (true)
            {
                GameSetup();
                Menu();

                while (_gameRunning) 
                {
                    MainGame();
                }
                Console.WriteLine(' ');
                Console.Write("Press any key");
                Console.ReadKey();
            }
        }
        
        static void Menu()
        {
            while (true)
            {
                Console.WriteLine("Welcome to Snake Ahead!");

                char input = Input.GetOption("" +
                    "Welcome to Snake Ahead!\n" +
                    "\n" +
                    "Chose one of the following options:\n" +
                    "1. Start Game\n" +
                    "2. Change Settings\n" +
                    "3. Change Skin\n", 
                    new char[] {'1', '2', '3'});

                switch (input)
                {
                    case '1':
                        return;
                    case '2':
                        Settings();
                        break;
                    case '3':
                        _appleSkin = Input.GetString("Current apple skin is '" + _appleSkin + "'. Input new apple skin", 1, 1)[0];
                        _snakeSkinHead = Input.GetString("Current snake head skin is '" + _snakeSkinHead + "'. Input new snake head skin", 1, 1)[0];
                        _snakeSkinTail = Input.GetString("Current snake body skin is '" + _snakeSkinTail + "'. Input new snake body skin", 1, 1)[0];
                        break;
                    default:
                        break;
                }

            }
        }

        static void Settings()
        {
            char input = Input.GetOption("" +
                "Chose one of the following options:\n" +
                "1. Change Game Speed\n",
                new char[] { '1' });

            switch (input)
            {
                case '1':
                    _gameSpeed = Input.GetInt("Input new Game Speed (Default value is 3000). The lower the value the faster the game.");
                    break;
                case '2':
                    _edgePortals = !_edgePortals;
                    Console.WriteLine("Edge portals set to " +  _edgePortals);
                    Console.ReadKey();
                    break;
                default:
                    break;
            }
        }

        static void GameSetup()
        {
            _playerPos.y = Console.WindowHeight / 2;
            _playerPos.x = Console.WindowWidth / 2;
            _input = System.ConsoleKey.Enter;
            _newInput = System.ConsoleKey.Enter;

            _tailLength = 0;
            _frameTimer = 0;

            _newApplePos = true;
            _applePos = (0, 0);

            _tailPos.Clear();

            _gameRunning = true;
        }

        static void GameOver()
        {
            _gameRunning = false;
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("You died!");
            Console.WriteLine("Your score: " + _tailLength);
        }

        static void MainGame()
        {
            Console.SetCursorPosition(0, 0);
            Console.Write("");
            GameLoop();
            if (_gameRunning)
            {
                DrawScreen();
            }

        }

        static void PlaceTile(char tileName, (int x, int y) pos)
        {
            try
            {
                Console.SetCursorPosition(pos.x, pos.y);
            }
            catch (Exception)
            {
                Console.SetCursorPosition(0, 0);
            }
            Console.Write(tileName);
        }

        static void GameLoop()
        {
            _frameTimer++;
            if (_frameTimer > _gameSpeed)
            {
                if (_newApplePos)
                {
                    _newApplePos = false;
                    SpawnApple();
                }
                if (_tailLength > 0)
                {
                    if (_tailPos.Count < 1)
                    {
                        _tailPos.Add(_playerPos);
                    }
                    MoveTail();
                }

                if (Console.KeyAvailable)
                {
                    _newInput = Console.ReadKey(true).Key;
                }
                if (_newInput == ConsoleKey.W || _newInput == ConsoleKey.A || _newInput == ConsoleKey.S || _newInput == ConsoleKey.D)
                {
                    _input = _newInput;
                }
                switch (_input)
                {
                    default:
                        break;
                    case ConsoleKey.W:
                        _playerPos.y -= 1;
                        break;
                    case ConsoleKey.S:
                        _playerPos.y += 1;
                        break;
                    case ConsoleKey.D:
                        _playerPos.x += 1;
                        CheckForTouch();
                        _playerPos.x += 1;
                        break;
                    case ConsoleKey.A:
                        _playerPos.x -= 1;
                        CheckForTouch();
                        _playerPos.x -= 1;
                        break;

                }
                CheckForTouch();

                int newPos = 0;
                if (LoopAround(_playerPos, out newPos) == "x")
                {
                    _playerPos.x = newPos;
                }
                else
                {
                    if (LoopAround(_playerPos, out newPos) == "y")
                    {
                        _playerPos.y = newPos;
                    }
                }
            }
        }

        static void CheckForTouch()
        {
            if (_playerPos.x == _applePos.x & _playerPos.y == _applePos.y)
            {
                _tailLength += 1;
                _newApplePos = true;
            }

            if ( _tailPos.Contains(_playerPos) )
            {
                GameOver();
            }
        }

        static string LoopAround((int x, int y) position, out int newPos)
        {
            if (position.x > Console.WindowWidth-4)
            {
                newPos = 4;
                return "x";
            }
            if (position.x < 4)
            {
                newPos = Console.WindowWidth-4;
                return "x";
            }

            if (position.y == Console.WindowHeight)
            {
                newPos = 1;
                return "y";
            }
            if (position.y == 0)
            {
                newPos = Console.WindowHeight-1;
                return "y";
            }
            newPos = 0;
            return "none";
        }

        static void MoveTail()
        {
            (int x, int y) lastPos = _tailPos[_tailPos.Count - 1];
            for (int i = _tailPos.Count - 1; i > -1; i--)
            {
                if (i == 0)
                {
                    _tailPos[i] = _playerPos;
                }
                else
                {
                    _tailPos[i] = _tailPos[i - 1];

                }
            }
            if(_tailLength > _tailPos.Count)
            {
                _tailPos.Add(lastPos);
            }
        }

        static void DrawScreen()
        {
            
            if (_frameTimer > _gameSpeed) {
                _frameTimer = 0;
                Console.Clear();
                PlaceTile(_snakeSkinHead, _playerPos);
                for (int i = 0; i < _tailPos.Count ; i++)
                {
                    PlaceTile(_snakeSkinTail, _tailPos[i]);
                }
                PlaceTile(_appleSkin, _applePos);
            }

        }

        /// <summary>
        /// Originally planned to draw edges but due to my incapabilities to make good code I am not using it.
        /// </summary>
        //static void DrawEdge()
        //{
        //    for (int i = 0; i < Console.WindowWidth - 3; i++)
        //    {
        //        PlaceTile('X', (i, 0));
        //    }
        //    for (int i = 0; i < Console.WindowWidth - 3; i++)
        //    {
        //        PlaceTile('X', (i, Console.WindowHeight - 1));
        //    }
        //}

        static void SpawnApple()
        {
            Random rnd = new Random();
            while (true)
            {
                _applePos.x = rnd.Next(3, Console.WindowWidth - 3);
                _applePos.y = rnd.Next(1,Console.WindowHeight - 1);
                if (_playerPos == _applePos || _tailPos.Contains(_applePos))
                {
                    continue;
                }
                if (!isEven(_applePos.x))
                {
                    continue;
                }
                break;
            }
        }

        static bool isEven(int n)
        {
            int rem = n % 2;
            if (rem == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
