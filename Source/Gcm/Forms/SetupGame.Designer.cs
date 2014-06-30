namespace Launcher.Forms
{
    partial class SetupGame
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
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.button1 = new System.Windows.Forms.Button();
      this.m_optionsPanel = new System.Windows.Forms.Panel();
      this.SuspendLayout();
      // 
      // m_OkButton
      // 
      this.m_OkButton.Location = new System.Drawing.Point(360, 90);
      // 
      // m_CancelButton
      // 
      this.m_CancelButton.Location = new System.Drawing.Point(279, 90);
      // 
      // textBox1
      // 
      this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.textBox1.Location = new System.Drawing.Point(13, 13);
      this.textBox1.Multiline = true;
      this.textBox1.Name = "textBox1";
      this.textBox1.Size = new System.Drawing.Size(206, 38);
      this.textBox1.TabIndex = 8;
      this.textBox1.Text = "If you are unsure which settings to choose, leave them at the default.";
      // 
      // button1
      // 
      this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.button1.Location = new System.Drawing.Point(198, 90);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(75, 23);
      this.button1.TabIndex = 9;
      this.button1.Text = "Reset";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // m_optionsPanel
      // 
      this.m_optionsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.m_optionsPanel.Location = new System.Drawing.Point(13, 48);
      this.m_optionsPanel.Name = "m_optionsPanel";
      this.m_optionsPanel.Size = new System.Drawing.Size(422, 35);
      this.m_optionsPanel.TabIndex = 10;
      // 
      // SetupGame
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(447, 125);
      this.Controls.Add(this.m_optionsPanel);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.textBox1);
      this.Name = "SetupGame";
      this.Text = "Game Settings";
      this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SetupGame_KeyDown);
      this.Controls.SetChildIndex(this.m_OkButton, 0);
      this.Controls.SetChildIndex(this.m_CancelButton, 0);
      this.Controls.SetChildIndex(this.textBox1, 0);
      this.Controls.SetChildIndex(this.button1, 0);
      this.Controls.SetChildIndex(this.m_optionsPanel, 0);
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel m_optionsPanel;
    }
}