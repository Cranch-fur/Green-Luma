using System;
using System.Windows.Forms;

namespace GreenLumaGUI
{
    public static class ExceptionManager
    {
        private const string exceptionProvider = "Green Luma Exception";


        public static void ShowException(Exception exception, bool detailed = false)
        {
            MessageBox.Show(detailed ? exception.ToString() : exception.Message, exceptionProvider, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
        }


        public static void PrintException(Exception exception, bool detailed = false)
        {
            string exceptionMessage = detailed ? exception.ToString() : exception.Message;
            Console.WriteLine($"[{exceptionProvider}] {exceptionMessage}");
        }
    }
}
