namespace Launcher.Forms
{
    partial class LoginBox
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
      this.m_tbUsername = new System.Windows.Forms.TextBox();
      this.m_acceptButton = new System.Windows.Forms.Button();
      this.m_cancelButton = new System.Windows.Forms.Button();
      this.m_tbPassword = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.m_registerButton = new System.Windows.Forms.Button();
      this.label3 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // m_tbUsername
      // 
      this.m_tbUsername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.m_tbUsername.Location = new System.Drawing.Point(67, 12);
      this.m_tbUsername.Name = "m_tbUsername";
      this.m_tbUsername.Size = new System.Drawing.Size(195, 20);
      this.m_tbUsername.TabIndex = 0;
      // 
      // m_acceptButton
      // 
      this.m_acceptButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.m_acceptButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.m_acceptButton.Location = new System.Drawing.Point(187, 90);
      this.m_acceptButton.Name = "m_acceptButton";
      this.m_acceptButton.Size = new System.Drawing.Size(75, 23);
      this.m_acceptButton.TabIndex = 2;
      this.m_acceptButton.Text = "OK";
      this.m_acceptButton.UseVisualStyleBackColor = true;
      // 
      // m_cancelButton
      // 
      this.m_cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.m_cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.m_cancelButton.Location = new System.Drawing.Point(25, 90);
      this.m_cancelButton.Name = "m_cancelButton";
      this.m_cancelButton.Size = new System.Drawing.Size(75, 23);
      this.m_cancelButton.TabIndex = 3;
      this.m_cancelButton.Text = "Cancel";
      this.m_cancelButton.UseVisualStyleBackColor = true;
      // 
      // m_tbPassword
      // 
      this.m_tbPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.m_tbPassword.Location = new System.Drawing.Point(67, 38);
      this.m_tbPassword.Name = "m_tbPassword";
      this.m_tbPassword.PasswordChar = '●';
      this.m_tbPassword.Size = new System.Drawing.Size(195, 20);
      this.m_tbPassword.TabIndex = 1;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(6, 12);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(55, 13);
      this.label1.TabIndex = 4;
      this.label1.Text = "Username";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(6, 38);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(53, 13);
      this.label2.TabIndex = 5;
      this.label2.Text = "Password";
      // 
      // m_registerButton
      // 
      this.m_registerButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.m_registerButton.DialogResult = System.Windows.Forms.DialogResult.Retry;
      this.m_registerButton.Location = new System.Drawing.Point(106, 90);
      this.m_registerButton.Name = "m_registerButton";
      this.m_registerButton.Size = new System.Drawing.Size(75, 23);
      this.m_registerButton.TabIndex = 6;
      this.m_registerButton.Text = "Register";
      this.m_registerButton.UseVisualStyleBackColor = true;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.ForeColor = System.Drawing.Color.DarkRed;
      this.label3.Location = new System.Drawing.Point(12, 65);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(35, 13);
      this.label3.TabIndex = 7;
      this.label3.Text = "label3";
      this.label3.Visible = false;
      // 
      // LoginBox
      // 
      this.AcceptButton = this.m_acceptButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.m_cancelButton;
      this.ClientSize = new System.Drawing.Size(274, 125);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.m_registerButton);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.m_tbPassword);
      this.Controls.Add(this.m_cancelButton);
      this.Controls.Add(this.m_acceptButton);
      this.Controls.Add(this.m_tbUsername);
      this.Name = "LoginBox";
      this.ShowIcon = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Login";
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox m_tbUsername;
        private System.Windows.Forms.Button m_acceptButton;
        private System.Windows.Forms.Button m_cancelButton;
        private System.Windows.Forms.TextBox m_tbPassword;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button m_registerButton;
        private System.Windows.Forms.Label label3;
    }
}