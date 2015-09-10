namespace WindowsHostDistributer.Dialogs
{
    partial class ClientForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClientForm));
            this.lvHosts = new System.Windows.Forms.ListView();
            this.lvLocalHosts = new System.Windows.Forms.ListView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hostsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addNewHostToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hiddenDomainsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertHostsFileIntoHostsxmlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gbHosts = new System.Windows.Forms.GroupBox();
            this.gbLocalHosts = new System.Windows.Forms.GroupBox();
            this.gbLog = new System.Windows.Forms.GroupBox();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.menuStrip1.SuspendLayout();
            this.gbHosts.SuspendLayout();
            this.gbLocalHosts.SuspendLayout();
            this.gbLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvHosts
            // 
            this.lvHosts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvHosts.FullRowSelect = true;
            this.lvHosts.GridLines = true;
            this.lvHosts.Location = new System.Drawing.Point(6, 19);
            this.lvHosts.Name = "lvHosts";
            this.lvHosts.Size = new System.Drawing.Size(182, 94);
            this.lvHosts.TabIndex = 0;
            this.lvHosts.UseCompatibleStateImageBehavior = false;
            this.lvHosts.View = System.Windows.Forms.View.Details;
            // 
            // lvLocalHosts
            // 
            this.lvLocalHosts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvLocalHosts.FullRowSelect = true;
            this.lvLocalHosts.GridLines = true;
            this.lvLocalHosts.Location = new System.Drawing.Point(6, 19);
            this.lvLocalHosts.Name = "lvLocalHosts";
            this.lvLocalHosts.Size = new System.Drawing.Size(148, 104);
            this.lvLocalHosts.TabIndex = 1;
            this.lvLocalHosts.UseCompatibleStateImageBehavior = false;
            this.lvLocalHosts.View = System.Windows.Forms.View.Details;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuToolStripMenuItem,
            this.hostsToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(913, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuToolStripMenuItem
            // 
            this.menuToolStripMenuItem.Name = "menuToolStripMenuItem";
            this.menuToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.menuToolStripMenuItem.Text = "Menu";
            // 
            // hostsToolStripMenuItem
            // 
            this.hostsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addNewHostToolStripMenuItem});
            this.hostsToolStripMenuItem.Name = "hostsToolStripMenuItem";
            this.hostsToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.hostsToolStripMenuItem.Text = "Hosts";
            // 
            // addNewHostToolStripMenuItem
            // 
            this.addNewHostToolStripMenuItem.Image = global::WindowsHostDistributer.Media.Database_Add;
            this.addNewHostToolStripMenuItem.Name = "addNewHostToolStripMenuItem";
            this.addNewHostToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.addNewHostToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.addNewHostToolStripMenuItem.Text = "Add new host";
            this.addNewHostToolStripMenuItem.Click += new System.EventHandler(this.addNewHostToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hiddenDomainsToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // hiddenDomainsToolStripMenuItem
            // 
            this.hiddenDomainsToolStripMenuItem.Name = "hiddenDomainsToolStripMenuItem";
            this.hiddenDomainsToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.hiddenDomainsToolStripMenuItem.Text = "Hidden domains";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.convertHostsFileIntoHostsxmlToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // convertHostsFileIntoHostsxmlToolStripMenuItem
            // 
            this.convertHostsFileIntoHostsxmlToolStripMenuItem.Image = global::WindowsHostDistributer.Media.Document_Blueprint;
            this.convertHostsFileIntoHostsxmlToolStripMenuItem.Name = "convertHostsFileIntoHostsxmlToolStripMenuItem";
            this.convertHostsFileIntoHostsxmlToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.convertHostsFileIntoHostsxmlToolStripMenuItem.Text = "Convert hosts file into hosts.xml";
            this.convertHostsFileIntoHostsxmlToolStripMenuItem.Click += new System.EventHandler(this.convertHostsFileIntoHostsxmlToolStripMenuItem_Click);
            // 
            // gbHosts
            // 
            this.gbHosts.Controls.Add(this.lvHosts);
            this.gbHosts.Location = new System.Drawing.Point(12, 38);
            this.gbHosts.Name = "gbHosts";
            this.gbHosts.Size = new System.Drawing.Size(194, 119);
            this.gbHosts.TabIndex = 3;
            this.gbHosts.TabStop = false;
            this.gbHosts.Text = "Domains";
            // 
            // gbLocalHosts
            // 
            this.gbLocalHosts.Controls.Add(this.lvLocalHosts);
            this.gbLocalHosts.Location = new System.Drawing.Point(12, 163);
            this.gbLocalHosts.Name = "gbLocalHosts";
            this.gbLocalHosts.Size = new System.Drawing.Size(160, 129);
            this.gbLocalHosts.TabIndex = 4;
            this.gbLocalHosts.TabStop = false;
            this.gbLocalHosts.Text = "Local domains";
            // 
            // gbLog
            // 
            this.gbLog.Controls.Add(this.txtLog);
            this.gbLog.Location = new System.Drawing.Point(212, 38);
            this.gbLog.Name = "gbLog";
            this.gbLog.Size = new System.Drawing.Size(243, 266);
            this.gbLog.TabIndex = 5;
            this.gbLog.TabStop = false;
            this.gbLog.Text = "Log";
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.BackColor = System.Drawing.Color.White;
            this.txtLog.Location = new System.Drawing.Point(6, 19);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(231, 241);
            this.txtLog.TabIndex = 0;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Windows Host Distributer Client";
            this.notifyIcon1.Visible = true;
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(913, 460);
            this.Controls.Add(this.gbLog);
            this.Controls.Add(this.gbLocalHosts);
            this.Controls.Add(this.gbHosts);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ClientForm";
            this.Text = "Windows Host Distributer Client";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.gbHosts.ResumeLayout(false);
            this.gbLocalHosts.ResumeLayout(false);
            this.gbLog.ResumeLayout(false);
            this.gbLog.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvHosts;
        private System.Windows.Forms.ListView lvLocalHosts;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuToolStripMenuItem;
        private System.Windows.Forms.GroupBox gbHosts;
        private System.Windows.Forms.GroupBox gbLocalHosts;
        private System.Windows.Forms.GroupBox gbLog;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.ToolStripMenuItem hostsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addNewHostToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hiddenDomainsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertHostsFileIntoHostsxmlToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
    }
}

