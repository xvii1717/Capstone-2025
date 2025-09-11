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
        private Button logoutButton;
        private string currentUsername;

        public SettingsForm(string username)
        {
            this.currentUsername = username;
            this.Size = new Size(400, 280);
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
            logoutButton = new Button {
                Text = "Logout",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Size = new Size(140, 40),
                Location = new Point(20, 120),
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            
            deleteAccountButton = new Button {
                Text = "Delete Account",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Size = new Size(140, 40),
                Location = new Point(20, 180),
                BackColor = Color.DarkRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            
            logoutButton.Click += (s, e) => {
                var result = MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    this.Close();
                    Application.OpenForms[0].Hide();
                    using (var loginForm = new LoginForm())
                    {
                        if (loginForm.ShowDialog() == DialogResult.OK)
                        {
                            ((Form1)Application.OpenForms[0]).CurrentUsername = loginForm.Username;
                            Application.OpenForms[0].Show();
                        }
                        else
                        {
                            Application.Exit();
                        }
                    }
                }
            };
            
            deleteAccountButton.Click += (s, e) => {
                var result = MessageBox.Show("Are you sure you want to delete your account? This cannot be undone.", "Delete Account", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    // Delete credentials
                    var userFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.txt");
                    if (File.Exists(userFile))
                    {
                        var lines = File.ReadAllLines(userFile).Where(line => !line.StartsWith(currentUsername + ":")).ToArray();
                        File.WriteAllLines(userFile, lines);
                    }
                    // Delete user data files
                    var calFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"calendarEvents_{currentUsername}.json");
                    var todoFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"todoList_{currentUsername}.json");
                    if (File.Exists(calFile)) File.Delete(calFile);
                    if (File.Exists(todoFile)) File.Delete(todoFile);
                    MessageBox.Show("Account deleted. Returning to login.");
                    Application.OpenForms[0].Hide();
                    using (var loginForm = new LoginForm())
                    {
                        if (loginForm.ShowDialog() == DialogResult.OK)
                        {
                            ((Form1)Application.OpenForms[0]).CurrentUsername = loginForm.Username;
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
            this.Controls.Add(logoutButton);
            this.Controls.Add(deleteAccountButton);
        }
    }
}
