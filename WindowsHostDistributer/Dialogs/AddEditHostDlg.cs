using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsHostDistributer.Dialogs
{
    public partial class AddEditHostDlg : Form
    {
        private readonly bool _isEdit;

        public string DomainName
        {
            get { return txtDomainName.Text; }
            set { txtDomainName.Text = value; }
        }

        public string IPAddress
        {
            get { return txtIPAddress.Text; }
            set { txtIPAddress.Text = value; }
        }

        public string Description
        {
            get { return txtDescription.Text; }
            set { txtDescription.Text = value; }
        }

        public bool LocalDomain
        {
            get { return cbLocal.Checked; }
            set { cbLocal.Checked = value; }
        }

        public bool HiddenDomain
        {
            get { return cbHidden.Checked; }
            set { cbHidden.Checked = value; }
        }

        public AddEditHostDlg(HostEntry host = null, bool isLocal = false)
        {
            InitializeComponent();

            _isEdit = host != null;
            cbLocal.Enabled = !_isEdit;

            Text = _isEdit ? "Edit domain" : "Add domain";
            btnAddEdit.Text = _isEdit ? "Edit" : "Add";

            LocalDomain = isLocal;

            // populate fields if host is not null
            if (host != null)
            {
                DomainName = host.Name;
                IPAddress = host.IP;
                Description = host.Description;
                HiddenDomain = host.Hidden;
            }

            Shown += (sender, e) => txtDomainName.Focus();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter &&
                !txtDescription.Focused)
            {
                btnAddEdit_click(this, new EventArgs());
                return true;
            }

            if (keyData == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void btnAddEdit_click(object sender, EventArgs e)
        {
            if (!Regex.IsMatch(txtDomainName.Text, GlobalRules.DomainRegex, RegexOptions.IgnoreCase))
            {
                MessageBox.Show(
                    "The domain name does not appear to be valid.",
                    _isEdit ? "Edit domain" : "Add domain",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            System.Net.IPAddress temp;
            if (!Regex.IsMatch(txtIPAddress.Text, GlobalRules.IPRegex, RegexOptions.IgnoreCase) ||
                !System.Net.IPAddress.TryParse(txtIPAddress.Text, out temp))
            {
                MessageBox.Show(
                    "The IP address field is not a valid IP.",
                    _isEdit ? "Edit domain" : "Add domain",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (!_isEdit &&
                HostsDatabase.Contains(LocalDomain, DomainName))
            {
                MessageBox.Show(
                    "The domain name you entered already exist in the " + (LocalDomain ? "local" : "remote") + " host list.\n" +
                    "Edit existing entry instead of attempting to overwrite.",
                    _isEdit ? "Edit domain" : "Add domain",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            DialogResult = DialogResult.OK;
        }
    }
}
