using SchachKI.src.game;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SchachKI.src.ui
{
    public class BoardRenderer
    {
        private const int FIELD_SIZE = 81;
        private Control _window;
        private PictureBox _board;
        private PieceData[] _startPositions;
        private Game _game;
        private PictureBox[,] pieces = new PictureBox[8, 8];

        private PictureBox _draggedPiece;
        private Point _dragStartPosition;
        private PictureBox _highlightBox;
        private PictureBox _lastFromHighlight;
        private PictureBox _lastMovedPiece;
        private FlowLayoutPanel _moveUiList;

        public BoardRenderer(Control window, PictureBox boardImage, FlowLayoutPanel moveUiList, Game game)
        {
            _moveUiList = moveUiList;
            _game = game;
            _window = window;
            _board = boardImage;
            string positionsJson = Encoding.Default.GetString(Properties.Resources.start_positions);
            _startPositions = JsonSerializer.Deserialize<PieceData[]>(positionsJson);

            _highlightBox = new PictureBox();
            _highlightBox.Size = new Size(FIELD_SIZE, FIELD_SIZE);
            _highlightBox.BackColor = Color.FromArgb(80, Color.LightGray);
            _highlightBox.Visible = false;
            _highlightBox.Parent = _board;
            _highlightBox.BringToFront();

            _lastFromHighlight = new PictureBox();
            _lastFromHighlight.Size = new Size(FIELD_SIZE, FIELD_SIZE);
            _lastFromHighlight.BackColor = Color.FromArgb(50, Color.Yellow);
            _lastFromHighlight.Visible = false;
            _lastFromHighlight.Parent = _board;
            _lastFromHighlight.BringToFront();

            _window.AllowDrop = true;
            _window.DragOver += DragOver;
            _window.DragDrop += DragDrop;
        }

        public PictureBox[,] GetPieces()
        {
            return pieces;
        }

        public void SetDefaultPositions()
        {
            ClearBoard();
            foreach(PieceData piece in _startPositions)
            {
                if (piece == null) continue;
                Place(Image.FromFile("Resources/img/icons/" + piece.Image + ".png"), piece.Field, piece.Image.Contains("white") ? 
                    "white" : "black", piece.Image);
            }
        }

        private void ClearBoard()
        {
            if (pieces.Length <= 0) return;
            foreach (PictureBox piece in pieces)
            {
                if (piece == null) continue;
                _window.Controls.Remove(piece);
            }
            pieces = new PictureBox[8, 8];
        }

        private void RemovePiece(Point loc)
        {
            PictureBox piece = pieces[loc.X, loc.Y];
            pieces[loc.X, loc.Y] = null;
            if (piece == null) return;
            _board.Controls.Remove(piece);
        }

        public void Place(Image image, string field, string color, string pieceName)
        {
            int x = "abcdefgh".IndexOf(char.ToLower(field[0]));
            int y = 8 - Convert.ToInt32(field[1].ToString());
            Place(image, x, y, color, pieceName);
        }

        private void Place(Image image, int x, int y, string color, string pieceName)
        {
            Console.WriteLine("image size: " + image.Size);
            PictureBox piece = new PictureBox();
            piece.Image = image;
            piece.Size = new Size(FIELD_SIZE, FIELD_SIZE);
            piece.SizeMode = PictureBoxSizeMode.Zoom;
            piece.BackColor = Color.Transparent;
            piece.Tag = color;
            piece.Name = pieceName;

            piece.Location = new Point(
                x * FIELD_SIZE,
                y * FIELD_SIZE
            );

            piece.Parent = _board;
            _board.Controls.Add(piece);

            piece.MouseEnter += MouseEnter;
            piece.MouseLeave += MouseLeave;
            piece.MouseDown += MouseDown;


            pieces[x, y] = piece;
            piece.BringToFront();
        }

        private string PieceMove(string color, string startField, string endField)
        {
            Point startCoords = GetCoordsFromField(startField);
            Point endCoords = GetCoordsFromField(endField);
            string killedPiece = null;
            PictureBox piece = pieces[startCoords.X, startCoords.Y];
            //wenn piece null ist kann es probleme machen, sollte aber nicht passieren ig
            if (piece == null) return null;
            if (pieces[endCoords.X, endCoords.Y] != null)
            {
                killedPiece = pieces[endCoords.X, endCoords.Y].Name.Split("_")[0];
                Console.WriteLine("Entfernte Figur: " + pieces[endCoords.X, endCoords.Y].Name);
                RemovePiece(endCoords);
            }
            piece.Location = GetPointFromField(endField);
            pieces[endCoords.X, endCoords.Y] = piece;
            pieces[startCoords.X, startCoords.Y] = null;
            ShowLastMove(startCoords, piece);
            AddToUIList(color, startField + " -> " + endField, piece.Name);
            return killedPiece;
        }


        private async void UserMove(string startField, string endField)
        {
            if (_game.isGameOver()) return;
            string? killedPiece = PieceMove(_game.getUserColor(), startField, endField);
            if (_game.getRules().IsWin(_game.getUserColor()))
            {
                SetGameOver(startField, endField);
                return;
            }
            ChessMove aiMove = await _game.UserMove(startField, endField, killedPiece, GetCurrentSetup());
            if (_game.isGameOver()) return;
            string? aiKilledPiece = PieceMove(aiMove.Color, aiMove.From, aiMove.To);
            if (_game.getRules().IsWin(_game.getUserColor()))
            {
                SetGameOver(startField, endField);
                return;
            }
            if (aiKilledPiece != null)
            {
                _game.AiPiecesDead.Add(aiKilledPiece);
            }
        }

        private void SetGameOver(string startField, string endField)
        {
            _game.setGameOver(true);
            AddToUIList(_game.getUserColor(), $"{startField} -> {endField} (Gewonnen)", "king_" + _game.getUserColor());
        }

        private void AddToUIList(string color, string move, string type)
        {
            Panel panel = new Panel();
            panel.Height = 40;
            panel.Width = _moveUiList.Width - 20;
            panel.BackColor = Color.FromArgb(20, 0, 0, 0);
            panel.Margin = new Padding(0);

            Panel colorBox = new Panel();
            colorBox.BackColor = color == "white" ? Color.White : Color.Black;
            colorBox.Size = new Size(15, 15);
            colorBox.Location = new Point(10, (panel.Height - colorBox.Height) / 2);

            Label playerLabel = new Label();
            playerLabel.Text = color == _game.getUserColor() ? "Du" : "KI";
            playerLabel.Font = new Font("Segoe UI Black", 12, FontStyle.Regular);
            playerLabel.ForeColor = Color.White;
            playerLabel.Location = new Point(colorBox.Right + 5, (panel.Height - TextRenderer.MeasureText(playerLabel.Text, playerLabel.Font).Height) / 2);
            playerLabel.AutoSize = true;

            Label moveLabel = new Label();
            moveLabel.Text = move;
            moveLabel.ForeColor = Color.Yellow;
            moveLabel.Font = new Font("Segoe UI Black", 14, FontStyle.Bold);
            moveLabel.Location = new Point((panel.Width - moveLabel.PreferredWidth) / 2, (panel.Height - moveLabel.PreferredHeight) / 2);
            moveLabel.AutoSize = true;

            PictureBox imageBox = new PictureBox();
            imageBox.Image = Image.FromFile("Resources/img/icons/" + type + ".png");
            imageBox.Size = new Size(25, 25);
            imageBox.SizeMode = PictureBoxSizeMode.Zoom;
            imageBox.Location = new Point(panel.Width - imageBox.Width - 10, (panel.Height - imageBox.Height) / 2);

            panel.Controls.Add(colorBox);
            panel.Controls.Add(playerLabel);
            panel.Controls.Add(moveLabel);
            panel.Controls.Add(imageBox);

            _moveUiList.Controls.Add(panel);
            _moveUiList.ScrollControlIntoView(panel);
        }

        private void ShowLastMove(Point startCoords, PictureBox lastMovedPiece)
        {
            if(_lastMovedPiece != null) 
            {
                _lastMovedPiece.BackColor = Color.Transparent;
            }
            _lastFromHighlight.Location = new Point(startCoords.X * FIELD_SIZE, startCoords.Y * FIELD_SIZE);
            _lastMovedPiece = lastMovedPiece;
            _lastMovedPiece.BackColor = Color.FromArgb(70, Color.Yellow);
            _lastFromHighlight.Visible = true;
        }

        private void DragOver(object? sender, DragEventArgs e)
        {
            if (_game.isGameOver()) return;
            if (!_game.isUserNext()) return;
            if (_draggedPiece == null)
            {
                e.Effect = DragDropEffects.None;
                return;
            }
            Point cursorPoint = _board.PointToClient(new Point(e.X, e.Y));
            int x = cursorPoint.X / FIELD_SIZE;
            int y = cursorPoint.Y / FIELD_SIZE;

            //wenn außerhalb des Feldes, abbrechen
            if(y > 7 || y < 0 || x > 7 || x < 0)
            {
                e.Effect = DragDropEffects.None;
                return;
            }
            string startField = (string) e.Data.GetData(typeof(string));
            string endField = GetFieldFromCoords(x, y);
            if (_game.getRules().isMoveOkay(startField + endField, _game.getUserColor()))
            {
                SetHighlight(x, y);
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                HideHighlight();
                e.Effect = DragDropEffects.None;
            }
        }

        private void DragDrop(object? sender, DragEventArgs e)
        {
            if (_game.isGameOver()) return;
            HideHighlight();
            if (!_game.isUserNext()) return;
            if (_draggedPiece == null)
            {
                e.Effect = DragDropEffects.None;
                return;
            }
            Point cursorPoint = _board.PointToClient(new Point(e.X, e.Y));
            int x = cursorPoint.X / FIELD_SIZE;
            int y = cursorPoint.Y / FIELD_SIZE;

            //wenn außerhalb des Feldes, abbrechen
            if (y > 7 || y < 0 || x > 7 || x < 0)
            {
                e.Effect = DragDropEffects.None;
                return;
            }
            string startField = (string)e.Data.GetData(typeof(string));
            string endField = GetFieldFromCoords(x, y);
            if (_game.getRules().isMoveOkay(startField + endField, _game.getUserColor()) && startField != endField)
            {
                UserMove(startField, endField);
            }
            else
            {
                _draggedPiece.Location = _dragStartPosition;
                Console.WriteLine("Zug nicht erlaubt: " + startField + endField);
            }
            _draggedPiece = null;
        }

        private void MouseDown(object? sender, MouseEventArgs e)
        {
            if (_game.isGameOver()) return;
            PictureBox piece = (PictureBox)sender;
            if (piece.Tag != _game.getUserColor()) return;
            _draggedPiece = piece;
            _dragStartPosition = _draggedPiece.Location;
            string startField = GetFieldFromLocation(_dragStartPosition);

            _draggedPiece.DoDragDrop(startField, DragDropEffects.Move);
        }

        private void MouseEnter(object? sender, EventArgs e)
        {
            if (_game.isGameOver()) return;
            PictureBox piece = (PictureBox)sender;
            if (piece.Tag != _game.getUserColor()) return;
            
            piece.BackColor = Color.FromArgb(_game.isUserNext() ? 100 : 50, Color.Gray);
        }

        private void MouseLeave(object? sender, EventArgs e)
        {
            PictureBox piece = (PictureBox)sender;
            if (piece.Tag != _game.getUserColor()) return;

            piece.BackColor = Color.Transparent;
        }

        private void SetHighlight(int x, int y)
        {
            _highlightBox.Image = _draggedPiece.Image;
            _highlightBox.Location = new Point(x * FIELD_SIZE, y * FIELD_SIZE);
            _highlightBox.Visible = true;
        }

        private void HideHighlight()
        {
            _highlightBox.Visible = false;
        }

        private string GetFieldFromLocation(Point loc)
        {
            int x = loc.X / FIELD_SIZE;
            int y = loc.Y / FIELD_SIZE;

            char[] columns = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
            char letter = columns[x];

            int num = 8 - y;
            Console.WriteLine($"loc:{letter}{num}");
            return $"{letter}{num}";
        }

        public static string GetFieldFromCoords(int x, int y)
        {
            char[] columns = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
            char letter = columns[x];
            int num = 8 - y;
            return $"{letter}{num}";
        }

        public static Point GetCoordsFromField(string field)
        {
            char letter = field[0];
            int num = int.Parse(field[1].ToString());

            int x = "abcdefgh".IndexOf(letter);
            int y = 8 - num;
            return new Point(x, y);
        }

        private Point GetPointFromField(string field)
        {
            Point coords = GetCoordsFromField(field);
            return new Point(coords.X * FIELD_SIZE, coords.Y * FIELD_SIZE);
        }
        public string GetCurrentSetup() 
        {
            StringBuilder fen = new StringBuilder();
            for(int i = 7; i >= 0; i--)
            {
                int empty = 0;
                for(int b = 0; b < 8; b++)
                {
                    PictureBox piece = pieces[b, i];
                    if (piece != null)
                    {
                        if (empty > 0) fen.Append(empty);
                        empty = 0;
                        fen.Append(GetFenChar(piece.Name));
                    }
                    else empty++;
                }
                if (empty > 0) fen.Append(empty);
                if (i > 0) fen.Append('/');
            }
            return fen.ToString();
        }

        private char GetFenChar(string name)
        {
            string[] parts = name.Split('_');
            if (parts.Length < 2) return '?';
            string piece = parts[0];
            string color = parts[1];
            char symbol = piece switch
            {
                "pawn" => 'p',
                "rook" => 'r',
                "knight" => 'n',
                "bishop" => 'b',
                "queen" => 'q',
                "king" => 'k',
                _ => '-'
            };
            return color == "white" ? char.ToUpper(symbol) : symbol;
        }
    }

    public class PieceData
    {
        [JsonPropertyName("image")]
        public string Image { get; set; }

        [JsonPropertyName("field")]
        public string Field { get; set; }
    }
}
