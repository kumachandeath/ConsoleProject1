namespace SnakeGame
{
    internal class Program
    {
        const int GAME_BOARD_SIZE = 30;
        const int EMPTY = 0;
        const int SNAKE = 1;
        const int FOOD = 2;
        const int FIRST_X = 15;
        const int FIRST_Y = 15;
        const int SPEED = 70;
        
        static int[] dx = { 0, 1, 0, -1 };
        static int[] dy = { 1, 0, -1, 0 };
        static Queue<int[]> queue = new Queue<int[]>(); // 뱀 몸통 좌표를 담을 큐
        static int foodx = 10; // 음식의 x 좌표
        static int foody = 10; // 음식의 y 좌표
        static int dir = 0; // → : 0, ↓ : 1, ← : 2, ↑ : 3
        static int snakeLength = 3;
        static int food = 0;

        
        static int[,] map;

        static int curX = FIRST_X;
        static int curY = FIRST_Y;

        static int nextX = curX + dx[dir];
        static int nextY = curY + dy[dir];

        static bool isGameOver = false;

        static void Main(string[] args)
        {
            GameStart();

            while (!isGameOver)
            {
                Render();
                Input();
                Update();
            }

            GameEnd();

        }

        static void GameStart()
        {
            Console.CursorVisible = false;
            
            isGameOver = false;
            map = new int[GAME_BOARD_SIZE, GAME_BOARD_SIZE];


            InitSnake(map);
            map[foodx, foody] = FOOD;

            Console.Clear();
            Console.WriteLine("┌──────────────────────────────────┐");
            Console.WriteLine("│                                  │");
            Console.WriteLine("│             뱀 게임!             │");
            Console.WriteLine("│                                  │");
            Console.WriteLine("└──────────────────────────────────┘");
            Console.WriteLine();
            Console.WriteLine("    계속하려면 아무키나 누르세요    ");
            Console.ReadKey();
        }

        static void GameEnd()
        {
            Console.Clear();
            Console.WriteLine("┌──────────────────────────────────┐");
            Console.WriteLine("│                                  │");
            Console.WriteLine("│            게임 오버!            │");
            Console.WriteLine("│                                  │");
            Console.WriteLine("└──────────────────────────────────┘");
            Console.WriteLine();
        }

        // 뱀의 위치 초기화
        static void InitSnake(int[,] map)
        {
            map[FIRST_X, FIRST_Y] = SNAKE;
            map[FIRST_X, FIRST_Y - 1] = SNAKE;
            map[FIRST_X, FIRST_Y - 2] = SNAKE;

            queue.Enqueue(new int[] { FIRST_X, FIRST_Y });
            queue.Enqueue(new int[] { FIRST_X, FIRST_Y - 1 });
            queue.Enqueue(new int[] { FIRST_X, FIRST_Y - 2 });
        }

        static void Render()
        {
            Console.Clear();
            Console.SetCursorPosition(0, 0); // 보드를 (0,0)부터 그리기 위해 커서 좌표 이동

            PrintMap();
        }

        static void Input()
        {
            // 방향키 입력을 받는 Thread 생성
            Thread thread = new Thread(() => GetDirection());
            thread.Start();
        }

        static void Update()
        {
            CheckGameOver();
            Thread.Sleep(SPEED);
        }

        static void CheckGameOver()
        {
            // 벽에 닿거나 뱀의 몸통에 닿으면 게임 종료
            if (nextX < 0 || nextY < 0 || nextX >= GAME_BOARD_SIZE || nextY >= GAME_BOARD_SIZE || map[nextX,nextY] == SNAKE )
            {
                Console.WriteLine("게임 종료!");
                isGameOver = true;
            }
            else
            {
                MoveSnake(nextX, nextY, map);
            }
            curX = nextX;
            curY = nextY;
        }

        // 게임판 출력
        static void PrintMap()
        {
            // 뱀이 다음에 이동할 좌표값
            nextX = curX + dx[dir];
            nextY = curY + dy[dir];

            // 음식 위치 초기화
            map[foodx, foody] = FOOD;

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    int curMap = map[i, j];
                    if (curMap == EMPTY)
                    {
                        Console.Write("□"); // 빈 공간
                    }
                    else if (curMap == SNAKE)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("■"); // 뱀
                        Console.ResetColor();
                    }
                    else if (curMap == FOOD)
                    {
                        Console.Write("●"); // 음식
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine("뱀 길이 : " + snakeLength);
            Console.WriteLine("먹은 음식 개수 : " + food);
        }

        // 방향키 입력을 받아서 뱀의 이동 방향 바꾸기
        static void GetDirection()
        {
            ConsoleKeyInfo input;
            while (true)
            {
                input = Console.ReadKey();
                switch (input.Key)
                {
                    case ConsoleKey.RightArrow:
                        dir = 0;
                        break;
                    case ConsoleKey.DownArrow:
                        dir = 1;
                        break;
                    case ConsoleKey.LeftArrow:
                        dir = 2;
                        break;
                    case ConsoleKey.UpArrow:
                        dir = 3;
                        break;
                    case ConsoleKey.Q:
                        break;

                }
            }
        }

        // 뱀의 이동과 음식의 위치 재설정
        static void MoveSnake(int x, int y, int[,] map)
        {
            // 다음에 이동할 좌표가 빈칸일때
            if (map[x, y] == EMPTY)
            {
                int[] last = queue.Dequeue(); // 뱀의 꼬리 부분 한 칸 없애기
                map[last[0], last[1]] = EMPTY;
            }
            // 다음에 이동할 좌표가 음식일때
            else if (map[x, y] == FOOD)
            {
                snakeLength++; // 뱀의 몸 길이 증가
                food++; // 먹은 음식 개수 증가

                // 빈칸 중에서 임의로 골라 별 좌표 업데이트
                do
                {
                    foodx = new Random().Next(1, GAME_BOARD_SIZE - 1);
                    foody = new Random().Next(1, GAME_BOARD_SIZE - 1);
                } while (map[foodx, foody] == SNAKE);
            }

            map[x, y] = SNAKE; // 다음 이동할 좌표를 뱀으로 바꿔줌
            queue.Enqueue(new int[] { x, y }); // 뱀 몸통 업데이트
        }
    }
}
