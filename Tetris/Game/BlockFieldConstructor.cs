using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.Game
{
    /// <summary>
    /// All possible shapes of tetromino.
    /// Use <see cref="TETROMINOSHAPES_AMOUNT"/> as the limit when iterating over all values.
    /// </summary>
    public enum TetrominoShapes
    {
        T_Piece,
        S_Piece,
        Z_Piece,
        Square_Piece,
        Long_Piece,
        L_Piece,
        J_Piece,
        TETROMINOSHAPES_AMOUNT
    }

    /// <summary>
    /// Stores the data of all tetrominos and handles creating new ones.
    /// </summary>
    public static class BlockFieldConstructor
    {
        public static readonly int[][][] TETROMINO_T = new int[][][]
        {
            //rotation 1
            new int[][]
            {
                new int[] { 0, 1 },
                new int[] { 1, 1 },
                new int[] { 0, 1 }
            },
            //rotation 2
            new int[][]
            {
                new int[] { 0, 1, 0 },
                new int[] { 1, 1, 1 }
            },
            //rotation 3
            new int[][]
            {
                new int[] { 1, 0 },
                new int[] { 1, 1 },
                new int[] { 1, 0 }
            },
            //rotation 4
            new int[][]
            {
                new int[] { 1, 1, 1 },
                new int[] { 0, 1, 0 }
            }
        };

        public static readonly int[][][] TETROMINO_S = new int[][][]
        {
            //rotation 1
            new int[][]
            {
                new int[] { 0, 2 },
                new int[] { 2, 2 },
                new int[] { 2, 0 }
            },
            //rotation 2
            new int[][]
            {
                new int[] { 2, 2, 0 },
                new int[] { 0, 2, 2 }
            },
            //rotation 3
            new int[][]
            {
                new int[] { 0, 2 },
                new int[] { 2, 2 },
                new int[] { 2, 0 }
            },
            //rotation 4
            new int[][]
            {
                new int[] { 2, 2, 0 },
                new int[] { 0, 2, 2 }
            }
        };

        public static readonly int[][][] TETROMINO_Z = new int[][][]
        {
            //rotation 1
            new int[][]
            {
                new int[] { 3, 0 },
                new int[] { 3, 3 },
                new int[] { 0, 3 }
            },
            //rotation 2
            new int[][]
            {
                new int[] { 0, 3, 3 },
                new int[] { 3, 3, 0 }
            },
            //rotation 3
            new int[][]
            {
                new int[] { 3, 0 },
                new int[] { 3, 3 },
                new int[] { 0, 3 }
            },
            //rotation 4
            new int[][]
            {
                new int[] { 0, 3, 3 },
                new int[] { 3, 3, 0 }
            }
        };

        public static readonly int[][][] TETROMINO_SQUARE = new int[][][]
        {
            //rotation 1
            new int[][]
            {
                new int[] { 4, 4 },
                new int[] { 4, 4 }
            },
            //rotation 2
            new int[][]
            {
                new int[] { 4, 4 },
                new int[] { 4, 4 }
            },
            //rotation 3
            new int[][]
            {
                new int[] { 4, 4 },
                new int[] { 4, 4 }
            },
            //rotation 4
            new int[][]
            {
                new int[] { 4, 4 },
                new int[] { 4, 4 }
            }
        };

        public static readonly int[][][] TETROMINO_LONG = new int[][][]
        {
            //rotation 1
            new int[][]
            {
                new int[] { 5},
                new int[] { 5},
                new int[] { 5},
                new int[] { 5}
            },
            //rotation 2
            new int[][]
            {
                new int[] { 5, 5, 5, 5 }
            },
            //rotation 3
            new int[][]
            {
                new int[] { 5},
                new int[] { 5},
                new int[] { 5},
                new int[] { 5}
            },
            //rotation 4
            new int[][]
            {
                new int[] { 5, 5, 5, 5 }
            }
        };

        public static readonly int[][][] TETROMINO_L = new int[][][]
        {
            //rotation 1
            new int[][]
            {
                new int[] { 0, 6},
                new int[] { 0, 6},
                new int[] { 6, 6}
            },
            //rotation 2
            new int[][]
            {
                new int[] { 6, 0, 0 },
                new int[] { 6, 6, 6 }
            },
            //rotation 3
            new int[][]
            {
                new int[] { 6, 6},
                new int[] { 6, 0},
                new int[] { 6, 0}
            },
            //rotation 4
            new int[][]
            {
                new int[] { 6, 6, 6 },
                new int[] { 0, 0, 6 }
            }
        };

        public static readonly int[][][] TETROMINO_J = new int[][][]
        {
            //rotation 1
            new int[][]
            {
                new int[] { 7, 0 },
                new int[] { 7, 0 },
                new int[] { 7, 7 }
            },
            //rotation 2
            new int[][]
            {
                new int[] { 7, 7, 7 },
                new int[] { 7, 0, 0 }
            },
            //rotation 3
            new int[][]
            {
                new int[] { 7, 7 },
                new int[] { 0, 7 },
                new int[] { 0, 7 }
            },
            //rotation 4
            new int[][]
            {
                new int[] { 0, 0, 7 },
                new int[] { 7, 7, 7 }
            }
        };

        private static readonly Random rand = new Random();
        private static int lastRand = -1;

        /// <summary>
        /// Get a 2D int array of the data for any given tetromino with the given rotation value (0 to 3).
        /// </summary>
        /// <param name="shape">Shape of the desired tetromino.<param>
        /// <param name="rotation">Rotation of the desired tetromino (0 to 3).</param>
        /// <returns></returns>
        public static int[][] GetTetrominoBlocks(TetrominoShapes shape, int rotation)
        {
            if (rotation < 0 || rotation > 3)
            {
                throw new ArgumentException("Rotation value out of bounds: Must be from 0 to 3.");
            }

            //Clone the 2D array of data for the desired tetromino shape.
            switch (shape)
            {
                case TetrominoShapes.T_Piece:
                    return TETROMINO_T[rotation].Clone() as int[][];
                case TetrominoShapes.S_Piece:
                    return TETROMINO_S[rotation].Clone() as int[][];
                case TetrominoShapes.Z_Piece:
                    return TETROMINO_Z[rotation].Clone() as int[][];
                case TetrominoShapes.Square_Piece:
                    return TETROMINO_SQUARE[rotation].Clone() as int[][];
                case TetrominoShapes.Long_Piece:
                    return TETROMINO_LONG[rotation].Clone() as int[][];
                case TetrominoShapes.L_Piece:
                    return TETROMINO_L[rotation].Clone() as int[][];
                case TetrominoShapes.J_Piece:
                    return TETROMINO_J[rotation].Clone() as int[][];
            }

            return null;
        }

        /// <summary>
        /// Create a random tetromino with the default rotation. This will never spit back the same tetromino twice in a row.
        /// </summary>
        /// <returns>Block field representing the created tetromino.</returns>
        public static BlockField CreateTetromino()
        {
            return CreateTetromino(0);
        }

        /// <summary>
        /// Generate a random tetromino with the given rotation. This will never spit back the same tetromino twice in a row.
        /// </summary>
        /// <param name="rotation">Rotation value for the tetromino.</param>
        /// <returns>Block field representing the created tetromino.</returns>
        public static BlockField CreateTetromino(int rotation)
        {
            //Make sure we're not reusing shapes from last random value.
            int shape = rand.Next((int)TetrominoShapes.TETROMINOSHAPES_AMOUNT);
            while (shape == lastRand)
            {
                shape = rand.Next((int)TetrominoShapes.TETROMINOSHAPES_AMOUNT);
            }

            lastRand = shape;

            return CreateTetromino((TetrominoShapes)shape, rotation);
        }

        /// <summary>
        /// Generate a tetromino with the given shape and rotation.
        /// </summary>
        /// <param name="shape">Type of the desired tetromino.</param>
        /// <param name="rotation">Rotation value for the desired tetromino.</param>
        /// <returns>Block field representing the created tetromino.</returns>
        public static BlockField CreateTetromino(TetrominoShapes shape, int rotation)
        {
            int[][] blocks = GetTetrominoBlocks(shape, rotation);

            return new BlockField(blocks, new System.Drawing.Point(0, 0), shape, rotation);
        }
    }
}
