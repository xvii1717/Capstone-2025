namespace ProductivityApp
{
    partial class Form1
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
        private System.Windows.Forms.Panel mainMenuPanel;
    private System.Windows.Forms.Panel todoPanel;
    private System.Windows.Forms.Panel calendarPanel;
    private System.Windows.Forms.ListBox todoListBox;
    private System.Windows.Forms.Button addTodoButton;
    private System.Windows.Forms.Panel calendarGridPanel;
    private System.Windows.Forms.ComboBox calendarViewComboBox;
    private System.Windows.Forms.ListBox eventListBox;
    private System.Windows.Forms.Button addEventButton;
        private System.Windows.Forms.Button settingsButton;
        private System.Windows.Forms.Button calendarButton;
        private System.Windows.Forms.Button todoButton;
    private System.Windows.Forms.Button backButton;
    private System.Windows.Forms.Button todoBackButton;

        private void InitializeComponent()
        {
            this.mainMenuPanel = new System.Windows.Forms.Panel();
            this.todoPanel = new System.Windows.Forms.Panel();
            this.calendarPanel = new System.Windows.Forms.Panel();
            this.todoListBox = new System.Windows.Forms.ListBox();
            this.addTodoButton = new System.Windows.Forms.Button();
            this.calendarGridPanel = new System.Windows.Forms.Panel();
            this.calendarViewComboBox = new System.Windows.Forms.ComboBox();
            this.eventListBox = new System.Windows.Forms.ListBox();
            this.addEventButton = new System.Windows.Forms.Button();
            this.settingsButton = new System.Windows.Forms.Button();
            this.calendarButton = new System.Windows.Forms.Button();
            this.todoButton = new System.Windows.Forms.Button();
            this.backButton = new System.Windows.Forms.Button();
            this.todoBackButton = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // mainMenuPanel
            this.mainMenuPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainMenuPanel.BackColor = System.Drawing.Color.White;
            this.mainMenuPanel.Controls.Add(this.settingsButton);
            this.mainMenuPanel.Controls.Add(this.calendarButton);
            this.mainMenuPanel.Controls.Add(this.todoButton);
            this.mainMenuPanel.Name = "mainMenuPanel";
            this.mainMenuPanel.Size = new System.Drawing.Size(800, 450);
            this.mainMenuPanel.TabIndex = 100;

            // settingsButton
            this.settingsButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.settingsButton.Location = new System.Drawing.Point(340, 80);
            this.settingsButton.Size = new System.Drawing.Size(120, 40);
            this.settingsButton.Text = "Settings";
            this.settingsButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.settingsButton.Click += new System.EventHandler(this.settingsButton_Click);

            // calendarButton
            this.calendarButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.calendarButton.Location = new System.Drawing.Point(340, 140);
            this.calendarButton.Size = new System.Drawing.Size(120, 40);
            this.calendarButton.Text = "Calendar";
            this.calendarButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.calendarButton.Click += new System.EventHandler(this.calendarButton_Click);

            // todoButton
            this.todoButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.todoButton.Location = new System.Drawing.Point(340, 200);
            this.todoButton.Size = new System.Drawing.Size(120, 40);
            this.todoButton.Text = "To-Do List";
            this.todoButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.todoButton.Click += new System.EventHandler(this.todoButton_Click);

            // backButton
            this.backButton.Location = new System.Drawing.Point(10, 10);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(100, 32);
            this.backButton.Text = "Back";
            this.backButton.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.backButton.Click += new System.EventHandler(this.backButton_Click);

            // todoBackButton
            this.todoBackButton.Location = new System.Drawing.Point(10, 10);
            this.todoBackButton.Name = "todoBackButton";
            this.todoBackButton.Size = new System.Drawing.Size(100, 32);
            this.todoBackButton.Text = "Back";
            this.todoBackButton.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.todoBackButton.Click += new System.EventHandler(this.todoBackButton_Click);

            // todoPanel
            this.todoPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.todoPanel.BackColor = System.Drawing.Color.WhiteSmoke;
            this.todoPanel.Controls.Add(this.todoBackButton);
            this.todoPanel.Controls.Add(this.todoListBox);
            this.todoPanel.Controls.Add(this.addTodoButton);
            this.todoPanel.Name = "todoPanel";
            this.todoPanel.Size = new System.Drawing.Size(800, 450);
            this.todoPanel.TabIndex = 101;

            // todoListBox
            this.todoListBox.Location = new System.Drawing.Point(120, 60);
            this.todoListBox.Size = new System.Drawing.Size(560, 300);
            this.todoListBox.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.todoListBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom;

            // addTodoButton
            this.addTodoButton.Location = new System.Drawing.Point(120, 370);
            this.addTodoButton.Size = new System.Drawing.Size(120, 36);
            this.addTodoButton.Text = "Add To-Do";
            this.addTodoButton.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.addTodoButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.addTodoButton.Click += new System.EventHandler(this.addTodoButton_Click);

            // calendarPanel
            this.calendarPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.calendarPanel.BackColor = System.Drawing.Color.WhiteSmoke;
            // Top bar controls
            var calendarTopBar = new System.Windows.Forms.Panel();
            calendarTopBar.Dock = System.Windows.Forms.DockStyle.Top;
            calendarTopBar.Height = 56;
            calendarTopBar.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            calendarTopBar.Padding = new System.Windows.Forms.Padding(16, 8, 16, 8);

            var todayButton = new System.Windows.Forms.Button();
            todayButton.Text = "Today";
            todayButton.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            todayButton.Size = new System.Drawing.Size(80, 32);
            todayButton.Location = new System.Drawing.Point(16, 12);
            todayButton.BackColor = System.Drawing.Color.FromArgb(40, 40, 40);
            todayButton.ForeColor = System.Drawing.Color.White;
            todayButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            todayButton.Click += new System.EventHandler(this.TodayButton_Click);
            calendarTopBar.Controls.Add(todayButton);

            var prevMonthButton = new System.Windows.Forms.Button();
            prevMonthButton.Text = "<";
            prevMonthButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            prevMonthButton.Size = new System.Drawing.Size(32, 32);
            prevMonthButton.Location = new System.Drawing.Point(110, 12);
            prevMonthButton.BackColor = System.Drawing.Color.FromArgb(40, 40, 40);
            prevMonthButton.ForeColor = System.Drawing.Color.White;
            prevMonthButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            prevMonthButton.Click += new System.EventHandler(this.PrevMonthButton_Click);
            calendarTopBar.Controls.Add(prevMonthButton);

            var nextMonthButton = new System.Windows.Forms.Button();
            nextMonthButton.Text = ">";
            nextMonthButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            nextMonthButton.Size = new System.Drawing.Size(32, 32);
            nextMonthButton.Location = new System.Drawing.Point(150, 12);
            nextMonthButton.BackColor = System.Drawing.Color.FromArgb(40, 40, 40);
            nextMonthButton.ForeColor = System.Drawing.Color.White;
            nextMonthButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            nextMonthButton.Click += new System.EventHandler(this.NextMonthButton_Click);
            calendarTopBar.Controls.Add(nextMonthButton);

            var monthLabel = new System.Windows.Forms.Label();
            monthLabel.Text = "Month Year";
            monthLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            monthLabel.ForeColor = System.Drawing.Color.White;
            monthLabel.Location = new System.Drawing.Point(200, 12);
            monthLabel.AutoSize = true;
            monthLabel.Name = "monthLabel";
            calendarTopBar.Controls.Add(monthLabel);

            this.calendarPanel.Controls.Add(calendarTopBar);
            this.calendarPanel.Controls.Add(this.backButton);
            this.backButton.Location = new System.Drawing.Point(16, 64);
            this.backButton.Size = new System.Drawing.Size(80, 32);
            this.backButton.Text = "Back";
            this.backButton.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.backButton.BackColor = System.Drawing.Color.FromArgb(40, 40, 40);
            this.backButton.ForeColor = System.Drawing.Color.White;
            this.backButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.calendarPanel.Controls.Add(this.calendarGridPanel);
            this.calendarPanel.Controls.Add(this.calendarViewComboBox);
            this.calendarPanel.Controls.Add(this.eventListBox);
            this.calendarPanel.Controls.Add(this.addEventButton);
            this.calendarPanel.Name = "calendarPanel";
            this.calendarPanel.Size = new System.Drawing.Size(800, 450);
            this.calendarPanel.TabIndex = 102;

            // calendarGridPanel
            this.calendarGridPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.calendarGridPanel.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.calendarGridPanel.BorderStyle = System.Windows.Forms.BorderStyle.None;

            // calendarViewComboBox
            this.calendarViewComboBox.Location = new System.Drawing.Point(220, 20);
            this.calendarViewComboBox.Size = new System.Drawing.Size(200, 28);
            this.calendarViewComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.calendarViewComboBox.Items.AddRange(new object[] {"Month View", "Week View"});
            this.calendarViewComboBox.SelectedIndex = 0;
            this.calendarViewComboBox.SelectedIndexChanged += new System.EventHandler(this.calendarViewComboBox_SelectedIndexChanged);

            // eventListBox
            this.eventListBox.Location = new System.Drawing.Point(500, 60);
            this.eventListBox.Size = new System.Drawing.Size(250, 300);
            this.eventListBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.eventListBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom;

            // addEventButton
            this.addEventButton.Location = new System.Drawing.Point(500, 370);
            this.addEventButton.Size = new System.Drawing.Size(120, 36);
            this.addEventButton.Text = "Add Event";
            this.addEventButton.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.addEventButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.addEventButton.Click += new System.EventHandler(this.addEventButton_Click);

            // Form1
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.mainMenuPanel);
            this.Controls.Add(this.todoPanel);
            this.Controls.Add(this.calendarPanel);
            this.Name = "Form1";
            this.Text = "ProductivityApp";
            this.ResumeLayout(false);
        }

        #endregion
    }
}
