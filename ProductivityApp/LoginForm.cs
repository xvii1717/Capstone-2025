using System;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ProductivityApp
{
    public partial class LoginForm : Form
    {
        public string Username { get; private set; }
        public LoginForm()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.Black;
            this.Padding = new Padding(0);
            this.Paint += LoginForm_Paint;
        }

        private void LoginForm_Paint(object sender, PaintEventArgs e)
        {
            var rect = this.ClientRectangle;
            using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(rect, Color.FromArgb(20, 30, 60), Color.FromArgb(40, 60, 120), 45F))
            {
                e.Graphics.FillRectangle(brush, rect);
            }
        }

        private void SetRoundedRegion(Control ctrl, int radius)
        {
            var path = CreateRoundedRectPath(new Rectangle(0, 0, ctrl.Width, ctrl.Height), radius);
            ctrl.Region = new Region(path);
        }

        private GraphicsPath CreateRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(rect.Left, rect.Top, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Top, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.Left, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseAllFigures();
            return path;
        }

        private void InitializeComponent()
        {
            this.Text = "Login";
            this.Size = new System.Drawing.Size(600, 400);
            this.BackColor = Color.Black;
            // Center panel in window
            var panel = new Panel {
                Size = new Size(400, 520),
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.None,
                Location = new Point((this.ClientSize.Width - 400) / 2, (this.ClientSize.Height - 520) / 2 + 60)
            };
            this.Resize += (s, e) => {
                panel.Location = new Point((this.ClientSize.Width - panel.Width) / 2, (this.ClientSize.Height - panel.Height) / 2 + 60);
                panel.Invalidate();
                this.Invalidate(); // Redraw gradient
            };
            this.Resize += (s, e) => {
                panel.Location = new Point((this.ClientSize.Width - panel.Width) / 2, (this.ClientSize.Height - panel.Height) / 2);
                panel.Invalidate();
                this.Invalidate(); // Redraw gradient
            };
            // Remove panel gradient paint
            var titleLabel = new Label {
                Name = "titleLabel",
                Text = "Welcome to\nProductivityApp",
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 100,
                Width = 400,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            panel.Controls.Add(titleLabel);
            this.Controls.Add(panel);
            // Force layout and redraw to ensure label text is visible
            panel.PerformLayout();
            this.PerformLayout();
            var usernameLabel = new Label { Text = "Username:", Left = 40, Top = 120, Width = 100, Font = new Font("Segoe UI", 12F), ForeColor = Color.White, BackColor = Color.Transparent };
            var passwordLabel = new Label { Text = "Password:", Left = 40, Top = 180, Width = 100, Font = new Font("Segoe UI", 12F), ForeColor = Color.White, BackColor = Color.Transparent };
            var usernameBox = new TextBox { Left = 160, Top = 120, Width = 200, Name = "usernameBox", Font = new Font("Segoe UI", 12F), BackColor = Color.FromArgb(30,30,30), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            var passwordBox = new TextBox { Left = 160, Top = 180, Width = 200, Name = "passwordBox", UseSystemPasswordChar = true, Font = new Font("Segoe UI", 12F), BackColor = Color.FromArgb(30,30,30), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            var loginButton = new Button { Text = "Login", Left = 80, Top = 240, Width = 100, Height = 40, Font = new Font("Segoe UI", 12F, FontStyle.Bold), BackColor = Color.FromArgb(40, 60, 120), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            loginButton.FlatAppearance.BorderSize = 0;
            loginButton.Resize += (s, e) => SetRoundedRegion(loginButton, 20);
            var registerButton = new Button { Text = "Register", Left = 200, Top = 240, Width = 100, Height = 40, Font = new Font("Segoe UI", 12F, FontStyle.Bold), BackColor = Color.FromArgb(60, 90, 180), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            registerButton.FlatAppearance.BorderSize = 0;
            registerButton.Resize += (s, e) => SetRoundedRegion(registerButton, 20);
            // Enter key triggers login
            usernameBox.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) loginButton.PerformClick(); };
            passwordBox.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) loginButton.PerformClick(); };
            loginButton.Click += (s, e) =>
            {
                var username = usernameBox.Text.Trim();
                var password = passwordBox.Text;
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Please enter both username and password.");
                    return;
                }
                var file = GetUserFile();
                bool userExists = false;
                if (File.Exists(file))
                {
                    foreach (var line in File.ReadAllLines(file))
                    {
                        var parts = line.Split(':');
                        if (parts.Length == 2 && parts[0] == username)
                        {
                            userExists = true;
                            break;
                        }
                    }
                }
                if (!userExists)
                {
                    MessageBox.Show("Username does not exist.");
                    return;
                }
                if (CheckCredentials(username, password))
                {
                    Username = username;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalid password.");
                }
            };
            registerButton.Click += (s, e) =>
            {
                var username = usernameBox.Text.Trim();
                var password = passwordBox.Text;
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Please enter both username and password.");
                    return;
                }
                if (RegisterUser(username, password))
                {
                    MessageBox.Show("Registration successful. You can now log in.");
                }
                else
                {
                    MessageBox.Show("Username already exists.");
                }
            };
            panel.Controls.Add(titleLabel);
            panel.Controls.Add(usernameLabel);
            panel.Controls.Add(passwordLabel);
            panel.Controls.Add(usernameBox);
            panel.Controls.Add(passwordBox);
            panel.Controls.Add(loginButton);
            panel.Controls.Add(registerButton);
            this.Controls.Add(panel);
        }

        private string GetUserFile() => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.txt");
        private string Hash(string input)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                return Convert.ToBase64String(bytes);
            }
        }
        private bool CheckCredentials(string username, string password)
        {
            var file = GetUserFile();
            if (!File.Exists(file)) return false;
            var hash = Hash(username + password);
            foreach (var line in File.ReadAllLines(file))
            {
                var parts = line.Split(':');
                if (parts.Length == 2 && parts[0] == username && parts[1] == hash)
                    return true;
            }
            return false;
        }
        private bool RegisterUser(string username, string password)
        {
            var file = GetUserFile();
            if (File.Exists(file))
            {
                foreach (var line in File.ReadAllLines(file))
                {
                    var parts = line.Split(':');
                    if (parts.Length == 2 && parts[0] == username)
                        return false; // User already exists
                }
            }
            // Add new user
            using (var stream = new FileStream(file, FileMode.Append, FileAccess.Write, FileShare.None))
            using (var writer = new StreamWriter(stream))
            {
                var hash = Hash(username + password);
                writer.WriteLine($"{username}:{hash}");
            }
            return true;
        }
    }
}
