using System;
using System.Drawing;
using System.Windows.Forms;
using Tetris.Game;

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
            /*T     */ Color.MediumPurple,
            /*S     */ Color.LightGreen,
            /*Z     */ Color.Pink,
            /*Square*/ Color.LightYellow,
            /*Long  */ Color.Azure,
            /*L     */ Color.Blue,
            /*J     */ Color.Yellow
            
        };

        /// <summary>
        /// Outline colors of each possible value in a block.
        /// </summary>
        public static readonly Color[] TETROMINO_ALT_COLORS = new Color[]
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
        /// Amount of lines needed per level before the level increases.
        /// </summary>
        public static readonly int LEVELUP_LINES_NEEDED = 10;

        /// <summary>
        /// The drop interval the game will start with at level 1.
        /// </summary>
        public static readonly int STARTING_DROP_INTERVAL = 25;

        /// <summary>
        /// Table of base point amounts for a single, double, triple and tetris respectively.
        /// </summary>
        public static readonly int[] SCORETABLE = new int[]
        {
            100, /*Single*/
            300, /*Double*/
            500, /*Triple*/
            800  /*Tetris*/
        };

        // Variables:

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
        public bool Running { get; private set; } = false;

        /// <summary>
        /// Current block field (tetromino) that moves around and can be controlled.
        /// </summary>
        public BlockField CurrentPiece { get; private set; }

        /// <summary>
        /// Next block field (tetromino) that will be used when the current piece has been dropped.
        /// </summary>
        public BlockField NextPiece { get; private set; }

        /// <summary>
        /// Piece that the player has stashed for later.
        /// </summary>
        public BlockField StashedPiece { get; private set; }

        /// <summary>
        /// Allow the player to stash the current piece
        /// </summary>
        public bool AllowStash { get; private set; }

        /// <summary>
        /// The score count in the current game.
        /// </summary>
        public int Score { get; private set; } = 0;

        /// <summary>
        /// Number of frames between a piece falling.
        /// </summary>
        public int DropInterval { get; private set; }

        /// <summary>
        /// Number of frames since last drop.
        /// </summary>
        public int DropTimer { get; private set; }

        /// <summary>
        /// The level count in the current game.
        /// </summary>
        public int Level { get; private set; }

        /// <summary>
        /// Number of lines cleared.
        /// </summary>
        public int LinesCleared { get; private set; }

        /// <summary>
        /// Number of line clears scored consecutively
        /// </summary>
        public int ComboCount { get; private set; }

        /// <summary>
        /// Number of frames to withhold locking the piece in place when reaching the bottom.
        /// </summary>
        public int GraceTimer { get; private set; }

        /// <summary>
        /// Whether or not to do the game over logic
        /// </summary>
        public bool GameOver { get; private set; }

        public TetrisGame(GameWnd wnd)
        {
            GameWnd = wnd;
            Field = new GameField();

            RecalculateCellPixelSize();

            StartNewGame();

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
            DropTimer++;
            if (DropTimer >= DropInterval)
            {
                DropTimer = 0;
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
                HandleScore(Field.CheckClearedLines());
                SwitchToNewPiece();
            }
        }

        /// <summary>
        /// Leave the current piece in place and create a new one that gets put on top of the field.
        /// </summary>
        private void SwitchToNewPiece()
        {
            //Create a random tetromino to spawn on the field
            CurrentPiece = NextPiece;
            PlacePieceAtTop(CurrentPiece);

            AllowStash = true;

            //Get the next piece
            NextPiece = BlockFieldConstructor.CreateTetromino();

            //Stop game if the piece couldn't spawn
            if (!Field.SpawnBlockField(CurrentPiece))
            {
                GameOver = true;
                Running = false;
            }
        }

        /// <summary>
        /// Place the given piece at the top of the game field.
        /// </summary>
        /// <param name="piece"></param>
        private void PlacePieceAtTop(BlockField piece)
        {
            piece.Position = new Point(
                (Field.FieldWidth / 2) - (piece.GetWidth() / 2),
                0
            );
        }

        /// <summary>
        /// Clear board and start game from the beginning.
        /// </summary>
        private void StartNewGame()
        {
            //Initialize field
            Field.Clear();

            //Initialize variables
            Score = 0;
            Level = 1;
            LinesCleared = 0;
            ComboCount = 0;
            DropInterval = STARTING_DROP_INTERVAL;
            DropTimer = 0;
            NextPiece = BlockFieldConstructor.CreateTetromino();
            StashedPiece = null;
            AllowStash = true;

            //Start game
            SwitchToNewPiece();
            Running = true;
            GameOver = false;
        }

        /// <summary>
        /// Adjust score, level etc. according to the lines cleared.
        /// </summary>
        /// <param name="lines">Number of lines cleared.</param>
        private void HandleScore(int lines)
        {
            if (lines == 0)
            {
                ComboCount = 0;
                return;
            }

            LinesCleared += lines;

            Score += SCORETABLE[lines - 1] * Level;
            Score += ComboCount * 50 * Level;

            int newLevel = (LinesCleared / 10) + 1;
            if (newLevel > Level) { Level = newLevel; }

            DropInterval = Math.Max(STARTING_DROP_INTERVAL - (Level-1 * 5), 2);

            ComboCount++;
        }

        /// <summary>
        /// If no piece has been stashed yet, put the current piece in the stash and get the next piece.
        /// If a stashed piece is present, swap the current and stashed pieces.
        /// </summary>
        private void StashPiece()
        {
            if (!AllowStash) { return; }

            //Clear current piece before swapping
            Field.SetFieldOfBlocks(CurrentPiece, true);

            //Move to stash if no piece is stashed, else swap current and stashed.
            if (StashedPiece == null)
            {
                StashedPiece = CurrentPiece;
                CurrentPiece = NextPiece;
                NextPiece = BlockFieldConstructor.CreateTetromino();
            } else
            {
                //swap
                (StashedPiece, CurrentPiece) = (CurrentPiece, StashedPiece);
            }

            AllowStash = false;

            PlacePieceAtTop(CurrentPiece);
        }

        /// <summary>
        /// Called when a key is pressed.
        /// </summary>
        /// <param name="key">Key that was pressed.</param>
        public void KeyPressed(Keys key)
        {
            if (!Running)
            {
                if (GameOver)
                {
                    DoGameOverControls(key);
                } else
                {
                    DoPausedControls(key);
                }
            } else
            {
                DoGameControls(key);
            }
        }

        /// <summary>
        /// Do the controls for the main game loop.
        /// </summary>
        /// <param name="key">Input key.</param>
        public void DoGameControls(Keys key)
        {
            switch (key)
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
                    DropTimer = 0;
                    DoPieceFall();
                    break;
                case Keys.Space:
                    Field.DropBlocks(CurrentPiece);
                    HandleScore(Field.CheckClearedLines());
                    SwitchToNewPiece();
                    break;
                case Keys.P:
                    Running = false;
                    break;
                case Keys.C:
                    StashPiece();
                    break;
            }
        }

        /// <summary>
        /// Do the controls for the pause screen.
        /// </summary>
        /// <param name="key">Input key.</param>
        public void DoPausedControls(Keys key)
        {
            switch (key)
            {
                case Keys.P:
                    Running = true;
                    break;
                case Keys.R:
                    StartNewGame();
                    break;
            }
        }

        /// <summary>
        /// Do the controls for the game over screen.
        /// </summary>
        /// <param name="key">Input key.</param>
        public void DoGameOverControls(Keys key)
        {
            switch (key)
            {
                case Keys.R:
                    StartNewGame();
                    break;
            }
        }
    }
}
