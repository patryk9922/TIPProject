
namespace ClientTIP
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.LoginTextField = new System.Windows.Forms.TextBox();
            this.PasswordTextField = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.LoginClickButton = new System.Windows.Forms.Button();
            this.RegisterClickButton = new System.Windows.Forms.Button();
            this.IPTextField = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ConnectClickButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // LoginTextField
            // 
            this.LoginTextField.Location = new System.Drawing.Point(59, 117);
            this.LoginTextField.Name = "LoginTextField";
            this.LoginTextField.Size = new System.Drawing.Size(258, 23);
            this.LoginTextField.TabIndex = 0;
            // 
            // PasswordTextField
            // 
            this.PasswordTextField.Location = new System.Drawing.Point(59, 171);
            this.PasswordTextField.Name = "PasswordTextField";
            this.PasswordTextField.Size = new System.Drawing.Size(258, 23);
            this.PasswordTextField.TabIndex = 1;
            this.PasswordTextField.UseSystemPasswordChar = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 117);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Login";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 174);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Hasło";
            // 
            // LoginClickButton
            // 
            this.LoginClickButton.Enabled = false;
            this.LoginClickButton.Location = new System.Drawing.Point(59, 211);
            this.LoginClickButton.Name = "LoginClickButton";
            this.LoginClickButton.Size = new System.Drawing.Size(107, 23);
            this.LoginClickButton.TabIndex = 4;
            this.LoginClickButton.Text = "Zaloguj";
            this.LoginClickButton.UseVisualStyleBackColor = true;
            this.LoginClickButton.Click += new System.EventHandler(this.LoginClickButton_Click);
            // 
            // RegisterClickButton
            // 
            this.RegisterClickButton.Enabled = false;
            this.RegisterClickButton.Location = new System.Drawing.Point(199, 211);
            this.RegisterClickButton.Name = "RegisterClickButton";
            this.RegisterClickButton.Size = new System.Drawing.Size(118, 23);
            this.RegisterClickButton.TabIndex = 5;
            this.RegisterClickButton.Text = "Zarejestruj";
            this.RegisterClickButton.UseVisualStyleBackColor = true;
            this.RegisterClickButton.Click += new System.EventHandler(this.RegisterClickButton_Click);
            // 
            // IPTextField
            // 
            this.IPTextField.Location = new System.Drawing.Point(74, 12);
            this.IPTextField.Name = "IPTextField";
            this.IPTextField.Size = new System.Drawing.Size(258, 23);
            this.IPTextField.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 15);
            this.label3.TabIndex = 7;
            this.label3.Text = "IP serwera";
            // 
            // ConnectClickButton
            // 
            this.ConnectClickButton.Location = new System.Drawing.Point(180, 56);
            this.ConnectClickButton.Name = "ConnectClickButton";
            this.ConnectClickButton.Size = new System.Drawing.Size(152, 28);
            this.ConnectClickButton.TabIndex = 8;
            this.ConnectClickButton.Text = "Połącz się ";
            this.ConnectClickButton.UseVisualStyleBackColor = true;
            this.ConnectClickButton.Click += new System.EventHandler(this.ConnectClickButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(74, 63);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 15);
            this.label4.TabIndex = 9;
            this.label4.Text = "Brak połączenia";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(352, 262);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ConnectClickButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.IPTextField);
            this.Controls.Add(this.RegisterClickButton);
            this.Controls.Add(this.LoginClickButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PasswordTextField);
            this.Controls.Add(this.LoginTextField);
            this.Name = "Form1";
            this.Text = "TIP_";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox LoginTextField;
        private System.Windows.Forms.TextBox PasswordTextField;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button LoginClickButton;
        private System.Windows.Forms.Button RegisterClickButton;
        private System.Windows.Forms.TextBox IPTextField;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button connect_ClickButton;
        private System.Windows.Forms.Button ConnectClickButton;
        private System.Windows.Forms.Label label4;
    }
}

