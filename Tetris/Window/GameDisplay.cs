using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tetris.Window
{
    public class GameDisplay : Panel
    {
        Pen WhitePen = new Pen(Color.White);

        public TetrisGame Game { get; set; }

        public GameDisplay()
        {
            DoubleBuffered = true;

            this.Paint += OnReDraw;
        }

        public void OnReDraw(object sender, PaintEventArgs e)
        {
            if (Game == null) { return; }

            int screenW = TetrisGame.GAME_WINDOW_SIZE_X;
            int screenH = TetrisGame.GAME_WINDOW_SIZE_Y;
            int cellsX = TetrisGame.GAME_GRID_SIZE_X;
            int cellsY = TetrisGame.GAME_GRID_SIZE_Y;
            int cellSize = Game.GAME_CELL_PIXEL_SIZE;
            int fieldX = cellsX * cellSize;
            int fieldY = cellsY * cellSize;

            BackColor = Color.Black;

            Point fieldPosition = new Point(
                (screenW / 2) - (fieldX / 2),
                (screenH / 2) - (fieldY / 2)
            );

            for (int x = 0; x < cellsX; x++)
            {
                for (int y = 0; y < cellsY; y++)
                {
                    int cX = (x * cellSize) + fieldPosition.X;
                    int cY = (y * cellSize) + fieldPosition.Y;

                    Rectangle cRect = new Rectangle(
                        cX, cY, cellSize, cellSize
                    );

                    if (Game.Field.Field[x][y] != 0)
                    {
                        Brush cellBrush = new SolidBrush(TetrisGame.TETROMINO_COLORS[Game.Field.Field[x][y]]);
                        e.Graphics.FillRectangle(cellBrush, cRect);
                        cellBrush.Dispose();
                    }
                }
            }

            Rectangle fieldOutLine = new Rectangle(
                fieldPosition.X,
                fieldPosition.Y,
                fieldX,
                fieldY
            );

            e.Graphics.DrawRectangle(WhitePen, fieldOutLine);
        }
    }
}
