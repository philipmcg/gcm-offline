namespace Launcher.Forms
{
    partial class TextForm
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
            this.Label = new System.Windows.Forms.Label();
            this.TextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // m_OkButton
            // 
            this.m_OkButton.Location = new System.Drawing.Point(201, 71);
            // 
            // m_CancelButton
            // 
            this.m_CancelButton.Location = new System.Drawing.Point(120, 71);
            // 
            // Label
            // 
            this.Label.AutoSize = true;
            this.Label.Location = new System.Drawing.Point(13, 13);
            this.Label.Name = "Label";
            this.Label.Size = new System.Drawing.Size(35, 13);
            this.Label.TabIndex = 2;
            this.Label.Text = "label1";
            // 
            // TextBox
            // 
            this.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBox.Location = new System.Drawing.Point(16, 41);
            this.TextBox.Name = "TextBox";
            this.TextBox.Size = new System.Drawing.Size(260, 20);
            this.TextBox.TabIndex = 3;
            this.TextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextForm_KeyDown);
            // 
            // TextForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(288, 106);
            this.Controls.Add(this.Label);
            this.Controls.Add(this.TextBox);
            this.Name = "TextForm";
            this.Text = "TextForm";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextForm_KeyDown);
            this.Controls.SetChildIndex(this.TextBox, 0);
            this.Controls.SetChildIndex(this.Label, 0);
            this.Controls.SetChildIndex(this.m_OkButton, 0);
            this.Controls.SetChildIndex(this.m_CancelButton, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label Label;
        public System.Windows.Forms.TextBox TextBox;
    }
}