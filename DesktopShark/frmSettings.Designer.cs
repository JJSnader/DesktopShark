namespace DesktopShark
{
    partial class frmSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnSave = new Button();
            label1 = new Label();
            cbAlwaysOnTop = new CheckBox();
            tbSeconds = new NumericUpDown();
            label2 = new Label();
            cbIAmSpeed = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)tbSeconds).BeginInit();
            SuspendLayout();
            // 
            // btnSave
            // 
            btnSave.BackColor = Color.Black;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Font = new Font("Segoe UI", 14F);
            btnSave.ForeColor = Color.White;
            btnSave.Location = new Point(208, 204);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(106, 40);
            btnSave.TabIndex = 0;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += btnClose_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 20F);
            label1.Location = new Point(64, 15);
            label1.Name = "label1";
            label1.Size = new Size(184, 37);
            label1.TabIndex = 1;
            label1.Text = "Shark Settings";
            // 
            // cbAlwaysOnTop
            // 
            cbAlwaysOnTop.AutoSize = true;
            cbAlwaysOnTop.Font = new Font("Segoe UI", 12F);
            cbAlwaysOnTop.Location = new Point(12, 82);
            cbAlwaysOnTop.Name = "cbAlwaysOnTop";
            cbAlwaysOnTop.Size = new Size(128, 25);
            cbAlwaysOnTop.TabIndex = 2;
            cbAlwaysOnTop.Text = "Always on Top";
            cbAlwaysOnTop.UseVisualStyleBackColor = true;
            // 
            // tbSeconds
            // 
            tbSeconds.Location = new Point(12, 153);
            tbSeconds.Name = "tbSeconds";
            tbSeconds.Size = new Size(120, 23);
            tbSeconds.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(12, 121);
            label2.Name = "label2";
            label2.Size = new Size(188, 21);
            label2.TabIndex = 4;
            label2.Text = "Seconds between Moving";
            // 
            // checkBox1
            // 
            cbIAmSpeed.AutoSize = true;
            cbIAmSpeed.Font = new Font("Segoe UI", 12F);
            cbIAmSpeed.Location = new Point(12, 204);
            cbIAmSpeed.Name = "checkBox1";
            cbIAmSpeed.Size = new Size(108, 25);
            cbIAmSpeed.TabIndex = 5;
            cbIAmSpeed.Text = "I Am Speed";
            cbIAmSpeed.UseVisualStyleBackColor = true;
            // 
            // frmSettings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(40, 40, 40);
            ClientSize = new Size(326, 268);
            Controls.Add(cbIAmSpeed);
            Controls.Add(label2);
            Controls.Add(tbSeconds);
            Controls.Add(cbAlwaysOnTop);
            Controls.Add(label1);
            Controls.Add(btnSave);
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "frmSettings";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "frmSettings";
            ((System.ComponentModel.ISupportInitialize)tbSeconds).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnSave;
        private Label label1;
        private CheckBox cbAlwaysOnTop;
        private NumericUpDown tbSeconds;
        private Label label2;
        private CheckBox cbIAmSpeed;
    }
}