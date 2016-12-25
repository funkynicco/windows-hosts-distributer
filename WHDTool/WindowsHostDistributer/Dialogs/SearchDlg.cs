using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsHostDistributer.Dialogs
{
    public partial class SearchDlg : Form
    {
        public string Domain
        {
            get { return txtDomain.Text; }
            set { txtDomain.Text = value; }
        }

        public string IPAddress
        {
            get { return txtIP.Text; }
            set { txtIP.Text = value; }
        }

        public string Description
        {
            get { return txtDescription.Text; }
            set { txtDescription.Text = value; }
        }

        public HostsSearchParameters SearchParameters
        {
            get { return new HostsSearchParameters(txtDomain.Text, txtIP.Text, txtDescription.Text); }
            set
            {
                txtDomain.Text = value.Domain;
                txtIP.Text = value.IPAddress;
                txtDescription.Text = value.Description;
            }
        }

        public SearchDlg()
        {
            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                button1_Click(this, new EventArgs());
                return true;
            }
            else if (keyData == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }

    public class HostsSearchParameters
    {
        public string Domain { get; set; }
        public string IPAddress { get; set; }
        public string Description { get; set; }

        public HostsSearchParameters(string domain, string ip, string description)
        {
            Domain = domain;
            IPAddress = ip;
            Description = description;
        }

        public void Reset()
        {
            Domain = string.Empty;
            IPAddress = string.Empty;
            Description = string.Empty;
        }

        public bool Matches(HostEntry host)
        {
            return
                (Domain.Length == 0 || host.Name.ToLower().Contains(Domain.ToLower())) &&
                (IPAddress.Length == 0 || host.IP.ToLower().Contains(IPAddress.ToLower())) &&
                (Description.Length == 0 || host.Description.ToLower().Contains(Description.ToLower()));
        }
    }
}
