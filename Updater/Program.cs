using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Updater
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            /*
            Command line indexes:
            0 - This updater path
            1 - Process Id
            2 - Url for new file
            3 - Path of WindowsHostDistributer.exe
            4 - Modification date
            5 - Silent operation
            */

            var args = Environment.GetCommandLineArgs();
            if (args.Length != 6)
            {
                MessageBox.Show(
                    "The Updater can only be launched by WindowsHostDistributer.",
                    "Updater",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            using (var wc = new WebClient())
            {
                var downloadTask = wc.DownloadDataTaskAsync(args[2]); // begin download

                var processId = int.Parse(args[1]);
                var modificationDateUtc = new DateTime(long.Parse(args[4]), DateTimeKind.Utc);
                var silent = string.Compare(args[5], "true", true) == 0;

                Process remoteProcess = null;
                try
                {
                    remoteProcess = Process.GetProcessById(processId);
                }
                catch { }

                if (remoteProcess != null)
                {
                    remoteProcess.WaitForExit();
                    remoteProcess.Dispose();
                }

                Thread.Sleep(1000);

                downloadTask.Wait();

                for (int i = 0; ; ++i)
                {
                    try
                    {
                        if (File.Exists(args[3]))
                            File.Delete(args[3]);

                        break; // delete succeeded
                    }
                    catch
                    {
                        if (i + 1 >= 5)
                            throw;

                        Thread.Sleep(1000);
                    }
                }

                File.WriteAllBytes(args[3], downloadTask.Result);
                File.SetLastWriteTimeUtc(args[3], modificationDateUtc);

                using (var process = Process.Start(new ProcessStartInfo(args[3], silent ? "-tray" : "")
                {
                    WorkingDirectory = Environment.CurrentDirectory
                }))
                {
                }
            }
        }
    }
}
