using SchachKI.src.ai;
using SchachKI.src.ui;

namespace SchachKI.src.game
{

    public class Game
    {
        public List<ChessMove> Moves;
        public List<string> UserPiecesDead;
        public List<string> AiPiecesDead;
        private Difficulty _difficulty;
        private string _userColor;
        private AiHandler _aiHandler;
        private Rules _rules;
        private bool _isGameOver;

        public Game(Difficulty difficulty, string userColor)
        {
            Console.WriteLine("game wird geladen");
            _isGameOver = false;
            _userColor = userColor;
            _rules = new Rules(this);
            Moves = new List<ChessMove>();
            UserPiecesDead = new List<string>();
            AiPiecesDead = new List<string>();
            _difficulty = difficulty;
            _aiHandler = new AiHandler(difficulty, getAiColor());
            Console.WriteLine("game geladen");
        }

        public string getUserColor()
        {
            return _userColor;
        }

        public async Task<ChessMove> UserMove(string from, string to, string? killedPiece, string setup)
        {
            ChessMove move = new ChessMove(from, to);
            move.Color = _userColor;
            Moves.Add(move);

            if (killedPiece != null)
            {
                AiPiecesDead.Add(killedPiece);
            }

            ChessMove aiMove = await AiMove(setup);
            return aiMove;
        }

        public async Task<ChessMove> AiMove(string setup)
        {
            ChessMove move = await _aiHandler.getNextLegalMove(this, getMovesAsString(), setup, string.Join(";", _rules.GetLegalMoves(getAiColor())));
            if(move == null)
            {
                return await AiMove(setup);
            }
            Moves.Add(move);
            Console.WriteLine(" ");
            Console.WriteLine(move.getCode());
            Console.WriteLine(getMovesAsString());
            Console.WriteLine(" ");
            return move;
        }

        public string[,] GetBoardArray()
        {
            //derzeitiges board simulieren
            string[,] board = new string[8, 8];

            // start positionen laden
            string json = System.Text.Encoding.Default.GetString(Properties.Resources.start_positions);
            var pieces = System.Text.Json.JsonSerializer.Deserialize<List<PieceData>>(json);
            foreach (var piece in pieces)
            {
                Point pos = BoardRenderer.GetCoordsFromField(piece.Field);
                board[pos.X, pos.Y] = piece.Image;
            }

            // alle gespielten Züge simulieren
            foreach (var move in Moves)
            {
                Point from = BoardRenderer.GetCoordsFromField(move.From);
                Point to = BoardRenderer.GetCoordsFromField(move.To);
                board[to.X, to.Y] = board[from.X, from.Y];
                board[from.X, from.Y] = null;
            }

            return board;
        }


        public string getNextPlayer()
        {
            if (Moves.Count == 0) return "white";
            return Moves[Moves.Count - 1].Color == "white" ? "black" : "white";
        }

        private string getAiColor() {
            return _userColor == "white" ? "black" : "white";
        }

        private string getMovesAsString()
        {
            if (Moves.Count <= 0) return "";
            return string.Join(";", Moves.Select(m => m.getCode()));
        }

        public Rules getRules()
        {
            return _rules;
        }

        public bool isUserNext()
        {
            return getNextPlayer() == _userColor;
        }

        public bool isGameOver()
        {
            return _isGameOver;
        }

        public void setGameOver(bool gameOver)
        {
            _isGameOver = gameOver;
        }

        public void addDeadPiece(string pieceName, bool badForUser)
        {
            if (badForUser)
            {
                UserPiecesDead.Add(pieceName);
            }
            else AiPiecesDead.Add(pieceName);
            int userScore = AiPiecesDead.Sum(p => getPieceValue(p));
            int aiScore = UserPiecesDead.Sum(p => getPieceValue(p));
            string userScoreUi = (userScore > aiScore ? "+" + (userScore - aiScore) : "");
            string aiScoreUi = (aiScore > userScore ? "+" + (aiScore - userScore) : "");
        }

        private int getPieceValue(string pieceName)
        {
            if (pieceName.Contains("pawn")) return 1;
            if (pieceName.Contains("knight")) return 3;
            if (pieceName.Contains("bishop")) return 3;
            if (pieceName.Contains("rook")) return 5;
            if (pieceName.Contains("queen")) return 9;
            return 0;
        }
    }

    public enum Difficulty
    {
        EASY,
        NORMAL,
        HARD
    }

    public class ChessMove
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Color { get; set; }

        public ChessMove(string from, string to)
        {
            From = from;
            To = to;
        }

        public ChessMove(string code)
        {
            From = code.Substring(0, 2);
            To = code.Substring(2, 2);
        }

        public string getCode()
        {
            return From + To;
        }
    }
}
