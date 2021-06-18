
namespace ClientTIP
{
    partial class MainWindow
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
        private void InitializeComponent()
        {
            this.UsernameTextField = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.CallButton = new System.Windows.Forms.Button();
            this.EndButton = new System.Windows.Forms.Button();
            this.LogoutButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // UsernameTextField
            // 
            this.UsernameTextField.Location = new System.Drawing.Point(131, 12);
            this.UsernameTextField.Name = "UsernameTextField";
            this.UsernameTextField.Size = new System.Drawing.Size(207, 23);
            this.UsernameTextField.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Nazwa użytkownika";
            // 
            // CallButton
            // 
            this.CallButton.Location = new System.Drawing.Point(189, 41);
            this.CallButton.Name = "CallButton";
            this.CallButton.Size = new System.Drawing.Size(75, 23);
            this.CallButton.TabIndex = 2;
            this.CallButton.Text = "Zadzwoń";
            this.CallButton.UseVisualStyleBackColor = true;
            this.CallButton.Click += new System.EventHandler(this.CallButton_Click);
            // 
            // EndButton
            // 
            this.EndButton.Enabled = false;
            this.EndButton.Location = new System.Drawing.Point(189, 70);
            this.EndButton.Name = "EndButton";
            this.EndButton.Size = new System.Drawing.Size(75, 23);
            this.EndButton.TabIndex = 3;
            this.EndButton.Text = "Zakończ rozmowę";
            this.EndButton.UseVisualStyleBackColor = true;
            this.EndButton.Click += new System.EventHandler(this.EndButton_Click);
            // 
            // LogoutButton
            // 
            this.LogoutButton.Location = new System.Drawing.Point(14, 70);
            this.LogoutButton.Name = "LogoutButton";
            this.LogoutButton.Size = new System.Drawing.Size(75, 23);
            this.LogoutButton.TabIndex = 4;
            this.LogoutButton.Text = "Wyloguj";
            this.LogoutButton.UseVisualStyleBackColor = true;
            this.LogoutButton.Click += new System.EventHandler(this.LogoutButton_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(361, 114);
            this.Controls.Add(this.LogoutButton);
            this.Controls.Add(this.EndButton);
            this.Controls.Add(this.CallButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.UsernameTextField);
            this.Name = "MainWindow";
            this.Text = "MainWindow";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox UsernameTextField;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button CallButton;
        private System.Windows.Forms.Button EndButton;
        private System.Windows.Forms.Button LogoutButton;
    }
}