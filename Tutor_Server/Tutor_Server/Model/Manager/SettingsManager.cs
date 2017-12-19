using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutor_Server.Model.Manager
{
    class SettingsManager
    {

        public static string DatabaseUsername
        {
            get { return Properties.Settings.Default.DatabaseUsername; }
            set { Properties.Settings.Default.DatabaseUsername = value; }
        }
        public static string DatabaseName
        {
            get { return Properties.Settings.Default.DatabaseName; }
            set { Properties.Settings.Default.DatabaseName = value; }
        }
        public static string DatabasePassword
        {
            get { return Properties.Settings.Default.DatabasePassword; }
            set { Properties.Settings.Default.DatabasePassword = value; }
        }
        public static string ServerRoot
        {
            get { return Properties.Settings.Default.ServerRoot; }
            set { Properties.Settings.Default.ServerRoot = value; }
        }
        public static int DatabasePort
        {
            get { return Properties.Settings.Default.DatabasePort; }
            set { Properties.Settings.Default.DatabasePort = value; }
        }
        public static int ServerPort
        {
            get { return Properties.Settings.Default.ServerPort; }
            set { Properties.Settings.Default.ServerPort = value; }
        }
        public static string HttpStartPath
        {
            get { return Properties.Settings.Default.HttpServerStartPath; }
            set { Properties.Settings.Default.HttpServerStartPath = value; }
        }
        public static string HttpStopPath
        {
            get { return Properties.Settings.Default.HttpServerStopPath; }
            set { Properties.Settings.Default.HttpServerStopPath = value; }
        }
        public static string SqlStartPath
        {
            get { return Properties.Settings.Default.SQLServerStartPath; }
            set { Properties.Settings.Default.SQLServerStartPath = value; }
        }
        public static string SqlStopPath
        {
            get { return Properties.Settings.Default.SQLServerStopPath; }
            set { Properties.Settings.Default.SQLServerStopPath = value; }
        }
        public static bool AnyIpUsed
        {
            get { return Properties.Settings.Default.AnyIpUsed; }
            set { Properties.Settings.Default.AnyIpUsed = value; }
        }
        public static string SpecificServerIP
        {
            get { return Properties.Settings.Default.SpecificServerIp; }
            set { Properties.Settings.Default.SpecificServerIp = value; }
        }

        public static void SaveSettings()
        {
            Properties.Settings.Default.Save();
        }
    }
}
