namespace Launcher.Forms
{
    partial class BalanceSides
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BalanceSides));
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.totalMenSlider = new System.Windows.Forms.TrackBar();
            this.totalMenLabel = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.infantryRatioSlider = new System.Windows.Forms.TrackBar();
            this.artilleryRatioSlider = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.infantryRatioLabel = new System.Windows.Forms.Label();
            this.artilleryRatioLabel = new System.Windows.Forms.Label();
            this.menGunRatioLabel = new System.Windows.Forms.Label();
            this.menGunRatioSlider = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.imbalancePanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.totalMenSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.infantryRatioSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.artilleryRatioSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.menGunRatioSlider)).BeginInit();
            this.imbalancePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_OkButton
            // 
            this.m_OkButton.Location = new System.Drawing.Point(515, 624);
            // 
            // m_CancelButton
            // 
            this.m_CancelButton.Location = new System.Drawing.Point(434, 624);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(273, 381);
            this.panel1.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.Black;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(301, 381);
            this.panel2.TabIndex = 3;
            // 
            // totalMenSlider
            // 
            this.totalMenSlider.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.totalMenSlider.Location = new System.Drawing.Point(11, 554);
            this.totalMenSlider.Name = "totalMenSlider";
            this.totalMenSlider.Size = new System.Drawing.Size(160, 45);
            this.totalMenSlider.TabIndex = 4;
            // 
            // totalMenLabel
            // 
            this.totalMenLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.totalMenLabel.AutoSize = true;
            this.totalMenLabel.Location = new System.Drawing.Point(177, 575);
            this.totalMenLabel.Name = "totalMenLabel";
            this.totalMenLabel.Size = new System.Drawing.Size(35, 13);
            this.totalMenLabel.TabIndex = 5;
            this.totalMenLabel.Text = "label1";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Location = new System.Drawing.Point(12, 164);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel2);
            this.splitContainer1.Size = new System.Drawing.Size(578, 384);
            this.splitContainer1.SplitterDistance = 273;
            this.splitContainer1.TabIndex = 6;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(353, 624);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "Reset";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Location = new System.Drawing.Point(11, 12);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(346, 146);
            this.textBox1.TabIndex = 8;
            this.textBox1.Text = resources.GetString("textBox1.Text");
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(272, 624);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 9;
            this.button2.Text = "Balance Inf";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // infantryRatioSlider
            // 
            this.infantryRatioSlider.Location = new System.Drawing.Point(3, 29);
            this.infantryRatioSlider.Name = "infantryRatioSlider";
            this.infantryRatioSlider.Size = new System.Drawing.Size(130, 45);
            this.infantryRatioSlider.TabIndex = 10;
            this.infantryRatioSlider.ValueChanged += new System.EventHandler(this.infantryRatioSlider_ValueChanged);
            // 
            // artilleryRatioSlider
            // 
            this.artilleryRatioSlider.Location = new System.Drawing.Point(3, 110);
            this.artilleryRatioSlider.Name = "artilleryRatioSlider";
            this.artilleryRatioSlider.Size = new System.Drawing.Size(133, 45);
            this.artilleryRatioSlider.TabIndex = 11;
            this.artilleryRatioSlider.ValueChanged += new System.EventHandler(this.artilleryRatioSlider_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(114, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "CS :: US Infantry Ratio";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 94);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "CS :: US Artillery Ratio";
            // 
            // infantryRatioLabel
            // 
            this.infantryRatioLabel.AutoSize = true;
            this.infantryRatioLabel.Location = new System.Drawing.Point(142, 29);
            this.infantryRatioLabel.Name = "infantryRatioLabel";
            this.infantryRatioLabel.Size = new System.Drawing.Size(35, 13);
            this.infantryRatioLabel.TabIndex = 14;
            this.infantryRatioLabel.Text = "label4";
            // 
            // artilleryRatioLabel
            // 
            this.artilleryRatioLabel.AutoSize = true;
            this.artilleryRatioLabel.Location = new System.Drawing.Point(142, 110);
            this.artilleryRatioLabel.Name = "artilleryRatioLabel";
            this.artilleryRatioLabel.Size = new System.Drawing.Size(35, 13);
            this.artilleryRatioLabel.TabIndex = 15;
            this.artilleryRatioLabel.Text = "label5";
            // 
            // menGunRatioLabel
            // 
            this.menGunRatioLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.menGunRatioLabel.AutoSize = true;
            this.menGunRatioLabel.Location = new System.Drawing.Point(177, 624);
            this.menGunRatioLabel.Name = "menGunRatioLabel";
            this.menGunRatioLabel.Size = new System.Drawing.Size(35, 13);
            this.menGunRatioLabel.TabIndex = 17;
            this.menGunRatioLabel.Text = "label1";
            // 
            // menGunRatioSlider
            // 
            this.menGunRatioSlider.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.menGunRatioSlider.Location = new System.Drawing.Point(11, 602);
            this.menGunRatioSlider.Name = "menGunRatioSlider";
            this.menGunRatioSlider.Size = new System.Drawing.Size(160, 45);
            this.menGunRatioSlider.TabIndex = 16;
            this.menGunRatioSlider.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(177, 554);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "US Infantry:";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(177, 602);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 13);
            this.label4.TabIndex = 19;
            this.label4.Text = "Men/Gun Ratio:";
            // 
            // imbalancePanel
            // 
            this.imbalancePanel.Controls.Add(this.label2);
            this.imbalancePanel.Controls.Add(this.infantryRatioSlider);
            this.imbalancePanel.Controls.Add(this.artilleryRatioSlider);
            this.imbalancePanel.Controls.Add(this.label3);
            this.imbalancePanel.Controls.Add(this.infantryRatioLabel);
            this.imbalancePanel.Controls.Add(this.artilleryRatioLabel);
            this.imbalancePanel.Location = new System.Drawing.Point(363, 13);
            this.imbalancePanel.Name = "imbalancePanel";
            this.imbalancePanel.Size = new System.Drawing.Size(227, 145);
            this.imbalancePanel.TabIndex = 20;
            // 
            // BalanceSides
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(602, 659);
            this.Controls.Add(this.imbalancePanel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menGunRatioLabel);
            this.Controls.Add(this.menGunRatioSlider);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.totalMenLabel);
            this.Controls.Add(this.totalMenSlider);
            this.MinimumSize = new System.Drawing.Size(400, 500);
            this.Name = "BalanceSides";
            this.Text = "Distribute Troops";
            this.ResizeEnd += new System.EventHandler(this.BalanceSides_ResizeEnd);
            this.SizeChanged += new System.EventHandler(this.BalanceSides_SizeChanged);
            this.Controls.SetChildIndex(this.totalMenSlider, 0);
            this.Controls.SetChildIndex(this.totalMenLabel, 0);
            this.Controls.SetChildIndex(this.splitContainer1, 0);
            this.Controls.SetChildIndex(this.button1, 0);
            this.Controls.SetChildIndex(this.textBox1, 0);
            this.Controls.SetChildIndex(this.button2, 0);
            this.Controls.SetChildIndex(this.m_OkButton, 0);
            this.Controls.SetChildIndex(this.m_CancelButton, 0);
            this.Controls.SetChildIndex(this.menGunRatioSlider, 0);
            this.Controls.SetChildIndex(this.menGunRatioLabel, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.label4, 0);
            this.Controls.SetChildIndex(this.imbalancePanel, 0);
            ((System.ComponentModel.ISupportInitialize)(this.totalMenSlider)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.infantryRatioSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.artilleryRatioSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.menGunRatioSlider)).EndInit();
            this.imbalancePanel.ResumeLayout(false);
            this.imbalancePanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TrackBar totalMenSlider;
        private System.Windows.Forms.Label totalMenLabel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TrackBar infantryRatioSlider;
        private System.Windows.Forms.TrackBar artilleryRatioSlider;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label infantryRatioLabel;
        private System.Windows.Forms.Label artilleryRatioLabel;
        private System.Windows.Forms.Label menGunRatioLabel;
        private System.Windows.Forms.TrackBar menGunRatioSlider;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel imbalancePanel;

    }
}