using System.Diagnostics;

namespace DesktopShark
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Process[] pname = Process.GetProcessesByName("DesktopShark");
            Application.Run(new frmMain(pname.Length - 1));
        }
    }
}