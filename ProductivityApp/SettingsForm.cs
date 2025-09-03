using System;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Drawing;

namespace ProductivityApp
{
    public class SettingsForm : Form
    {
        private Button deleteAccountButton;

        public SettingsForm()
        {
            this.Size = new Size(400, 220);
            this.Text = "Settings";
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;

            var titleLabel = new Label {
                Text = "Settings",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 20),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            var infoLabel = new Label {
                Text = "Manage your account below:",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.White,
                Location = new Point(20, 60),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            deleteAccountButton = new Button {
                Text = "Delete Account",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Size = new Size(140, 40),
                Location = new Point(20, 120),
                BackColor = Color.DarkRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            deleteAccountButton.Click += (s, e) => {
                var result = MessageBox.Show("Are you sure you want to delete your account? This cannot be undone.", "Delete Account", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    // Delete credentials
                    var userFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.txt");
                    if (File.Exists(userFile))
                    {
                        var lines = File.ReadAllLines(userFile).Where(line => !line.StartsWith(Form1.CurrentUsername + ":")).ToArray();
                        File.WriteAllLines(userFile, lines);
                    }
                    // Delete user data files
                    var calFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"calendarEvents_{Form1.CurrentUsername}.json");
                    var todoFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"todoList_{Form1.CurrentUsername}.json");
                    if (File.Exists(calFile)) File.Delete(calFile);
                    if (File.Exists(todoFile)) File.Delete(todoFile);
                    MessageBox.Show("Account deleted. Returning to login.");
                    Application.OpenForms[0].Hide();
                    using (var loginForm = new LoginForm())
                    {
                        if (loginForm.ShowDialog() == DialogResult.OK)
                        {
                            Form1.CurrentUsername = loginForm.Username;
                            Application.OpenForms[0].Show();
                        }
                        else
                        {
                            Application.Exit();
                        }
                    }
                    this.Close();
                }
            };
            this.Controls.Add(titleLabel);
            this.Controls.Add(infoLabel);
            this.Controls.Add(deleteAccountButton);
        }
    }
}
