using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsHostDistributer.Distributer;

namespace WindowsHostDistributer.Dialogs
{
    public partial class ServerForm : Form
    {
        private readonly DistributerServer _distributeServer = new DistributerServer();

        public ServerForm()
        {
            InitializeComponent();
            Font = new Font(Configuration.FontFamily, Configuration.FontSize);

            IPAddress ip;
            string bindAddress;
            if (ServerConfiguration.GetValue("Bind", out bindAddress))
                ip = IPAddress.Parse(bindAddress);
            else
                ip = IPAddress.Any;

            int port;
            if (!ServerConfiguration.GetValue("Port", out port))
            {
                MessageBox.Show("The port value does not exist in server.xml", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            if (!_distributeServer.Start(new IPEndPoint(ip, port)))
            {
                MessageBox.Show(
                    "Failed to listen on " + ip.ToString() + ":" + port,
                    "Server",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            new Timer() { Interval = 50, Enabled = true }.Tick += (sender, e) =>
                {
                    _distributeServer.Process();
                    Logger.ProcessMessage((msg) =>
                        {
                            AddLog(string.Format(
                                "[{0}][{1}] {2}",
                                msg.UtcDate.ToLocalTime().ToString("HH:mm:ss"),
                                Enum.GetName(typeof(LogType), msg.Type),
                                msg.Text));
                        });
                };
        }

        private void AddLog(string text)
        {
            if (txtLog.Text.Length > 0)
                txtLog.AppendText("\r\n");

            txtLog.AppendText(text);
            txtLog.Select(txtLog.Text.Length, 0);
        }
    }
}
