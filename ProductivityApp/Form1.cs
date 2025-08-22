using System.Windows.Forms;

namespace ProductivityApp
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            // Start in normal window, but allow fullscreen
            this.WindowState = FormWindowState.Normal;
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;
        }

        private int todoCount = 1;
        private void addTodoButton_Click(object sender, System.EventArgs e)
        {
            todoListBox.Items.Add($"To-Do Node {todoCount++}");
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // Toggle fullscreen with F11
            if (e.KeyCode == Keys.F11)
            {
                if (this.WindowState == FormWindowState.Normal)
                {
                    this.WindowState = FormWindowState.Maximized;
                    this.FormBorderStyle = FormBorderStyle.None;
                }
                else
                {
                    this.WindowState = FormWindowState.Normal;
                    this.FormBorderStyle = FormBorderStyle.Sizable;
                }
            }
        }
    }
}
