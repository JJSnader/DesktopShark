using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopShark
{
    internal class Settings
    {
        public bool AlwaysOnTop { get; set; }
        public decimal SecondsBetweenMoving { get; set; }
        public bool ChaseCursorEnabled { get; set; }
        public bool FollowCursor { get; set; }
        public bool IAmSpeed { get; set; }
        public int ChaseProbability { get; set; }
        public string[] SharkNames { get; set; }

        public Settings()
        {
            AlwaysOnTop = true;
            SecondsBetweenMoving = 20;
            IAmSpeed = false;
            ChaseCursorEnabled = false;
            FollowCursor = false;
            ChaseProbability = 10;
            SharkNames = ["John", "Michael", "David", "James", "Robert", "William", "Christopher", "Matthew", "Daniel", "Joseph", "Charles", "Thomas", "Richard", "Mark", "Anthony", "Kevin", "Brian", "Edward", "Paul", "Steven"];
        }
    }
    internal static class SettingsFilePath
    {
        public static string GetSettingsFilePath(int instanceID)
        {
            return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), $"{Application.ProductName}\\SharkSettings{instanceID}.json");
        }
        public static string GetSettingsDirectory()
        {
            return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Application.ProductName ?? "");
        }
    }
}
