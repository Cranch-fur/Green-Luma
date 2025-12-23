using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GreenLumaGUI
{
    public partial class Form_Main : Form
    {
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private const int WM_NCACTIVATE = 0x0086;

        private void SetIsWindowDarkTitleBar(bool isWindowDarkTitleBar)
        {
            if (Environment.OSVersion.Version.Major < 10)
                return;

            int OSBuild = Environment.OSVersion.Version.Build;
            int OSAttribute = (OSBuild >= 19041) ? 20 : 19; // 2004+ => 20, older => 19.

            int useDarkTitleBar = isWindowDarkTitleBar ? 1 : 0;
            int hr = DwmSetWindowAttribute(this.Handle, OSAttribute, ref useDarkTitleBar, sizeof(int));

            if (hr != 0)
            {
                int altOSAttribute = OSAttribute == 20 ? 19 : 20;
                hr = DwmSetWindowAttribute(this.Handle, altOSAttribute, ref useDarkTitleBar, sizeof(int));
            }

            /* Force window redraw. */
            SendMessage(this.Handle, WM_NCACTIVATE, IntPtr.Zero, IntPtr.Zero);
            SendMessage(this.Handle, WM_NCACTIVATE, new IntPtr(1), IntPtr.Zero);
        }





        private void Luma_UpdateStatus(bool playSound = false)
        {
            if (LumaManager.IsInstalled())
            {
                gradientProgressBar_LumaStatus.StartColor = Color.LimeGreen;
                gradientProgressBar_LumaStatus.EndColor = Color.SpringGreen;
                gradientProgressBar_LumaStatus.Refresh();


                button_LumaInstall.Text = "Delete";


                if (playSound)
                    SoundManager.PlaySound(Properties.Resources.SND_Activate);
            }
            else
            {
                gradientProgressBar_LumaStatus.StartColor = Color.Crimson;
                gradientProgressBar_LumaStatus.EndColor = Color.IndianRed;
                gradientProgressBar_LumaStatus.Refresh();


                button_LumaInstall.Enabled = Directory.Exists(SteamManager.GetClientPath());
                button_LumaInstall.Text = "Install";


                if (playSound)
                    SoundManager.PlaySound(Properties.Resources.SND_Deactivate);
            }


            gradientProgressBar_LumaStatus.Value = 100;
        }






        public Form_Main()
        {
            InitializeComponent();
        }
        private void Form_Main_LoadTheme(bool updateCheckbox = false)
        {
            if (ThemeManager.GetIsDarkMode())
            {
                SetIsWindowDarkTitleBar(true);
                this.BackColor = Color.Black;

                label_SteamClient.ForeColor = Color.White;
                textBox_SteamClient.BackColor = Color.Black;
                textBox_SteamClient.ForeColor = Color.White;
                button_SteamClient.BackColor = Color.Black;
                button_SteamClient.ForeColor = Color.White;

                label_SteamApps.ForeColor = Color.White;
                textBox_SteamApps.BackColor = Color.Black;
                textBox_SteamApps.ForeColor = Color.White;
                
                button_SteamDB.BackColor = Color.Black;
                button_SteamDB.ForeColor = Color.White;

                button_CSRIN.BackColor = Color.Black;
                button_CSRIN.ForeColor = Color.White;

                button_SteamApps_Clear.BackColor = Color.Black;
                button_SteamApps_Clear.ForeColor = Color.White;

                button_LumaInstall.BackColor = Color.Black;
                button_LumaInstall.ForeColor = Color.White;

                checkBox_DarkTheme.ForeColor = Color.White;

                if (updateCheckbox)
                    checkBox_DarkTheme.Checked = true;
            }
            else
            {
                SetIsWindowDarkTitleBar(false);
                this.BackColor = Color.White;

                label_SteamClient.ForeColor = Color.Black;
                textBox_SteamClient.BackColor = Color.White;
                textBox_SteamClient.ForeColor = Color.Black;
                button_SteamClient.BackColor = Color.White;
                button_SteamClient.ForeColor = Color.Black;

                label_SteamApps.ForeColor = Color.Black;
                textBox_SteamApps.BackColor = Color.White;
                textBox_SteamApps.ForeColor = Color.Black;

                button_SteamDB.BackColor = Color.White;
                button_SteamDB.ForeColor = Color.Black;

                button_CSRIN.BackColor = Color.White;
                button_CSRIN.ForeColor = Color.Black;

                button_SteamApps_Clear.BackColor = Color.White;
                button_SteamApps_Clear.ForeColor = Color.Black;

                button_LumaInstall.BackColor = Color.White;
                button_LumaInstall.ForeColor = Color.Black;

                checkBox_DarkTheme.ForeColor = Color.Black;

                if (updateCheckbox)
                    checkBox_DarkTheme.Checked = false;
            }
        }
        private void Form_Main_Load(object sender, EventArgs e)
        {
            Form_Main_LoadTheme(true);
            this.Icon = Properties.Resources.Icon;

            string steamClientPath = SteamManager.GetClientPath();
            if (steamClientPath == null || Directory.Exists(steamClientPath) == false)
            {
                steamClientPath = WindowsRegistry.GetData_SZ(new WindowsRegistry.SubKey(@"HKEY_CURRENT_USER\Software\Valve\Steam"), "SteamPath");
                if (steamClientPath != null && Directory.Exists(steamClientPath))
                {
                    string normalizedPath = Path.GetFullPath(steamClientPath);
                    SteamManager.SetClientPath(normalizedPath);
                    SteamManager.StoreClientPath();


                    textBox_SteamClient.Text = normalizedPath;
                }
            }
            else
            {
                textBox_SteamClient.Text = steamClientPath;
            }

            List<string> gamesList = LumaManager.GetGamesList();
            if (gamesList != null && gamesList.Count > 0)
                textBox_SteamApps.Text = string.Join(Environment.NewLine, gamesList);

            if (SteamManager.IsRunning())
                SteamManager.Close();

            Luma_UpdateStatus();
        }






        private void button_SteamClient_MouseClick(object sender, MouseEventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Specify your \"Steam\" installation directory";


                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string userProvidedPath = folderDialog.SelectedPath;
                    if (Directory.Exists(userProvidedPath))
                    {
                        SteamManager.SetClientPath(userProvidedPath);
                        SteamManager.StoreClientPath();


                        textBox_SteamClient.Text = userProvidedPath;
                        Luma_UpdateStatus();
                    }
                }
            }
        }

        




        private List<string> GetUserProvidedGamesList()
        {
            return new List<string>(textBox_SteamApps.Lines);
        }


        private void textBox_SteamGames_TextChanged(object sender, EventArgs e)
        {
            List<string> userProvidedGames = GetUserProvidedGamesList();
            if (userProvidedGames.Count == 0)
            {
                LumaManager.ClearGamesList();
                return;
            }


            LumaManager.ClearGamesList();
            LumaManager.WriteGamesList(userProvidedGames);
        }


        private void textBox_SteamGames_Leave(object sender, EventArgs e)
        {
            if (SteamManager.IsRunning())
            {
                if (SteamManager.Close())
                    SteamManager.Start();
            }
        }






        private void button_SteamDB_MouseClick(object sender, MouseEventArgs e)
        {
            Process.Start(@"https://steamdb.info/");
            SoundManager.PlaySound(Properties.Resources.SND_Press);
        }

        private void button_CSRIN_MouseClick(object sender, MouseEventArgs e)
        {
            Process.Start(@"https://cs.rin.ru/forum/viewtopic.php?f=29&t=103709");
            SoundManager.PlaySound(Properties.Resources.SND_Press);
        }

        private void button_SteamGames_Clear_MouseClick(object sender, MouseEventArgs e)
        {
            textBox_SteamApps.Clear();
            SoundManager.PlaySound(Properties.Resources.SND_Press);
        }






        private void button_LumaInstall_MouseClick(object sender, MouseEventArgs e)
        {
            if (SteamManager.IsRunning())
                SteamManager.Close();


            if (LumaManager.IsInstalled())
            {
                try
                {
                    string dynamicLibraryPath = LumaManager.GetDynamicLibraryPath();
                    if (File.Exists(dynamicLibraryPath))
                        File.Delete(dynamicLibraryPath);
                }
                catch (Exception ex)
                {
                    ExceptionManager.ShowException(ex);
                }
            }
            else
            {
                try
                {
                    string dynamicLibraryPath = LumaManager.GetDynamicLibraryPath();
                    if (File.Exists(dynamicLibraryPath))
                        File.Delete(dynamicLibraryPath);


                    File.Copy(LumaManager.GetLocalDynamicLibraryPath(), dynamicLibraryPath);


                    try
                    {
                        SteamManager.ClearPackageCache();
                    }
                    catch { }


                    if (textBox_SteamApps.Text.Length > 0 && LumaManager.GetGamesList().Count == 0)
                    {
                        LumaManager.WriteGamesList(GetUserProvidedGamesList());
                    }
                }
                catch (Exception ex)
                {
                    ExceptionManager.ShowException(ex);
                }
            }


            SteamManager.Start();
            Luma_UpdateStatus(true);
        }

        private void checkBox_DarkTheme_CheckedChanged(object sender, EventArgs e)
        {
            ThemeManager.SetIsDarkMode(checkBox_DarkTheme.Checked);
            Form_Main_LoadTheme();
        }
    }
}
