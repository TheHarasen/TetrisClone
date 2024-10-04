using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Tetris.Game
{
    /// <summary>
    /// Holds the state of the blocks in the game field.
    /// </summary>
    public class GameField
    {
        /// <summary>
        /// Width of the game field in blocks.
        /// </summary>
        private int FieldWidth;
        /// <summary>
        /// Height of the game field in blocks.
        /// </summary>
        private int FieldHeight;
        /// <summary>
        /// The field of blocks. Any non-zero entry is inhabited by a block.
        /// </summary>
        public int[][] Field { get; private set; }

        /// <summary>
        /// Instantiates the game field with the height and width of the game.
        /// </summary>
        public GameField()
        {
            FieldWidth = TetrisGame.GAME_GRID_SIZE_X;
            FieldHeight = TetrisGame.GAME_GRID_SIZE_Y;

            Field = new int[FieldWidth][];
            for (int i = 0; i < Field.Length; i++)
            {
                Field[i] = new int[FieldHeight];
            }
        }

        /// <summary>
        /// Set all values on the game field to 0.
        /// </summary>
        public void Clear()
        {
            for (int x = 0; x < FieldWidth; x++)
            {
                for (int y = 0; y < FieldHeight; y++)
                {
                    Field[x][y] = 0;
                }
            }
        }

        /// <summary>
        /// Move the given block field by a certain amount of blocks.
        /// Returns if the move was successful (i.e. not prevented by other blocks or out of bounds).
        /// </summary>
        /// <param name="blocks">Block field to move.</param>
        /// <param name="x">Blocks to move in the x-direction.</param>
        /// <param name="y">Blocks to move in the y-direction.</param>
        /// <returns>True of the move was successful.</returns>
        public bool MoveBlocks(BlockField blocks, int x, int y)
        {
            bool valid = true;

            //Clear blocks in old position
            SetFieldOfBlocks(blocks, true);

            //Make new field in new position
            BlockField next = new BlockField(blocks);
            next.Position = new Point(next.Position.X + x, next.Position.Y + y);

            //Check if new position is valid
            if (!BlocksPositionValid(next)) valid = false;

            //Increment position if valid
            if (valid)
            {
                blocks.CopyPosition(next);
            }

            //Set blocks in new position
            SetFieldOfBlocks(blocks, false);

            return valid;
        }

        /// <summary>
        /// Move block field down until it is prevented from moving.
        /// </summary>
        /// <param name="blocks">Block field to move.</param>
        public void DropBlocks(BlockField blocks)
        {
            bool move = true;

            while (move)
            {
                move = MoveBlocks(blocks, 0, 1);
            }
        }

        /// <summary>
        /// Put a block field on the game field, if it can fit.
        /// </summary>
        /// <param name="blocks">Block field to spawn.</param>
        /// <returns>True if block field could fit and was spawned.</returns>
        public bool SpawnBlockField(BlockField blocks)
        {
            if (!BlocksPositionValid(blocks)) return false;

            SetFieldOfBlocks(blocks, false);

            return true;
        }

        /// <summary>
        /// Rotate a block field around on the game field and shift it around until it can fit.
        /// The rotation will only be undone if it spills over the top of the field.
        /// </summary>
        /// <param name="blocks">Block field to rotate.</param>
        public void RotateBlockField(BlockField blocks)
        {
            //Clear the field first
            SetFieldOfBlocks(blocks, true);

            //Make the rotation
            blocks.Rotate(false);

            //Ensure the fied is within the bounds of the game field after rotation
            SetBlockFieldWithinBounds(blocks);

            //Move up until it's in a valid position
            while (!BlocksPositionValid(blocks))
            {
                blocks.Position = new Point(blocks.Position.X, blocks.Position.Y - 1);

                //If it spills over the top of the field, undo rotation and stop moving
                if (blocks.Position.Y < 0)
                {
                    blocks.Rotate(true);
                    break;
                }
            }

            //Set the new blocks
            SetFieldOfBlocks(blocks, false);
        }

        /// <summary>
        /// Adjust position of block field until it's within the field's bounds.
        /// Return true if the field was out of bounds and had to be moved.
        /// </summary>
        /// <param name="blocks">Block field to move.</param>
        /// <returns>True if the field was out of bounds and had to be moved.</returns>
        public bool SetBlockFieldWithinBounds(BlockField blocks)
        {
            bool moved = false;

            int x = blocks.Position.X;
            int y = blocks.Position.Y;
            int w = blocks.GetWidth();
            int h = blocks.GetHeight();

            //Check horizontally
            if (x < 0)
            {
                blocks.Position = new Point(0, y);
                moved = true;
            } else if ((x + w - 1) > FieldWidth - 1)
            {
                blocks.Position = new Point(FieldWidth - w, y);
                moved = true;
            }
            
            //Check vertically
            if (y < 0)
            {
                blocks.Position = new Point(x, 0);
                moved = true;
            }
            else if ((y + h - 1) > FieldHeight - 1)
            {
                blocks.Position = new Point(x, FieldHeight - h);
                moved = true;
            }

            return moved;
        }

        /// <summary>
        /// Run through all blocks of given block field and set or clear the corresponding values in the game field.
        /// Any zero-values in the block field are ignored.
        /// </summary>
        /// <param name="blocks">Block field to reference.</param>
        /// <param name="clear">Whether to clear the game field blocks.
        /// If false, fills the game field values with the block field values instead.</param>
        public void SetFieldOfBlocks(BlockField blocks, bool clear)
        {
            for (int x = 0; x < blocks.Blocks.Length; x++)
            {
                for (int y = 0; y < blocks.Blocks[x].Length; y++)
                {
                    if (blocks.Blocks[x][y] != 0)
                    {
                        SetBlock(
                            x + blocks.Position.X,
                            y + blocks.Position.Y,
                            clear ? 0 : blocks.Blocks[x][y]
                        );
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the given block field can fit in its position.
        /// It will not fit if it's out of bounds or overlapping other blocks.
        /// </summary>
        /// <param name="blocks">Block field to check against.</param>
        /// <returns>True if the block field can fit in its position.</returns>
        public bool BlocksPositionValid(BlockField blocks)
        {
            //Bounds check
            if (!BlockFieldInBounds(blocks)) return false;

            //Overlap check
            for (int x = 0; x < blocks.Blocks.Length; x++)
            {
                for (int y = 0; y < blocks.Blocks[x].Length; y++)
                {
                    //Get absolute positions of each block field entry
                    int cx = x + blocks.Position.X;
                    int cy = y + blocks.Position.Y;

                    if ((Field[cx][cy] != 0) &&
                        (blocks.Blocks[x][y] != 0))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Set the block at the given coordinates to the given value (if in bounds.)
        /// </summary>
        /// <param name="x">x-position of the block.</param>
        /// <param name="y">y-position of the block.</param>
        /// <param name="value">Value to set the block to.</param>
        /// <returns>True if the given coordinates were within the bounds of the game field.</returns>
        public bool SetBlock(int x, int y, int value)
        {
            //Bounds check
            if (!InBounds(x, y)) return false;

            Field[x][y] = value;

            return true;
        }

        /// <summary>
        /// Check if a given block field is within the bounds of the game field.
        /// </summary>
        /// <param name="blocks">Block field to check against.</param>
        /// <returns>True if the block field was within bounds of the game field.</returns>
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

        /// <summary>
        /// Check if a given set of coordinates is within the bounds of the game field.
        /// </summary>
        /// <param name="x">x-coordinate to check.</param>
        /// <param name="y">y-coordinate to check.</param>
        /// <returns>True if the coordinates are within the bounds of the game field.</returns>
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

        /// <summary>
        /// Check for any completed lines on the game field and clear them if necessary.
        /// </summary>
        /// <returns>Amount of lines cleared.</returns>
        public int CheckClearedLines()
        {
            int numLines = 0;

            //Check all lines from bottom to top
            for (int i = FieldHeight-1; i >= 0; i--)
            {
                //Shift all above lines down if this one is cleared.
                if (CheckLineClear(i))
                {
                    ShiftRowsDown(i);
                    i++; //Go back once to account for shifted rows

                    numLines++;
                }
            }

            return numLines;
        }

        /// <summary>
        /// Check if the given row of the game field is cleared (all values are non-zero).
        /// </summary>
        /// <param name="row">Row number.</param>
        /// <returns>Whether the line is cleared.</returns>
        public bool CheckLineClear(int row)
        {
            for (int i = 0; i < FieldWidth; i++)
            {
                if (Field[i][row] == 0) return false;
            }

            return true;
        }

        /// <summary>
        /// Start at the given row, and then shift all rows above it down one row.
        /// </summary>
        /// <param name="startRow">Row to start the operation from.</param>
        public void ShiftRowsDown(int startRow)
        {
            for (int i = startRow; i >= 0; i--)
            {
                //Add an empty line if we're at the top row.
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
