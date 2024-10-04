using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tetris;
using Tetris.Window;

namespace Tetris
{
    public partial class GameWnd : Form
    {
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
