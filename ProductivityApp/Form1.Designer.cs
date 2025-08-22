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
        private System.Windows.Forms.Button addTodoButton;
        private System.Windows.Forms.ListBox todoListBox;

        private void InitializeComponent()
        {
            this.addTodoButton = new System.Windows.Forms.Button();
            this.todoListBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // addTodoButton
            // 
            this.addTodoButton.Location = new System.Drawing.Point(12, 12);
            this.addTodoButton.Name = "addTodoButton";
            this.addTodoButton.Size = new System.Drawing.Size(120, 40);
            this.addTodoButton.TabIndex = 0;
            this.addTodoButton.Text = "Add To-Do";
            this.addTodoButton.UseVisualStyleBackColor = true;
            this.addTodoButton.Click += new System.EventHandler(this.addTodoButton_Click);
            // 
            // todoListBox
            // 
            this.todoListBox.FormattingEnabled = true;
            this.todoListBox.ItemHeight = 16;
            this.todoListBox.Location = new System.Drawing.Point(12, 60);
            this.todoListBox.Name = "todoListBox";
            this.todoListBox.Size = new System.Drawing.Size(776, 372);
            this.todoListBox.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.addTodoButton);
            this.Controls.Add(this.todoListBox);
            this.Name = "Form1";
            this.Text = "ProductivityApp";
            this.ResumeLayout(false);
        }

        #endregion
    }
}
