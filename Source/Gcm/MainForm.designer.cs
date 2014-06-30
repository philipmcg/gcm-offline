namespace Launcher
{
    partial class MainForm
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
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
      this.progressBar = new System.Windows.Forms.ProgressBar();
      this.statusLabel = new System.Windows.Forms.Label();
      this.launchBattleButton = new System.Windows.Forms.Button();
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.gameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.prepareScenarioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
      this.startScourgeOfWarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.closeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
      this.sOWToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.openLogFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.deleteTemporaryFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.disableMusicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.enableBugleCallsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.disableBugleCallsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.disableAllNonGCMModsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.enableGCMHotkeysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.devToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.startSingleplayerCampaignToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.uploadDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.uploadScreenshotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.continueSingleplayerCampaignToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.crashToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.installScenarioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.reportPCSpecsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.killSOWProcessesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.panel1 = new System.Windows.Forms.Panel();
      this.startSowButton = new System.Windows.Forms.Button();
      this.checkVersionTimer = new System.Windows.Forms.Timer(this.components);
      this.reportBattleTimer = new System.Windows.Forms.Timer(this.components);
      this.menuStrip1.SuspendLayout();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // progressBar
      // 
      this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.progressBar.Location = new System.Drawing.Point(13, 289);
      this.progressBar.Name = "progressBar";
      this.progressBar.Size = new System.Drawing.Size(503, 23);
      this.progressBar.TabIndex = 1;
      // 
      // statusLabel
      // 
      this.statusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.statusLabel.AutoSize = true;
      this.statusLabel.Location = new System.Drawing.Point(13, 270);
      this.statusLabel.Name = "statusLabel";
      this.statusLabel.Size = new System.Drawing.Size(37, 13);
      this.statusLabel.TabIndex = 2;
      this.statusLabel.Text = "Status";
      // 
      // launchBattleButton
      // 
      this.launchBattleButton.Location = new System.Drawing.Point(3, 59);
      this.launchBattleButton.Name = "launchBattleButton";
      this.launchBattleButton.Size = new System.Drawing.Size(150, 50);
      this.launchBattleButton.TabIndex = 3;
      this.launchBattleButton.Text = "Create Battle";
      this.launchBattleButton.UseVisualStyleBackColor = true;
      // 
      // menuStrip1
      // 
      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gameToolStripMenuItem,
            this.sOWToolStripMenuItem,
            this.devToolStripMenuItem});
      this.menuStrip1.Location = new System.Drawing.Point(0, 0);
      this.menuStrip1.Name = "menuStrip1";
      this.menuStrip1.Size = new System.Drawing.Size(528, 24);
      this.menuStrip1.TabIndex = 4;
      this.menuStrip1.Text = "menuStrip1";
      // 
      // gameToolStripMenuItem
      // 
      this.gameToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.prepareScenarioToolStripMenuItem,
            this.toolStripSeparator2,
            this.startScourgeOfWarToolStripMenuItem,
            this.toolStripSeparator1,
            this.closeToolStripMenuItem1});
      this.gameToolStripMenuItem.Name = "gameToolStripMenuItem";
      this.gameToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
      this.gameToolStripMenuItem.Text = "Game";
      // 
      // prepareScenarioToolStripMenuItem
      // 
      this.prepareScenarioToolStripMenuItem.Name = "prepareScenarioToolStripMenuItem";
      this.prepareScenarioToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
      this.prepareScenarioToolStripMenuItem.Text = "Create Battle";
      // 
      // toolStripSeparator2
      // 
      this.toolStripSeparator2.Name = "toolStripSeparator2";
      this.toolStripSeparator2.Size = new System.Drawing.Size(179, 6);
      // 
      // startScourgeOfWarToolStripMenuItem
      // 
      this.startScourgeOfWarToolStripMenuItem.Name = "startScourgeOfWarToolStripMenuItem";
      this.startScourgeOfWarToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
      this.startScourgeOfWarToolStripMenuItem.Text = "Start Scourge of War";
      this.startScourgeOfWarToolStripMenuItem.Click += new System.EventHandler(this.startScourgeOfWarToolStripMenuItem_Click);
      // 
      // toolStripSeparator1
      // 
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(179, 6);
      // 
      // closeToolStripMenuItem1
      // 
      this.closeToolStripMenuItem1.Name = "closeToolStripMenuItem1";
      this.closeToolStripMenuItem1.Size = new System.Drawing.Size(182, 22);
      this.closeToolStripMenuItem1.Text = "Close";
      // 
      // sOWToolStripMenuItem
      // 
      this.sOWToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openLogFileToolStripMenuItem,
            this.deleteTemporaryFilesToolStripMenuItem,
            this.disableMusicToolStripMenuItem,
            this.enableBugleCallsToolStripMenuItem,
            this.disableBugleCallsToolStripMenuItem,
            this.disableAllNonGCMModsToolStripMenuItem,
            this.enableGCMHotkeysToolStripMenuItem});
      this.sOWToolStripMenuItem.Name = "sOWToolStripMenuItem";
      this.sOWToolStripMenuItem.Size = new System.Drawing.Size(45, 20);
      this.sOWToolStripMenuItem.Text = "SOW";
      // 
      // openLogFileToolStripMenuItem
      // 
      this.openLogFileToolStripMenuItem.Name = "openLogFileToolStripMenuItem";
      this.openLogFileToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
      this.openLogFileToolStripMenuItem.Text = "Open Log File";
      // 
      // deleteTemporaryFilesToolStripMenuItem
      // 
      this.deleteTemporaryFilesToolStripMenuItem.Name = "deleteTemporaryFilesToolStripMenuItem";
      this.deleteTemporaryFilesToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
      this.deleteTemporaryFilesToolStripMenuItem.Text = "Delete temporary files";
      this.deleteTemporaryFilesToolStripMenuItem.ToolTipText = "Clears any scenarios, log files and temporary files that are more than two days o" +
    "ld.";
      // 
      // disableMusicToolStripMenuItem
      // 
      this.disableMusicToolStripMenuItem.Name = "disableMusicToolStripMenuItem";
      this.disableMusicToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
      this.disableMusicToolStripMenuItem.Text = "Turn off Menu Music";
      this.disableMusicToolStripMenuItem.ToolTipText = "Disables the Scourge of War menu music.";
      // 
      // enableBugleCallsToolStripMenuItem
      // 
      this.enableBugleCallsToolStripMenuItem.Name = "enableBugleCallsToolStripMenuItem";
      this.enableBugleCallsToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
      this.enableBugleCallsToolStripMenuItem.Text = "Turn on Bugle Calls";
      this.enableBugleCallsToolStripMenuItem.ToolTipText = "Enables the in-game bugle calls if they were disabled.";
      // 
      // disableBugleCallsToolStripMenuItem
      // 
      this.disableBugleCallsToolStripMenuItem.Name = "disableBugleCallsToolStripMenuItem";
      this.disableBugleCallsToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
      this.disableBugleCallsToolStripMenuItem.Text = "Turn off Bugle Calls";
      this.disableBugleCallsToolStripMenuItem.ToolTipText = "Disables the in-game bugle calls.";
      // 
      // disableAllNonGCMModsToolStripMenuItem
      // 
      this.disableAllNonGCMModsToolStripMenuItem.Name = "disableAllNonGCMModsToolStripMenuItem";
      this.disableAllNonGCMModsToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
      this.disableAllNonGCMModsToolStripMenuItem.Text = "Disable all non-GCM mods";
      // 
      // enableGCMHotkeysToolStripMenuItem
      // 
      this.enableGCMHotkeysToolStripMenuItem.Name = "enableGCMHotkeysToolStripMenuItem";
      this.enableGCMHotkeysToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
      this.enableGCMHotkeysToolStripMenuItem.Text = "Enable GCM Hotkeys";
      this.enableGCMHotkeysToolStripMenuItem.ToolTipText = "Installs GCM keyboard.csv to your Work directory.";
      // 
      // devToolStripMenuItem
      // 
      this.devToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startSingleplayerCampaignToolStripMenuItem,
            this.uploadDataToolStripMenuItem,
            this.uploadScreenshotToolStripMenuItem,
            this.continueSingleplayerCampaignToolStripMenuItem,
            this.crashToolStripMenuItem,
            this.installScenarioToolStripMenuItem,
            this.reportPCSpecsToolStripMenuItem,
            this.killSOWProcessesToolStripMenuItem});
      this.devToolStripMenuItem.Name = "devToolStripMenuItem";
      this.devToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
      this.devToolStripMenuItem.Text = "Dev";
      this.devToolStripMenuItem.Visible = false;
      // 
      // startSingleplayerCampaignToolStripMenuItem
      // 
      this.startSingleplayerCampaignToolStripMenuItem.Name = "startSingleplayerCampaignToolStripMenuItem";
      this.startSingleplayerCampaignToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
      this.startSingleplayerCampaignToolStripMenuItem.Text = "Start Campaign";
      // 
      // uploadDataToolStripMenuItem
      // 
      this.uploadDataToolStripMenuItem.Name = "uploadDataToolStripMenuItem";
      this.uploadDataToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
      this.uploadDataToolStripMenuItem.Text = "Upload Data";
      // 
      // uploadScreenshotToolStripMenuItem
      // 
      this.uploadScreenshotToolStripMenuItem.Name = "uploadScreenshotToolStripMenuItem";
      this.uploadScreenshotToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
      this.uploadScreenshotToolStripMenuItem.Text = "Upload Screenshot";
      this.uploadScreenshotToolStripMenuItem.Click += new System.EventHandler(this.uploadScreenshotToolStripMenuItem_Click);
      // 
      // continueSingleplayerCampaignToolStripMenuItem
      // 
      this.continueSingleplayerCampaignToolStripMenuItem.Name = "continueSingleplayerCampaignToolStripMenuItem";
      this.continueSingleplayerCampaignToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
      // 
      // crashToolStripMenuItem
      // 
      this.crashToolStripMenuItem.Name = "crashToolStripMenuItem";
      this.crashToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
      this.crashToolStripMenuItem.Text = "Crash";
      this.crashToolStripMenuItem.Click += new System.EventHandler(this.crashToolStripMenuItem_Click);
      // 
      // installScenarioToolStripMenuItem
      // 
      this.installScenarioToolStripMenuItem.Name = "installScenarioToolStripMenuItem";
      this.installScenarioToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
      this.installScenarioToolStripMenuItem.Text = "Install Scenario";
      // 
      // reportPCSpecsToolStripMenuItem
      // 
      this.reportPCSpecsToolStripMenuItem.Name = "reportPCSpecsToolStripMenuItem";
      this.reportPCSpecsToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
      this.reportPCSpecsToolStripMenuItem.Text = "Report PC Specs";
      // 
      // killSOWProcessesToolStripMenuItem
      // 
      this.killSOWProcessesToolStripMenuItem.Name = "killSOWProcessesToolStripMenuItem";
      this.killSOWProcessesToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
      this.killSOWProcessesToolStripMenuItem.Text = "Kill SOW Processes";
      this.killSOWProcessesToolStripMenuItem.Click += new System.EventHandler(this.killSOWProcessesToolStripMenuItem_Click);
      // 
      // panel1
      // 
      this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(220)))), ((int)(((byte)(192)))));
      this.panel1.Controls.Add(this.startSowButton);
      this.panel1.Controls.Add(this.launchBattleButton);
      this.panel1.Location = new System.Drawing.Point(13, 27);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(503, 240);
      this.panel1.TabIndex = 5;
      // 
      // startSowButton
      // 
      this.startSowButton.Location = new System.Drawing.Point(3, 3);
      this.startSowButton.Name = "startSowButton";
      this.startSowButton.Size = new System.Drawing.Size(150, 50);
      this.startSowButton.TabIndex = 5;
      this.startSowButton.Text = "Start Scourge of War";
      this.startSowButton.UseVisualStyleBackColor = true;
      this.startSowButton.Click += new System.EventHandler(this.startSowButton_Click);
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(220)))), ((int)(((byte)(192)))));
      this.ClientSize = new System.Drawing.Size(528, 324);
      this.Controls.Add(this.panel1);
      this.Controls.Add(this.statusLabel);
      this.Controls.Add(this.progressBar);
      this.Controls.Add(this.menuStrip1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MainMenuStrip = this.menuStrip1;
      this.Name = "MainForm";
      this.Text = "GCM Launcher";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MapToolMainForm_FormClosing);
      this.Load += new System.EventHandler(this.MainForm_Load);
      this.Shown += new System.EventHandler(this.MainForm_Shown);
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      this.panel1.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Button launchBattleButton;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem gameToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem prepareScenarioToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem devToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uploadDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sOWToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteTemporaryFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disableMusicToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disableBugleCallsToolStripMenuItem;
        private System.Windows.Forms.Timer checkVersionTimer;
        private System.Windows.Forms.ToolStripMenuItem uploadScreenshotToolStripMenuItem;
        private System.Windows.Forms.Button startSowButton;
        private System.Windows.Forms.ToolStripMenuItem openLogFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disableAllNonGCMModsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem continueSingleplayerCampaignToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startSingleplayerCampaignToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem enableBugleCallsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem enableGCMHotkeysToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem crashToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem installScenarioToolStripMenuItem;
        private System.Windows.Forms.Timer reportBattleTimer;
        private System.Windows.Forms.ToolStripMenuItem reportPCSpecsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem killSOWProcessesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem startScourgeOfWarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem1;
    }
}

