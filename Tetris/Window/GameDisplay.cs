using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tetris.Game;

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

            BackColor = Color.Black;
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
            int fieldW = TetrisGame.GAME_GRID_SIZE_X * Game.GAME_CELL_PIXEL_SIZE;
            int fieldH = TetrisGame.GAME_GRID_SIZE_Y * Game.GAME_CELL_PIXEL_SIZE;

            //Get top-left position of the game field
            Point fieldPos = new Point(
                (screenW / 2) - (fieldW / 2),
                (screenH / 2) - (fieldH / 2)
            );

            DrawFieldCells(e.Graphics, fieldPos);

            DrawDropPieceGhost(e.Graphics, fieldPos);

            DrawFieldOutLine(e.Graphics, fieldPos);
        }

        private void DrawFieldCells(Graphics graphics, Point fieldPos)
        {
            int cellsW = TetrisGame.GAME_GRID_SIZE_X;
            int cellsH = TetrisGame.GAME_GRID_SIZE_Y;
            int cellSize = Game.GAME_CELL_PIXEL_SIZE;

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
                        int type = Game.Field.Field[x][y];

                        Brush cellBrush = new SolidBrush(TetrisGame.TETROMINO_COLORS[type]);
                        Pen cellPen = new Pen(TetrisGame.TETROMINO_OUTLINE_COLORS[type]);
                        graphics.FillRectangle(cellBrush, cRect);
                        graphics.DrawRectangle(cellPen, cRect);
                        cellBrush.Dispose();
                        cellPen.Dispose();
                    }
                }
            }
        }

        private void DrawDropPieceGhost(Graphics graphics, Point fieldPos)
        {
            int cellSize = Game.GAME_CELL_PIXEL_SIZE;

            BlockField ghost = Game.Field.GetDropGhost(Game.CurrentPiece);
            if (ghost != null)
            {
                int cX = ghost.Position.X;
                int cY = ghost.Position.Y;
                int cW = ghost.GetWidth();
                int cH = ghost.GetHeight();

                int type = (int)ghost.Shape + 1;

                Pen cellPen = new Pen(TetrisGame.TETROMINO_OUTLINE_COLORS[type]);
                Brush cellBrush = new SolidBrush(TetrisGame.TETROMINO_GHOST_COLORS[type]);

                for (int x = 0; x < cW; x++)
                {
                    for (int y = 0; y < cH; y++)
                    {
                        int val = ghost.Blocks[x][y];

                        //Don't waste time drawing an empty block
                        if (val == 0) { continue; }

                        Rectangle cRect = new Rectangle(
                            ((cX + x) * cellSize) + fieldPos.X,
                            ((cY + y) * cellSize) + fieldPos.Y,
                            cellSize,
                            cellSize
                        );

                        graphics.FillRectangle(cellBrush, cRect);
                        graphics.DrawRectangle(cellPen, cRect);
                    }
                }

                cellPen.Dispose();
                cellBrush.Dispose();
            }
        }

        private void DrawFieldOutLine(Graphics graphics, Point fieldPos)
        {
            //Draw an outline of the field
            Rectangle fieldOutLine = new Rectangle(
                fieldPos.X,
                fieldPos.Y,
                TetrisGame.GAME_GRID_SIZE_X * Game.GAME_CELL_PIXEL_SIZE,
                TetrisGame.GAME_GRID_SIZE_Y * Game.GAME_CELL_PIXEL_SIZE
            );

            graphics.DrawRectangle(WhitePen, fieldOutLine);
        }
        
        private void DrawNextPeice()
        {

        }
    }
}
