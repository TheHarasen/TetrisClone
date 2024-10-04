using System;
using System.Windows.Forms;
using Tetris.Game;
using System.Drawing;
using System.Security.Policy;

namespace Tetris
{
    /// <summary>
    /// Holds various globals and the internal state of the game of Tetris.
    /// </summary>
    public class TetrisGame
    {
        //Static stuff:

        /// <summary>
        /// The game field's width in cells.
        /// </summary>
        public static readonly int GAME_GRID_SIZE_X = 10;
        /// <summary>
        /// The game field's height in cells.
        /// </summary>
        public static readonly int GAME_GRID_SIZE_Y = 20;

        /// <summary>
        /// The game display's width in pixels.
        /// </summary>
        public static readonly int GAME_WINDOW_SIZE_X = 800;
        /// <summary>
        /// The game display's height in pixels.
        /// </summary>
        public static readonly int GAME_WINDOW_SIZE_Y = 600;

        /// <summary>
        /// The number of milliseconds inbetween each frame.
        /// </summary>
        public static readonly int GAME_FRAMERATE_MILLIS = 1000 / 60;

        /// <summary>
        /// The margin around the game field in pixels.
        /// </summary>
        public static readonly int GAME_FIELD_PIXEL_MARGIN = 20;

        /// <summary>
        /// Main colors of each possible value in a block.
        /// </summary>
        public static readonly Color[] TETROMINO_COLORS = new Color[]
        {
            /*None  */ Color.White,
            /*T     */ Color.DarkMagenta,
            /*S     */ Color.Green,
            /*Z     */ Color.Red,
            /*Square*/ Color.Yellow,
            /*Long  */ Color.LightBlue,
            /*L     */ Color.DarkBlue,
            /*J     */ Color.Orange
        };
        /// <summary>
        /// Outline colors of each possible value in a block.
        /// </summary>
        public static readonly Color[] TETROMINO_OUTLINE_COLORS = new Color[]
        {
            /*None  */ Color.White,
            /*T     */ Color.MediumPurple,
            /*S     */ Color.LightGreen,
            /*Z     */ Color.Pink,
            /*Square*/ Color.LightYellow,
            /*Long  */ Color.Azure,
            /*L     */ Color.Blue,
            /*J     */ Color.Yellow
        };
        /// <summary>
        /// Alpha value to be applied to the ghost piece color.
        /// </summary>
        public static readonly int TETROMINO_GHOST_ALPHA = 50;
        /// <summary>
        /// Ghost colors of each possible value in a block.
        /// </summary>
        public static readonly Color[] TETROMINO_GHOST_COLORS = new Color[]
        {
            /*None  */ Color.White,
            /*T     */ Color.FromArgb(TETROMINO_GHOST_ALPHA, TETROMINO_COLORS[1]),
            /*S     */ Color.FromArgb(TETROMINO_GHOST_ALPHA, TETROMINO_COLORS[2]),
            /*Z     */ Color.FromArgb(TETROMINO_GHOST_ALPHA, TETROMINO_COLORS[3]),
            /*Square*/ Color.FromArgb(TETROMINO_GHOST_ALPHA, TETROMINO_COLORS[4]),
            /*Long  */ Color.FromArgb(TETROMINO_GHOST_ALPHA, TETROMINO_COLORS[5]),
            /*L     */ Color.FromArgb(TETROMINO_GHOST_ALPHA, TETROMINO_COLORS[6]),
            /*J     */ Color.FromArgb(TETROMINO_GHOST_ALPHA, TETROMINO_COLORS[7])
        };


        /// <summary>
        /// Width/height of each cell in pixels.
        /// </summary>
        public int GAME_CELL_PIXEL_SIZE { get; private set; } = 10;

        /// <summary>
        /// Game field of this instance.
        /// </summary>
        public GameField Field { get; private set; }

        /// <summary>
        /// Window to render to.
        /// </summary>
        public GameWnd GameWnd { get; private set; }

        /// <summary>
        /// Timer that ticks every frame.
        /// </summary>
        public Timer FrameTimer { get; private set; }

        /// <summary>
        /// Counter that increments every frame.
        /// </summary>
        public int FrameCounter { get; private set; }

        /// <summary>
        /// Whether or not to do game logic every frame.
        /// </summary>
        public bool Running { get; private set; }

        /// <summary>
        /// Current block field (tetromino) that moves around and can be controlled.
        /// </summary>
        public BlockField CurrentPiece { get; private set; }
        public BlockField NextPiece { get; private set; }

        public TetrisGame(GameWnd wnd)
        {
            GameWnd = wnd;
            Field = new GameField();

            RecalculateCellPixelSize();

            Running = true;

            //Initialize piece
            NextPiece = BlockFieldConstructor.CreateTetromino();
            SwitchToNewPiece();

            //Start timer
            FrameTimer = new Timer();
            FrameTimer.Tick += new EventHandler(Update);
            FrameTimer.Interval = GAME_FRAMERATE_MILLIS; // in miliseconds
            FrameTimer.Start();
        }

        /// <summary>
        /// Calculate the size of each cell in pixels based on the field/window size and margin.
        /// </summary>
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
            } else //field is wider than screen
            {
                GAME_CELL_PIXEL_SIZE =
                    (GAME_WINDOW_SIZE_X - GAME_FIELD_PIXEL_MARGIN) / GAME_GRID_SIZE_X;
            }
        }

        /// <summary>
        /// Update the current frame and run game logic.
        /// </summary>
        private void Update(object sender, EventArgs e)
        {
            //Draw frame and increment frame counter
            GameWnd.Display.Invalidate();
            FrameCounter++;

            if (Running)
            {
                UpdateGameState();
            }
        }

        /// <summary>
        /// Update the state of the game.
        /// </summary>
        private void UpdateGameState()
        {
            if (FrameCounter % 10 == 0)
            {
                DoPieceFall();
            }
        }

        /// <summary>
        /// Make the current piece fall down one tile.
        /// </summary>
        private void DoPieceFall()
        {
            if (!Field.MoveBlocks(CurrentPiece, 0, 1))
            {
                Field.CheckClearedLines();
                SwitchToNewPiece();
            }
        }

        /// <summary>
        /// Discard the current piece and create a new one that gets put on top of the field.
        /// </summary>
        private void SwitchToNewPiece()
        {
            //Create a random tetromino to spawn on the field
            CurrentPiece = NextPiece;
            CurrentPiece.Position = new Point(
                (Field.FieldWidth / 2) - (CurrentPiece.GetWidth() / 2),
                0
            );

            NextPiece = BlockFieldConstructor.CreateTetromino();

            //Stop game if the piece couldn't spawn
            if (!Field.SpawnBlockField(CurrentPiece))
            {
                Running = false;
            }
        }

        /// <summary>
        /// Clear board and start game from the beginning.
        /// </summary>
        private void ResetGame()
        {
            Field.Clear();
            SwitchToNewPiece();
        }

        /// <summary>
        /// Called when a key is pressed.
        /// </summary>
        /// <param name="key">Key that was pressed.</param>
        public void KeyPressed(Keys key)
        {
            switch(key)
            {
                case Keys.Left:
                    Field.MoveBlocks(CurrentPiece, -1, 0);
                    break;
                case Keys.Right:
                    Field.MoveBlocks(CurrentPiece, 1, 0);
                    break;
                case Keys.Up:
                    Field.RotateBlockField(CurrentPiece);
                    break;
                case Keys.Down:
                    DoPieceFall();
                    break;
                case Keys.Space:
                    Field.DropBlocks(CurrentPiece);
                    Field.CheckClearedLines();
                    SwitchToNewPiece();
                    break;
                case Keys.P:
                    Running = !Running;
                    break;
                case Keys.R:
                    Running = true;
                    ResetGame();
                    break;
            }
        }
    }
}
