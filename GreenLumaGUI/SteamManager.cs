using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace GreenLumaGUI
{
    public static class SteamManager
    {
        private static string clientPath = WindowsRegistry.GetData_SZ(WindowsRegistry.CommonEntries.steamClientPath);
        public static string GetClientPath()
        {
            return clientPath;
        }
        public static void SetClientPath(string newClientPath)
        {
            clientPath = newClientPath;
        }
        public static void StoreClientPath()
        {
            WindowsRegistry.SetData_SZ(WindowsRegistry.CommonEntries.steamClientPath, clientPath);
        }


        public static string GetExecutablePath()
        {
            string clientPath = GetClientPath();
            if (clientPath == null || Directory.Exists(clientPath) == false)
                return null;


            return Path.Combine(clientPath, "steam.exe");
        }
        public static string GetExecutableName()
        {
            string executablePath = GetExecutablePath();
            if (executablePath == null || Directory.Exists(executablePath) == false)
                return null;


            return Path.GetFileName(executablePath);
        }


        public static string GetPackageCachePath()
        {
            string clientPath = GetClientPath();
            if (clientPath == null || Directory.Exists(clientPath) == false)
                return null;


            return Path.Combine(clientPath, "appcache", "packageinfo.vdf");
        }






        public static bool ClearPackageCache()
        {
            try
            {
                string packageCachePath = GetPackageCachePath();
                if (File.Exists(packageCachePath))
                    File.Delete(packageCachePath);


                return true;
            }
            catch (Exception ex)
            {
                ExceptionManager.ShowException(ex);
                return false; 
            }
        }






        public static bool IsRunning()
        {
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                try
                {
                    if (process.ProcessName.ToLower().Contains("steam"))
                    {
                        return true;
                    }
                }
                catch { }
            }


            return false;
        }
        public static bool Close()
        {
            Process[] steamProcesses = Process.GetProcesses()
                .Where(p =>
                {
                    try
                    {
                        return p.ProcessName.ToLower().Contains("steam");
                    }
                    catch
                    {
                        return false;
                    }
                })
                .ToArray();


            foreach (Process process in steamProcesses)
            {
                try
                {
                    if (!process.HasExited)
                    {
                        process.Kill();
                        process.WaitForExit(5000);
                    }
                }
                catch { }
            }

            
            DateTime timeout = DateTime.Now.AddSeconds(5);
            while (IsRunning())
            {
                if (DateTime.Now > timeout)
                {
                    return false;
                }


                Thread.Sleep(100);
            }


            return true;
        }
        public static bool Start()
        {
            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.WorkingDirectory = GetClientPath();
                    process.StartInfo.FileName = GetExecutablePath();
                    process.StartInfo.UseShellExecute = false;
                    process.Start();


                    if (process == null)
                        return false;


                    if (process.HasExited)
                        return false;


                    return true;
                }
            }
            catch 
            { 
                return false; 
            }
        }
    }
}
