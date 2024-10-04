using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tetris.Window
{
    /// <summary>
    /// Displays the state of the game.
    /// </summary>
    public class GameDisplay : Panel
    {
        Pen WhitePen = new Pen(Color.White);

        public TetrisGame Game { get; set; }

        public GameDisplay()
        {
            DoubleBuffered = true;

            this.Paint += OnReDraw;
        }

        /// <summary>
        /// Runs when the panel is redrawn (when the Invalidate method is called on it).
        /// </summary>
        public void OnReDraw(object sender, PaintEventArgs e)
        {
            if (Game == null) { return; }

            //Get various size properties
            int screenW = TetrisGame.GAME_WINDOW_SIZE_X;
            int screenH = TetrisGame.GAME_WINDOW_SIZE_Y;
            int cellsW = TetrisGame.GAME_GRID_SIZE_X;
            int cellsH = TetrisGame.GAME_GRID_SIZE_Y;
            int cellSize = Game.GAME_CELL_PIXEL_SIZE;
            int fieldW = cellsW * cellSize;
            int fieldH = cellsH * cellSize;

            //Draw background
            BackColor = Color.Black;

            //Get top-left position of the game field
            Point fieldPos = new Point(
                (screenW / 2) - (fieldW / 2),
                (screenH / 2) - (fieldH / 2)
            );

            //Color each cell of the game field depending on their state
            for (int x = 0; x < cellsW; x++)
            {
                for (int y = 0; y < cellsH; y++)
                {
                    int cX = (x * cellSize) + fieldPos.X;
                    int cY = (y * cellSize) + fieldPos.Y;

                    Rectangle cRect = new Rectangle(
                        cX, cY, cellSize, cellSize
                    );

                    if (Game.Field.Field[x][y] != 0)
                    {
                        //Create a brush with the appropriate color
                        Brush cellBrush = new SolidBrush(TetrisGame.TETROMINO_COLORS[Game.Field.Field[x][y]]);
                        e.Graphics.FillRectangle(cellBrush, cRect);
                        cellBrush.Dispose();
                    }
                }
            }

            //Draw an outline of the field
            Rectangle fieldOutLine = new Rectangle(
                fieldPos.X,
                fieldPos.Y,
                fieldW,
                fieldH
            );

            e.Graphics.DrawRectangle(WhitePen, fieldOutLine);
        }
    }
}
