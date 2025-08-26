using System.Windows.Forms;

namespace ProductivityApp
{
    public class SettingsForm : Form
    {
        public SettingsForm()
        {
            this.Text = "Settings";
            this.Width = 300;
            this.Height = 200;

            var label = new Label
            {
                Text = "Settings go here.",
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };
            this.Controls.Add(label);
        }
    }
}
