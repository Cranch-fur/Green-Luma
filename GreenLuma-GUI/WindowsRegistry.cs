using Microsoft.Win32;
using System;

namespace GreenLumaGUI
{
    internal class WindowsRegistry
    {
        public struct SubKey
        {
            public readonly string FullPath;
            public readonly string ShortPath;
            public readonly string Hive;

            public SubKey(string fullPath)
            {
                FullPath = fullPath;

                // Remove Hive from the registry path, start it with the Key instead.
                ShortPath = fullPath.Substring(fullPath.IndexOf(@"\") + 1);

                // Remove everything except Hive from registry path.
                Hive = fullPath.Substring(0, fullPath.IndexOf('\\'));
            }
        }
        // BINARY = byte[]
        // DWORD = int (Int32)
        // QWORD = long (Int64)
        // SZ = string

        // "HKEY_CURRENT_USER" --> Hive
        // "HKEY_CURRENT_USER\SOFTWARE" --> Key
        // "HKEY_CURRENT_USER\SOFTWARE\Microsoft" --> Sub-Key
        // "HKEY_CURRENT_USER\SOFTWARE\Microsoft\Internet Explorer" --> Sub-Key
        // "HKEY_CURRENT_USER\SOFTWARE\Microsoft\Internet Explorer\Start Page (REG_SZ)" --> Value
        public static readonly SubKey defaultSubKey = new SubKey(@"HKEY_CURRENT_USER\SOFTWARE\GreenLuma");






        public class CommonEntries
        {
            public const string steamClientPath = "Steam Client Path";
        }






        public static RegistryKey GetRegistryKeyFromHiveName(string registryHive)
        {
            switch (registryHive)
            {
                case "HKEY_CLASSES_ROOT":
                    return Registry.ClassesRoot;

                case "HKEY_CURRENT_USER":
                    return Registry.CurrentUser;

                case "HKEY_LOCAL_MACHINE":
                    return Registry.LocalMachine;

                case "HKEY_USERS":
                    return Registry.Users;

                case "HKEY_CURRENT_CONFIG":
                    return Registry.CurrentConfig;

                default:
                    return null;
            }
        }






        public static bool GetSubKeyExist(SubKey subKey)
        {
            RegistryKey registryKey = GetRegistryKeyFromHiveName(subKey.Hive);
            if (registryKey == null)
                return false;


            try
            {
                using (RegistryKey registrySubKey = registryKey.OpenSubKey(subKey.ShortPath, false))
                {
                    return registrySubKey != null;
                }
            }
            catch
            {
                return false;
            }
        }
        public static bool GetSubKeyExist()
        {
            return GetSubKeyExist(defaultSubKey);
        }


        public static bool DestroySubKey(SubKey subKey)
        {
            RegistryKey registryKey = GetRegistryKeyFromHiveName(subKey.Hive);
            if (registryKey == null)
                return false;


            try
            {
                using (RegistryKey registrySubKey = registryKey.OpenSubKey(subKey.ShortPath, true))
                {
                    if (registrySubKey != null)
                    {
                        registryKey.DeleteSubKeyTree(subKey.ShortPath);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
        public static bool DestroySubKey()
        {
            return DestroySubKey(defaultSubKey);
        }






        public static byte[] GetData_BINARY(SubKey subKey, string entryName, byte[] defaultReturn = null)
        {
            object data = Registry.GetValue(subKey.FullPath, entryName, defaultReturn);
            if (data != null && data is byte[])
            {
                return (byte[])data;
            }
            else
                return defaultReturn;
        }
        public static byte[] GetData_BINARY(string entryName, byte[] defaultReturn = null)
        {
            return GetData_BINARY(defaultSubKey, entryName, defaultReturn);
        }




        public static int GetData_DWORD(SubKey subKey, string entryName, int defaultReturn = -1)
        {
            object data = Registry.GetValue(subKey.FullPath, entryName, defaultReturn);
            if (data != null && data is int)
            {
                return Convert.ToInt32(data);
            }
            else
                return defaultReturn;
        }
        public static int GetData_DWORD(string entryName, int defaultReturn = -1)
        {
            return GetData_DWORD(defaultSubKey, entryName, defaultReturn);
        }




        public static long GetData_QWORD(SubKey subKey, string entryName, long defaultReturn = -1)
        {
            object data = Registry.GetValue(subKey.FullPath, entryName, defaultReturn);
            if (data != null && data is long)
            {
                return Convert.ToInt32(data);
            }
            else
                return defaultReturn;
        }
        public static long GetData_QWORD(string entryName, long defaultReturn = -1)
        {
            return GetData_QWORD(defaultSubKey, entryName, defaultReturn);
        }




        public static string GetData_SZ(SubKey subKey, string entryName, string defaultReturn = null)
        {
            object data = Registry.GetValue(subKey.FullPath, entryName, defaultReturn);
            if (data != null && data is string)
            {
                return Convert.ToString(data);
            }
            else
                return defaultReturn;
        }
        public static string GetData_SZ(string entryName, string defaultReturn = null)
        {
            return GetData_SZ(defaultSubKey, entryName, defaultReturn);
        }






        public static bool SetData_BINARY(SubKey subKey, string entryName, byte[] data)
        {
            try
            {
                Registry.SetValue(subKey.FullPath, entryName, data, RegistryValueKind.Binary);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool SetData_BINARY(string entryName, byte[] data)
        {
            return SetData_BINARY(defaultSubKey, entryName, data);
        }




        public static bool SetData_DWORD(SubKey subKey, string entryName, int data)
        {
            try
            {
                Registry.SetValue(subKey.FullPath, entryName, data, RegistryValueKind.DWord);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool SetData_DWORD(string entryName, int data)
        {
            return SetData_DWORD(defaultSubKey, entryName, data);
        }




        public static bool SetData_QWORD(SubKey subKey, string entryName, long data)
        {
            try
            {
                Registry.SetValue(subKey.FullPath, entryName, data, RegistryValueKind.QWord);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool SetData_QWORD(string entryName, long data)
        {
            return SetData_QWORD(defaultSubKey, entryName, data);
        }




        public static bool SetData_SZ(SubKey subKey, string entryName, string data)
        {
            try
            {
                Registry.SetValue(subKey.FullPath, entryName, data, RegistryValueKind.String);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool SetData_SZ(string entryName, string data)
        {
            return SetData_SZ(defaultSubKey, entryName, data);
        }
    }
}
