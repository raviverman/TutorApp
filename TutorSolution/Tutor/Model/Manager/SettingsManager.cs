using System;

namespace Tutor.Model.Manager
{
    public class SettingsManager 
    {
        public static String ServerIp {
            get
            {
                return (String)Properties.Settings.Default["serverip"];
            }
            set
            {
                Properties.Settings.Default["serverip"] = value;
            }
        }
        public static int ServerPort
        {
            get
            {
                return (int)Properties.Settings.Default["serverport"];
            }
            set
            {
                Properties.Settings.Default["serverport"] = value;
            }
        }
        public static String Username
        {
            get
            {
                return (String)Properties.Settings.Default["username"];
            }
            set
            {
                Properties.Settings.Default["username"] = value;
            }
        }
        public static String Password
        {
            get
            {
                return (String)Properties.Settings.Default["password"];
            }
            set
            {
                Properties.Settings.Default["password"] = value;
            }
        }
        public static String AuthKey
        {
            get
            {
                return (String)Properties.Settings.Default["authkey"];
            }
            set
            {
                Properties.Settings.Default["authkey"] = value;
            }
        }
        public static String FullName
        {
            get { return (String)Properties.Settings.Default["fullname"]; }
            set { Properties.Settings.Default["fullname"] = value; }
        }
        public static String ProfilePath
        {
            get { return (String)Properties.Settings.Default["profilepath"]; }
            set { Properties.Settings.Default["profilepath"] = value;}
        }
        public static String AccountType
        {
            get { return (String)Properties.Settings.Default["accounttype"]; }
            set { Properties.Settings.Default["accounttype"] = value; }
        }
        public static bool IsLoggedIn
        {
            get { return (bool)Properties.Settings.Default["isloggedin"]; }
            set {
                Properties.Settings.Default["isloggedin"] = value;
            }
        }
        public static bool KeepLoggedIn
        {
            get { return (bool)Properties.Settings.Default["keeploggedin"]; }
            set
            {
                Properties.Settings.Default["keeploggedin"] = value;
            }
        }

        public static void SaveSettings()
        {
            string l = Username + Password + FullName + AuthKey + AccountType;
            Properties.Settings.Default.Save();
        }

        public static void SaveServerSettings()
        {
            //Properties.Settings.
        }

        public static void UpdateImage()
        {
            string temp = ProfilePath;
            ProfilePath = "/Assets/blank-profile-picture.png";
            ProfilePath = temp;
        }
        public static void LogoutUpdate()
        {
            Username = "Username";
            FullName = "User-name";
            Password = "";
            AccountType = "";
            IsLoggedIn = false;
            ProfilePath = "/Assets/blank-profile-picture.png";
        }
    }
}
