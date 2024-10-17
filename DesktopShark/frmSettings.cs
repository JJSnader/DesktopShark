using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace DesktopShark
{
    public partial class frmSettings : Form
    {
        private Settings? _settings;

        public frmSettings()
        {
            InitializeComponent();

            if (!File.Exists(Settings.SettingsFilePath))
            {
                _settings = new Settings();
                File.WriteAllText(Settings.SettingsFilePath, JsonConvert.SerializeObject(_settings));
            }
            else
            {
                var settings = File.ReadAllText(Settings.SettingsFilePath);
                _settings = JsonConvert.DeserializeObject<Settings>(settings);
                if (_settings == null)
                {
                    _settings = new Settings();
                    File.WriteAllText(Settings.SettingsFilePath, JsonConvert.SerializeObject(_settings));
                }
            }

            cbAlwaysOnTop.Checked = _settings.AlwaysOnTop;
            tbSeconds.Value = _settings.SecondsBetweenMoving;
            cbChaseCursor.Checked = _settings.ChaseCursorEnabled;
            cbIAmSpeed.Checked = _settings.IAmSpeed;
            cbFollowCursor.Checked = _settings.FollowCursor;
            tbChaseProb.Value = _settings.ChaseProbability;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            _settings = new Settings()
            {
                AlwaysOnTop = cbAlwaysOnTop.Checked,
                SecondsBetweenMoving = tbSeconds.Value,
                IAmSpeed = cbIAmSpeed.Checked,
                ChaseCursorEnabled = cbChaseCursor.Checked,
                FollowCursor = cbFollowCursor.Checked,
                ChaseProbability = (int)tbChaseProb.Value
            };

            File.WriteAllText(Settings.SettingsFilePath, JsonConvert.SerializeObject(_settings));
            Close();
        }
    }
}
