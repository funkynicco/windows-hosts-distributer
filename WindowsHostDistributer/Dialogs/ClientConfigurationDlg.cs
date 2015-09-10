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
    public partial class ClientConfigurationDlg : Form
    {
        public string Address
        {
            get { return txtAddress.Text; }
            set { txtAddress.Text = value; }
        }

        public int Port
        {
            get { return int.Parse(txtPort.Text); }
            set { txtPort.Text = value.ToString(); }
        }

        public string Key
        {
            get { return txtKey.Text; }
            set { txtKey.Text = value; }
        }

        public ClientConfigurationDlg()
        {
            InitializeComponent();

            Font = new Font(Configuration.FontFamily, Configuration.FontSize);

            Address = Configuration.Address;
            Port = Configuration.Port;
            Key = Configuration.Key;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtAddress.Text.Length == 0)
            {
                MessageBox.Show("Address must be entered");
                return;
            }

            int port;
            if (txtPort.Text.Length == 0 ||
                !int.TryParse(txtPort.Text, out port) ||
                port <= 0 ||
                port >= ushort.MaxValue)
            {
                MessageBox.Show("Port value must be between 1 and " + ushort.MaxValue);
                return;
            }

            if (Key.Length == 0)
            {
                MessageBox.Show("Key must be entered");
                return;
            }

            DialogResult = DialogResult.OK;
        }
    }
}
