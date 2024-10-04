using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.Game
{
    public class BlockField
    {
        public int[][] Blocks { get; set; }
        public Point Position { get; set; }
        public int Rotation { get; set; }
        public TetrominoShapes Shape { get; set; }

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
        }

        public void CopyPosition(BlockField o)
        {
            this.Position = new Point(o.Position.X, o.Position.Y);
        }

        public void CopyBlocks(BlockField o)
        {
            this.Blocks = o.Blocks.Clone() as int[][];
        }

        public int GetWidth()
        {
            return Blocks.Length;
        }

        public int GetHeight()
        {
            if (Blocks.Length == 0) return 0;

            return Blocks[0].Length;
        }

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
