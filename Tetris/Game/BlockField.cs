using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.Game
{
    /// <summary>
    /// A field of blocks, usually used to represent a tetromino to manupulate on the game field.
    /// </summary>
    public class BlockField
    {
        /// <summary>
        /// 2D array with the minimum size needed for containing the shape.
        /// </summary>
        public int[][] Blocks { get; private set; }
        /// <summary>
        /// Game field position of the top-left block in this block field.
        /// </summary>
        public Point Position { get; set; }
        /// <summary>
        /// Rotation value of the shape.
        /// </summary>
        public int Rotation { get; private set; }
        /// <summary>
        /// Shape contained within this block field.
        /// </summary>
        public TetrominoShapes Shape { get; private set; }

        public BlockField(int[][] Blocks, Point Position, TetrominoShapes shape, int rotation)
        {
            this.Blocks = Blocks;
            this.Position = Position;
            Shape = shape;
            Rotation = rotation;
        }
        public BlockField(BlockField o)
        {
            CopyBlocks(o);
            CopyPosition(o);
            Shape = o.Shape;
            Rotation = o.Rotation;
        }

        /// <summary>
        /// Copy the position of another block field.
        /// </summary>
        /// <param name="o">Other block field.</param>
        public void CopyPosition(BlockField o)
        {
            this.Position = new Point(o.Position.X, o.Position.Y);
        }

        /// <summary>
        /// Copy the block array of another block field.
        /// </summary>
        /// <param name="o">Other block field.</param>
        public void CopyBlocks(BlockField o)
        {
            this.Blocks = o.Blocks.Clone() as int[][];
        }

        /// <summary>
        /// Get the width of the block array.
        /// </summary>
        /// <returns>The width of the block array.</returns>
        public int GetWidth()
        {
            return Blocks.Length;
        }

        /// <summary>
        /// Get the height of the block array.
        /// </summary>
        /// <returns>The height of the block array.</returns>
        public int GetHeight()
        {
            if (Blocks.Length == 0) return 0;

            return Blocks[0].Length;
        }

        public void Nudge(int dX, int dY)
        {
            Position = new Point(Position.X + dX, Position.Y + dY);
        }

        /// <summary>
        /// Rotate the block field by redefining its block array.
        /// </summary>
        /// <param name="anticlockwise">True if the field should be rotated anti-clockwise instead of clockwise.</param>
        public void Rotate(bool anticlockwise)
        {
            if (anticlockwise)
            {
                Rotation++;
                Rotation %= 3;
            } else
            {
                Rotation--;
                if (Rotation < 0) Rotation = 3;
            }

            this.Blocks = BlockFieldConstructor.GetTetrominoBlocks(Shape, Rotation);
        }
    }
}
