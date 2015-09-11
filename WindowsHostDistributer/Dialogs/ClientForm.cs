using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsHostDistributer.Distributer;

namespace WindowsHostDistributer.Dialogs
{
    public partial class ClientForm : Form
    {
        private readonly DistributerClient _client = new DistributerClient();
        private readonly HostsFileSynchronizer _synchronizer;
        private HostsSearchParameters _searchParameters = new HostsSearchParameters(string.Empty, string.Empty, string.Empty);

        public ClientForm(HostsFileSynchronizer synchronizer)
        {
            _synchronizer = synchronizer;
            InitializeComponent();
            Font = new Font(Configuration.FontFamily, Configuration.FontSize);

            lvHosts.Columns.AddRange(new ColumnHeader[]
                {
                    new ColumnHeader() {Text = "Name" },
                    new ColumnHeader() {Text = "IP" },
                    new ColumnHeader() {Text = "Description" }
                });
            lvLocalHosts.Columns.AddRange(new ColumnHeader[]
                {
                    new ColumnHeader() {Text = "Name" },
                    new ColumnHeader() {Text = "IP" },
                    new ColumnHeader() {Text = "Description" }
                });

            hiddenDomainsToolStripMenuItem.Checked = Configuration.ShowHiddenHosts;
            hiddenDomainsToolStripMenuItem.Click += (sender, e) =>
                  {
                      Configuration.ShowHiddenHosts = !Configuration.ShowHiddenHosts;
                      hiddenDomainsToolStripMenuItem.Checked = Configuration.ShowHiddenHosts;
                      EventCallbackSystem.InvokeCallback("HostsListUpdated", false);
                  };

            RegisterEvents();

            do
            {
                if (!_client.Connect(Configuration.Address, Configuration.Port))
                {
                    var res = MessageBox.Show(
                        "Failed to connect to " + Configuration.Address + ":" + Configuration.Port +
                        "\nThe configuration window will now open.\nCanceling will terminate the application.",
                        "WHD",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Warning);

                    if (res != DialogResult.OK)
                        Environment.Exit(0);

                    using (var dlg = new ClientConfigurationDlg())
                    {
                        if (dlg.ShowDialog() != DialogResult.OK)
                            Environment.Exit(0);

                        Configuration.Address = dlg.Address;
                        Configuration.Port = dlg.Port;
                        Configuration.Key = dlg.Key;
                    }
                }
            }
            while (!_client.IsConnected);

            new Timer() { Interval = 100, Enabled = true }.Tick += (sender, e) =>
                {
                    _client.Process();
                    Logger.ProcessMessage((msg) =>
                        {
                            AddLog(string.Format(
                                "[{0}][{1}] {2}",
                                msg.UtcDate.ToLocalTime().ToString("HH:mm:ss"),
                                Enum.GetName(typeof(LogType), msg.Type),
                                msg.Text));
                        });
                };

            UpdateInterface();
            Resize += (sender, e) => UpdateInterface();

            lvHosts.KeyDown += LvHosts_KeyDown;
            lvLocalHosts.KeyDown += LvLocalHosts_KeyDown;
            lvHosts.MouseDoubleClick += LvHosts_MouseDoubleClick;
            lvLocalHosts.MouseDoubleClick += LvLocalHosts_MouseDoubleClick;

            notifyIcon1.MouseDoubleClick += (sender, e) => Show();

            Shown += ClientForm_Shown;
            FormClosing += ClientForm_FormClosing;

            startWithWindowsToolStripMenuItem.Checked = Configuration.StartWithWindows;

            lvHosts.ListViewItemSorter = new HostsListViewSorter();
            lvLocalHosts.ListViewItemSorter = new HostsListViewSorter();

            lvHosts.ColumnClick += (sender, e) => ColumnClickHandler(lvHosts, e.Column);
            lvLocalHosts.ColumnClick += (sender, e) => ColumnClickHandler(lvLocalHosts, e.Column);
        }

        private void ColumnClickHandler(ListView lv, int column)
        {
            var sorter = lv.ListViewItemSorter as HostsListViewSorter;
            if (sorter == null)
                return;

            var col = HostsSortingColumn.Unsorted;
            switch (column)
            {
                case 0: col = HostsSortingColumn.Name; break;
                case 1: col = HostsSortingColumn.IP; break;
            }

            if (sorter.Sorting != col)
            {
                sorter.Sorting = col;
                sorter.Inverted = false;
            }
            else
                sorter.Inverted = !sorter.Inverted;

            lv.Sort();
        }

        private async void ClientForm_Shown(object sender, EventArgs e)
        {
#if !DEBUG
            if (Configuration.EnableAutomaticUpdates)
            {
                var updateCheck = await UpdateManager.CheckUpdate();
                if (updateCheck.Result == UpdateCheckResult.UpdateAvailable)
                {
                    UpdateManager.BeginUpdate(updateCheck.Date, !Visible);
                    Application.Exit();
                }
            }
#endif // !DEBUG

            WindowState = Configuration.WindowState;
            Size = Configuration.WindowSize;
            Location = Configuration.WindowLocation;
        }

        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Configuration.WindowLocation = Location;
            Configuration.WindowSize = Size;
            Configuration.WindowState = WindowState;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Win32.WM_SYSCOMMAND)
            {
                if (m.WParam.ToInt32() == Win32.SC_MINIMIZE)
                {
                    // minimize to tray
                    Hide();
                    m.Result = IntPtr.Zero;
                    return;
                }
            }

            base.WndProc(ref m);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.A | Keys.Control))
            {
                addNewHostToolStripMenuItem_Click(this, new EventArgs());
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void LvLocalHosts_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lvLocalHosts.SelectedItems.Count == 1)
            {
                var entry = lvLocalHosts.SelectedItems[0].Tag as HostEntry;

                using (var dlg = new AddEditHostDlg(entry, true))
                {
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        HostsDatabase.Remove(true, entry.Name);
                        HostsDatabase.Add(true, dlg.DomainName, dlg.IPAddress, dlg.Description, dlg.HiddenDomain);
                        HostsDatabase.Save();

                        EventCallbackSystem.InvokeCallback("HostsListUpdated", true);
                    }
                }
            }
        }

        private void LvHosts_MouseDoubleClick(object sender, MouseEventArgs e) // do not use mouse event args as we pass null..
        {
            if (lvHosts.SelectedItems.Count == 1)
            {
                var entry = lvHosts.SelectedItems[0].Tag as HostEntry;

                using (var dlg = new AddEditHostDlg(entry, false))
                {
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        _client.SendRemoveDomain(entry.Name);
                        _client.SendAddDomain(dlg.DomainName, dlg.IPAddress, dlg.Description, dlg.HiddenDomain);
                    }
                }
            }
        }

        private void LvLocalHosts_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                LvLocalHosts_MouseDoubleClick(sender, null);
            }
            else if (e.KeyCode == Keys.Delete)
            {
                e.Handled = true;

                if (lvLocalHosts.SelectedItems.Count > 0)
                {
                    var str =
                        lvLocalHosts.SelectedItems.Count > 1 ?
                        "Are you sure you wish to delete " + lvLocalHosts.SelectedItems.Count + " local domains?\nThis action cannot be undone." :
                        "Are you sure you wish to delete this local domain?\nThis action cannot be undone.";

                    var res = MessageBox.Show(
                        str,
                        "Confirm remove local domain(s)",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (res == DialogResult.Yes)
                    {
                        foreach (ListViewItem lvi in lvLocalHosts.SelectedItems)
                        {
                            // remove manually
                            HostsDatabase.Remove(true, (lvi.Tag as HostEntry).Name);
                        }

                        HostsDatabase.Save();
                        EventCallbackSystem.InvokeCallback("HostsListUpdated", true);
                    }
                }
            }
        }

        private void LvHosts_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                LvHosts_MouseDoubleClick(sender, null);
            }
            else if (e.KeyCode == Keys.Delete)
            {
                e.Handled = true;

                if (lvHosts.SelectedItems.Count > 0)
                {
                    var str =
                        lvHosts.SelectedItems.Count > 1 ?
                        "Are you sure you wish to delete " + lvHosts.SelectedItems.Count + " domains from the server?\nThis action cannot be undone." :
                        "Are you sure you wish to delete this domain from the server?\nThis action cannot be undone.";

                    var res = MessageBox.Show(
                        str,
                        "Confirm remove remote domain(s)",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (res == DialogResult.Yes)
                    {
                        foreach (ListViewItem lvi in lvHosts.SelectedItems)
                        {
                            _client.SendRemoveDomain((lvi.Tag as HostEntry).Name);
                        }
                    }
                }
            }
        }

        private void AddLog(string text)
        {
            if (txtLog.Text.Length > 0)
                txtLog.AppendText("\r\n");

            txtLog.AppendText(text);
            txtLog.Select(txtLog.Text.Length, 0);
        }

        private void RegisterEvents()
        {
            EventCallbackSystem.RegisterCallback<ClientAuthorizedEvent>("ClientAuthorized", () =>
                {
                    Logger.Log(LogType.Debug, "Authorized to server.");
                });

            EventCallbackSystem.RegisterCallback<HostsListUpdatedEvent>("HostsListUpdated", (changed) =>
                 {
                     lvHosts.BeginUpdate();
                     lvLocalHosts.BeginUpdate();
                     lvHosts.Items.Clear();
                     lvLocalHosts.Items.Clear();
                     HostsDatabase.Lock();

                     var lviList = new List<ListViewItem>(Math.Max(HostsDatabase.Hosts.Count(), HostsDatabase.LocalHosts.Count()));

                     foreach (var host in HostsDatabase.Hosts)
                     {
                         if ((Configuration.ShowHiddenHosts ||
                            !host.Hidden) &&
                            _searchParameters.Matches(host))
                         {
                             var lvi = new ListViewItem(host.Name);
                             lvi.SubItems.Add(host.IP);
                             lvi.SubItems.Add(host.Description);
                             lvi.Tag = host;
                             //lvHosts.Items.Add(lvi);
                             lviList.Add(lvi);
                         }
                     }
                     lvHosts.Items.AddRange(lviList.ToArray());
                     if (lvHosts.Items.Count < HostsDatabase.Hosts.Count())
                     {
                         gbHosts.Text = string.Format(
                             "Domains ({0} of {1})",
                             lvHosts.Items.Count.FormatNumber(),
                             HostsDatabase.Hosts.Count().FormatNumber());
                     }
                     else
                         gbHosts.Text = string.Format("Domains ({0})", lvHosts.Items.Count.FormatNumber());
                     lviList.Clear();

                     foreach (var host in HostsDatabase.LocalHosts)
                     {
                         if ((Configuration.ShowHiddenHosts ||
                            !host.Hidden) &&
                            _searchParameters.Matches(host))
                         {
                             var lvi = new ListViewItem(host.Name);
                             lvi.SubItems.Add(host.IP);
                             lvi.SubItems.Add(host.Description);
                             lvi.Tag = host;
                             //lvLocalHosts.Items.Add(lvi);
                             lviList.Add(lvi);
                         }
                     }
                     lvLocalHosts.Items.AddRange(lviList.ToArray());
                     if (lvLocalHosts.Items.Count < HostsDatabase.LocalHosts.Count())
                     {
                         gbLocalHosts.Text = string.Format(
                             "Local domains ({0} of {1})",
                             lvLocalHosts.Items.Count.FormatNumber(),
                             HostsDatabase.LocalHosts.Count().FormatNumber());
                     }
                     else
                         gbLocalHosts.Text = string.Format("Local domains ({0})", lvLocalHosts.Items.Count.FormatNumber());

                     HostsDatabase.Unlock();
                     lvHosts.EndUpdate();
                     lvLocalHosts.EndUpdate();
                     //Logger.Log(LogType.Debug, "Hosts list updated.");

                     UpdateInterface();

                     if (changed) // only synchronize if the hostsdatabase was literally changed - ignore searching
                         _synchronizer.PostSynchronize();
                 });
        }

        private void UpdateInterface()
        {
            var listViewArea = new Rectangle(5, menuStrip1.Height + 5, Convert.ToInt32(ClientSize.Width * 0.60) - 10, ClientSize.Height - 5);

            #region lvHosts and lvLocalHosts
            int gbHeight = (listViewArea.Height - menuStrip1.Height - 10) / 2;

            // Name, IP, Description columns
            gbHosts.Location = new Point(listViewArea.X, listViewArea.Y);
            gbHosts.Size = new Size(listViewArea.Width, gbHeight);

            gbLocalHosts.Location = new Point(listViewArea.X, gbHosts.Location.Y + gbHosts.Size.Height + 5);
            gbLocalHosts.Size = new Size(listViewArea.Width, gbHeight);

            lvHosts.Columns[0].Width = Convert.ToInt32(lvHosts.ClientSize.Width * 0.40);
            lvHosts.Columns[1].Width = Convert.ToInt32(lvHosts.ClientSize.Width * 0.20);
            lvHosts.Columns[2].Width = Convert.ToInt32(lvHosts.ClientSize.Width * 0.40) - 1;

            lvLocalHosts.Columns[0].Width = Convert.ToInt32(lvLocalHosts.ClientSize.Width * 0.40);
            lvLocalHosts.Columns[1].Width = Convert.ToInt32(lvLocalHosts.ClientSize.Width * 0.20);
            lvLocalHosts.Columns[2].Width = Convert.ToInt32(lvLocalHosts.ClientSize.Width * 0.40) - 1;
            #endregion

            #region txtLog
            gbLog.Location = new Point(listViewArea.Right + 5, menuStrip1.Height + 5);
            gbLog.Size = new Size(ClientSize.Width - gbLog.Location.X - 10, ClientSize.Height - (menuStrip1.Height + 10));
            #endregion
        }

        private void addNewHostToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dlg = new AddEditHostDlg())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if (dlg.LocalDomain)
                    {
                        HostsDatabase.Add(true, dlg.DomainName, dlg.IPAddress, dlg.Description, dlg.HiddenDomain);
                        HostsDatabase.Save();
                        EventCallbackSystem.InvokeCallback("HostsListUpdated", true);
                    }
                    else
                        _client.SendAddDomain(dlg.DomainName, dlg.IPAddress, dlg.Description, dlg.HiddenDomain);
                }
            }
        }

        #region Convert hosts file into hosts.xml (menu item)
        private void convertHostsFileIntoHostsxmlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog()
            {
                Title = "Select hosts file"
            })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var windowsDomains = new Dictionary<string, HostEntry>();
                    var lines = File.ReadAllLines(ofd.FileName);
                    for (int i = 0; i < lines.Length; ++i)
                    {
                        int posComment = lines[i].IndexOf('#');
                        if (posComment != -1)
                            lines[i] = lines[i].Substring(0, posComment);
                        lines[i] = lines[i].Trim();
                        if (lines[i].Length > 0)
                        {
                            var data = lines[i].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                            if (data.Length >= 2 &&
                                Regex.IsMatch(data[0], GlobalRules.IPRegex, RegexOptions.IgnoreCase))
                            {
                                for (int j = 1; j < data.Length; ++j)
                                {
                                    if (Regex.IsMatch(data[j], GlobalRules.DomainRegex, RegexOptions.IgnoreCase))
                                    {
                                        if (!windowsDomains.ContainsKey(data[j].ToLower()))
                                            windowsDomains.Add(data[j].ToLower(), new HostEntry(data[j].ToLower(), data[0], string.Empty, false));
                                        else
                                            Logger.Log(LogType.Warning, "Multiple entries of domain in: {0}", lines[i]);
                                    }
                                }
                            }
                            else
                                Logger.Log(LogType.Debug, "Ignored line: {0}", lines[i]);
                        }
                    }

                    if (windowsDomains.Count == 0)
                    {
                        MessageBox.Show("No domains found in selected hosts file.", "Convert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var res = MessageBox.Show(
                        windowsDomains.Count + " domains found. Do you want them all to be set as hidden?",
                        "Convert",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Information);

                    if (res == DialogResult.Cancel)
                        return;

                    var hidden = res == DialogResult.Yes;

                    var sb = new StringBuilder(4096);
                    sb.AppendLine("<?xml version=\"1.0\"?>");
                    sb.AppendLine();
                    sb.AppendLine("<Database>");
                    sb.AppendLine("\t<Hosts>");
                    foreach (var host in windowsDomains.Values)
                    {
                        sb.AppendLine(string.Format(
                            "\t\t<Host Name=\"{0}\" IP=\"{1}\" Description=\"{2}\"{3} />",
                            host.Name.Escape(EscapeLanguage.Xml),
                            host.IP.Escape(EscapeLanguage.Xml),
                            host.Description.Escape(EscapeLanguage.Xml),
                            hidden ? " Hidden=\"True\"" : ""));
                    }
                    sb.AppendLine("\t</Hosts>");
                    sb.Append("</Database>");

                    using (var sfd = new SaveFileDialog()
                    {
                        Title = "Save hosts.xml as ...",
                        Filter = "XML File (*.xml)|*.xml"
                    })
                    {
                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            File.WriteAllText(sfd.FileName, sb.ToString());
                        }
                    }
                }
            }
        }
        #endregion

        private void startWithWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            startWithWindowsToolStripMenuItem.Checked = !startWithWindowsToolStripMenuItem.Checked;
            Configuration.StartWithWindows = startWithWindowsToolStripMenuItem.Checked;
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            using (var dlg = new SearchDlg())
            {
                dlg.SearchParameters = _searchParameters;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    _searchParameters = dlg.SearchParameters;
                    EventCallbackSystem.InvokeCallback("HostsListUpdated", false);
                }
            }
        }
    }
}
