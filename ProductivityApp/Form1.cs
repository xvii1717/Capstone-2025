using System;
using System.Windows.Forms;

namespace ProductivityApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            ShowMainMenu();
        }

        private void addTodoButton_Click(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Enter a new to-do item:", "Add To-Do", "");
            if (!string.IsNullOrWhiteSpace(input))
            {
                todoListBox.Items.Add(input);
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
