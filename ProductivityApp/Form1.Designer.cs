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
    // ...existing code...

    // ...existing code...
    private System.Windows.Forms.Button addTodoButton;
    private System.Windows.Forms.ListBox todoListBox;
    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
    private System.Windows.Forms.Panel todoPanel;
    private System.Windows.Forms.Panel calendarPanel;
    private System.Windows.Forms.Button showHideTodoButton;
    private System.Windows.Forms.Button showHideCalendarButton;
    private System.Windows.Forms.MonthCalendar monthCalendar;

        private void InitializeComponent()
        {
            this.addTodoButton = new System.Windows.Forms.Button();
            this.todoListBox = new System.Windows.Forms.ListBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.todoPanel = new System.Windows.Forms.Panel();
            this.calendarPanel = new System.Windows.Forms.Panel();
            this.showHideTodoButton = new System.Windows.Forms.Button();
            this.showHideCalendarButton = new System.Windows.Forms.Button();
            this.monthCalendar = new System.Windows.Forms.MonthCalendar();
            this.SuspendLayout();

            // todoPanel
            //
            this.todoPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.todoPanel.BackColor = System.Drawing.Color.White;
            this.todoPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.todoPanel.Controls.Add(this.addTodoButton);
            this.todoPanel.Controls.Add(this.todoListBox);
            this.todoPanel.Location = new System.Drawing.Point(20, 90);
            this.todoPanel.Name = "todoPanel";
            this.todoPanel.Size = new System.Drawing.Size(370, 320);
            this.todoPanel.TabIndex = 10;

            // addTodoButton
            // 
            this.addTodoButton.Location = new System.Drawing.Point(10, 10);
            this.addTodoButton.Name = "addTodoButton";
            this.addTodoButton.Size = new System.Drawing.Size(120, 40);
            this.addTodoButton.TabIndex = 0;
            this.addTodoButton.Text = "Add To-Do";
            this.addTodoButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addTodoButton.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            this.addTodoButton.ForeColor = System.Drawing.Color.White;
            this.addTodoButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.addTodoButton.FlatAppearance.BorderSize = 0;
            this.addTodoButton.Click += new System.EventHandler(this.addTodoButton_Click);

            // todoListBox
            // 
            this.todoListBox.FormattingEnabled = true;
            this.todoListBox.ItemHeight = 20;
            this.todoListBox.Location = new System.Drawing.Point(10, 60);
            this.todoListBox.Name = "todoListBox";
            this.todoListBox.Size = new System.Drawing.Size(340, 240);
            this.todoListBox.TabIndex = 1;
            this.todoListBox.BackColor = System.Drawing.Color.FromArgb(236, 240, 241);
            this.todoListBox.Font = new System.Drawing.Font("Segoe UI", 11F);

            // showHideTodoButton
            //
            this.showHideTodoButton.Location = new System.Drawing.Point(20, 50);
            this.showHideTodoButton.Name = "showHideTodoButton";
            this.showHideTodoButton.Size = new System.Drawing.Size(120, 32);
            this.showHideTodoButton.TabIndex = 11;
            this.showHideTodoButton.Text = "Hide To-Do";
            this.showHideTodoButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.showHideTodoButton.BackColor = System.Drawing.Color.FromArgb(39, 174, 96);
            this.showHideTodoButton.ForeColor = System.Drawing.Color.White;
            this.showHideTodoButton.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.showHideTodoButton.FlatAppearance.BorderSize = 0;
            this.showHideTodoButton.Click += new System.EventHandler(this.showHideTodoButton_Click);

            // calendarPanel
            //
            this.calendarPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)
            | System.Windows.Forms.AnchorStyles.Left)));
            this.calendarPanel.BackColor = System.Drawing.Color.White;
            this.calendarPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.calendarPanel.Controls.Add(this.monthCalendar);
            this.calendarPanel.Location = new System.Drawing.Point(410, 90);
            this.calendarPanel.Name = "calendarPanel";
            this.calendarPanel.Size = new System.Drawing.Size(370, 320);
            this.calendarPanel.TabIndex = 20;

            // monthCalendar
            //
            this.monthCalendar.Location = new System.Drawing.Point(10, 10);
            this.monthCalendar.Name = "monthCalendar";
            this.monthCalendar.TabIndex = 21;

            // showHideCalendarButton
            //
            this.showHideCalendarButton.Location = new System.Drawing.Point(410, 50);
            this.showHideCalendarButton.Name = "showHideCalendarButton";
            this.showHideCalendarButton.Size = new System.Drawing.Size(120, 32);
            this.showHideCalendarButton.TabIndex = 22;
            this.showHideCalendarButton.Text = "Hide Calendar";
            this.showHideCalendarButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.showHideCalendarButton.BackColor = System.Drawing.Color.FromArgb(192, 57, 43);
            this.showHideCalendarButton.ForeColor = System.Drawing.Color.White;
            this.showHideCalendarButton.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.showHideCalendarButton.FlatAppearance.BorderSize = 0;
            this.showHideCalendarButton.Click += new System.EventHandler(this.showHideCalendarButton_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.fileToolStripMenuItem,
                this.settingsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 28);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.BackColor = System.Drawing.Color.FromArgb(44, 62, 80);
            this.menuStrip1.ForeColor = System.Drawing.Color.White;
            this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 11F);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(50, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(120, 28);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(80, 24);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.showHideTodoButton);
            this.Controls.Add(this.showHideCalendarButton);
            this.Controls.Add(this.todoPanel);
            this.Controls.Add(this.calendarPanel);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "Productivity App";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
