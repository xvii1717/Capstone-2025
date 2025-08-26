using System.IO;
using System.Text.Json;
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ProductivityApp
{
    public partial class Form1 : Form
    {
        private int currentYear = DateTime.Now.Year;
        private int currentMonth = DateTime.Now.Month;

        private const string CalendarDataFile = "calendarEvents.json";
        private const string TodoDataFile = "todoList.json";

        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            ShowMainMenu();
            LoadData();
            RenderCalendar(currentYear, currentMonth);
            calendarGridPanel.Resize += (s, e) =>
            {
                RenderCalendar(currentYear, currentMonth);
            };

            // To-Do List: Right-click to remove
            todoListBox.MouseUp += (s, e) =>
            {
                if (e.Button == MouseButtons.Right && todoListBox.SelectedItem != null)
                {
                    var result = MessageBox.Show($"Remove '{todoListBox.SelectedItem}'?", "Remove To-Do", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        todoListBox.Items.Remove(todoListBox.SelectedItem);
                    }
                }
            };

            this.FormClosing += (s, e) => SaveData();
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
                                // Right-click to remove event
                                eventLabel.MouseUp += (s2, e2) =>
                                {
                                    if (e2.Button == MouseButtons.Right)
                                    {
                                        var result = MessageBox.Show($"Remove event '{evt}'?", "Remove Event", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                        if (result == DialogResult.Yes)
                                        {
                                            calendarEvents[date].RemoveAt((int)eventLabel.Tag);
                                            if (calendarEvents[date].Count == 0)
                                                calendarEvents.Remove(date);
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
// ...existing code...

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
    }
}
