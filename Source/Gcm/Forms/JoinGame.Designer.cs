namespace Launcher.Forms
{
    partial class JoinGame
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
            this.GameList = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_OkButton
            // 
            this.m_OkButton.Location = new System.Drawing.Point(139, 59);
            this.m_OkButton.Text = "Join";
            // 
            // m_CancelButton
            // 
            this.m_CancelButton.Location = new System.Drawing.Point(58, 59);
            // 
            // GameList
            // 
            this.GameList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.GameList.FormattingEnabled = true;
            this.GameList.Location = new System.Drawing.Point(77, 9);
            this.GameList.Name = "GameList";
            this.GameList.Size = new System.Drawing.Size(118, 21);
            this.GameList.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Game ID";
            // 
            // JoinGame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(226, 94);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.GameList);
            this.Name = "JoinGame";
            this.Text = "Join Game";
            this.Controls.SetChildIndex(this.GameList, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.m_OkButton, 0);
            this.Controls.SetChildIndex(this.m_CancelButton, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox GameList;

    }
}