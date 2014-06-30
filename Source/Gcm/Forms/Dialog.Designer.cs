namespace Launcher.Forms
{
    partial class Dialog
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
            this.m_OkButton = new System.Windows.Forms.Button();
            this.m_CancelButton = new System.Windows.Forms.Button();
            this.m_label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_OkButton
            // 
            this.m_OkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_OkButton.Location = new System.Drawing.Point(201, 91);
            this.m_OkButton.Name = "m_OkButton";
            this.m_OkButton.Size = new System.Drawing.Size(75, 23);
            this.m_OkButton.TabIndex = 0;
            this.m_OkButton.Text = "OK";
            this.m_OkButton.UseVisualStyleBackColor = true;
            // 
            // m_CancelButton
            // 
            this.m_CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_CancelButton.Location = new System.Drawing.Point(120, 91);
            this.m_CancelButton.Name = "m_CancelButton";
            this.m_CancelButton.Size = new System.Drawing.Size(75, 23);
            this.m_CancelButton.TabIndex = 1;
            this.m_CancelButton.Text = "Cancel";
            this.m_CancelButton.UseVisualStyleBackColor = true;
            // 
            // m_label
            // 
            this.m_label.AutoSize = true;
            this.m_label.Location = new System.Drawing.Point(13, 13);
            this.m_label.Name = "m_label";
            this.m_label.Size = new System.Drawing.Size(35, 13);
            this.m_label.TabIndex = 2;
            this.m_label.Text = "label1";
            this.m_label.Visible = false;
            // 
            // Dialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_CancelButton;
            this.ClientSize = new System.Drawing.Size(288, 126);
            this.Controls.Add(this.m_label);
            this.Controls.Add(this.m_CancelButton);
            this.Controls.Add(this.m_OkButton);
            this.Name = "Dialog";
            this.ShowIcon = false;
            this.Text = "Dialog";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.Button m_OkButton;
        protected System.Windows.Forms.Button m_CancelButton;
        private System.Windows.Forms.Label m_label;
    }
}