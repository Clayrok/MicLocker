using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;


namespace MicLocker.ContextMenuItems
{
    class ContextMenu_SetVolume : ToolStripMenuItem
    {
        public ContextMenu_SetVolume()
        {
            Text = "Set Volume ";

            ((ToolStripDropDownMenu)DropDown).ShowImageMargin = false;

            InitSubMenu();
        }

        void InitSubMenu()
        {
            for (int i = 1; i <= 100; i++)
            {
                DropDown.Items.Add(new ContextMenu_VolumeItem(i + "%", i));
            }
        }
    }
}