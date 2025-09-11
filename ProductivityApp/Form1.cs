using System.IO;
using System.Text.Json;
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using ProductivityApp.Data;

namespace ProductivityApp
{
    public static class GraphicsExtensions
    {
        public static void FillRoundedRectangle(this Graphics g, Brush brush, Rectangle rect, int radius)
        {
            using (var path = GetRoundedRectanglePath(rect, radius))
            {
                g.FillPath(brush, path);
            }
        }

        public static void DrawRoundedRectangle(this Graphics g, Pen pen, Rectangle rect, int radius)
        {
            using (var path = GetRoundedRectanglePath(rect, radius))
            {
                g.DrawPath(pen, path);
            }
        }

        private static GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(rect.Left, rect.Top, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Top, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.Left, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseAllFigures();
            return path;
        }
    }
    public partial class Form1 : Form
    {
        public string? CurrentUsername { get; set; }
        private Timer animationTimer;
        private float animationProgress = 0f;
        private Panel? animatingFromPanel;
        private Panel? animatingToPanel;
        private bool isAnimating = false;
        
        // Interactive card regions for click detection
        private Rectangle calendarCardRect;
        private Rectangle todoCardRect;
        private Rectangle settingsCardRect;

        private int currentYear = DateTime.Now.Year;
        private int currentMonth = DateTime.Now.Month;
        private string CalendarDataFile => $"calendarEvents_{CurrentUsername}.json";
        private string TodoDataFile => $"todoList_{CurrentUsername}.json";

        private Panel currentPanel;
        private Panel targetPanel;
        private int animationStep = 0;
        private const int animationSteps = 10;

        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            
            // Initialize animation timer
            animationTimer = new Timer();
            animationTimer.Interval = 16; // ~60 FPS
            animationTimer.Tick += AnimationTimer_Tick;
            
            // Hide old buttons - we'll use interactive cards instead
            settingsButton.Visible = false;
            calendarButton.Visible = false;
            todoButton.Visible = false;
            
            // Add mouse click handler for interactive cards
            mainMenuPanel.MouseClick += MainMenuPanel_MouseClick;
            
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

        }

        // Animation system for smooth transitions
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (currentPanel == null || targetPanel == null) return;

            animationStep++;
            float progress = (float)animationStep / animationSteps;
            
            // Easing function for smooth animation
            progress = EaseInOutCubic(progress);
            
            // Use location-based slide animation instead of opacity
            var slideDistance = 50;
            var currentX = (int)(slideDistance * (1 - progress));
            var targetX = (int)(-slideDistance * progress);
            
            currentPanel.Location = new Point(currentX, currentPanel.Location.Y);
            targetPanel.Location = new Point(targetX, targetPanel.Location.Y);
            targetPanel.Visible = true;
            
            if (animationStep >= animationSteps)
            {
                // Animation complete
                currentPanel.Visible = false;
                currentPanel.Location = new Point(0, currentPanel.Location.Y);
                targetPanel.Location = new Point(0, targetPanel.Location.Y);
                
                animationTimer.Stop();
                currentPanel = null;
                targetPanel = null;
                animationStep = 0;
            }
        }
        
        private float EaseInOutCubic(float t)
        {
            return t < 0.5f ? 4 * t * t * t : 1 - (float)Math.Pow(-2 * t + 2, 3) / 2;
        }
        
        private async void AnimateToPanel(Panel from, Panel to)
        {
            if (animationTimer.Enabled) return; // Already animating
            
            currentPanel = from;
            targetPanel = to;
            animationStep = 0;
            
            // Start animation
            animationTimer.Start();
        }

        private void AnimateTextBoxFocus(TextBox textBox, bool focused)
        {
            var targetColor = focused ? Color.FromArgb(100, 150, 255) : Color.FromArgb(60, 255, 255, 255);
            var targetBorderColor = focused ? Color.FromArgb(70, 130, 180) : Color.FromArgb(80, 255, 255, 255);
            
            // Create smooth color transition
            var timer = new Timer { Interval = 16 };
            var steps = 10;
            var currentStep = 0;
            var originalBackColor = textBox.BackColor;
            
            timer.Tick += (s, e) =>
            {
                currentStep++;
                float progress = (float)currentStep / steps;
                progress = EaseInOutCubic(progress);
                
                if (currentStep >= steps)
                {
                    timer.Stop();
                    timer.Dispose();
                }
                
                textBox.Invalidate();
            };
            
            timer.Start();
        }

        // Modern UI Paint Events
        private void MainMenuPanel_Paint(object sender, PaintEventArgs e)
        {
            var panel = sender as Panel;
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Create gradient background with more depth
            var rect = panel.ClientRectangle;
            using (var brush = new LinearGradientBrush(
                rect, Color.FromArgb(10, 10, 18), Color.FromArgb(20, 25, 35), 135f))
            {
                g.FillRectangle(brush, rect);
            }
            
            // Add subtle overlay pattern
            using (var overlayBrush = new LinearGradientBrush(
                new Rectangle(0, 0, rect.Width, rect.Height / 3),
                Color.FromArgb(15, 255, 255, 255), Color.FromArgb(3, 255, 255, 255), 90f))
            {
                g.FillRectangle(overlayBrush, new Rectangle(0, 0, rect.Width, rect.Height / 3));
            }
            
            // Subtle animated particles effect
            var random = new Random(DateTime.Now.Millisecond);
            using (var particleBrush = new SolidBrush(Color.FromArgb(20, 120, 220, 255)))
            {
                for (int i = 0; i < 12; i++)
                {
                    var x = random.Next(rect.Width);
                    var y = random.Next(rect.Height);
                    var size = random.Next(1, 3);
                    g.FillEllipse(particleBrush, x, y, size, size);
                }
            }
            
            // Modern grid layout with interactive cards
            var cardWidth = 220;
            var cardHeight = 100;
            var cardSpacing = 25;
            var gridStartX = (rect.Width - (cardWidth * 3 + cardSpacing * 2)) / 2;
            var gridStartY = 50;
            
            // Top row - Main navigation cards (clickable) - store rectangles for click detection
            calendarCardRect = new Rectangle(gridStartX, gridStartY, cardWidth, cardHeight);
            todoCardRect = new Rectangle(gridStartX + cardWidth + cardSpacing, gridStartY, cardWidth, cardHeight);
            settingsCardRect = new Rectangle(gridStartX + (cardWidth + cardSpacing) * 2, gridStartY, cardWidth, cardHeight);
            
            DrawInteractiveCard(g, calendarCardRect, "📅 Calendar", "Manage your schedule", Color.FromArgb(70, 130, 180), "calendar");
            DrawInteractiveCard(g, todoCardRect, "✅ To-Do List", "Track your tasks", Color.FromArgb(255, 87, 34), "todo");
            DrawInteractiveCard(g, settingsCardRect, "⚙️ Settings", "Configure your app", Color.FromArgb(108, 117, 125), "settings");
            
            // Middle row - Information cards
            var middleRowY = gridStartY + cardHeight + cardSpacing;
            DrawDashboardCard(g, new Rectangle(gridStartX, middleRowY, cardWidth, cardHeight), 
                "🤖 JARVIS Assistant", "AI-powered productivity", Color.FromArgb(156, 39, 176));
            DrawWeatherWidget(g, new Rectangle(gridStartX + cardWidth + cardSpacing, middleRowY, cardWidth, cardHeight));
            DrawDashboardCard(g, new Rectangle(gridStartX + (cardWidth + cardSpacing) * 2, middleRowY, cardWidth, cardHeight), 
                "📊 Analytics", "Track your progress", Color.FromArgb(40, 167, 69));
            
            // Bottom row - Quick stats
            var bottomRowY = middleRowY + cardHeight + cardSpacing;
            DrawDashboardCard(g, new Rectangle(gridStartX, bottomRowY, cardWidth, cardHeight), 
                "📈 Today's Events", GetTodayEventsCount() + " scheduled", Color.FromArgb(0, 188, 212));
            DrawDashboardCard(g, new Rectangle(gridStartX + cardWidth + cardSpacing, bottomRowY, cardWidth, cardHeight), 
                "🎯 Active Tasks", GetTodoCount() + " pending", Color.FromArgb(255, 193, 7));
            DrawDashboardCard(g, new Rectangle(gridStartX + (cardWidth + cardSpacing) * 2, bottomRowY, cardWidth, cardHeight), 
                "⚡ Quick Stats", $"Welcome back, {CurrentUsername ?? "User"}!", Color.FromArgb(220, 53, 69));
            
            // Time and date display (positioned below cards)
            var now = DateTime.Now;
            var centerX = rect.Width / 2;
            var timeY = bottomRowY + cardHeight + 30;
            
            using (var timeFont = new Font(new FontFamily("Segoe UI"), 24F, FontStyle.Bold))
            using (var timeBrush = new SolidBrush(Color.White))
            {
                var timeText = now.ToString("HH:mm");
                var timeSize = g.MeasureString(timeText, timeFont);
                var timeX = centerX - (timeSize.Width / 2);
                g.DrawString(timeText, timeFont, timeBrush, timeX, timeY);
            }
            
            using (var dateFont = new Font(new FontFamily("Segoe UI"), 11F, FontStyle.Regular))
            using (var dateBrush = new SolidBrush(Color.FromArgb(180, 180, 200)))
            {
                var dateText = now.ToString("dddd, MMMM dd, yyyy");
                var dateSize = g.MeasureString(dateText, dateFont);
                var dateX = centerX - (dateSize.Width / 2);
                g.DrawString(dateText, dateFont, dateBrush, dateX, timeY + 35);
            }
            
            // Productivity quote/tip
            using (var quoteFont = new Font(new FontFamily("Segoe UI"), 10F, FontStyle.Italic))
            using (var quoteBrush = new SolidBrush(Color.FromArgb(140, 160, 180)))
            {
                var quote = GetProductivityTip();
                var quoteSize = g.MeasureString(quote, quoteFont);
                var quoteX = centerX - (quoteSize.Width / 2);
                g.DrawString(quote, quoteFont, quoteBrush, quoteX, timeY + 60);
            }
            
            // Bottom status bar
            DrawStatusBar(g, new Rectangle(0, rect.Height - 40, rect.Width, 40));
        }
        
        private void MainMenuPanel_MouseClick(object sender, MouseEventArgs e)
        {
            // Check if click is within any interactive card
            if (calendarCardRect.Contains(e.Location))
            {
                calendarButton_Click(sender, e);
            }
            else if (todoCardRect.Contains(e.Location))
            {
                todoButton_Click(sender, e);
            }
            else if (settingsCardRect.Contains(e.Location))
            {
                settingsButton_Click(sender, e);
            }
        }
        
        private void DrawDashboardCard(Graphics g, Rectangle rect, string title, string subtitle, Color accentColor)
        {
            // Card background with glass morphism effect
            using (var cardBrush = new SolidBrush(Color.FromArgb(60, 255, 255, 255)))
            using (var borderPen = new Pen(Color.FromArgb(80, 255, 255, 255), 1))
            using (var cardPath = GetRoundedRectanglePath(rect, 15))
            {
                g.FillPath(cardBrush, cardPath);
                g.DrawPath(borderPen, cardPath);
            }
            
            // Accent bar
            using (var accentBrush = new SolidBrush(accentColor))
            using (var accentPath = GetRoundedRectanglePath(new Rectangle(rect.X, rect.Y, 5, rect.Height), 3))
            {
                g.FillPath(accentBrush, accentPath);
            }
            
            // Title
            using (var titleFont = new Font("Segoe UI", 14F, FontStyle.Bold))
            using (var titleBrush = new SolidBrush(Color.White))
            {
                g.DrawString(title, titleFont, titleBrush, rect.X + 20, rect.Y + 15);
            }
            
            // Subtitle
            using (var subtitleFont = new Font("Segoe UI", 11F, FontStyle.Regular))
            using (var subtitleBrush = new SolidBrush(Color.FromArgb(200, 200, 220)))
            {
                g.DrawString(subtitle, subtitleFont, subtitleBrush, rect.X + 20, rect.Y + 45);
            }
        }
        
        private string GetTodayEventsCount()
        {
            try
            {
                using (var context = new ProductivityDbContext())
                {
                    var today = DateTime.Today;
                    var count = context.CalendarEvents
                        .Where(e => e.EventDate.Date == today)
                        .Count();
                    return count.ToString();
                }
            }
            catch
            {
                return "0";
            }
        }
        
        private string GetTodoCount()
        {
            try
            {
                using (var context = new ProductivityDbContext())
                {
                    var count = context.TodoItems
                        .Where(t => !t.IsCompleted)
                        .Count();
                    return count.ToString();
                }
            }
            catch
            {
                return "0";
            }
        }
        
        private string GetProductivityTip()
        {
            var tips = new[]
            {
                "💡 Focus on one task at a time for better productivity",
                "🎯 Set clear goals for each day",
                "⏰ Take regular breaks to maintain focus",
                "📝 Write down your thoughts to clear your mind",
                "🌟 Celebrate small wins along the way",
                "🔄 Review and adjust your priorities regularly",
                "💪 Start with the most challenging task first",
                "🎨 Organize your workspace for better efficiency"
            };
            
            var random = new Random();
            return tips[random.Next(tips.Length)];
        }
        
        private void DrawStatusBar(Graphics g, Rectangle rect)
        {
            // Status bar background
            using (var statusBrush = new SolidBrush(Color.FromArgb(40, 0, 0, 0)))
            {
                g.FillRectangle(statusBrush, rect);
            }
            
            // Status text
            using (var statusFont = new Font("Segoe UI", 9F, FontStyle.Regular))
            using (var statusTextBrush = new SolidBrush(Color.FromArgb(150, 150, 170)))
            {
                var statusText = $"System Status: Online | Last Updated: {DateTime.Now:HH:mm} | User: {CurrentUsername ?? "Guest"}";
                g.DrawString(statusText, statusFont, statusTextBrush, rect.X + 10, rect.Y + 12);
            }
            
            // Connection indicator
            using (var indicatorBrush = new SolidBrush(Color.FromArgb(0, 255, 0)))
            {
                g.FillEllipse(indicatorBrush, rect.Right - 25, rect.Y + 15, 8, 8);
            }
        }
        
        private void DrawQuickActionButtons(Graphics g, Rectangle parentRect, int startY)
        {
            var buttonWidth = 100;
            var buttonHeight = 35;
            var buttonSpacing = 15;
            
            var buttons = new[]
            {
                new { Text = "📝 New Task", Icon = "📝" },
                new { Text = "📅 Schedule", Icon = "📅" },
                new { Text = "📊 Reports", Icon = "📊" },
                new { Text = "⚙️ Settings", Icon = "⚙️" },
                new { Text = "🔍 Search", Icon = "🔍" }
            };
            
            // Center the buttons horizontally
            var totalButtonWidth = (buttonWidth * buttons.Length) + (buttonSpacing * (buttons.Length - 1));
            var startX = (parentRect.Width - totalButtonWidth) / 2;
            
            for (int i = 0; i < buttons.Length; i++)
            {
                var buttonX = startX + (i * (buttonWidth + buttonSpacing));
                var buttonRect = new Rectangle(buttonX, startY, buttonWidth, buttonHeight);
                DrawQuickActionButton(g, buttonRect, buttons[i].Text, Color.FromArgb(70, 130, 180));
            }
        }
        
        private void DrawQuickActionButton(Graphics g, Rectangle rect, string text, Color accentColor)
        {
            // Button background with hover effect
            using (var buttonBrush = new LinearGradientBrush(
                rect, Color.FromArgb(80, 255, 255, 255), Color.FromArgb(40, 255, 255, 255), 90f))
            using (var borderPen = new Pen(Color.FromArgb(100, 255, 255, 255), 1))
            using (var buttonPath = GetRoundedRectanglePath(rect, 8))
            {
                g.FillPath(buttonBrush, buttonPath);
                g.DrawPath(borderPen, buttonPath);
            }
            
            // Button text
            using (var buttonFont = new Font("Segoe UI", 10F, FontStyle.Regular))
            using (var textBrush = new SolidBrush(Color.White))
            {
                var textSize = g.MeasureString(text, buttonFont);
                var textX = rect.X + (rect.Width - textSize.Width) / 2;
                var textY = rect.Y + (rect.Height - textSize.Height) / 2;
                g.DrawString(text, buttonFont, textBrush, textX, textY);
            }
        }
        
        private void DrawInteractiveCard(Graphics g, Rectangle rect, string title, string subtitle, Color accentColor, string cardType)
        {
            // Interactive card background with enhanced styling
            using (var cardBrush = new LinearGradientBrush(
                rect, Color.FromArgb(80, 255, 255, 255), Color.FromArgb(40, 255, 255, 255), 90f))
            using (var borderPen = new Pen(Color.FromArgb(120, 255, 255, 255), 2))
            using (var cardPath = GetRoundedRectanglePath(rect, 15))
            {
                g.FillPath(cardBrush, cardPath);
                g.DrawPath(borderPen, cardPath);
            }
            
            // Enhanced accent bar
            using (var accentBrush = new SolidBrush(accentColor))
            using (var accentPath = GetRoundedRectanglePath(new Rectangle(rect.X, rect.Y, 6, rect.Height), 3))
            {
                g.FillPath(accentBrush, accentPath);
            }
            
            // Click indicator
            using (var clickBrush = new SolidBrush(Color.FromArgb(100, 255, 255, 255)))
            {
                g.FillEllipse(clickBrush, rect.Right - 25, rect.Y + 10, 12, 12);
            }
            
            // Title
            using (var titleFont = new Font("Segoe UI", 16F, FontStyle.Bold))
            using (var titleBrush = new SolidBrush(Color.White))
            {
                g.DrawString(title, titleFont, titleBrush, rect.X + 25, rect.Y + 20);
            }
            
            // Subtitle
            using (var subtitleFont = new Font("Segoe UI", 12F, FontStyle.Regular))
            using (var subtitleBrush = new SolidBrush(Color.FromArgb(220, 220, 240)))
            {
                g.DrawString(subtitle, subtitleFont, subtitleBrush, rect.X + 25, rect.Y + 50);
            }
            
            // Action hint
            using (var hintFont = new Font("Segoe UI", 9F, FontStyle.Italic))
            using (var hintBrush = new SolidBrush(Color.FromArgb(180, 180, 200)))
            {
                g.DrawString("Click to open", hintFont, hintBrush, rect.X + 25, rect.Y + 75);
            }
        }
        
        private void DrawWeatherWidget(Graphics g, Rectangle rect)
        {
            // Weather widget background
            using (var weatherBrush = new SolidBrush(Color.FromArgb(60, 255, 255, 255)))
            using (var borderPen = new Pen(Color.FromArgb(80, 255, 255, 255), 1))
            using (var weatherPath = GetRoundedRectanglePath(rect, 15))
            {
                g.FillPath(weatherBrush, weatherPath);
                g.DrawPath(borderPen, weatherPath);
            }
            
            // Weather accent bar
            using (var accentBrush = new SolidBrush(Color.FromArgb(33, 150, 243)))
            using (var accentPath = GetRoundedRectanglePath(new Rectangle(rect.X, rect.Y, 5, rect.Height), 3))
            {
                g.FillPath(accentBrush, accentPath);
            }
            
            // Weather title
            using (var titleFont = new Font("Segoe UI", 14F, FontStyle.Bold))
            using (var titleBrush = new SolidBrush(Color.White))
            {
                g.DrawString("🌤️ Weather", titleFont, titleBrush, rect.X + 20, rect.Y + 15);
            }
            
            // Temperature
            using (var tempFont = new Font("Segoe UI", 18F, FontStyle.Bold))
            using (var tempBrush = new SolidBrush(Color.White))
            {
                g.DrawString("22°C", tempFont, tempBrush, rect.X + 20, rect.Y + 40);
            }
            
            // Weather description
            using (var descFont = new Font("Segoe UI", 11F, FontStyle.Regular))
            using (var descBrush = new SolidBrush(Color.FromArgb(200, 200, 220)))
            {
                g.DrawString("Sunny & Clear", descFont, descBrush, rect.X + 20, rect.Y + 70);
            }
        }
        
        // Helper methods for rounded rectangles
        private GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(rect.Left, rect.Top, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Top, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.Left, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseAllFigures();
            return path;
        }

        private void TodoPanel_Paint(object sender, PaintEventArgs e)
        {
            var panel = sender as Panel;
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            
            // Create gradient background
            var rect = panel.ClientRectangle;
            using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                rect, Color.FromArgb(20, 20, 30), Color.FromArgb(30, 30, 45), 90f))
            {
                g.FillRectangle(brush, rect);
            }
            
            // Add title
            using (var titleFont = new Font("Segoe UI", 22F, FontStyle.Bold))
            using (var titleBrush = new SolidBrush(Color.White))
            {
                var title = "✓ Task Management";
                g.DrawString(title, titleFont, titleBrush, 60, 25);
            }
        }

        private void ModernButton_Paint(object sender, PaintEventArgs e)
        {
            var button = sender as Button;
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            var rect = new Rectangle(0, 0, button.Width, button.Height);
            int radius = 12;
            
            // Create rounded rectangle path
            var path = new GraphicsPath();
            path.AddArc(rect.Left, rect.Top, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Top, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.Left, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseAllFigures();
            
            // Fill with gradient
            using (var brush = new LinearGradientBrush(
                rect, button.BackColor, 
                Color.FromArgb(Math.Min(255, button.BackColor.R + 20),
                              Math.Min(255, button.BackColor.G + 20),
                              Math.Min(255, button.BackColor.B + 20)), 90f))
            {
                g.FillPath(brush, path);
            }
            
            // Add subtle shadow effect
            using (var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
            {
                var shadowRect = new Rectangle(2, 2, button.Width, button.Height);
                var shadowPath = new GraphicsPath();
                shadowPath.AddArc(shadowRect.Left, shadowRect.Top, radius, radius, 180, 90);
                shadowPath.AddArc(shadowRect.Right - radius, shadowRect.Top, radius, radius, 270, 90);
                shadowPath.AddArc(shadowRect.Right - radius, shadowRect.Bottom - radius, radius, radius, 0, 90);
                shadowPath.AddArc(shadowRect.Left, shadowRect.Bottom - radius, radius, radius, 90, 90);
                shadowPath.CloseAllFigures();
                g.FillPath(shadowBrush, shadowPath);
            }
            
            // Draw text
            var textRect = new Rectangle(0, 0, button.Width, button.Height);
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            using (var textBrush = new SolidBrush(button.ForeColor))
            {
                g.DrawString(button.Text, button.Font, textBrush, textRect, sf);
            }
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
            
            // Update month label
            var monthLabel = calendarGridPanel.Parent.Controls.OfType<Panel>().FirstOrDefault()?.Controls["monthLabel"] as Label;
            if (monthLabel != null)
            {
                monthLabel.Text = $"{new DateTime(year, month, 1):MMMM yyyy}";
            }
            
            // Weekday headers
            string[] weekdays = { "SUN", "MON", "TUE", "WED", "THU", "FRI", "SAT" };
            int cellWidth = calendarGridPanel.Width / 7;
            int cellHeight = calendarGridPanel.Height / 7;
            Font headerFont = new Font("Segoe UI", 11, FontStyle.Bold);
            for (int i = 0; i < 7; i++)
            {
                var header = new Label
                {
                    Text = weekdays[i],
                    ForeColor = Color.FromArgb(200, 200, 220),
                    Font = headerFont,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Width = cellWidth,
                    Height = cellHeight,
                    Left = i * cellWidth,
                    Top = 0,
                    BackColor = Color.FromArgb(25, 25, 35)
                };
                header.Paint += (s, e) => {
                    var g = e.Graphics;
                    var rect = new Rectangle(0, 0, header.Width, header.Height);
                    using (var brush = new LinearGradientBrush(rect, 
                        Color.FromArgb(35, 35, 50), Color.FromArgb(25, 25, 35), 90f))
                    {
                        g.FillRectangle(brush, rect);
                    }
                };
                calendarGridPanel.Controls.Add(header);
            }

            var firstDay = new DateTime(year, month, 1);
            int daysInMonth = DateTime.DaysInMonth(year, month);
            int startDayOfWeek = (int)firstDay.DayOfWeek;
            Font dayFont = new Font("Segoe UI", 10, FontStyle.Bold);
            Font eventFont = new Font("Segoe UI", 9, FontStyle.Regular);
            Color cellColor = Color.FromArgb(35, 35, 50);
            Color todayCellColor = Color.FromArgb(70, 130, 180);
            Color eventColor = Color.White;
            Color dotColor = Color.FromArgb(100, 200, 255);
            var today = DateTime.Today;
            for (int week = 0, day = 1 - startDayOfWeek; week < 6; week++)
            {
                for (int col = 0; col < 7; col++, day++)
                {
                    var isToday = day > 0 && day <= daysInMonth && new DateTime(year, month, day) == today;
                    var currentCellColor = isToday ? todayCellColor : cellColor;
                    
                    var cellPanel = new Panel
                    {
                        Width = cellWidth - 6,
                        Height = cellHeight - 6,
                        Left = col * cellWidth + 3,
                        Top = (week + 1) * cellHeight + 3,
                        BackColor = currentCellColor,
                        BorderStyle = BorderStyle.None,
                        Margin = new Padding(3),
                        Tag = (day > 0 && day <= daysInMonth) ? new DateTime(year, month, day) : null
                    };
                    cellPanel.Paint += (s, e) =>
                    {
                        var g = e.Graphics;
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        var rect = new Rectangle(0, 0, cellPanel.Width, cellPanel.Height);
                        int radius = 12;
                        
                        // Create gradient for cell
                        using (var brush = new LinearGradientBrush(rect, 
                            currentCellColor, 
                            Color.FromArgb(Math.Min(255, currentCellColor.R + 15),
                                          Math.Min(255, currentCellColor.G + 15),
                                          Math.Min(255, currentCellColor.B + 15)), 135f))
                        {
                            g.FillRoundedRectangle(brush, rect, radius);
                        }
                        
                        // Add border for today
                        if (isToday)
                        {
                            using (var pen = new Pen(Color.FromArgb(150, 255, 255, 255), 2))
                            {
                                g.DrawRoundedRectangle(pen, rect, radius);
                            }
                        }
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
                var loginPanel = this.Controls["loginPanel"] as Panel;
                if (loginPanel != null && loginPanel.Visible)
                {
                    AnimateToPanel(loginPanel, mainMenuPanel);
                }
                else
                {
                    mainMenuPanel.Visible = true;
                    calendarPanel.Visible = false;
                    todoPanel.Visible = false;
                }
                
                // Hide login panel if it exists
                if (this.Controls["loginPanel"] != null)
                    this.Controls["loginPanel"].Visible = false;
            }
            
            private void ShowCalendar()
            {
                if (mainMenuPanel.Visible)
                    AnimateToPanel(mainMenuPanel, calendarPanel);
                else
                {
                    mainMenuPanel.Visible = false;
                    calendarPanel.Visible = true;
                    todoPanel.Visible = false;
                }
            }
            
            private void ShowTodo()
            {
                if (mainMenuPanel.Visible)
                    AnimateToPanel(mainMenuPanel, todoPanel);
                {
                    mainMenuPanel.Visible = false;
                    calendarPanel.Visible = false;
                    todoPanel.Visible = true;
                }
            }
            
            private void settingsButton_Click(object sender, EventArgs e)
            {
                using (var settingsForm = new SettingsForm(CurrentUsername))
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
                if (calendarPanel.Visible)
                    AnimateToPanel(calendarPanel, mainMenuPanel);
                else
                    ShowMainMenu();
            }

            private void todoBackButton_Click(object sender, EventArgs e)
            {
                if (todoPanel.Visible)
                    AnimateToPanel(todoPanel, mainMenuPanel);
                else
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

            public void LogoutUser()
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

            private string CheckRememberMe()
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
                    BackColor = Color.FromArgb(10, 10, 15)
                };
                
                // Modern gradient background with glass morphism effect
                loginPanel.Paint += (s, e) => {
                    var rect = loginPanel.ClientRectangle;
                    var g = e.Graphics;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    
                    // Primary gradient
                    using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                        rect, Color.FromArgb(15, 15, 25), Color.FromArgb(25, 25, 40), 135f))
                    {
                        g.FillRectangle(brush, rect);
                    }
                    
                    // Add subtle overlay pattern
                    using (var overlayBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
                        new Rectangle(0, 0, rect.Width, rect.Height / 2),
                        Color.FromArgb(20, 255, 255, 255), Color.FromArgb(5, 255, 255, 255), 90f))
                    {
                        g.FillRectangle(overlayBrush, new Rectangle(0, 0, rect.Width, rect.Height / 2));
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
                
                // Glass morphism login card
                var loginCard = new Panel
                {
                    Size = new Size(380, 480),
                    BackColor = Color.FromArgb(40, 45, 45, 65),
                    Location = new Point(35, 85)
                };
                
                loginCard.Paint += (s, e) => {
                    var g = e.Graphics;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    var rect = new Rectangle(0, 0, loginCard.Width, loginCard.Height);
                    int radius = 20;
                    
                    // Create rounded rectangle path
                    var path = new System.Drawing.Drawing2D.GraphicsPath();
                    path.AddArc(rect.Left, rect.Top, radius, radius, 180, 90);
                    path.AddArc(rect.Right - radius, rect.Top, radius, radius, 270, 90);
                    path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
                    path.AddArc(rect.Left, rect.Bottom - radius, radius, radius, 90, 90);
                    path.CloseAllFigures();
                    
                    // Glass morphism background
                    using (var brush = new SolidBrush(Color.FromArgb(60, 255, 255, 255)))
                    {
                        g.FillPath(brush, path);
                    }
                    
                    // Border
                    using (var pen = new Pen(Color.FromArgb(80, 255, 255, 255), 1))
                    {
                        g.DrawPath(pen, path);
                    }
                };
                
                // Title
                var titleLabel = new Label
                {
                    Text = "🤖 JARVIS Assistant",
                    Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                    ForeColor = Color.White,
                    Location = new Point(50, 30),
                    Size = new Size(280, 40),
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = Color.Transparent
                };
                
                var subtitleLabel = new Label
                {
                    Text = "Your Intelligent Productivity Companion",
                    Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                    ForeColor = Color.FromArgb(200, 200, 220),
                    Location = new Point(50, 75),
                    Size = new Size(280, 25),
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = Color.Transparent
                };
                
                // Username controls
                var usernameLabel = new Label
                {
                    Text = "👤 Username",
                    Location = new Point(50, 130),
                    Size = new Size(120, 25),
                    Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                    ForeColor = Color.White,
                    BackColor = Color.Transparent
                };
                
                var usernameBox = new TextBox
                {
                    Name = "usernameBox",
                    Location = new Point(50, 160),
                    Size = new Size(280, 35),
                    Font = new Font("Segoe UI", 12F),
                    BackColor = Color.FromArgb(25, 25, 35),
                    ForeColor = Color.White,
                    BorderStyle = BorderStyle.None,
                    Padding = new Padding(10)
                };
                
                usernameBox.Paint += (s, e) => {
                    var textBox = s as TextBox;
                    var g = e.Graphics;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    var rect = new Rectangle(0, 0, textBox.Width, textBox.Height);
                    using (var brush = new SolidBrush(Color.FromArgb(40, 255, 255, 255)))
                    using (var pen = new Pen(Color.FromArgb(60, 255, 255, 255), 1))
                    {
                        g.FillRoundedRectangle(brush, rect, 8);
                        g.DrawRoundedRectangle(pen, rect, 8);
                    }
                };
                
                // Password controls
                var passwordLabel = new Label
                {
                    Text = "🔒 Password",
                    Location = new Point(50, 210),
                    Size = new Size(120, 25),
                    Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                    ForeColor = Color.White,
                    BackColor = Color.Transparent
                };
                
                var passwordBox = new TextBox
                {
                    Name = "passwordBox",
                    Location = new Point(50, 240),
                    Size = new Size(240, 35),
                    UseSystemPasswordChar = true,
                    Font = new Font("Segoe UI", 12F),
                    BackColor = Color.FromArgb(25, 25, 35),
                    ForeColor = Color.White,
                    BorderStyle = BorderStyle.None,
                    Padding = new Padding(10),
                    TabIndex = 1
                };
                
                passwordBox.Paint += (s, e) => {
                    var textBox = s as TextBox;
                    var g = e.Graphics;
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    var rect = new Rectangle(0, 0, textBox.Width, textBox.Height);
                    using (var brush = new SolidBrush(Color.FromArgb(40, 255, 255, 255)))
                    using (var pen = new Pen(Color.FromArgb(60, 255, 255, 255), 1))
                    {
                        g.FillRoundedRectangle(brush, rect, 8);
                        g.DrawRoundedRectangle(pen, rect, 8);
                    }
                };
                
                // Show/Hide password button
                var showPasswordButton = new Button
                {
                    Text = "👁",
                    Location = new Point(300, 240),
                    Size = new Size(30, 35),
                    Font = new Font("Segoe UI", 10F),
                    BackColor = Color.FromArgb(45, 45, 65),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                showPasswordButton.FlatAppearance.BorderSize = 0;
                showPasswordButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(65, 65, 85);
                showPasswordButton.Paint += ModernButton_Paint;
                
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
                    Location = new Point(50, 290),
                    Size = new Size(250, 25),
                    Font = new Font("Segoe UI", 11F),
                    ForeColor = Color.FromArgb(200, 200, 220),
                    BackColor = Color.Transparent,
                    CheckAlign = ContentAlignment.MiddleLeft,
                    TextAlign = ContentAlignment.MiddleLeft
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
                    Text = "🚀 Login",
                    Location = new Point(50, 340),
                    Size = new Size(90, 45),
                    Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                    BackColor = Color.FromArgb(70, 130, 180),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                loginButton.FlatAppearance.BorderSize = 0;
                loginButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(90, 150, 200);
                loginButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(50, 110, 160);
                loginButton.Paint += ModernButton_Paint;
                
                var registerButton = new Button
                {
                    Text = "📝 Register",
                    Location = new Point(150, 340),
                    Size = new Size(100, 45),
                    Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                    BackColor = Color.FromArgb(108, 117, 125),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                registerButton.FlatAppearance.BorderSize = 0;
                registerButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(128, 137, 145);
                registerButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(88, 97, 105);
                registerButton.Paint += ModernButton_Paint;
                
                var forgotPasswordButton = new Button
                {
                    Text = "🔑 Forgot",
                    Location = new Point(260, 340),
                    Size = new Size(80, 45),
                    Font = new Font("Segoe UI", 11F, FontStyle.Regular),
                    BackColor = Color.FromArgb(220, 53, 69),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                forgotPasswordButton.FlatAppearance.BorderSize = 0;
                forgotPasswordButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 73, 89);
                forgotPasswordButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(200, 33, 49);
                forgotPasswordButton.Paint += ModernButton_Paint;
                
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
                
                // Set tab order for smooth navigation
                usernameBox.TabIndex = 0;
                passwordBox.TabIndex = 1;
                loginButton.TabIndex = 2;
                registerButton.TabIndex = 3;
                
                // Add smooth focus transitions
                usernameBox.Enter += (s, e) => AnimateTextBoxFocus(usernameBox, true);
                usernameBox.Leave += (s, e) => AnimateTextBoxFocus(usernameBox, false);
                passwordBox.Enter += (s, e) => AnimateTextBoxFocus(passwordBox, true);
                passwordBox.Leave += (s, e) => AnimateTextBoxFocus(passwordBox, false);
                
                // Add controls to login card
                loginCard.Controls.Add(titleLabel);
                loginCard.Controls.Add(subtitleLabel);
                loginCard.Controls.Add(usernameLabel);
                loginCard.Controls.Add(usernameBox);
                loginCard.Controls.Add(passwordLabel);
                loginCard.Controls.Add(passwordBox);
                loginCard.Controls.Add(showPasswordButton);
                loginCard.Controls.Add(strengthLabel);
                loginCard.Controls.Add(rememberCheckBox);
                loginCard.Controls.Add(securityLabel);
                loginCard.Controls.Add(securityBox);
                loginCard.Controls.Add(loginButton);
                loginCard.Controls.Add(registerButton);
                loginCard.Controls.Add(forgotPasswordButton);
                
                // Add login card to center panel
                centerPanel.Controls.Add(loginCard);
                loginPanel.Controls.Add(centerPanel);
                this.Controls.Add(loginPanel);
                
                // Initial positioning
                centerPanel.Location = new Point((loginPanel.Width - centerPanel.Width) / 2, (loginPanel.Height - centerPanel.Height) / 2);
            }
    }
}
