using System;
using System.Windows.Forms;
using Tetris.Game;
using System.Drawing;
using System.Security.Policy;

namespace Tetris
{
    public class TetrisGame
    {
        //Static stuff:

        public static readonly int GAME_GRID_SIZE_X = 10;
        public static readonly int GAME_GRID_SIZE_Y = 20;

        public static readonly int GAME_WINDOW_SIZE_X = 800;
        public static readonly int GAME_WINDOW_SIZE_Y = 600;

        public static readonly int GAME_FRAMERATE_MILLIS = 1000 / 60;

        public static readonly int GAME_FIELD_PIXEL_MARGIN = 20;

        public static readonly Color[] TETROMINO_COLORS = new Color[]
        {
            /*None  */ Color.White,
            /*T     */ Color.Purple,
            /*S     */ Color.Green,
            /*Z     */ Color.Red,
            /*Square*/ Color.Yellow,
            /*Long  */ Color.LightBlue,
            /*L     */ Color.DarkBlue,
            /*J     */ Color.Orange
        };



        public int GAME_CELL_PIXEL_SIZE { get; private set; } = 10;

        public GameField Field { get; private set; }

        public GameWnd GameWnd { get; private set; }

        public Timer FrameTick { get; private set; }

        public int FrameTimer { get; private set; }

        public bool Running { get; private set; }

        public BlockField CurrentBlockField { get; private set; }

        public TetrisGame(GameWnd wnd)
        {
            GameWnd = wnd;
            Field = new GameField();

            RecalculateCellPixelSize();

            Running = true;

            CurrentBlockField = BlockFieldConstructor.CreateTetromino(0);
            CurrentBlockField.Position = new Point(3, 0);
            Field.SpawnBlockField(CurrentBlockField);

            FrameTick = new Timer();
            FrameTick.Tick += new EventHandler(Update);
            FrameTick.Interval = GAME_FRAMERATE_MILLIS; // in miliseconds
            FrameTick.Start();
        }

        private void RecalculateCellPixelSize()
        {
            float fieldRatio =
                (float)GAME_GRID_SIZE_X / (float)GAME_GRID_SIZE_Y;
            float screenRatio =
                (float)GAME_WINDOW_SIZE_X / (float)GAME_WINDOW_SIZE_Y;

            if (screenRatio > fieldRatio) //screen is wider than field
            {
                GAME_CELL_PIXEL_SIZE =
                    (GAME_WINDOW_SIZE_Y - GAME_FIELD_PIXEL_MARGIN) / GAME_GRID_SIZE_Y;
            } else
            {
                GAME_CELL_PIXEL_SIZE =
                    (GAME_WINDOW_SIZE_X - GAME_FIELD_PIXEL_MARGIN) / GAME_GRID_SIZE_X;
            }
        }

        private void Update(object sender, EventArgs e)
        {
            GameWnd.Display.Invalidate();
            FrameTimer++;

            if (Running)
            {
                UpdateGameState();
            }
        }

        private void UpdateGameState()
        {
            if (FrameTimer % 10 == 0)
            {
                DoPieceMove();
            }
        }

        private void DoPieceMove()
        {
            if (!Field.MoveBlocks(CurrentBlockField, 0, 1))
            {
                Field.CheckClearedLines();
                SwitchToNewPiece();
            }
        }

        private void SwitchToNewPiece()
        {
            CurrentBlockField = BlockFieldConstructor.CreateTetromino(0);
            CurrentBlockField.Position = new Point(4, 0);

            if (!Field.SpawnBlockField(CurrentBlockField))
            {
                Running = false;
            }
        }

        public void KeyPressed(Keys key)
        {
            switch(key)
            {
                case Keys.Left:
                    Field.MoveBlocks(CurrentBlockField, -1, 0);
                    break;
                case Keys.Right:
                    Field.MoveBlocks(CurrentBlockField, 1, 0);
                    break;
                case Keys.Up:
                    Field.RotateBlockField(CurrentBlockField);
                    break;
                case Keys.Space:
                    Field.DropBlocks(CurrentBlockField);
                    Field.CheckClearedLines();
                    SwitchToNewPiece();
                    break;
                case Keys.P:
                    Running = !Running;
                    break;
            }
        }
    }
}
