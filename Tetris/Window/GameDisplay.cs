using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tetris.Game;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Tetris.Window
{
    /// <summary>
    /// Displays the state of the game.
    /// </summary>
    public class GameDisplay : Panel
    {
        private readonly Pen WhitePen = new Pen(Color.White);
        private readonly Brush WhiteBrush = new SolidBrush(Color.White);
        private readonly Font textFont = new Font(FontFamily.GenericSansSerif, 20);

        private readonly Brush PausedOverlay = new SolidBrush(Color.FromArgb(150, Color.Black));

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

            //scratch values for text
            float textX;
            float textY;

            //Get top-left position of the game field
            Point fieldPos = new Point(
                (screenW / 2) - (fieldW / 2),
                (screenH / 2) - (fieldH / 2)
            );

            DrawFieldCells(e.Graphics, fieldPos);

            DrawDropPieceGhost(e.Graphics, fieldPos);

            if (!Game.Running)
            {
                Rectangle overlay = new Rectangle(fieldPos.X, fieldPos.Y, fieldW, fieldH);

                DrawNotRunningOverlay(e.Graphics, overlay);
            }

            DrawFieldOutLine(e.Graphics, fieldPos);

            Point nextPiecePos = new Point(
                fieldPos.X + fieldW + TetrisGame.GAME_FIELD_PIXEL_MARGIN,
                fieldPos.Y + 100
            );

            DrawNextPeice(e.Graphics, nextPiecePos);

            Point stashedPiecePos = new Point(
                fieldPos.X - (TetrisGame.GAME_FIELD_PIXEL_MARGIN * 3) - (Game.GAME_CELL_PIXEL_SIZE * 4),
                fieldPos.Y + 100
            );

            DrawStashedPiece(e.Graphics, stashedPiecePos);

            //Draw score
            textX = fieldPos.X + fieldW + TetrisGame.GAME_FIELD_PIXEL_MARGIN;
            textY = fieldPos.Y;

            e.Graphics.DrawString(string.Format("Score: {0}", Game.Score), textFont, WhiteBrush, textX, textY);

            textY += 40f;

            e.Graphics.DrawString(string.Format("Level: {0}", Game.Level), textFont, WhiteBrush, textX, textY);
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
                    Point cPos = new Point(
                        (x * cellSize) + fieldPos.X,
                        (y * cellSize) + fieldPos.Y
                    );

                    if (Game.Field.Field[x][y] != 0)
                    {
                        //Create a brush with the appropriate color
                        int type = Game.Field.Field[x][y];

                        Brush cellBrush = new SolidBrush(TetrisGame.TETROMINO_COLORS[type]);
                        Pen cellPen = new Pen(TetrisGame.TETROMINO_OUTLINE_COLORS[type]);
                        DrawCell(graphics, cPos, cellBrush, cellPen);
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

                        //Don't draw an empty block
                        if (val == 0) { continue; }

                        Point poxPos = new Point(
                            ((cX + x) * cellSize) + fieldPos.X,
                            ((cY + y) * cellSize) + fieldPos.Y
                        );

                        DrawCell(graphics, poxPos, cellBrush, cellPen);
                    }
                }

                cellPen.Dispose();
                cellBrush.Dispose();
            }
        }

        private void DrawNotRunningOverlay(Graphics graphics, Rectangle fieldRect)
        {
            graphics.FillRectangle(PausedOverlay, fieldRect);

            float textX;
            float textY;

            if (Game.GameOver)
            {
                textX = fieldRect.X + 70;
                textY = fieldRect.Y + (fieldRect.Height / 2) - 40;

                graphics.DrawString("Game Over!", textFont, WhiteBrush, textX, textY);

                textX -= 35;
                textY += 40;

                graphics.DrawString("Press R to Restart.", textFont, WhiteBrush, textX, textY);
            }
            else
            {
                textX = fieldRect.X + 90;
                textY = fieldRect.Y + (fieldRect.Height / 2) - 40;

                graphics.DrawString("Paused", textFont, WhiteBrush, textX, textY);

                textX -= 25;
                textY += 40;

                graphics.DrawString("R: Restart\nP: Unpause", textFont, WhiteBrush, textX, textY);
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
        
        private void DrawNextPeice(Graphics graphics, Point nextPiecePos)
        {
            if (Game.NextPiece == null) { return; }

            int margin = TetrisGame.GAME_FIELD_PIXEL_MARGIN;

            int nextPieceSize = (Game.GAME_CELL_PIXEL_SIZE * 4) + (2 * margin);

            //Draw outline
            Rectangle nextPieceOutLine = new Rectangle(
                nextPiecePos.X, nextPiecePos.Y, nextPieceSize, nextPieceSize
            );

            graphics.DrawRectangle(WhitePen, nextPieceOutLine);

            int pieceX = nextPiecePos.X +
                margin +
                ((4 - Game.NextPiece.GetWidth()) * Game.GAME_CELL_PIXEL_SIZE / 2);
            int pieceY = nextPiecePos.Y +
                margin +
                ((4 - Game.NextPiece.GetHeight()) * Game.GAME_CELL_PIXEL_SIZE / 2);

            int type = (int)Game.NextPiece.Shape + 1;

            Pen cellPen = new Pen(TetrisGame.TETROMINO_OUTLINE_COLORS[type]);
            Brush cellBrush = new SolidBrush(TetrisGame.TETROMINO_COLORS[type]);

            int[][] blocks = Game.NextPiece.Blocks;

            for (int x = 0; x < blocks.Length; x++)
            {
                for (int y = 0; y < blocks[x].Length; y++)
                {
                    if (blocks[x][y] == 0) { continue; }

                    Point cPos = new Point(
                        pieceX + (x * Game.GAME_CELL_PIXEL_SIZE),
                        pieceY + (y * Game.GAME_CELL_PIXEL_SIZE)
                    );

                    DrawCell(graphics, cPos, cellBrush, cellPen);
                }
            }

            cellPen.Dispose();
            cellBrush.Dispose();

            //Draw "Next" text
            float textX = nextPiecePos.X;
            float textY = nextPiecePos.Y + (margin * 2) +
                (Game.GAME_CELL_PIXEL_SIZE * 4);

            graphics.DrawString("Next", textFont, WhiteBrush, textX, textY);
        }

        private void DrawStashedPiece(Graphics graphics, Point stashedPiecePos)
        {
            int margin = TetrisGame.GAME_FIELD_PIXEL_MARGIN;

            int stashedPieceSize = (Game.GAME_CELL_PIXEL_SIZE * 4) + (2 * margin);

            //Draw outline
            Rectangle stashedPieceOutline = new Rectangle(
                stashedPiecePos.X, stashedPiecePos.Y, stashedPieceSize, stashedPieceSize
            );

            graphics.DrawRectangle(WhitePen, stashedPieceOutline);

            //Draw "Stashed" text
            float textX = stashedPiecePos.X;
            float textY = stashedPiecePos.Y + (margin * 2) +
                (Game.GAME_CELL_PIXEL_SIZE * 4);

            graphics.DrawString("Stashed", textFont, WhiteBrush, textX, textY);

            //Skip drawing the piece if it doesn't exist
            if (Game.StashedPiece == null) { return; }

            int pieceX = stashedPiecePos.X +
                margin +
                ((4 - Game.StashedPiece.GetWidth()) * Game.GAME_CELL_PIXEL_SIZE / 2);
            int pieceY = stashedPiecePos.Y +
                margin +
                ((4 - Game.StashedPiece.GetHeight()) * Game.GAME_CELL_PIXEL_SIZE / 2);

            int type = (int)Game.StashedPiece.Shape + 1;

            Pen cellPen = new Pen(TetrisGame.TETROMINO_OUTLINE_COLORS[type]);
            Brush cellBrush = new SolidBrush(TetrisGame.TETROMINO_COLORS[type]);

            int[][] blocks = Game.StashedPiece.Blocks;

            for (int x = 0; x < blocks.Length; x++)
            {
                for (int y = 0; y < blocks[x].Length; y++)
                {
                    if (blocks[x][y] == 0) { continue; }

                    Point cPos = new Point(
                        pieceX + (x * Game.GAME_CELL_PIXEL_SIZE),
                        pieceY + (y * Game.GAME_CELL_PIXEL_SIZE)
                    );

                    DrawCell(graphics, cPos, cellBrush, cellPen);
                }
            }

            cellPen.Dispose();
            cellBrush.Dispose();
        }

        private void DrawCell(Graphics graphics, Point pos, Brush fillCol, Pen outlineCol)
        {
            Rectangle cell = new Rectangle(pos.X, pos.Y, Game.GAME_CELL_PIXEL_SIZE, Game.GAME_CELL_PIXEL_SIZE);

            graphics.FillRectangle(fillCol, cell);
            graphics.DrawRectangle(outlineCol, cell);
        }
    }
}
