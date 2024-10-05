using System;
using System.Windows.Forms;
using Tetris.Window;

namespace Tetris
{
    /// <summary>
    /// Window that contains the game's display and forwards keyboard inputs to the game.
    /// </summary>
    public partial class GameWnd : Form
    {
        /// <summary>
        /// Game display (contains the game).
        /// </summary>
        public static GameDisplay Display { get; private set; }

        public GameWnd(GameDisplay display)
        {
            Display = display;

            InitializeComponent();
        }

        private void GameWnd_Load(object sender, EventArgs e)
        {
            this.ClientSize = Display.Size;
            this.Controls.Add(Display);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Display.Game?.KeyPressed(keyData);

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
