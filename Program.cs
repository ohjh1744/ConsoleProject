using static ConsoleProJect.Program;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ConsoleProJect
{
    internal class Program
    {
        //0.지뢰가없는상태, 1.지뢰가있는상태,
        //2.지뢰가 없어 안전하다는것을 확인한 상태, 3.지뢰가 있다는 것을 확인한 상태, 지뢰와의 거리가 확인된 상태
        enum Mine
        {
            NoMine, Mine, SafeZone, MineChecked, DistChekced
        }

        public struct GameData
        {
            //플레이어 커서위치
            public int playerX;
            public int playerY;
            //지뢰 위치
            public int mineX;
            public int mineY;
            //맵 사이즈
            public int mapXSize;
            public int mapYSize;
            //맵 현재상태체크
            public int[,] map;
            //지뢰에서 떨어진 거리
            public int[,] farFromMine;
            //도전 횟수
            public int tryCount;
            //지뢰 찾기여부
            public bool isMineFind;
            //사용자 입력키
            public ConsoleKeyInfo inputKey;
        }
        static GameData gameData = new GameData();
        

        static void Main(string[] args)
        {
            Start();
            while (gameData.tryCount > 0)
            {
                Render();
                if (gameData.isMineFind == true)
                {
                    break;
                }
                InputKey();
            }
            GameEnd();
        }

        static void Start()
        {
            MainScene();
;            
            gameData.playerX = 10;
            gameData.playerY = 10;
            gameData.mapXSize = 22;
            gameData.mapYSize = 22;
            gameData.map = new int[gameData.mapYSize, gameData.mapXSize];
            gameData.farFromMine = new int[gameData.mapYSize, gameData.mapXSize];
            gameData.tryCount = 10;
            gameData.isMineFind = false;
            Console.CursorVisible = false;

            MinePos();       
        }

        static void MainScene()
        {
            Console.WriteLine("#####################################");
            Console.WriteLine("#           지뢰    찾기            #");
            Console.WriteLine("#####################################");
            Console.WriteLine("[게임 설명]");
            Console.WriteLine("마을에 지뢰가 있다고 한다.");
            Console.WriteLine("마을 사람들 안전을 위해서 지뢰를 제거하자.");
            Console.WriteLine("주어진 기회동안 지뢰를 찾으면 승리한다.");
            Console.WriteLine("C키를 통해 근처에 지뢰가 있는지 확인할 수 있으며,");
            Console.WriteLine("지뢰가 없을 시 지뢰에서부터 현 위치까지의 거리가 나타난다.");
            Console.WriteLine("10칸 이상의 거리는 0으로 표시. 10칸 미만은 그대로 표시.");
            Console.WriteLine();
            Console.WriteLine("[게임 키]");
            Console.WriteLine("C : 지뢰 확인");
            Console.WriteLine("키보드 화살표 : 상하좌우 이동");
            Console.WriteLine();
            Console.WriteLine("(게임을 시작하려면 엔터를 누르십시오.)");
            Console.ReadLine();

        }
        static void MinePos()
        {
            Random random = new Random();
            gameData.mineX = random.Next(1, gameData.mapXSize - 1);
            gameData.mineY = random.Next(1, gameData.mapYSize - 1);
            gameData.map[gameData.mineY, gameData.mineX] = (int)Mine.Mine;
        }

        static void Render()
        {
            Console.Clear();
            WriteBackGround();
            WritePlayer();
            WriteTryCount();
        }

        static void WriteBackGround()
        {
            Console.SetCursorPosition(0, 0);
            for (int i = 0; i < gameData.mapYSize; i++)
            {
                for (int j = 0; j < gameData.mapXSize; j++)
                {
                    Console.SetCursorPosition(j, i);
                    //가장자리는 그냥 그림 채우기
                    if (i == 0 || i == gameData.mapYSize-1 || j == 0 || j == gameData.mapXSize - 1)
                    {
                        Console.Write("■");
                    }

                    switch ((Mine)gameData.map[i, j])
                    {
                        case Mine.SafeZone:
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("●");
                            Console.ResetColor();
                            break;
                        case Mine.MineChecked:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("★");
                            Console.ResetColor();
                            break;
                        case Mine.DistChekced:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            //지뢰와의 거리가 10이상인경우 0으로 표시
                            if (gameData.farFromMine[i, j] >= 10)
                            {
                                Console.Write("0");
                            }
                            //10 미만인경우 그대로 거리 표시
                            else
                            {
                                Console.Write(gameData.farFromMine[i, j]);
                            }
                            Console.ResetColor();
                            break;
                        //NoMine과 YesMine
                        default:
                            Console.Write("□");
                            break;
                    }
                }
                Console.WriteLine();
            }
        }

        static void WritePlayer()
        {
            Console.SetCursorPosition(gameData.playerX, gameData.playerY);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("▲");
            Console.ResetColor();
        }

        static void WriteTryCount()
        {
            Console.SetCursorPosition(0, 23);
            Console.WriteLine($"남은 도전 횟수: {gameData.tryCount}");
        }

        static void InputKey()
        {
            gameData.inputKey = Console.ReadKey(true);

            switch (gameData.inputKey.Key)
            {
                case ConsoleKey.LeftArrow:
                    if (gameData.playerX > 1)
                    {
                        gameData.playerX--;
                    }
                    break;
                case ConsoleKey.RightArrow:
                    if (gameData.playerX < 20)
                    {
                        gameData.playerX++;
                    }
                    break;
                case ConsoleKey.UpArrow:
                    if (gameData.playerY > 1)
                    {
                        gameData.playerY--;
                    }
                    break;
                case ConsoleKey.DownArrow:
                    if (gameData.playerY < 20)
                    {
                        gameData.playerY++;
                    }
                    break;
                case ConsoleKey.C:
                    gameData.tryCount--;
                    CheckMine();
                    break;
            }
        }


        static void CheckMine()
        {
            int checkPointX = gameData.playerX;
            int checkPointY = gameData.playerY;
            for(int i = checkPointY-1; i <= checkPointY+1; i++)
            {
                for(int j = checkPointX-1; j <= checkPointX+1; j++)
                {
                    //마인 위치를 제외한 player가 지뢰를 체크한 곳
                    if (i == checkPointY && j == checkPointX && gameData.map[i, j] != (int)Mine.Mine)
                    {
                        gameData.map[i, j] = (int)Mine.DistChekced;
                        gameData.farFromMine[i, j] = CheckDist(checkPointX, checkPointY);
                    }
                    //마인 위치가 아닌 곳
                    else if (gameData.map[i, j] == (int)Mine.NoMine)
                    {
                        gameData.map[i, j] = (int)Mine.SafeZone;
                    }
                    //마인 위치가 맞는 곳
                    else if (gameData.map[i,j] == (int)Mine.Mine)
                    {
                        gameData.map[i, j] = (int)Mine.MineChecked;
                        gameData.isMineFind = true;
                    }

                    //switch ((Mine)gameData.map[i, j])
                    //{
                    //    case Mine.NoMine:
                    //    case Mine.DistChekced:
                    //    case Mine.SafeZone:
                    //        if (i == checkPointY && j == checkPointX )
                    //        {
                    //            gameData.map[i, j] = (int)Mine.DistChekced;
                    //            gameData.farFromMine[i, j] = CheckDist(checkPointX, checkPointY);
                    //            continue;
                    //        }
                    //        gameData.map[i, j] = (int)Mine.SafeZone;
                    //        break;
                    //    case Mine.Mine:
                    //        gameData.isMineFind = true;
                    //        gameData.map[i, j] = (int)Mine.MineChecked;
                    //        break;
                    //}

                }
            }
        }


        static int CheckDist(int x, int y)
        {
            int[,] dist = new int[gameData.mapYSize, gameData.mapXSize];
            int[] dx = new int[4] { 1, -1, 0, 0 };
            int[] dy = new int[4] { 0, 0, -1, 1 };

            Queue<(int , int)> q = new Queue<(int, int)>();
            q.Enqueue((y, x));
            while (q.Count != 0)
            {
                int cx = q.Peek().Item2;
                int cy = q.Peek().Item1;
                q.Dequeue();

                if(cx == gameData.mineX && cy == gameData.mineY)
                {
                    return dist[cy, cx];
                }
                for(int i = 0; i < 4; i++)
                {
                    int nx = cx + dx[i];
                    int ny = cy + dy[i];
                    if(nx <= 0 || ny <= 0 || nx >= gameData.mapXSize-1 || ny >= gameData.mapYSize-1)
                    {                       
                            continue;
                    }
                    if (dist[ny, nx] == 0) {
                        q.Enqueue((ny, nx));
                        dist[ny, nx] = dist[cy, cx] + 1;

                    }
                }
            }
            return 0;
        }

        static void GameEnd()
        {
            if (gameData.isMineFind == true)
            {
                Win();
            }
            else if (gameData.isMineFind == false)
            {
                Lose();
            }
        }
        static void Win()
        {
            Console.SetCursorPosition(0, gameData.mapYSize + 2);
            Console.WriteLine("지뢰를 발견했습니다!");
        }
        static void Lose()
        {
            ShowAnswer();
            Console.SetCursorPosition(0, gameData.mapYSize + 2);
            Console.WriteLine("지뢰를 발견하지 못했습니다.");
        }

        static void ShowAnswer()
        {
            Console.SetCursorPosition(gameData.mineX, gameData.mineY);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("★");
            Console.ResetColor();
        }


    }
}
