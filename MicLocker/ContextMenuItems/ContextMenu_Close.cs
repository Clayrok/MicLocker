using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace MicLocker.ContextMenuItems
{
    class ContextMenu_Close : ToolStripMenuItem
    {
        public ContextMenu_Close()
        {
            Text = "Close";
        }

        protected override void OnClick(System.EventArgs e)
        {
            Application.Exit();
        }
    }
}
