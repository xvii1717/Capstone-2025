using System;
using System.Windows.Forms;
using ProductivityApp.Services;

namespace ProductivityApp
{
	static class Program
	{
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			
			// Initialize database
			try
			{
				DatabaseInitializer.InitializeAsync().Wait();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Failed to initialize database: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			
			Application.Run(new Form1());
		}
	}
}
