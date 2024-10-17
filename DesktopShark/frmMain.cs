using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;

namespace DesktopShark
{
    public partial class frmMain : Form
    {
        bool _isFacingLeft;
        // dragging vars
        private bool isDragging = false;
        private Point dragCursor;
        private Point dragForm;
        // movement vars
        private bool _chaseCursor = false;
        private Point targetLocation;
        // timers
        System.Windows.Forms.Timer _moveTimer;
        System.Windows.Forms.Timer _idleTimer;
        System.Windows.Forms.Timer _changeIdleImageTimer;
        System.Windows.Forms.Timer _showCursorTimer;
        // objects
        private MemoryStream? _memoryStream;
        private Settings? _settings;
        private Random _rand;
        private SoundPlayer _player;
        // chase vars
        private bool allowCursorGrab;
        private int letGoCounter;
        private bool _cursorGrabbed = false;

        private const int DefaultMoveInterval = 50;
        private int _instanceID;

        public frmMain(int instanceID)
        {
            InitializeComponent();
            _instanceID = instanceID;
            LoadSettings(); 

            _rand = new Random(DateTime.Now.GetHashCode());

            _moveTimer = new System.Windows.Forms.Timer() { Interval = DefaultMoveInterval };
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

            _changeIdleImageTimer = new System.Windows.Forms.Timer();
            _changeIdleImageTimer.Interval = 3400;
            _changeIdleImageTimer.Tick += ChangeIdleImageTimer_Elapsed;

            if (_settings?.FollowCursor ?? false)
                _moveTimer.Start();
            else
                _idleTimer.Start();

            _showCursorTimer = new System.Windows.Forms.Timer();
            _showCursorTimer.Interval = 5000;
            _showCursorTimer.Tick += ShowCursorTimer_Elapsed;

            pictureBox1.MouseDown += frmMain_MouseDown;
            // Handle the MouseMove event for dragging
            pictureBox1.MouseMove += new MouseEventHandler(frmMain_MouseMove);
            // Handle the MouseUp event to stop dragging
            pictureBox1.MouseUp += new MouseEventHandler(frmMain_MouseUp);

            // Randomize if it starts looking left or right
            if (_rand.Next(0, 2) == 0)
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
            // Create the settings directory if it doesn't exist
            if (!Directory.Exists(SettingsFilePath.GetSettingsDirectory()))
            {
                Directory.CreateDirectory(SettingsFilePath.GetSettingsDirectory());
            }
            // Get settings
            if (File.Exists(SettingsFilePath.GetSettingsFilePath(_instanceID)))
            {
                var settings = File.ReadAllText(SettingsFilePath.GetSettingsFilePath(_instanceID));
                _settings = JsonConvert.DeserializeObject<Settings>(settings);
                if (_settings == null)
                {
                    _settings = new Settings();
                    File.WriteAllText(SettingsFilePath.GetSettingsFilePath(_instanceID), JsonConvert.SerializeObject(_settings));
                }
            }
            else
            {
                _settings = new Settings();
                File.WriteAllText(SettingsFilePath.GetSettingsFilePath(_instanceID), JsonConvert.SerializeObject(_settings));
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
            int newX, newY;

            var virtualScreen = SystemInformation.VirtualScreen;

            do
            {
                newX = _rand.Next(virtualScreen.Left, virtualScreen.Right - Width);
                newY = _rand.Next(virtualScreen.Top, virtualScreen.Bottom - Height);
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

        private void SetIdleImage(bool canTurn)
        {
            if (_isFacingLeft)
            {
                if (canTurn && _rand.Next(0, 10) < 4)
                {
                    pictureBox1.Image = LoadGifFromBytes(Properties.Resources.turnL);
                    _changeIdleImageTimer.Start();
                }
                else
                {
                    pictureBox1.Image = LoadGifFromBytes(Properties.Resources.idleL);
                }
            }
            else
            {
                if (canTurn && _rand.Next(0, 10) < 4)
                {
                    pictureBox1.Image = LoadGifFromBytes(Properties.Resources.turnR);
                    _changeIdleImageTimer.Start();
                }
                else
                {
                    pictureBox1.Image = LoadGifFromBytes(Properties.Resources.idleR);
                }
            }
        }

        #region Events

        private void IdleTimer_Elapsed(object? sender, EventArgs e)
        {
            if (_settings?.ChaseCursorEnabled ?? false)
            {
                if (_cursorGrabbed)
                {
                    _cursorGrabbed = false;
                    Cursor = Cursors.Default;
                }
                if (_rand.Next(0, 100) < (_settings?.ChaseProbability ?? 10))
                {
                    _chaseCursor = true;
                    _idleTimer.Stop();
                    if (_isFacingLeft)
                    {
                        pictureBox1.Image = LoadGifFromBytes(Properties.Resources.chaseL);
                    }
                    else
                    {
                        pictureBox1.Image = LoadGifFromBytes(Properties.Resources.chaseR);
                    }
                    _player = new SoundPlayer(new MemoryStream(Properties.Resources.chase));
                    _player.PlayLooping();
                    TopMost = true;
                    ChaseCursor();
                    _moveTimer.Interval = 1;
                    _moveTimer.Start();
                }
                else if (_settings?.FollowCursor ?? false)
                {
                    FollowCursor();
                }
                else
                {
                    MoveToRandomLocation();
                }
            }
            else if (_settings?.FollowCursor ?? false)
            {
                FollowCursor();
            }
            else
            {
                MoveToRandomLocation();
            }
        }

        private void ChangeIdleImageTimer_Elapsed(object? sender, EventArgs e)
        {
            _changeIdleImageTimer.Stop();
            SetIdleImage(false);
        }

        private void MoveTimer_Elapsed(object? sender, EventArgs e)
        {
            if (_chaseCursor)
            {
                ChaseCursor();
                return;
            }

            if ( _settings?.FollowCursor ?? false)
            {
                FollowCursor();
                return;
            }

            // Calculate the distance to the target
            int deltaX = targetLocation.X - this.Location.X;
            int deltaY = targetLocation.Y - this.Location.Y;

            // Move the form 5 pixels closer to the target
            if (Math.Abs(deltaX) > 5 || Math.Abs(deltaY) > 5)
            {
                int moveX = Math.Sign(deltaX) * Math.Min(5, Math.Abs(deltaX));
                int moveY = Math.Sign(deltaY) * Math.Min(5, Math.Abs(deltaY));

                Location = new Point(this.Location.X + moveX, this.Location.Y + moveY);
            }
            else
            {
                // Stop the timer when the form reaches the target location
                _moveTimer.Stop();
                SetIdleImage(true);
                _idleTimer.Start();
            }
        }

        private void ShowCursorTimer_Elapsed(object? sender, EventArgs e)
        {
            _showCursorTimer.Stop();
            Cursor.Show();
        }

        private void frmMain_MouseDown(object? sender, MouseEventArgs e)
        {
            if (_cursorGrabbed)
                return;

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
                _player?.Stop();
                SetIdleImage(false);
                var frm = new frmSettings(_instanceID);
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
                    _moveTimer.Interval = DefaultMoveInterval;
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
            else if (_cursorGrabbed)
            {
                Point formCenter = new Point(Left + Width / 2, Top + Height / 2);
                Cursor.Position = formCenter;
            }
        }

        private void frmMain_MouseUp(object? sender, MouseEventArgs e)
        {
            if (_cursorGrabbed)
                return; 

            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
                SetIdleImage(true);
                if (_settings?.FollowCursor ?? false)
                    _moveTimer.Start();
                else
                    _idleTimer.Start();
            }
        }

        #endregion
        #region Cursor Chasing

        private void ChaseCursor()
        {
            Point cursorPosition = Cursor.Position;

            // Get the center position of the form
            Point formCenter = new(Left + Width / 2, Top + Height / 2);

            double distanceToCursor = GetDistance(formCenter, cursorPosition);

            // Move towards the cursor if not on top of it
            if (distanceToCursor > 20)
            {
                MoveTowardCursor(cursorPosition, formCenter);
            }
            else
            {
                Cursor.Hide();
                _showCursorTimer.Start();
                TopMost = _settings?.AlwaysOnTop ?? false;
                Cursor.Position = formCenter;
                _cursorGrabbed = true;
                _chaseCursor = false;
                _player.Stop();
                SetIdleImage(true);
                _moveTimer.Interval = DefaultMoveInterval;
                if (!(_settings?.FollowCursor ?? false))
                {
                    _moveTimer.Stop();
                    _idleTimer.Start();
                }
            }
        }

        private void FollowCursor()
        {
            Point cursorPosition = Cursor.Position;
            Point formCenter = new Point(Left + Width / 2, Top + Height / 2);
            double distanceToCursor = GetDistance(formCenter, cursorPosition);
            if (distanceToCursor > 20)
            {
                MoveTowardCursor(cursorPosition, formCenter);
            }
        }

        private void MoveTowardCursor(Point cursorPosition, Point formCenter)
        {
            // Calculate the direction vector towards the cursor
            int deltaX = cursorPosition.X - formCenter.X;
            int deltaY = cursorPosition.Y - formCenter.Y;

            // Normalize the direction to move the form in small steps
            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            double stepX = (deltaX / distance) * 5; // Move 5 pixels per tick in X direction
            double stepY = (deltaY / distance) * 5; // Move 5 pixels per tick in Y direction

            // Update the form's position by adding the calculated step
            if (stepX < 0 && !_isFacingLeft)
            {
                _isFacingLeft = true;
                if (_chaseCursor)
                    pictureBox1.Image = LoadGifFromBytes(Properties.Resources.chaseL);
                else 
                    pictureBox1.Image = LoadGifFromBytes(Properties.Resources.swimL);
            }
            else if (stepX > 0 && _isFacingLeft)
            {
                _isFacingLeft = false;
                if (_chaseCursor)
                    pictureBox1.Image = LoadGifFromBytes(Properties.Resources.chaseR);
                else
                    pictureBox1.Image = LoadGifFromBytes(Properties.Resources.swimR);
            }
            Left += (int)stepX;
            Top += (int)stepY;
        }

        private double GetDistance(Point point1, Point point2)
        {
            // Calculate the distance between two points
            int dx = point1.X - point2.X;
            int dy = point1.Y - point2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        #endregion
    }
}
