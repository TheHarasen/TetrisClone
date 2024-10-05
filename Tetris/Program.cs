using System;
using System.Drawing;
using System.Windows.Forms;
using Tetris.Window;

namespace Tetris
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            GameDisplay display = new GameDisplay();
            display.Size = new Size(
                TetrisGame.GAME_WINDOW_SIZE_X,
                TetrisGame.GAME_WINDOW_SIZE_Y
            );

            GameWnd gameWnd = new GameWnd(display);
            TetrisGame game = new TetrisGame(gameWnd);
            display.Game = game;

            Application.Run(gameWnd);
        }
    }
}
