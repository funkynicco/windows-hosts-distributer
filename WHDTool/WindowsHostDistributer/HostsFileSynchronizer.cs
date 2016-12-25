using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsHostDistributer
{
    public class HostsFileSynchronizer : IDisposable
    {
        private readonly Thread _thread;
        private readonly AutoResetEvent _synchronizeRequestEvent = new AutoResetEvent(false);
        private readonly ManualResetEvent _terminateEvent = new ManualResetEvent(false);

        /// <summary>
        /// Gets the Windows 'hosts' filepath.
        /// </summary>
        public static string WindowsHostsFilepath
        {
            get
            {
                //return "hosts";
                return Path.Combine(
                    Environment.GetEnvironmentVariable("WINDIR"),
                    @"system32\drivers\etc\hosts");
            }
        }

        public HostsFileSynchronizer()
        {
            (_thread = new Thread(WorkerThread)).Start();
        }

        public void Dispose()
        {
            _terminateEvent.Set();
            _thread.Join();

            // dispose
            _terminateEvent.Dispose();
            _synchronizeRequestEvent.Dispose();
        }

        /// <summary>
        /// Posts a synchronization request to the worker thread.
        /// <para>This call does not block the executing thread.</para>
        /// </summary>
        public void PostSynchronize()
        {
            _synchronizeRequestEvent.Set();
        }

        private void WorkerThread()
        {
            var waits = new WaitHandle[]
                {
                    _synchronizeRequestEvent,
                    _terminateEvent
                };

            while (true)
            {
                switch (WaitHandle.WaitAny(waits))
                {
                    case 0: // synchronize
                        DoSynchronize();
                        break;
                    case 1: // terminate
                        return;
                }
            }
        }

        private void DoSynchronize()
        {
            var myDomains = new Dictionary<string, HostEntry>();
            HostsDatabase.Lock();
            foreach (var host in HostsDatabase.LocalHosts) // local comes first because we can overwrite a remote domain
            {
                myDomains.Add(
                    host.Name.ToLower(),
                    host);
            }
            foreach (var host in HostsDatabase.Hosts) // remote hosts
            {
                var name = host.Name.ToLower();
                if (!myDomains.ContainsKey(name))
                    myDomains.Add(name, host);
            }
            HostsDatabase.Unlock();

            // load the windows hosts file
            var windowsDomains = new Dictionary<string, HostEntry>();
            var lines = File.ReadAllLines(WindowsHostsFilepath);
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

            // compare ...
            var needRebuild = false;
            foreach (var key in myDomains.Keys) // check if all "myDomains" exist in the hosts file
            {
                if (!windowsDomains.ContainsKey(key) ||
                    windowsDomains[key].IP != myDomains[key].IP ||
                    windowsDomains[key].Description != myDomains[key].Description)
                {
                    needRebuild = true;
                    break;
                }
            }

            if (!needRebuild)
            {
                // check if windows domains file have any domains that shouldnt be there ...
                foreach (var key in windowsDomains.Keys)
                {
                    if (!myDomains.ContainsKey(key))
                    {
                        needRebuild = true;
                        break;
                    }
                }
            }

            if (needRebuild)
            {
                var sb = new StringBuilder(32768);
                sb.AppendLine("# Copyright (c) 1993-2009 Microsoft Corp.");
                sb.AppendLine("#");
                sb.AppendLine("# This is a sample HOSTS file used by Microsoft TCP/IP for Windows.");
                sb.AppendLine("#");
                sb.AppendLine("# This file contains the mappings of IP addresses to host names. Each");
                sb.AppendLine("# entry should be kept on an individual line. The IP address should");
                sb.AppendLine("# be placed in the first column followed by the corresponding host name.");
                sb.AppendLine("# The IP address and the host name should be separated by at least one");
                sb.AppendLine("# space.");
                sb.AppendLine("#");
                sb.AppendLine("# Additionally, comments (such as these) may be inserted on individual");
                sb.AppendLine("# lines or following the machine name denoted by a '#' symbol.");
                sb.AppendLine("#");
                sb.AppendLine("# For example:");
                sb.AppendLine("#");
                sb.AppendLine("#      102.54.94.97     rhino.acme.com          # source server");
                sb.AppendLine("#       38.25.63.10     x.acme.com              # x client host");
                sb.AppendLine();
                sb.AppendLine("# localhost name resolution is handled within DNS itself.");
                sb.AppendLine("#	127.0.0.1       localhost");
                sb.AppendLine("#	::1             localhost");
                sb.AppendLine();

                int n = 0;
                foreach (var kv in myDomains)
                {
                    if (n++ > 0)
                        sb.AppendLine();

                    if (!string.IsNullOrEmpty(kv.Value.Description))
                        sb.AppendFormat("{0}    {1} # {2}", kv.Value.IP.PadRight(15, ' '), kv.Value.Name, kv.Value.Description);
                    else
                        sb.AppendFormat("{0}    {1}", kv.Value.IP.PadRight(15, ' '), kv.Value.Name);
                }

                for (int i = 0; i < 10; ++i)
                {
                    try
                    {
                        File.WriteAllText(WindowsHostsFilepath, sb.ToString());
                        break;
                    }
                    catch (IOException ex)
                    {
                        if (i + 1 >= 10)
                        {
                            MessageBox.Show(
                                "Fatal error: The hosts file appears to be locked after 10 attempts to write to it.\nMessage: " + ex.Message,
                                "Host Synchronize",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                        else
                            Thread.Sleep(500);
                    }
                }
            }
        }
    }
}
