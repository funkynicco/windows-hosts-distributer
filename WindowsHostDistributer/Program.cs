using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsHostDistributer.Dialogs;

namespace WindowsHostDistributer
{
    static class Program
    {
        // The unique Guid for this application to prevent running more than one instance at any given time.
        public const string ClientInstanceGuid = "{BBDEEC2E-75AC-4453-BE9B-23138083F684}";
        public const string ServerInstanceGuid = "{26B10B80-5BFE-4937-88D4-3445CF874C67}";

        // Any protocol changes should increment this version value in order to disallow uploads with mismatching version number.
        public const int Version = 1;

        //public const string DomainAddress = "localhost:2004";
        public const string DomainAddress = "whd.nprog.com";

        public static readonly string UpdateAddress = "https://" + DomainAddress + "/version.txt";
        public static readonly string DownloadUpdateAddress = "https://" + DomainAddress + "/WindowsHostDistributer.exe";

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var serverMode = false;

            var args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; ++i)
            {
                if (string.Compare(args[i], "-server", true) == 0)
                {
                    serverMode = true;
                }
                else if (string.Compare(args[i], "-dumpver", true) == 0)
                {
                    // write version.txt
                    File.WriteAllText("version.txt", string.Format(
                        "{0}|{1}",
                        Version,
                        File.GetLastWriteTimeUtc(Application.ExecutablePath).Ticks));
                    return;
                }
                else if (string.Compare(args[i], "-autostart", true) == 0)
                {
                    Environment.CurrentDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                }
            }

            using (var instance = new SingleInstanceChecker())
            {
                var registerInstanceResult = instance.RegisterInstance(serverMode ? ServerInstanceGuid : ClientInstanceGuid);
                if (registerInstanceResult == RegisterInstanceResult.InstanceAlreadyExist)
                {
                    MessageBox.Show(
                        "Another instance is already running on this computer.",
                        "WHD",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (registerInstanceResult != RegisterInstanceResult.Succeeded)
                {
                    MessageBox.Show(string.Format("An error occurred attempting to register single-instance.\nCode: {0}",
                        Enum.GetName(typeof(RegisterInstanceResult), registerInstanceResult)),
                        "WHD",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (!HostsDatabase.Load())
                    HostsDatabase.Save(); // create file...

                if (serverMode)
                {
                    if (!ServerConfiguration.Load("server.xml"))
                    {
                        MessageBox.Show("Failed to load server.xml", "WHD", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    Application.Run(new ServerForm());
                }
                else
                {
                    try
                    {
                        File.Open(HostsFileSynchronizer.WindowsHostsFilepath, FileMode.Open, FileAccess.ReadWrite)
                            .Dispose(); // close it right away
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            "Failed to open hosts file with read/write access.\n" + ex.Message +
                            "\n\nYou must run this program as administrator or change the permissions of the hosts file.",
                            "WHD",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }

                    if (Configuration.IsFirstRun)
                    {
                        using (var dlg = new ClientConfigurationDlg())
                        {
                            if (dlg.ShowDialog() != DialogResult.OK)
                                return;

                            Configuration.Address = dlg.Address;
                            Configuration.Port = dlg.Port;
                            Configuration.Key = dlg.Key;

                            Configuration.IsFirstRun = false;
                        }
                    }

                    using (var hfs = new HostsFileSynchronizer())
                    using (var form = new ClientForm(hfs))
                    {
                        Application.Run(form);
                    }
                }
            }
        }
    }
}
