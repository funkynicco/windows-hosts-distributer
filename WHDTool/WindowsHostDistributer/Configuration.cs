using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsHostDistributer
{
    public static class Configuration
    {
        private const string RegistryKey = @"Software\nProg\WindowsHostDistributer";
        private const string RunRegistryKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string ApplicationName = "WindowsHostDistributer";
        private const string ServerApplicationName = "WindowsHostDistributerServer";

        #region Registry Functions
        private static Dictionary<string, object> _cache = new Dictionary<string, object>();

        private static int ReadInteger(string name, int defaultValue = 0)
        {
            object cachedValue;
            if (_cache.TryGetValue(name, out cachedValue) &&
                cachedValue is int)
                return (int)cachedValue;

            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(RegistryKey))
                {
                    var val = key.GetValue(name);
                    if (val != null &&
                        val is int)
                        return (int)val;
                }
            }
            catch { }

            return defaultValue;
        }

        private static void WriteInteger(string name, int value)
        {
            _cache[name] = value;
            using (var key = Registry.CurrentUser.CreateSubKey(RegistryKey))
                key.SetValue(name, value, RegistryValueKind.DWord);
        }

        private static string ReadString(string name, string defaultValue = null)
        {
            object cachedValue;
            if (_cache.TryGetValue(name, out cachedValue) &&
                cachedValue is string)
                return (string)cachedValue;

            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(RegistryKey))
                {
                    var val = key.GetValue(name);
                    if (val != null &&
                        val is string)
                        return (string)val;
                }
            }
            catch { }

            return defaultValue;
        }

        private static void WriteString(string name, string value)
        {
            _cache[name] = value;
            using (var key = Registry.CurrentUser.CreateSubKey(RegistryKey))
                key.SetValue(name, value, RegistryValueKind.String);
        }
        #endregion

        /// <summary>
        /// Gets or sets a value indicating wether the application will start with windows.
        /// <para>The application path with be appended with a tray flag: -tray</para>
        /// </summary>
        public static bool StartWithWindows
        {
            get
            {
                using (var registry = Registry.CurrentUser.OpenSubKey(RunRegistryKey))
                {
                    var value = registry.GetValue(ApplicationName);
                    if (value != null &&
                        value is string)
                        return true;
                }

                return false;
            }
            set
            {
                using (var registry = Registry.CurrentUser.OpenSubKey(RunRegistryKey, true))
                {
                    if (value)
                    {
                        string path = string.Format("\"{0}\" -autostart", Application.ExecutablePath.Replace('/', '\\'));
                        registry.SetValue(ApplicationName, path, RegistryValueKind.String);
                    }
                    else
                        registry.DeleteValue(ApplicationName, false);
                }
            }
        }

        public static bool ServerStartWithWindows
        {
            get
            {
                using (var registry = Registry.CurrentUser.OpenSubKey(RunRegistryKey))
                {
                    var value = registry.GetValue(ServerApplicationName);
                    if (value != null &&
                        value is string)
                        return true;
                }

                return false;
            }
            set
            {
                using (var registry = Registry.CurrentUser.OpenSubKey(RunRegistryKey, true))
                {
                    if (value)
                    {
                        string path = string.Format("\"{0}\" -server -autostart", Application.ExecutablePath.Replace('/', '\\'));
                        registry.SetValue(ServerApplicationName, path, RegistryValueKind.String);
                    }
                    else
                        registry.DeleteValue(ServerApplicationName, false);
                }
            }
        }

        public static bool IsFirstRun
        {
            get { return ReadInteger("IsFirstRun", 1) != 0; }
            set { WriteInteger("IsFirstRun", value ? 1 : 0); }
        }

        public static string Address
        {
            get { return ReadString("Address", "127.0.0.1"); }
            set { WriteString("Address", value); }
        }

        public static int Port
        {
            get { return ReadInteger("Port", 9995); }
            set { WriteInteger("Port", value); }
        }

        public static string Key
        {
            get { return ReadString("Key", string.Empty); }
            set { WriteString("Key", value); }
        }

        public static string FontFamily
        {
            get { return ReadString("FontFamily", "Consolas"); }
            set { WriteString("FontFamily", value); }
        }

        public static int FontSize
        {
            get { return ReadInteger("FontSize", 9); }
            set { WriteInteger("FontSize", value); }
        }

        public static bool ShowHiddenHosts
        {
            get { return ReadInteger("ShowHiddenHosts", 0) != 0; }
            set { WriteInteger("ShowHiddenHosts", value ? 1 : 0); }
        }

        public static bool EnableAutomaticUpdates
        {
            get { return ReadInteger("EnableAutomaticUpdates", 1) != 0; }
            set { WriteInteger("EnableAutomaticUpdates", value ? 1 : 0); }
        }

        public static Point WindowLocation
        {
            get
            {
                return new Point(
                    ReadInteger("X", 50),
                    ReadInteger("Y", 50));
            }
            set
            {
                WriteInteger("X", value.X);
                WriteInteger("Y", value.Y);
            }
        }

        public static Size WindowSize
        {
            get
            {
                return new Size(
                    ReadInteger("Width", 1280),
                    ReadInteger("Height", 720));
            }
            set
            {
                WriteInteger("Width", value.Width);
                WriteInteger("Height", value.Height);
            }
        }

        public static FormWindowState WindowState
        {
            get { return (FormWindowState)ReadInteger("WindowState", (int)FormWindowState.Normal); }
            set { WriteInteger("WindowState", (int)value); }
        }
    }
}
