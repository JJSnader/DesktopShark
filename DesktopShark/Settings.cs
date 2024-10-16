using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopShark
{
    internal class Settings
    {
        public const string SettingsFilePath = @"C:\ProgramData\sharksettings.json";

        public bool AlwaysOnTop { get; set; }
        public decimal SecondsBetweenMoving { get; set; }
        public bool IAmSpeed { get; set; }
    }
}
