using System.Windows.Forms;

namespace ProductivityApp
{
    public class SettingsForm : Form
    {
        public SettingsForm()
        {
            this.Text = "Settings";
            this.Size = new System.Drawing.Size(400, 300);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            var label = new Label
            {
                Text = "Settings go here.",
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Font = new System.Drawing.Font("Segoe UI", 12F)
            };
            this.Controls.Add(label);
        }
    }
}
