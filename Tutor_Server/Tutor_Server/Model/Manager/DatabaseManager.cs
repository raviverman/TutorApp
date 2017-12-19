using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutor_Server.Model.Manager
{
    public class DatabaseManager
    {
        private string connectionString;//= "datasource=localhost;port=3306;username=root;password=;database=tutor_server";
        public string ErrorReason { get; private set; } = "";

        public DatabaseManager()
        {
            connectionString = $"datasource=localhost;port={SettingsManager.DatabasePort};username={SettingsManager.DatabaseUsername};password={SettingsManager.DatabasePassword};database={SettingsManager.DatabaseName}";
        }
        public DatabaseManager(string ConnectionString)
        {
            connectionString = ConnectionString;
        }

        public (MySqlDataReader,MySqlConnection) RunQuery(string query)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand command = new MySqlCommand(query, connection)
            {
                CommandTimeout = 5000
            };

            try
            {
                connection.Open();
                var  reader = command.ExecuteReader();
                if (!reader.HasRows && reader.RecordsAffected == 0)
                    return (null,connection);
                return (reader,connection);
            }
            catch (Exception e)
            {
                StatusManager.PushMessage( e.Message + "QUERY : " + query,StatusType.Error); 
            }

            return (null,connection);
        }

        public bool SetUpDatabase()
        {
            //create comments table
            string successString = "";
            (MySqlDataReader reader, var Connection) = RunQuery(@"CREATE TABLE `comments` (
                                                                 `userid` varchar(128) NOT NULL,
                                                                 `videoid` varchar(128) NOT NULL,
                                                                 `content` varchar(256) NOT NULL,
                                                                 `likes` int(255) NOT NULL DEFAULT '0',
                                                                 `dislikes` int(255) NOT NULL DEFAULT '0',
                                                                 `date` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
                                                                 `uid` varchar(128) NOT NULL,
                                                                 PRIMARY KEY(`uid`)
                                                                ) ENGINE = InnoDB DEFAULT CHARSET = latin1");
            if (ErrorReason.Length < 5)
                successString += "Comments";
            ErrorReason = "";
            Connection.Close();
            (reader,Connection) = RunQuery(@"CREATE TABLE `account_data` (
                                             `username` varchar(50) NOT NULL,
                                             `fullname` varchar(64) NOT NULL,
                                             `acctype` varchar(32) NOT NULL,
                                             `type` int(10) NOT NULL,
                                             `profileimage` varchar(256) DEFAULT '/Defaults/default_pp.png',
                                             PRIMARY KEY (`username`)
                                            ) ENGINE=InnoDB DEFAULT CHARSET=latin1");
            if (ErrorReason.Length < 5)
                successString += ", Account_Data";
            ErrorReason = "";
            Connection.Close();
            (reader,Connection) = RunQuery(@"CREATE TABLE `course_details` (
                                             `courseid` varchar(128) NOT NULL,
                                             `coursename` varchar(64) NOT NULL,
                                             `authorid` varchar(128) NOT NULL,
                                             `rating` float NOT NULL DEFAULT '0',
                                             `createdon` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
                                             `videocount` int(11) NOT NULL DEFAULT '0',
                                             `ratingCount` int(11) NOT NULL DEFAULT '0',
                                             PRIMARY KEY (`courseid`)
                                            ) ENGINE=InnoDB DEFAULT CHARSET=latin1");
            if (ErrorReason.Length < 5)
                successString += ", Course_details";
            ErrorReason = "";

            Connection.Close();
            (reader, Connection) = RunQuery(@"CREATE TABLE `favorites` (
                                             `userid` varchar(128) NOT NULL,
                                             `videoid` varchar(128) NOT NULL
                                            ) ENGINE=InnoDB DEFAULT CHARSET=latin1");
            if (ErrorReason.Length < 5)
                successString += ", Favorites";
            Connection.Close();
            ErrorReason = "";
            (reader, Connection) = RunQuery(@"CREATE TABLE `tag_details` (
                                             `tag` varchar(50) NOT NULL,
                                             `videoid` varchar(128) NOT NULL
                                            ) ENGINE=InnoDB DEFAULT CHARSET=latin1");
            if (ErrorReason.Length < 5)
                successString += ", Tag_details";
            ErrorReason = "";
            Connection.Close();
            (reader, Connection) = RunQuery(@"CREATE TABLE `user_login` (
                                             `username` varchar(50) NOT NULL,
                                             `password` varchar(128) NOT NULL,
                                             `authkey` varchar(128) DEFAULT NULL,
                                             `lastloggedon` datetime DEFAULT NULL,
                                             PRIMARY KEY (`username`)
                                            ) ENGINE=InnoDB DEFAULT CHARSET=latin1");
            if (ErrorReason.Length < 5)
                successString += ", User_login";
            ErrorReason = "";
            Connection.Close();
            (reader, Connection) = RunQuery(@"CREATE TABLE `video_details` (
                                         `videoid` varchar(128) NOT NULL,
                                         `path` varchar(256) NOT NULL,
                                         `thumbnail` varchar(256) NOT NULL DEFAULT '/Defaults/default_thumbnail.png',
                                         `title` varchar(256) NOT NULL DEFAULT 'Title Unavailable',
                                         `description` varchar(512) DEFAULT 'No description available.',
                                         `width` int(255) NOT NULL DEFAULT '1280',
                                         `height` int(255) NOT NULL DEFAULT '720',
                                         `duration` time NOT NULL DEFAULT '00:00:00',
                                         `authorid` varchar(128) NOT NULL,
                                         `authorname` varchar(64) NOT NULL,
                                         `authorimage` varchar(256) NOT NULL DEFAULT '/Defaults/default_pp.png',
                                         `course` varchar(64) NOT NULL,
                                         `courseid` varchar(128) NOT NULL,
                                         `lastupdatedon` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
                                         `likes` int(255) NOT NULL DEFAULT '0',
                                         `dislikes` int(255) NOT NULL DEFAULT '0',
                                         PRIMARY KEY (`videoid`)
                                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1");
            if (ErrorReason.Length < 5)
                successString += ", Video_Details ";
            ErrorReason = "";
            Connection.Close();
            if (successString.Length > 5)
                StatusManager.PushMessage("TABLE " + successString + "created successfully.", StatusType.Success);
            else
                StatusManager.PushMessage("Error in table creation", StatusType.Error);
            return true;
        }
    }
}
