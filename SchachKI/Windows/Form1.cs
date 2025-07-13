using SchachKI.src.ai;
using SchachKI.src.game;
using SchachKI.src.ui;

namespace SchachKI
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            test();
        }

        public async void test()
        {
            Game game = new Game(Difficulty.HARD, "white");
            BoardRenderer boardRenderer = new BoardRenderer(this, chessBoard, moveList, game);
            boardRenderer.SetDefaultPositions();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void moveList_Paint(object sender, PaintEventArgs e)
        {
            
        }
    }

}
