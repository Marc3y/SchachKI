namespace SchachKI
{
    partial class MainWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            chessBoard = new PictureBox();
            moveList = new FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)chessBoard).BeginInit();
            SuspendLayout();
            // 
            // chessBoard
            // 
            chessBoard.BackgroundImage = Properties.Resources.background;
            chessBoard.Location = new Point(38, 36);
            chessBoard.Name = "chessBoard";
            chessBoard.Size = new Size(649, 649);
            chessBoard.TabIndex = 1;
            chessBoard.TabStop = false;
            // 
            // moveList
            // 
            moveList.AutoScroll = true;
            moveList.BorderStyle = BorderStyle.FixedSingle;
            moveList.FlowDirection = FlowDirection.TopDown;
            moveList.Location = new Point(693, 36);
            moveList.Name = "moveList";
            moveList.Padding = new Padding(0, 0, 10, 0);
            moveList.Size = new Size(365, 649);
            moveList.TabIndex = 4;
            moveList.WrapContents = false;
            moveList.Paint += moveList_Paint;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(33, 33, 33);
            ClientSize = new Size(1091, 724);
            Controls.Add(moveList);
            Controls.Add(chessBoard);
            Name = "MainWindow";
            Text = "Schach gegen KI - GFS Marcus Kohr";
            Load += MainWindow_Load;
            ((System.ComponentModel.ISupportInitialize)chessBoard).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private PictureBox chessBoard;
        private FlowLayoutPanel moveList;
    }
}
