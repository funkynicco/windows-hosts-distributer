using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsHostDistributer
{
    public static class UpdateManager
    {
        public async static Task<UpdateCheck> CheckUpdate()
        {
            using (var wc = new WebClient())
            {
                try
                {
                    var ret = await wc.DownloadStringTaskAsync(Program.UpdateAddress); // <version>|<date>
                    
                    var version = int.Parse(ret.Substring(0, ret.IndexOf("|")));
                    //var date = DateTime.SpecifyKind(DateTime.Parse(ret.Substring(ret.IndexOf("|") + 1)), DateTimeKind.Utc);
                    var date = new DateTime(long.Parse(ret.Substring(ret.IndexOf('|') + 1)), DateTimeKind.Utc);
                    var result = version > Program.Version ? UpdateCheckResult.UpdateAvailable : UpdateCheckResult.NoUpdateNeeded;
                    return new UpdateCheck(result, version, date);
                }
                catch
                {
                    return new UpdateCheck(UpdateCheckResult.CheckFailed);
                }
            }
        }

        public static void BeginUpdate(DateTime modificationDateUtc, bool silent)
        {
            // extract updater from this exe's resources
            string updaterPath = Path.Combine(Environment.CurrentDirectory, "Updater.exe");
            File.WriteAllBytes(updaterPath, Media.Updater);

            // grab current process id so Updater.exe can wait for it to close
            int processId;
            using (var current = Process.GetCurrentProcess())
                processId = current.Id;

            List<string> args = new List<string>();
            args.Add(processId.ToString()); // Process id
            args.Add("\"" + Program.DownloadUpdateAddress.Replace("\"", "\"\"") + "\""); // Download address
            args.Add("\"" + Application.ExecutablePath.Replace("\"", "\"\"") + "\""); // Replace path
            args.Add("\"" + modificationDateUtc.Ticks + "\""); // modificationDateUtc
            args.Add("\"" + (silent ? "true" : "false") + "\""); // silent

            using (var process = Process.Start(new ProcessStartInfo(updaterPath, string.Join(" ", args))
            {
                WorkingDirectory = Environment.CurrentDirectory
            }))
            { }
        }
    }

    public class UpdateCheck
    {
        /// <summary>
        /// Gets the result of the update check.
        /// </summary>
        public UpdateCheckResult Result { get; private set; }
        /// <summary>
        /// Gets the version number of the latest version.
        /// </summary>
        public int Version { get; private set; }
        /// <summary>
        /// Gets the date of the latest version.
        /// </summary>
        public DateTime Date { get; private set; }

        public UpdateCheck(UpdateCheckResult result, int version, DateTime date)
        {
            Result = result;
            Version = version;
            Date = date;
        }

        public UpdateCheck(UpdateCheckResult result)
        {
            Result = result;
            Version = Program.Version;
            Date = DateTime.UtcNow;
        }
    }

    public enum UpdateCheckResult
    {
        NoUpdateNeeded,
        UpdateAvailable,
        CheckFailed
    }
}
