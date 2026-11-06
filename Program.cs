using System.Text.Json;

public class Player
{
    public bool IsCPU { get; set; }
    public string Symbol {  get; set; }
}

public class Board
{
    public string[] horizontal = new string[3] { "1", "2", "3" };
    public string[] vertical = new string[3] { "A", "B", "C" };

    private Dictionary<string, string> boardPositions = new Dictionary<string, string>();
  
    public Board()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                string boardplace = $"{vertical[i]}{horizontal[j]}";
                boardPositions.Add(boardplace, "-");
            }
        }
    }

    public void DrawBoard()
    {
        // write column names
        Console.WriteLine("    1    2    3");
        Console.WriteLine();
        for (int i = 0; i < vertical.Length; i++)
        {
            // write row names
            Console.Write($" {vertical[i]} ");
            // write board places
            for (int j = 0; j < horizontal.Length; j++)
            {
                string boardplace = $"{vertical[i]}{horizontal[j]}";
                Console.Write($" {boardPositions[boardplace]}   ");
            }
            Console.WriteLine();
            Console.WriteLine();
        }
    }

    public bool MakeMove(string position, string symbol)
    {
        // If the position is a position on the board
        // and the position is not already taken.
        if (boardPositions.ContainsKey(position) && boardPositions[position] == "-")
        {
            boardPositions[position] = symbol;
            return true;
        }
     return false;
    }

    public List<string> GetPossibleMoves()
    {
        List<string> possibleMoves = new List<string>();

        foreach (KeyValuePair<string, string> move in boardPositions)
        {
            if (move.Value == "-")
            {
                possibleMoves.Add(move.Key);
            }
        }
        return possibleMoves;
    }

    public bool CheckWinner(string symbol)
    {
        List<string> moves = new List<string>();

        foreach (KeyValuePair<string, string> move in boardPositions)
        {
            // first collect the moves of the player
            if (move.Value == symbol)
            {
                moves.Add(move.Key);
            }
        }

        // check diagonal
        if (moves.Contains("A1") &&
            moves.Contains("B2") &&
            moves.Contains("C3"))
        {
            return true;
        }

        if (moves.Contains("C1") &&
            moves.Contains("B2") &&
            moves.Contains("A3"))
        {
            return true;
        }

        // check vertical
        foreach ( string row in vertical)
        {
            if (moves.Count(kv => kv.StartsWith(row)) == vertical.Length)
            {
                return true;
            }
        }

        // check horizontal
        foreach( string column in horizontal)
        {
            if (moves.Count(kv => kv.EndsWith(column)) == horizontal.Length)
            {
                return true;
            }
        }
        return false;
    }
}

public class Game
{
    private readonly Board board;
    private Player CPU;
    private Player player;
    private Player? currentPlayer;

    public Game()
    {
        board = new Board();

        CPU = new Player();
        CPU.IsCPU = true;
        CPU.Symbol = "O";

        player = new Player();
        player.IsCPU = false;
        player.Symbol = "O";
    }

    private bool Move()
    {
        string move;

        if (currentPlayer == null)
        {
            throw new InvalidOperationException(
                "The game has not started yet.");
        }

        string symbol = currentPlayer.Symbol;

        if (currentPlayer.IsCPU)
        {
            Console.WriteLine("CPU makes a move");
            List<string> possibleMoves = board.GetPossibleMoves();
            move = possibleMoves[Random.Shared.Next(0, possibleMoves.Count)];    
            Console.WriteLine($"CPU move: {move}");

        }
        else
        {
            Console.WriteLine("Your turn");
            Console.Write("Make your move: ");
            move = Console.ReadLine() ?? "";
        }

        return board.MakeMove(move, symbol);

    }

    public void NextPlayer()
    {
        currentPlayer = currentPlayer == CPU ? player : CPU;
    }

    public void Start()
    {
        // draw the initial board
        board.DrawBoard();
        // bepaal wie er begint
        currentPlayer = WhoIsGoingFirst();
        // start de game loop
        while (board.GetPossibleMoves().Count > 0) // and onbody won yet
        {
            if (!Move())
            {
                continue;
            }

            board.DrawBoard();
            if (board.CheckWinner(currentPlayer.Symbol))
            {
                Console.WriteLine($"The winner is {currentPlayer.Symbol}");

                break;
            }

            NextPlayer();

        }
    }

    private Player WhoIsGoingFirst()
    {
        if (Random.Shared.Next(0, 2) == 1)
        {
            return CPU;
        }
        else
        {
            return player;
        }
    }
}

public class Program
{
    public static void Main()
    {
        Game game = new Game();
        game.Start();
    }
}