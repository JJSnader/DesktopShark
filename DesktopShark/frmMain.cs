using Microsoft.Win32;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;

namespace DesktopShark
{
    public partial class frmMain : Form
    {
        bool _isFacingLeft;

        private bool isDragging = false;
        private Point dragCursor;
        private Point dragForm;

        private Point targetLocation;

        System.Windows.Forms.Timer _moveTimer;
        System.Windows.Forms.Timer _idleTimer;

        private MemoryStream? _memoryStream;
        private Settings? _settings;

        public frmMain()
        {
            InitializeComponent();
            LoadSettings(); 

            _moveTimer = new System.Windows.Forms.Timer() { Interval = 50 };
            if (_settings?.IAmSpeed ?? false)
            {
                _moveTimer.Interval = 1;
            }
            _moveTimer.Tick += MoveTimer_Elapsed;

            TopMost = _settings?.AlwaysOnTop ?? true;

            _idleTimer = new System.Windows.Forms.Timer();
            _idleTimer.Interval = (int)(_settings?.SecondsBetweenMoving ?? 10) * 1000;
            if (_settings?.IAmSpeed ?? false)
            {
                _idleTimer.Interval = 500;
            }
            _idleTimer.Tick += IdleTimer_Elapsed;
            _idleTimer.Start();

            pictureBox1.MouseDown += frmMain_MouseDown;
            // Handle the MouseMove event for dragging
            pictureBox1.MouseMove += new MouseEventHandler(frmMain_MouseMove);
            // Handle the MouseUp event to stop dragging
            pictureBox1.MouseUp += new MouseEventHandler(frmMain_MouseUp);

            // Randomize if it starts looking left or right
            var r = new Random(DateTime.Now.GetHashCode());
            if (r.Next(0, 2) == 0)
            {
                _isFacingLeft = true;
                pictureBox1.Image = LoadGifFromBytes(Properties.Resources.idleL);
            }
            else
            {
                _isFacingLeft = false;
                pictureBox1.Image = LoadGifFromBytes(Properties.Resources.idleR);
            }
        }

        private void LoadSettings()
        {

            // Get settings
            if (File.Exists(Settings.SettingsFilePath))
            {
                var settings = File.ReadAllText(Settings.SettingsFilePath);
                _settings = JsonConvert.DeserializeObject<Settings>(settings);
                if (_settings == null)
                {
                    _settings = new Settings()
                    {
                        AlwaysOnTop = true,
                        SecondsBetweenMoving = 10,
                        IAmSpeed = false
                    };
                    File.WriteAllText(Settings.SettingsFilePath, JsonConvert.SerializeObject(_settings));
                }
            }
            else
            {
                _settings = new Settings()
                {
                    AlwaysOnTop = true,
                    SecondsBetweenMoving = 10,
                    IAmSpeed = false
                };
                File.WriteAllText(Settings.SettingsFilePath, JsonConvert.SerializeObject(_settings));
            }
        }

        private Image LoadGifFromBytes(byte[] gifBytes)
        {
            _memoryStream = new MemoryStream(gifBytes);

            var originalImage = Image.FromStream(_memoryStream, true, true);

            return originalImage;
        }

        private void MoveToRandomLocation()
        {
            Random rand = new Random();
            int newX, newY;

            var virtualScreen = SystemInformation.VirtualScreen;

            do
            {
                newX = rand.Next(virtualScreen.Left, virtualScreen.Right - Width);
                newY = rand.Next(virtualScreen.Top, virtualScreen.Bottom - Height);
            }
            while ((Math.Abs(newX - this.Location.X) < 100 || Math.Abs(newY - this.Location.Y) < 100)
            && (Math.Abs(newX - Location.X) > 200 || Math.Abs(newY - Location.Y) > 200));

            targetLocation = new Point(newX, newY);
            if (newX > Location.X)
            {
                if (_isFacingLeft)
                {
                    _isFacingLeft = false;
                    pictureBox1.Image = LoadGifFromBytes(Properties.Resources.flip);
                }
                pictureBox1.Image = LoadGifFromBytes(Properties.Resources.swimR);
            }
            else
            {
                if (!_isFacingLeft)
                {
                    _isFacingLeft = true;
                    pictureBox1.Image = LoadGifFromBytes(Properties.Resources.flip);
                    Thread.Sleep(100);
                }
                pictureBox1.Image = LoadGifFromBytes(Properties.Resources.swimL);
            }

            _idleTimer.Stop();
            _moveTimer.Start(); // Start the timer to move the form
        }

        private void SetIdleImage()
        {
            if (_isFacingLeft)
            {
                pictureBox1.Image = LoadGifFromBytes(Properties.Resources.idleL);
            }
            else
            {
                pictureBox1.Image = LoadGifFromBytes(Properties.Resources.idleR);
            }
        }

        #region Events

        private void IdleTimer_Elapsed(object? sender, EventArgs e)
        {
            MoveToRandomLocation();
        }

        private void MoveTimer_Elapsed(object? sender, EventArgs e)
        {
            // Calculate the distance to the target
            int deltaX = targetLocation.X - this.Location.X;
            int deltaY = targetLocation.Y - this.Location.Y;

            // Move the form 5 pixels closer to the target
            if (Math.Abs(deltaX) > 5 || Math.Abs(deltaY) > 5)
            {
                int moveX = Math.Sign(deltaX) * Math.Min(5, Math.Abs(deltaX));
                int moveY = Math.Sign(deltaY) * Math.Min(5, Math.Abs(deltaY));

                this.Location = new Point(this.Location.X + moveX, this.Location.Y + moveY);
            }
            else
            {
                // Stop the timer when the form reaches the target location
                _moveTimer.Stop();
                SetIdleImage();
                _idleTimer.Start();
            }
        }

        private void frmMain_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _moveTimer.Stop();
                _idleTimer.Stop();
                if (_isFacingLeft)
                {
                    pictureBox1.Image = LoadGifFromBytes(Properties.Resources.dragL);
                }
                else
                {
                    pictureBox1.Image = LoadGifFromBytes(Properties.Resources.dragR);
                }
                isDragging = true;
                dragCursor = Cursor.Position;
                dragForm = this.Location;
            }
            else if (e.Button == MouseButtons.Right)
            {
                _moveTimer.Stop();
                _idleTimer.Stop();
                SetIdleImage();
                var frm = new frmSettings();
                frm.ShowDialog();

                LoadSettings();
                TopMost = _settings?.AlwaysOnTop ?? true;
                if (_settings?.IAmSpeed ?? false)
                {
                    _moveTimer.Interval = 1;
                    _idleTimer.Interval = 500;
                }
                else
                {
                    _moveTimer.Interval = 50;
                    _idleTimer.Interval = (int)(_settings?.SecondsBetweenMoving ?? 10) * 1000;                    
                }

                _idleTimer.Start();
            }
        }

        private void frmMain_MouseMove(object? sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursor));
                this.Location = Point.Add(dragForm, new Size(dif));
            }
        }

        private void frmMain_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
                SetIdleImage();
                _idleTimer.Start();
            }
        }

        #endregion
    }
}
