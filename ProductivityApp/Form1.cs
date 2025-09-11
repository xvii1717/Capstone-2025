using System.IO;
using System.Text.Json;
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ProductivityApp
{
    public partial class Form1 : Form
    {
        private int currentYear = DateTime.Now.Year;
        private int currentMonth = DateTime.Now.Month;
        public static string? CurrentUsername { get; set; }
        private string CalendarDataFile => $"calendarEvents_{CurrentUsername}.json";
        private string TodoDataFile => $"todoList_{CurrentUsername}.json";

        private Button logoutButton;

        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            ShowLoginPanel();
            calendarGridPanel.Resize += (s, e) =>
            {
                RenderCalendar(currentYear, currentMonth);
            };

            // To-Do List: double-click or right-click to remove and sync with calendar
            todoListBox.MouseUp += (s, e) =>
            {
                if ((e.Button == MouseButtons.Right || e.Button == MouseButtons.Left) && todoListBox.SelectedItem != null)
                {
                    var item = todoListBox.SelectedItem.ToString();
                    var result = MessageBox.Show($"Remove '{item}'? This will also remove it from the calendar.", "Remove To-Do", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        if (item != null) todoListBox.Items.Remove(item);
                        // Remove from calendar if present
                        foreach (var date in calendarEvents.Keys.ToList())
                        {
                            calendarEvents[date].RemoveAll(ev => ev.Contains(item));
                            if (calendarEvents[date].Count == 0)
                                calendarEvents.Remove(date);
                        }
                        SaveData();
                        RenderCalendar(currentYear, currentMonth);
                    }
                }
            };
            todoListBox.DoubleClick += (s, e) =>
            {
                if (todoListBox.SelectedItem != null)
                {
                    var item = todoListBox.SelectedItem.ToString();
                    var result = MessageBox.Show($"Remove '{item}'? This will also remove it from the calendar.", "Remove To-Do", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        if (item != null) todoListBox.Items.Remove(item);
                        foreach (var date in calendarEvents.Keys.ToList())
                        {
                            calendarEvents[date].RemoveAll(ev => ev.Contains(item));
                            if (calendarEvents[date].Count == 0)
                                calendarEvents.Remove(date);
                        }
                        SaveData();
                        RenderCalendar(currentYear, currentMonth);
                    }
                }
            };

            // Calendar event: right-click to remove and sync with to-do list
            this.FormClosing += (s, e) => SaveData();

            logoutButton = new Button {
                Text = "Logout",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Size = new Size(120, 40),
                BackColor = Color.FromArgb(60, 90, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            // Center horizontally and place below other buttons
            logoutButton.Location = new Point((mainMenuPanel.Width - logoutButton.Width) / 2, 260);
            mainMenuPanel.Resize += (s, e) => {
                logoutButton.Location = new Point((mainMenuPanel.Width - logoutButton.Width) / 2, 260);
            };
            logoutButton.Click += (s, e) => {
                LogoutUser();
            };
            mainMenuPanel.Controls.Add(logoutButton);
        }

        private void SaveData()
        {
            // Save calendar events
            var serializableEvents = calendarEvents.ToDictionary(
                kvp => kvp.Key.ToString("o"),
                kvp => kvp.Value
            );
            File.WriteAllText(CalendarDataFile, JsonSerializer.Serialize(serializableEvents));

            // Save to-do list
            var todos = new List<string>();
            foreach (var item in todoListBox.Items)
                todos.Add(item.ToString());
            File.WriteAllText(TodoDataFile, JsonSerializer.Serialize(todos));
        }

        private void LoadData()
        {
            // Load calendar events
            if (File.Exists(CalendarDataFile))
            {
                var json = File.ReadAllText(CalendarDataFile);
                var loaded = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json);
                calendarEvents.Clear();
                foreach (var kvp in loaded)
                {
                    if (DateTime.TryParse(kvp.Key, null, System.Globalization.DateTimeStyles.RoundtripKind, out var dt))
                        calendarEvents[dt] = kvp.Value;
                }
            }

            // Load to-do list
            if (File.Exists(TodoDataFile))
            {
                var json = File.ReadAllText(TodoDataFile);
                var todos = JsonSerializer.Deserialize<List<string>>(json);
                todoListBox.Items.Clear();
                foreach (var item in todos)
                    todoListBox.Items.Add(item);
            }
        }

        private void TodayButton_Click(object sender, EventArgs e)
        {
            currentYear = DateTime.Now.Year;
            currentMonth = DateTime.Now.Month;
            RenderCalendar(currentYear, currentMonth);
        }

        private void PrevMonthButton_Click(object sender, EventArgs e)
        {
            currentMonth--;
            if (currentMonth < 1)
            {
                currentMonth = 12;
                currentYear--;
            }
            RenderCalendar(currentYear, currentMonth);
        }

        private void NextMonthButton_Click(object sender, EventArgs e)
        {
            currentMonth++;
            if (currentMonth > 12)
            {
                currentMonth = 1;
                currentYear++;
            }
            RenderCalendar(currentYear, currentMonth);
        }

        private Dictionary<DateTime, List<string>> calendarEvents = new Dictionary<DateTime, List<string>>();

        private void RenderCalendar(int year, int month)
        {
            calendarGridPanel.Controls.Clear();
            calendarGridPanel.SuspendLayout();
            // Weekday headers
            string[] weekdays = { "SUN", "MON", "TUE", "WED", "THU", "FRI", "SAT" };
            int cellWidth = calendarGridPanel.Width / 7;
            int cellHeight = calendarGridPanel.Height / 7;
            Font headerFont = new Font("Segoe UI", 10, FontStyle.Bold);
            for (int i = 0; i < 7; i++)
            {
                var header = new Label
                {
                    Text = weekdays[i],
                    ForeColor = Color.White,
                    Font = headerFont,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Width = cellWidth,
                    Height = cellHeight,
                    Left = i * cellWidth,
                    Top = 0,
                    BackColor = Color.FromArgb(30, 30, 30)
                };
                calendarGridPanel.Controls.Add(header);
            }

            var firstDay = new DateTime(year, month, 1);
            int daysInMonth = DateTime.DaysInMonth(year, month);
            int startDayOfWeek = (int)firstDay.DayOfWeek;
            Font dayFont = new Font("Segoe UI", 10, FontStyle.Bold);
            Font eventFont = new Font("Segoe UI", 9, FontStyle.Regular);
            Color cellColor = Color.FromArgb(40, 40, 40);
            Color eventColor = Color.White;
            Color dotColor = Color.LightSkyBlue;
            for (int week = 0, day = 1 - startDayOfWeek; week < 6; week++)
            {
                for (int col = 0; col < 7; col++, day++)
                {
                    var cellPanel = new Panel
                    {
                        Width = cellWidth - 4,
                        Height = cellHeight - 4,
                        Left = col * cellWidth + 2,
                        Top = (week + 1) * cellHeight + 2,
                        BackColor = cellColor,
                        BorderStyle = BorderStyle.None,
                        Margin = new Padding(2),
                        Tag = (day > 0 && day <= daysInMonth) ? new DateTime(year, month, day) : null
                    };
                    cellPanel.Paint += (s, e) =>
                    {
                        var g = e.Graphics;
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        var rect = new Rectangle(0, 0, cellPanel.Width, cellPanel.Height);
                        int radius = 16;
                        var path = new System.Drawing.Drawing2D.GraphicsPath();
                        path.AddArc(rect.Left, rect.Top, radius, radius, 180, 90);
                        path.AddArc(rect.Right - radius, rect.Top, radius, radius, 270, 90);
                        path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
                        path.AddArc(rect.Left, rect.Bottom - radius, radius, radius, 90, 90);
                        path.CloseAllFigures();
                        g.FillPath(new SolidBrush(cellColor), path);
                    };
                    if (day > 0 && day <= daysInMonth)
                    {
                        var date = new DateTime(year, month, day);
                        var dayLabel = new Label
                        {
                            Text = day.ToString(),
                            ForeColor = Color.White,
                            Font = dayFont,
                            Location = new Point(8, 6),
                            AutoSize = true,
                            BackColor = Color.Transparent
                        };
                        cellPanel.Controls.Add(dayLabel);
                        if (calendarEvents.ContainsKey(date))
                        {
                            int y = 28;
                            for (int i = 0; i < calendarEvents[date].Count; i++)
                            {
                                var evt = calendarEvents[date][i];
                                var dot = new Label
                                {
                                    Text = "●",
                                    ForeColor = dotColor,
                                    Font = eventFont,
                                    Location = new Point(8, y),
                                    AutoSize = true,
                                    BackColor = Color.Transparent
                                };
                                cellPanel.Controls.Add(dot);
                                var eventLabel = new Label
                                {
                                    Text = evt,
                                    ForeColor = eventColor,
                                    Font = eventFont,
                                    Location = new Point(24, y),
                                    AutoSize = true,
                                    BackColor = Color.Transparent,
                                    Tag = i // index for removal
                                };
                                // Right-click to remove event and sync with to-do list
                                eventLabel.MouseUp += (s2, e2) =>
                                {
                                    if (e2.Button == MouseButtons.Right)
                                    {
                                        var result = MessageBox.Show($"Remove event '{evt}'? This will also remove it from the to-do list if present.", "Remove Event", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                        if (result == DialogResult.Yes)
                                        {
                                            calendarEvents[date].RemoveAt((int)eventLabel.Tag);
                                            if (calendarEvents[date].Count == 0)
                                                calendarEvents.Remove(date);
                                            // Remove from to-do list if present
                                            foreach (var todo in todoListBox.Items.Cast<string>().ToList())
                                            {
                                                // Extract base to-do text (before date in parentheses)
                                                var baseTodo = todo;
                                                int idx = baseTodo.IndexOf("(");
                                                if (idx > 0)
                                                    baseTodo = baseTodo.Substring(0, idx).Trim();
                                                // Remove "To-Do:" prefix if present
                                                if (baseTodo.StartsWith("To-Do:"))
                                                    baseTodo = baseTodo.Substring(7).Trim();
                                                // Extract base event text (remove "To-Do:" if present)
                                                var baseEvt = evt;
                                                if (baseEvt.StartsWith("To-Do:"))
                                                    baseEvt = baseEvt.Substring(7).Trim();
                                                // Compare base texts
                                                if (string.Equals(baseTodo, baseEvt, StringComparison.OrdinalIgnoreCase))
                                                    todoListBox.Items.Remove(todo);
                                            }
                                            SaveData();
                                            RenderCalendar(year, month);
                                        }
                                    }
                                };
                                cellPanel.Controls.Add(eventLabel);
                                y += 22;
                            }
                        }
                        // Add click event to add event to this day
                        cellPanel.Click += (s, e) =>
                        {
                            string input = Microsoft.VisualBasic.Interaction.InputBox($"Add event for {date.ToShortDateString()}", "Add Event", "");
                            if (!string.IsNullOrWhiteSpace(input))
                            {
                                if (!calendarEvents.ContainsKey(date))
                                    calendarEvents[date] = new List<string>();
                                calendarEvents[date].Add(input);
                                RenderCalendar(year, month);
                            }
                        };
                    }
                    calendarGridPanel.Controls.Add(cellPanel);
                }
            }
            calendarGridPanel.ResumeLayout();
        }

        private void addTodoButton_Click(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Enter a new to-do item:", "Add To-Do", "");
            if (!string.IsNullOrWhiteSpace(input))
            {
                var result = MessageBox.Show("Do you want to assign a date to this to-do item?", "Assign Date", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    using (var datePicker = new MonthCalendar())
                    {
                        Form pickerForm = new Form
                        {
                            Text = "Pick a Date",
                            Size = new System.Drawing.Size(250, 220),
                            StartPosition = FormStartPosition.CenterParent
                        };
                        datePicker.MaxSelectionCount = 1;
                        datePicker.Dock = DockStyle.Fill;
                        pickerForm.Controls.Add(datePicker);
                        Button okButton = new Button
                        {
                            Text = "OK",
                            Dock = DockStyle.Bottom,
                            DialogResult = DialogResult.OK
                        };
                        pickerForm.Controls.Add(okButton);
                        pickerForm.AcceptButton = okButton;
                        if (pickerForm.ShowDialog() == DialogResult.OK)
                        {
                            DateTime selectedDate = datePicker.SelectionStart;
                            todoListBox.Items.Add($"{input} ({selectedDate.ToShortDateString()})");
                            // Add to calendar events
                            if (!calendarEvents.ContainsKey(selectedDate))
                                calendarEvents[selectedDate] = new List<string>();
                            calendarEvents[selectedDate].Add("To-Do: " + input);
                            RenderCalendar(selectedDate.Year, selectedDate.Month);
                        }
                        else
                        {
                            todoListBox.Items.Add(input);
                        }
                    }
                }
                else
                {
                    todoListBox.Items.Add(input);
                }
            }
        }

        private void calendarViewComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Custom calendar: view switching logic can be implemented here if needed
        }

        private void addEventButton_Click(object sender, EventArgs e)
        {
            var selectedDate = DateTime.Now;
            string input = Microsoft.VisualBasic.Interaction.InputBox($"Add event for {selectedDate.ToShortDateString()}", "Add Event", "");
            if (!string.IsNullOrWhiteSpace(input))
            {
                eventListBox.Items.Add($"{selectedDate.ToShortDateString()}: {input}");
            }
        }
            
            private void ShowMainMenu()
            {
                mainMenuPanel.Visible = true;
                calendarPanel.Visible = false;
                todoPanel.Visible = false;
            }
            
            private void ShowCalendar()
            {
                mainMenuPanel.Visible = false;
                calendarPanel.Visible = true;
                todoPanel.Visible = false;
            }
            
            private void ShowTodo()
            {
                mainMenuPanel.Visible = false;
                calendarPanel.Visible = false;
                todoPanel.Visible = true;
            }
            
            private void settingsButton_Click(object sender, EventArgs e)
            {
                using (var settingsForm = new SettingsForm())
                {
                    settingsForm.ShowDialog();
                }
            }
            
            private void calendarButton_Click(object sender, EventArgs e)
            {
                ShowCalendar();
            }
            
            private void todoButton_Click(object sender, EventArgs e)
            {
                ShowTodo();
            }
            
            private void backButton_Click(object sender, EventArgs e)
            {
                ShowMainMenu();
            }

            private void todoBackButton_Click(object sender, EventArgs e)
            {
                ShowMainMenu();
            }

            // Authentication Methods
            private string GetUserFile() => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.txt");
            private string GetSessionFile() => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "session.txt");
            private string GetSecurityFile() => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "security.txt");
            
            private string HashPassword(string username, string password, string salt)
            {
                using (var pbkdf2 = new Rfc2898DeriveBytes(password + username, Encoding.UTF8.GetBytes(salt), 10000))
                {
                    return Convert.ToBase64String(pbkdf2.GetBytes(32));
                }
            }
            
            private string GenerateSalt()
            {
                using (var rng = RandomNumberGenerator.Create())
                {
                    byte[] saltBytes = new byte[16];
                    rng.GetBytes(saltBytes);
                    return Convert.ToBase64String(saltBytes);
                }
            }

            private void LogoutUser()
            {
                CurrentUsername = null;
                ClearRememberMe();
                ShowLoginPanel();
            }

            private void SaveRememberMe(string username)
            {
                var sessionData = $"{username}:{DateTime.Now.AddDays(30):o}";
                File.WriteAllText(GetSessionFile(), sessionData);
            }

            private void ClearRememberMe()
            {
                if (File.Exists(GetSessionFile()))
                    File.Delete(GetSessionFile());
            }

            private string? CheckRememberMe()
            {
                if (!File.Exists(GetSessionFile())) return null;
                
                try
                {
                    var sessionData = File.ReadAllText(GetSessionFile());
                    var parts = sessionData.Split(':');
                    if (parts.Length >= 2)
                    {
                        var username = parts[0];
                        var expiryStr = string.Join(":", parts.Skip(1));
                        if (DateTime.TryParse(expiryStr, out var expiry) && expiry > DateTime.Now)
                        {
                            return username;
                        }
                    }
                }
                catch { }
                
                ClearRememberMe();
                return null;
            }

            private bool IsAccountLocked(string username)
            {
                if (!File.Exists(GetSecurityFile())) return false;
                
                try
                {
                    foreach (var line in File.ReadAllLines(GetSecurityFile()))
                    {
                        var parts = line.Split(':');
                        if (parts.Length == 3 && parts[0] == username)
                        {
                            var attempts = int.Parse(parts[1]);
                            var lastAttempt = DateTime.Parse(parts[2]);
                            
                            // Lock for 15 minutes after 5 failed attempts
                            if (attempts >= 5 && DateTime.Now.Subtract(lastAttempt).TotalMinutes < 15)
                                return true;
                        }
                    }
                }
                catch { }
                
                return false;
            }

            private void RecordFailedAttempt(string username)
            {
                var securityFile = GetSecurityFile();
                var lines = File.Exists(securityFile) ? File.ReadAllLines(securityFile).ToList() : new List<string>();
                
                var userLineIndex = -1;
                var attempts = 1;
                
                for (int i = 0; i < lines.Count; i++)
                {
                    var parts = lines[i].Split(':');
                    if (parts.Length >= 1 && parts[0] == username)
                    {
                        userLineIndex = i;
                        if (parts.Length >= 2 && int.TryParse(parts[1], out var existingAttempts))
                        {
                            var lastAttempt = parts.Length >= 3 ? DateTime.Parse(parts[2]) : DateTime.MinValue;
                            
                            // Reset attempts if last attempt was more than 15 minutes ago
                            if (DateTime.Now.Subtract(lastAttempt).TotalMinutes > 15)
                                attempts = 1;
                            else
                                attempts = existingAttempts + 1;
                        }
                        break;
                    }
                }
                
                var newLine = $"{username}:{attempts}:{DateTime.Now:o}";
                
                if (userLineIndex >= 0)
                    lines[userLineIndex] = newLine;
                else
                    lines.Add(newLine);
                
                File.WriteAllLines(securityFile, lines);
            }

            private void ClearFailedAttempts(string username)
            {
                var securityFile = GetSecurityFile();
                if (!File.Exists(securityFile)) return;
                
                var lines = File.ReadAllLines(securityFile).Where(line => 
                    !line.StartsWith(username + ":")).ToArray();
                
                if (lines.Length == 0)
                    File.Delete(securityFile);
                else
                    File.WriteAllLines(securityFile, lines);
            }

            private string GenerateSecurityQuestion(string username)
            {
                // Simple security question for password recovery
                return $"What is your favorite color? (Set during registration for {username})";
            }

            private bool ValidateSecurityAnswer(string username, string answer)
            {
                var userFile = GetUserFile();
                if (!File.Exists(userFile)) return false;
                
                foreach (var line in File.ReadAllLines(userFile))
                {
                    var parts = line.Split(':');
                    if (parts.Length >= 4 && parts[0] == username)
                    {
                        return parts[3].Equals(answer, StringComparison.OrdinalIgnoreCase);
                    }
                }
                return false;
            }

            private void ResetPassword(string username, string newPassword)
            {
                var userFile = GetUserFile();
                if (!File.Exists(userFile)) return;
                
                var lines = File.ReadAllLines(userFile).ToList();
                for (int i = 0; i < lines.Count; i++)
                {
                    var parts = lines[i].Split(':');
                    if (parts.Length >= 3 && parts[0] == username)
                    {
                        var salt = GenerateSalt();
                        var hash = HashPassword(username, newPassword, salt);
                        var securityAnswer = parts.Length >= 4 ? parts[3] : "";
                        lines[i] = $"{username}:{salt}:{hash}:{securityAnswer}";
                        break;
                    }
                }
                File.WriteAllLines(userFile, lines);
            }

            private int CalculatePasswordStrength(string password)
            {
                int score = 0;
                if (password.Length >= 8) score++;
                if (password.Length >= 12) score++;
                if (password.Any(char.IsUpper)) score++;
                if (password.Any(char.IsLower)) score++;
                if (password.Any(char.IsDigit)) score++;
                if (password.Any(c => "!@#$%^&*()_+-=[]{}|;:,.<>?".Contains(c))) score++;
                return Math.Min(score, 5);
            }

            private string GetPasswordStrengthText(int strength)
            {
                return strength switch
                {
                    0 or 1 => "Very Weak",
                    2 => "Weak", 
                    3 => "Fair",
                    4 => "Good",
                    5 => "Strong",
                    _ => "Unknown"
                };
            }

            private Color GetPasswordStrengthColor(int strength)
            {
                return strength switch
                {
                    0 or 1 => Color.Red,
                    2 => Color.Orange,
                    3 => Color.Yellow,
                    4 => Color.LightGreen,
                    5 => Color.Green,
                    _ => Color.Gray
                };
            }
            
            private bool CheckCredentials(string username, string password)
            {
                var file = GetUserFile();
                if (!File.Exists(file)) return false;
                
                foreach (var line in File.ReadAllLines(file))
                {
                    var parts = line.Split(':');
                    if (parts.Length >= 3 && parts[0] == username)
                    {
                        var storedSalt = parts[1];
                        var storedHash = parts[2];
                        var inputHash = HashPassword(username, password, storedSalt);
                        return storedHash == inputHash;
                    }
                }
                return false;
            }
            
            private bool RegisterUser(string username, string password, string securityAnswer = "")
            {
                var file = GetUserFile();
                if (File.Exists(file))
                {
                    foreach (var line in File.ReadAllLines(file))
                    {
                        var parts = line.Split(':');
                        if (parts.Length >= 1 && parts[0] == username)
                            return false; // User already exists
                    }
                }
                
                // Add new user with salt, hash, and security answer
                var salt = GenerateSalt();
                var hash = HashPassword(username, password, salt);
                
                using (var stream = new FileStream(file, FileMode.Append, FileAccess.Write, FileShare.None))
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine($"{username}:{salt}:{hash}:{securityAnswer}");
                }
                return true;
            }

            // Login Panel Methods
            private void ShowLoginPanel()
            {
                mainMenuPanel.Visible = false;
                calendarPanel.Visible = false;
                todoPanel.Visible = false;
                
                // Check for remembered session first
                var rememberedUser = CheckRememberMe();
                if (rememberedUser != null)
                {
                    CurrentUsername = rememberedUser;
                    LoadData();
                    ShowMainMenu();
                    RenderCalendar(currentYear, currentMonth);
                    return;
                }
                
                // Create login panel if it doesn't exist
                if (this.Controls["loginPanel"] == null)
                {
                    CreateLoginPanel();
                }
                
                this.Controls["loginPanel"].Visible = true;
                this.Controls["loginPanel"].BringToFront();
            }
            
            private void CreateLoginPanel()
            {
                var loginPanel = new Panel
                {
                    Name = "loginPanel",
                    Dock = DockStyle.Fill,
                    BackColor = Color.Black
                };
                
                // Gradient background
                loginPanel.Paint += (s, e) => {
                    var rect = loginPanel.ClientRectangle;
                    using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(rect, Color.FromArgb(20, 30, 60), Color.FromArgb(40, 60, 120), 45F))
                    {
                        e.Graphics.FillRectangle(brush, rect);
                    }
                };
                
                // Center container
                var centerPanel = new Panel
                {
                    Size = new Size(450, 650),
                    BackColor = Color.Transparent,
                    Anchor = AnchorStyles.None
                };
                
                // Position center panel
                loginPanel.Resize += (s, e) => {
                    centerPanel.Location = new Point((loginPanel.Width - centerPanel.Width) / 2, (loginPanel.Height - centerPanel.Height) / 2);
                };
                
                // Title
                var titleLabel = new Label
                {
                    Text = "Welcome to\nProductivityApp",
                    Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                    ForeColor = Color.White,
                    Dock = DockStyle.Top,
                    Height = 100,
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = Color.Transparent
                };
                
                // Username controls
                var usernameLabel = new Label
                {
                    Text = "Username:",
                    Left = 40,
                    Top = 120,
                    Width = 100,
                    Font = new Font("Segoe UI", 12F),
                    ForeColor = Color.White,
                    BackColor = Color.Transparent
                };
                
                var usernameBox = new TextBox
                {
                    Name = "usernameBox",
                    Left = 160,
                    Top = 120,
                    Width = 200,
                    Font = new Font("Segoe UI", 12F),
                    BackColor = Color.FromArgb(30, 30, 30),
                    ForeColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };
                
                // Password controls
                var passwordLabel = new Label
                {
                    Text = "Password:",
                    Left = 40,
                    Top = 180,
                    Width = 100,
                    Font = new Font("Segoe UI", 12F),
                    ForeColor = Color.White,
                    BackColor = Color.Transparent
                };
                
                var passwordBox = new TextBox
                {
                    Name = "passwordBox",
                    Left = 160,
                    Top = 180,
                    Width = 170,
                    UseSystemPasswordChar = true,
                    Font = new Font("Segoe UI", 12F),
                    BackColor = Color.FromArgb(30, 30, 30),
                    ForeColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };
                
                // Show/Hide password button
                var showPasswordButton = new Button
                {
                    Text = "👁",
                    Left = 340,
                    Top = 180,
                    Width = 30,
                    Height = passwordBox.Height,
                    Font = new Font("Segoe UI", 10F),
                    BackColor = Color.FromArgb(60, 60, 60),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                showPasswordButton.FlatAppearance.BorderSize = 0;
                
                // Password strength indicator
                var strengthLabel = new Label
                {
                    Text = "",
                    Left = 160,
                    Top = 210,
                    Width = 200,
                    Font = new Font("Segoe UI", 9F),
                    BackColor = Color.Transparent
                };
                
                // Remember me checkbox
                var rememberCheckBox = new CheckBox
                {
                    Text = "Remember me for 30 days",
                    Left = 40,
                    Top = 240,
                    Width = 200,
                    Font = new Font("Segoe UI", 10F),
                    ForeColor = Color.White,
                    BackColor = Color.Transparent
                };
                
                // Security question for registration
                var securityLabel = new Label
                {
                    Text = "Security Question: What is your favorite color?",
                    Left = 40,
                    Top = 270,
                    Width = 320,
                    Font = new Font("Segoe UI", 10F),
                    ForeColor = Color.LightGray,
                    BackColor = Color.Transparent,
                    Visible = false
                };
                
                var securityBox = new TextBox
                {
                    Name = "securityBox",
                    Left = 40,
                    Top = 295,
                    Width = 320,
                    Font = new Font("Segoe UI", 11F),
                    BackColor = Color.FromArgb(30, 30, 30),
                    ForeColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle,
                    Visible = false
                };
                
                // Buttons
                var loginButton = new Button
                {
                    Text = "Login",
                    Left = 40,
                    Top = 340,
                    Width = 100,
                    Height = 40,
                    Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                    BackColor = Color.FromArgb(40, 60, 120),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                loginButton.FlatAppearance.BorderSize = 0;
                
                var registerButton = new Button
                {
                    Text = "Register",
                    Left = 160,
                    Top = 340,
                    Width = 100,
                    Height = 40,
                    Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                    BackColor = Color.FromArgb(60, 90, 180),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                registerButton.FlatAppearance.BorderSize = 0;
                
                var forgotPasswordButton = new Button
                {
                    Text = "Forgot Password?",
                    Left = 280,
                    Top = 340,
                    Width = 120,
                    Height = 40,
                    Font = new Font("Segoe UI", 10F),
                    BackColor = Color.FromArgb(80, 80, 80),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                forgotPasswordButton.FlatAppearance.BorderSize = 0;
                
                // Event handlers
                showPasswordButton.Click += (s, e) =>
                {
                    passwordBox.UseSystemPasswordChar = !passwordBox.UseSystemPasswordChar;
                    showPasswordButton.Text = passwordBox.UseSystemPasswordChar ? "👁" : "🙈";
                };
                
                passwordBox.TextChanged += (s, e) =>
                {
                    if (securityBox.Visible) // Only show strength during registration
                    {
                        var strength = CalculatePasswordStrength(passwordBox.Text);
                        strengthLabel.Text = $"Password Strength: {GetPasswordStrengthText(strength)}";
                        strengthLabel.ForeColor = GetPasswordStrengthColor(strength);
                    }
                };
                
                registerButton.Click += (s, e) =>
                {
                    if (!securityBox.Visible)
                    {
                        // Show registration fields
                        securityLabel.Visible = true;
                        securityBox.Visible = true;
                        strengthLabel.Visible = true;
                        centerPanel.Height = 700;
                        registerButton.Text = "Complete Registration";
                        registerButton.Top = 390;
                        loginButton.Top = 390;
                        forgotPasswordButton.Top = 390;
                        return;
                    }
                    
                    var username = usernameBox.Text.Trim();
                    var password = passwordBox.Text;
                    var securityAnswer = securityBox.Text.Trim();
                    
                    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(securityAnswer))
                    {
                        MessageBox.Show("Please fill in all fields.");
                        return;
                    }
                    
                    var strength = CalculatePasswordStrength(password);
                    if (strength < 3)
                    {
                        MessageBox.Show("Password is too weak. Please use a stronger password with uppercase, lowercase, numbers, and symbols.");
                        return;
                    }
                    
                    if (RegisterUser(username, password, securityAnswer))
                    {
                        MessageBox.Show("Registration successful! You can now log in.");
                        // Reset form
                        passwordBox.Clear();
                        securityBox.Clear();
                        securityLabel.Visible = false;
                        securityBox.Visible = false;
                        strengthLabel.Visible = false;
                        centerPanel.Height = 650;
                        registerButton.Text = "Register";
                        registerButton.Top = 340;
                        loginButton.Top = 340;
                        forgotPasswordButton.Top = 340;
                    }
                    else
                    {
                        MessageBox.Show("Username already exists. Please choose a different username.");
                    }
                };
                
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
                    
                    if (IsAccountLocked(username))
                    {
                        MessageBox.Show("Account is temporarily locked due to multiple failed login attempts. Please try again in 15 minutes.");
                        return;
                    }
                    
                    if (CheckCredentials(username, password))
                    {
                        CurrentUsername = username;
                        ClearFailedAttempts(username);
                        
                        if (rememberCheckBox.Checked)
                        {
                            SaveRememberMe(username);
                        }
                        
                        loginPanel.Visible = false;
                        LoadData();
                        ShowMainMenu();
                        RenderCalendar(currentYear, currentMonth);
                    }
                    else
                    {
                        RecordFailedAttempt(username);
                        MessageBox.Show("Invalid username or password.");
                        passwordBox.Clear();
                    }
                };
                
                forgotPasswordButton.Click += (s, e) =>
                {
                    var username = usernameBox.Text.Trim();
                    if (string.IsNullOrWhiteSpace(username))
                    {
                        MessageBox.Show("Please enter your username first.");
                        return;
                    }
                    
                    var question = GenerateSecurityQuestion(username);
                    var answer = Microsoft.VisualBasic.Interaction.InputBox(question, "Security Question", "");
                    
                    if (string.IsNullOrWhiteSpace(answer))
                        return;
                    
                    if (ValidateSecurityAnswer(username, answer))
                    {
                        var newPassword = Microsoft.VisualBasic.Interaction.InputBox("Enter your new password:", "Reset Password", "");
                        if (!string.IsNullOrWhiteSpace(newPassword) && newPassword.Length >= 6)
                        {
                            ResetPassword(username, newPassword);
                            MessageBox.Show("Password reset successfully! You can now log in with your new password.");
                            passwordBox.Clear();
                        }
                        else
                        {
                            MessageBox.Show("Password must be at least 6 characters long.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Incorrect security answer.");
                    }
                };
                
                // Add controls to center panel
                centerPanel.Controls.Add(titleLabel);
                centerPanel.Controls.Add(usernameLabel);
                centerPanel.Controls.Add(passwordLabel);
                centerPanel.Controls.Add(usernameBox);
                centerPanel.Controls.Add(passwordBox);
                centerPanel.Controls.Add(showPasswordButton);
                centerPanel.Controls.Add(strengthLabel);
                centerPanel.Controls.Add(rememberCheckBox);
                centerPanel.Controls.Add(securityLabel);
                centerPanel.Controls.Add(securityBox);
                centerPanel.Controls.Add(loginButton);
                centerPanel.Controls.Add(registerButton);
                centerPanel.Controls.Add(forgotPasswordButton);
                
                loginPanel.Controls.Add(centerPanel);
                this.Controls.Add(loginPanel);
                
                // Initial positioning
                centerPanel.Location = new Point((loginPanel.Width - centerPanel.Width) / 2, (loginPanel.Height - centerPanel.Height) / 2);
            }
    }
}
