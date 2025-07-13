using SchachKI.src.ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchachKI.src.game
{
    public class Rules
    {
        private Game _game;
        
        public Rules(Game game)
        {
            _game = game;
        }


        public bool isMoveOkay(string move, string color)
        {
            List<string> legalMoves = GetLegalMoves(color);
            bool okay = legalMoves.Contains(move);
            Console.WriteLine("checking move: " + move + " its" + (okay ? " legal" : "illegal"));
            Console.WriteLine("Allowed: " + string.Join(", ", legalMoves));
            return okay;
        }

        public List<string> GetLegalMoves(string color)
        {
            string[,] board = _game.GetBoardArray();
            List<string> moves = new List<string>();

            for(int x = 0; x < 8; x++)
            {
                for(int y = 0; y < 8; y++)
                {
                    string piece = board[x, y];
                    if (piece == null || !piece.EndsWith(color)) continue;
                    string type = piece.Split('_')[0];
                    Point from = new Point(x, y);

                    moves.AddRange(GetLegalMovesForPiece(type, from, board, color));
                }
            }

            return moves;
        }

        private List<string> GetLegalMovesForPiece(string type, Point from, string[,] board, string color)
        {
            List<string> moves = new List<string>();

            for (int tx = 0; tx < 8; tx++)
            {
                for (int ty = 0; ty < 8; ty++)
                {
                    Point to = new Point(tx, ty);
                    if (from == to) continue;

                    if (!isLegalMove(type, from, to, board, color)) continue;

                    if (IsKingInDanger(from, to, board, color)) continue;

                    moves.Add(BoardRenderer.GetFieldFromCoords(from.X, from.Y) + BoardRenderer.GetFieldFromCoords(to.X, to.Y));
                }
            }

            return moves;
        }

        private bool IsKingInDanger(Point from, Point to, string[,] board, string color)
        {
            string[,] sim = (string[,])board.Clone();
            sim[to.X, to.Y] = sim[from.X, from.Y];
            sim[from.X, from.Y] = null;

            return IsKingInCheck(sim, color);
        }

        private bool isLegalMove(string type, Point from, Point to, string[,] board, string color)
        {
            switch (type)
            {
                case "pawn":
                    return isLegalPawnMove(from, to, board, color);
                case "rook":
                    return isLegalRookMove(from, to, board, color);
                case "bishop":
                    return isLegalBishopMove(from, to, board, color);
                case "queen":
                    return isLegalQueenMove(from, to, board, color);
                case "king":
                    return isLegalKingMove(from, to, board, color);
                case "knight":
                    return isLegalKnightMove(from, to, board, color);
                default:
                    return false;
            }
        }

        public bool IsWin(string color)
        {
            string[,] board = _game.GetBoardArray();
            string opponentKing = "king_" + OpponentColor(color);

            foreach (string piece in board)
            {
                if (piece == opponentKing) return false;
            }

            return true;
        }

        private bool isLegalPawnMove(Point from, Point to, string[,] board, string color)
        {
            if (board[to.X, to.Y]?.EndsWith(color) == true) return false;
            int dx = to.X - from.X; // bewegung in x richtung
            int dy = to.Y - from.Y; // bewegung in y richtung
            int absDx = Math.Abs(dx); // horizontaler abstand
            int absDy = Math.Abs(dy); // vertikaler abstand
            int dirY = color == "white" ? -1 : 1;

            //vorraussetzung: x richtung nicht geändert, y eins nach vorne, keine andere figur im weg
            if (dx == 0 && dy == dirY && board[to.X, to.Y] == null) return true;
            //vorraussetzung: x richtung nicht geändert, y zwei nach vorne, an startposition, keine andere figur im weg
            if (dx == 0 && dy == 2 * dirY && from.Y == (color == "white" ? 6 : 1) && board[to.X, to.Y] == null && board[from.X, from.Y + dirY] == null) return true;
            //vorraussetzung: x richtung geändert, y eins nach vorne, gegnerische figur geschlagen
            if (absDx == 1 && dy == dirY && board[to.X, to.Y]?.EndsWith(OpponentColor(color)) == true) return true;
            return false;
        }

        private bool isLegalRookMove(Point from, Point to, string[,] board, string color)
        {
            if (board[to.X, to.Y]?.EndsWith(color) == true) return false;
            int dx = to.X - from.X; // bewegung in x richtung
            int dy = to.Y - from.Y; // bewegung in y richtung
            int absDx = Math.Abs(dx); // horizontaler abstand
            int absDy = Math.Abs(dy); // vertikaler abstand
            int dirY = color == "white" ? -1 : 1; 

            //wenn bewegung in beide richtungen illegal
            if (dx != 0 && dy != 0) return false;
            //vorraussetzung: keine figuren im weg
            return isPathClear(from, to, board);
        }

        private bool isLegalBishopMove(Point from, Point to, string[,] board, string color)
        {
            if (board[to.X, to.Y]?.EndsWith(color) == true) return false;
            int dx = to.X - from.X; // bewegung in x richtung
            int dy = to.Y - from.Y; // bewegung in y richtung
            int absDx = Math.Abs(dx); // horizontaler abstand
            int absDy = Math.Abs(dy); // vertikaler abstand

            //wenn nicht diagonal dann illegal
            if (absDx != absDy) return false;
            //vorraussetzung: keine figuren im weg
            return isPathClear(from, to, board);
        }

        private bool isLegalQueenMove(Point from, Point to, string[,] board, string color)
        {
            if (board[to.X, to.Y]?.EndsWith(color) == true) return false;
            int dx = to.X - from.X; // bewegung in x richtung
            int dy = to.Y - from.Y; // bewegung in y richtung
            int absDx = Math.Abs(dx); // horizontaler abstand
            int absDy = Math.Abs(dy); // vertikaler abstand

            //wenn diagonal oder vertikal oder horizontal + weg frei dann legal
            if (absDx == absDy || dx == 0 || dy == 0) return isPathClear(from, to, board);
            return false;
        }

        private bool isLegalKingMove(Point from, Point to, string[,] board, string color)
        {
            if (board[to.X, to.Y]?.EndsWith(color) == true) return false;
            int dx = to.X - from.X; // bewegung in x richtung
            int dy = to.Y - from.Y; // bewegung in y richtung
            int absDx = Math.Abs(dx); // horizontaler abstand
            int absDy = Math.Abs(dy); // vertikaler abstand

            //wenn distanz 1 oder kleiner dann legal
            return absDx <= 1 && absDy <= 1;
        }

        private bool isLegalKnightMove(Point from, Point to, string[,] board, string color)
        {
            if (board[to.X, to.Y]?.EndsWith(color) == true) return false;
            int dx = to.X - from.X; // bewegung in x richtung
            int dy = to.Y - from.Y; // bewegung in y richtung
            int absDx = Math.Abs(dx); // horizontaler abstand
            int absDy = Math.Abs(dy); // vertikaler abstand

            //wenn bewegung wie ein L dann legal
            return (absDx == 2 && absDy == 1) || (absDx == 1 && absDy == 2);
        }


        private bool isPathClear(Point from, Point to, string[,] board)
        {
            int dx = Math.Sign(to.X - from.X); // richtungsvektor in x richtung (-1, 0 oder 1)
            int dy = Math.Sign(to.Y - from.Y); // richtungsvektor in y richtung (-1, 0 oder 1)
            int steps = Math.Max(Math.Abs(to.X - from.X), Math.Abs(to.Y - from.Y)); // anzahl zu überquerende felder

            //jedes feld wird überprüft (außer start und endpunkt) ob es leer ist
            for (int i = 1; i < steps; i++)
            {
                int x = from.X + dx * i;
                int y = from.Y + dy * i;
                if (board[x, y] != null) return false;
            }
            return true;
        }

        private bool IsKingInCheck(string[,] board, string color)
        {
            Point kingPos = FindKing(board, color);
            string enemyColor = color == "white" ? "black" : "white";
            // checken ob eine gegnerische figur könig bedroht
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    string piece = board[x, y];
                    if (piece != null && piece.EndsWith(enemyColor))
                    {
                        string type = piece.Split('_')[0];
                        if (isLegalMove(type, new Point(x, y), kingPos, board, enemyColor))
                            return true;
                    }
                }
            }
            return false;
        }

        private static Point FindKing(string[,] board, string color)
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (board[x, y] == "king_" + color)
                        return new Point(x, y);
                }
            }
            return new Point(-1, -1); //king nicht gefunden
        }

        private string OpponentColor(string color) => color == "white" ? "black" : "white";
    }
}
