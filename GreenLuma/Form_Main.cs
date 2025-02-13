using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;


namespace GreenLuma
{
    public partial class Form_Main : Form
    {
        private void LumaStatus(bool playSound = false)
        {
            if (Luma.IsInstalled())
            {
                gradientProgressBar_LumaStatus.StartColor = Color.LimeGreen;
                gradientProgressBar_LumaStatus.EndColor = Color.SpringGreen;
                gradientProgressBar_LumaStatus.Refresh();


                button_LumaInstall.Text = "Delete";


                if (playSound)
                    SoundController.PlaySound(Properties.Resources.Activate);
            }
            else
            {
                gradientProgressBar_LumaStatus.StartColor = Color.Crimson;
                gradientProgressBar_LumaStatus.EndColor = Color.IndianRed;
                gradientProgressBar_LumaStatus.Refresh();


                button_LumaInstall.Enabled = Directory.Exists(Steam.GetClientPath());
                button_LumaInstall.Text = "Install";


                if (playSound)
                    SoundController.PlaySound(Properties.Resources.Deactivate);
            }


            gradientProgressBar_LumaStatus.Value = 100;
        }






        public Form_Main()
        {
            InitializeComponent();
        }
        private void Form_Main_Load(object sender, EventArgs e)
        {
            this.Icon = Properties.Resources.Icon;


            string steamClientPath = Steam.GetClientPath();
            if (steamClientPath == null || Directory.Exists(steamClientPath) == false)
            {
                steamClientPath = WindowsRegistry.GetData_SZ(new WindowsRegistry.SubKey(@"HKEY_CURRENT_USER\Software\Valve\Steam"), "SteamPath");
                if (steamClientPath != null && Directory.Exists(steamClientPath))
                {
                    string normalizedPath = Path.GetFullPath(steamClientPath);
                    Steam.SetClientPath(normalizedPath);
                    Steam.StoreClientPath();


                    textBox_SteamClient.Text = normalizedPath;
                }
            }
            else
            {
                textBox_SteamClient.Text = steamClientPath;
            }


            List<string> gamesList = Luma.GetGamesList();
            if (gamesList != null && gamesList.Count > 0)
                textBox_SteamGames.Text = string.Join(Environment.NewLine, gamesList);


            if (Steam.IsRunning())
                Steam.Close();


            LumaStatus();
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
                        Steam.SetClientPath(userProvidedPath);
                        Steam.StoreClientPath();


                        textBox_SteamClient.Text = userProvidedPath;
                        LumaStatus();
                    }
                }
            }
        }

        




        private List<string> GetUserProvidedGamesList()
        {
            return new List<string>(textBox_SteamGames.Lines);
        }


        private void textBox_SteamGames_TextChanged(object sender, EventArgs e)
        {
            List<string> userProvidedGames = GetUserProvidedGamesList();
            if (userProvidedGames.Count == 0)
            {
                Luma.ClearGamesList();
                return;
            }


            Luma.ClearGamesList();
            Luma.WriteGamesList(userProvidedGames);
        }


        private void textBox_SteamGames_Leave(object sender, EventArgs e)
        {
            if (Steam.IsRunning())
            {
                if (Steam.Close())
                    Steam.Start();
            }
        }






        private void button_LumaInstall_MouseClick(object sender, MouseEventArgs e)
        {
            if (Steam.IsRunning())
                Steam.Close();


            if (Luma.IsInstalled())
            {
                try
                {
                    string dynamicLibraryPath = Luma.GetDynamicLibraryPath();
                    if (File.Exists(dynamicLibraryPath))
                        File.Delete(dynamicLibraryPath);
                }
                catch (Exception ex)
                {
                    ExceptionController.ShowException(ex);
                }
            }
            else
            {
                try
                {
                    string dynamicLibraryPath = Luma.GetDynamicLibraryPath();
                    if (File.Exists(dynamicLibraryPath))
                        File.Delete(dynamicLibraryPath);


                    File.Copy(Luma.GetLocalDynamicLibraryPath(), dynamicLibraryPath);


                    try
                    {
                        Steam.ClearPackageCache();
                    }
                    catch { }


                    if (textBox_SteamGames.Text.Length > 0 && Luma.GetGamesList().Count == 0)
                    {
                        Luma.WriteGamesList(GetUserProvidedGamesList());
                    }
                }
                catch (Exception ex)
                {
                    ExceptionController.ShowException(ex);
                }
            }


            Steam.Start();
            LumaStatus(true);
        }
    }
}
