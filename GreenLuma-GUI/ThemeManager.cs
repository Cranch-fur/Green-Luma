using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenLumaGUI
{
    public static class ThemeManager
    {
        private static readonly WindowsRegistry.SubKey WindowsThemeKey = new WindowsRegistry.SubKey(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
        private const string LumaThemeModeKey = "Theme Mode";

        public enum ThemeMode
        {
            FollowSystem = -1,
            Light = 0,
            Dark = 1
        }






        private static bool IsWindowsDark()
        {
            /* AppsUseLightTheme: 1 = light, 0 = dark */
            int appsUseLightTheme = WindowsRegistry.GetData_DWORD(WindowsThemeKey, "AppsUseLightTheme", 1);
            return appsUseLightTheme == 0;
        }

        private static ThemeMode GetAppThemeMode()
        {
            int appThemeMode = WindowsRegistry.GetData_DWORD(LumaThemeModeKey, (int)ThemeMode.FollowSystem);
            return (ThemeMode)appThemeMode;
        }

        private static void SetAppThemeMode(ThemeMode mode)
        {
            WindowsRegistry.SetData_DWORD(LumaThemeModeKey, (int)mode);
        }

        public static bool GetIsDarkMode()
        {
            switch (GetAppThemeMode())
            {
                case ThemeMode.Dark: return true;
                case ThemeMode.Light: return false;
                default: return IsWindowsDark();
            }
        }

        public static void SetIsDarkMode(bool isDarkMode)
        {
            SetAppThemeMode(isDarkMode ? ThemeMode.Dark : ThemeMode.Light);
        }
    }
}
