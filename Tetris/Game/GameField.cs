using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Tetris.Game
{
    public class GameField
    {
        private int FieldWidth;
        private int FieldHeight;
        public int[][] Field { get; private set; }
        private BlockField testBlocks;

        public GameField()
        {
            FieldWidth = TetrisGame.GAME_GRID_SIZE_X;
            FieldHeight = TetrisGame.GAME_GRID_SIZE_Y;

            Field = new int[FieldWidth][];
            for (int i = 0; i < Field.Length; i++)
            {
                Field[i] = new int[FieldHeight];
            }

            //test
            testBlocks = BlockFieldConstructor.CreateTetromino(TetrominoShapes.T_Piece, 0);
            testBlocks.Position = new Point(FieldWidth / 2, 0);
        }

        public void TestUpdate()
        {
            MoveBlocks(testBlocks, 0, 1);
        }

        public bool MoveBlocks(BlockField blocks, int x, int y)
        {
            bool valid = true;

            //Clear blocks in old position
            SetFieldOfBlocks(blocks, true);

            //test block collision...
            BlockField next = new BlockField(blocks);
            next.Position = new Point(next.Position.X + x, next.Position.Y + y);

            if (!BlocksPositionValid(next)) valid = false;

            //Increment position
            if (valid)
            {
                blocks.Position = new Point(next.Position.X, next.Position.Y);
            }

            //Set blocks in new position
            SetFieldOfBlocks(blocks, false);

            return valid;
        }

        public void DropBlocks(BlockField blocks)
        {
            bool move = true;

            while (move)
            {
                move = MoveBlocks(blocks, 0, 1);
            }
        }

        /// <summary>
        /// Returns whether the spawn position was valid
        /// </summary>
        /// <param name="blocks"></param>
        /// <returns></returns>
        public bool SpawnBlockField(BlockField blocks)
        {
            if (!BlocksPositionValid(blocks)) return false;

            SetFieldOfBlocks(blocks, false);

            return true;
        }

        public void RotateBlockField(BlockField blocks)
        {
            SetFieldOfBlocks(blocks, true);

            blocks.Rotate(false);

            SetBlockFieldWithinBounds(blocks);

            while (!BlocksPositionValid(blocks))
            {
                blocks.Position = new Point(blocks.Position.X, blocks.Position.Y - 1);
                if (blocks.Position.Y < 0)
                {
                    blocks.Rotate(true);
                    break;
                }
            }

            SetFieldOfBlocks(blocks, false);
        }

        //Adjust position of blockfield until it's within the field's bounds
        //Return true if the field was within bounds and had to be moved
        public bool SetBlockFieldWithinBounds(BlockField blocks)
        {
            bool moved = false;

            if (blocks.Position.X < 0)
            {
                blocks.Position = new Point(0, blocks.Position.Y);
                moved = true;
            } else if ((blocks.Position.X + blocks.GetWidth() - 1) > FieldWidth)
            {
                blocks.Position = new Point(blocks.Position.X - blocks.GetWidth() - 1, blocks.Position.Y);
                moved = true;
            }

            if (blocks.Position.Y < 0)
            {
                blocks.Position = new Point(blocks.Position.X, 0);
                moved = true;
            }
            else if ((blocks.Position.Y + blocks.GetHeight() - 1) > FieldHeight)
            {
                blocks.Position = new Point(blocks.Position.X, blocks.Position.Y - blocks.GetHeight() - 1);
                moved = true;
            }

            return moved;
        }

        public void SetFieldOfBlocks(BlockField blocks, bool clear)
        {
            for (int x = 0; x < blocks.Blocks.Length; x++)
            {
                for (int y = 0; y < blocks.Blocks[x].Length; y++)
                {
                    if (blocks.Blocks[x][y] != 0)
                    {
                        SetFieldBlock(
                            x + blocks.Position.X,
                            y + blocks.Position.Y,
                            clear ? 0 : blocks.Blocks[x][y]
                        );
                    }
                }
            }
        }

        public bool BlocksPositionValid(BlockField blocks)
        {
            if (!BlockFieldInBounds(blocks)) return false;

            for (int x = 0; x < blocks.Blocks.Length; x++)
            {
                for (int y = 0; y < blocks.Blocks[x].Length; y++)
                {
                    int cx = x + blocks.Position.X;
                    int cy = y + blocks.Position.Y;

                    if ((Field[cx][cy] != 0) &&
                        (blocks.Blocks[x][y] != 0))
                        return false;
                }
            }

            return true;
        }

        public bool SetFieldBlock(int x, int y, int value)
        {
            //Bounds check
            if (!InBounds(x, y)) return false;

            Field[x][y] = value;

            return true;
        }

        private bool BlockFieldInBounds(BlockField blocks)
        {
            for (int x = 0; x < blocks.Blocks.Length; x++)
            {
                for (int y = 0; y < blocks.Blocks[x].Length; y++)
                {
                    if (!InBounds(x + blocks.Position.X, y + blocks.Position.Y))
                        return false;
                }
            }

            return true;
        }

        private bool InBounds(int x, int y)
        {
            if (
                (x < 0) || (y < 0) ||
                (x >= FieldWidth) || (y >= FieldHeight)
               )
            {
                return false;
            } else
            {
                return true;
            }
        }

        public int CheckClearedLines()
        {
            int numLines = 0;

            for (int i = FieldHeight-1; i >= 0; i--)
            {
                if (CheckLineClear(i))
                {
                    ShiftRowsDown(i);
                    i++; //Go back once to account for shifted rows
                }
            }

            return numLines;
        }

        public bool CheckLineClear(int row)
        {
            for (int i = 0; i < FieldWidth; i++)
            {
                if (Field[i][row] == 0) return false;
            }

            return true;
        }

        public void ShiftRowsDown(int startRow)
        {
            for (int i = startRow; i >= 0; i--)
            {
                //Last row
                if (i == 0)
                {
                    for (int j = 0; j < FieldWidth; j++)
                    {
                        Field[j][i] = 0;
                    }
                } else
                {
                    for (int j = 0; j < FieldWidth; j++)
                    {
                        Field[j][i] = Field[j][i - 1];
                    }
                }
            }
        }
    }
}
