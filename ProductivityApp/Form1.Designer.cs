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
    private System.Windows.Forms.MonthCalendar monthCalendar;
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
            this.monthCalendar = new System.Windows.Forms.MonthCalendar();
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
            this.calendarPanel.Controls.Add(this.backButton);
            this.calendarPanel.Controls.Add(this.monthCalendar);
            this.calendarPanel.Name = "calendarPanel";
            this.calendarPanel.Size = new System.Drawing.Size(800, 450);
            this.calendarPanel.TabIndex = 102;

            // monthCalendar
            this.monthCalendar.Location = new System.Drawing.Point(220, 60);
            this.monthCalendar.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom;

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
