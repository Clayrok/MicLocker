using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

using MicLocker.ContextMenuItems;

namespace MicLocker
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            s_ApplicationContext = new MyCustomApplicationContext();

            Application.Run(s_ApplicationContext);
        }

        public static MyCustomApplicationContext s_ApplicationContext { get; private set; }
    }


    public class MyCustomApplicationContext : ApplicationContext
    {
        private NotifyIcon trayIcon;

        public MyCustomApplicationContext()
        {
            InitTrayIcon();
            InitContextMenu();

            TryGetLastVolumeValue();
            SetDeviceVolume();

            m_VolumeHeader.Text = "Volume: " + s_CurrentVolume + "%";
            m_LoopIntervalHeader.Text = "Interval: " + LOOP_INTERVAL_MILLISECONDS + "ms";

            DeviceVolumeUpdate();
        }

        async void DeviceVolumeUpdate()
        {
            while (!s_IsAppExiting)
            {
                await Task.Delay(LOOP_INTERVAL_MILLISECONDS);
                SetDeviceVolume();
            }
        }

        private void InitTrayIcon()
        {
            trayIcon = new NotifyIcon();
            trayIcon.Icon = new Icon(RES_FOLDER + "icon.ico");
            trayIcon.Visible = true;
        }

        private void InitContextMenu()
        {
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip.ShowImageMargin = false;

            contextMenuStrip.Items.Add(m_VolumeHeader);
            contextMenuStrip.Items.Add(m_LoopIntervalHeader);

            contextMenuStrip.Items.Add(new ToolStripSeparator());

            contextMenuStrip.Items.Add(new ContextMenu_SetVolume());
            contextMenuStrip.Items.Add(new ToolStripSeparator());
            contextMenuStrip.Items.Add(new ContextMenu_Close());

            trayIcon.ContextMenuStrip = contextMenuStrip;
        }

        public void TryGetLastVolumeValue()
        {
            if (!(File.Exists(FILE_PATH)))
            {
                Application.Exit();
            }

            string content = File.ReadAllText(FILE_PATH);
            int parsed = 0;
            if (!(Int32.TryParse(content, out parsed)))
            {
                s_CurrentVolume = 0;
                File.WriteAllText(FILE_PATH, "0");
            }
            else
            {
                s_CurrentVolume = (int)MathF.Ceiling((parsed / 65536.0f) * 100f);
            }
        }

        public void SetVolume(int _Value)
        {
            s_CurrentVolume = _Value;
            SaveVolumeValue();
            SetDeviceVolume();

            m_VolumeHeader.Text = "Volume: " + s_CurrentVolume + "%";
        }

        private void SetDeviceVolume()
        {
            string appPath = Application.ExecutablePath;

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WorkingDirectory = appPath.Remove(appPath.LastIndexOf('\\') + 1);
            startInfo.FileName = RES_FOLDER + "nircmdc.exe";
            startInfo.Arguments = " setsysvolume " + ((int)((65536.0f / 100.0f) * s_CurrentVolume)).ToString() + " default_record";
            startInfo.CreateNoWindow = true;

            Process.Start(startInfo);
        }

        public void SaveVolumeValue()
        {
            if (!File.Exists(FILE_PATH))
            {
                File.Create(FILE_PATH);
            }
            File.WriteAllText(FILE_PATH, ((int)((65536.0f / 100.0f) * s_CurrentVolume)).ToString());
        }

        protected override void ExitThreadCore()
        {
            base.ExitThreadCore();

            s_IsAppExiting = true;
            trayIcon.Visible = false;
            trayIcon.Dispose();

            Application.Exit();
        }

        private ToolStripLabel m_VolumeHeader = new ToolStripLabel("Volume: ");
        private ToolStripLabel m_LoopIntervalHeader = new ToolStripLabel("Interval: ");

        private bool s_IsAppExiting = false;
        private int s_CurrentVolume = 0;
        private const string RES_FOLDER = "res\\";
        private const string FILE_PATH = RES_FOLDER + "LastVolumeValue.txt";
        private const int LOOP_INTERVAL_MILLISECONDS = 500;
    }
}