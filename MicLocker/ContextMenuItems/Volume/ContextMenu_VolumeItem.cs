using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace MicLocker.ContextMenuItems
{
    class ContextMenu_VolumeItem : ToolStripMenuItem
    {
        public ContextMenu_VolumeItem(string _Text, int _VolumeValue)
        {
            Text = _Text;
            m_VolumeValue = _VolumeValue;

            Click += ContextMenu_VolumeItem_Click;
        }

        private void ContextMenu_VolumeItem_Click(object sender, EventArgs e)
        {
            Program.s_ApplicationContext.SetVolume(m_VolumeValue);
        }

        private int m_VolumeValue = 0;
    }
}
