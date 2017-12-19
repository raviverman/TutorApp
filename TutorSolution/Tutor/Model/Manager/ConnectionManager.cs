using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tutor.Model.Data;

namespace Tutor.Model.Manager
{
    public class ConnectionManager
    {
        private static TcpClient client = new TcpClient();
        private static TcpClient _backupClient = new TcpClient();
        private static string serverAddress = SettingsManager.ServerIp;
        private static int portAddress = SettingsManager.ServerPort;
        private static Stopwatch connectionTimeout = new Stopwatch();

        public static void UpdateConnectionSettings()
        {
           serverAddress = SettingsManager.ServerIp;
           portAddress = SettingsManager.ServerPort;
        }

        public static bool IsConnected
        {
            get
            {
                return client.Connected;
            }
        }

        public async static Task<bool> Connect()
        {
            try
            {
                client = new TcpClient();
                await client.ConnectAsync(serverAddress, portAddress);
                connectionTimeout.Start();
            }
            catch (Exception)
            {

            }
            AppNotificationManager.PushMessage(new AppNotification() { Message = String.Format("Server Connected : {0} ",client.Connected)});
            return client.Connected;
        }

        public static void Disconnect()
        {
            if (client.Connected)
            {
                client.Close();
            }
        }

        public static async Task<Object> SendRequestAsync(Object subrequest, RequestType type, ResponseType resptype)
        {
            if (!client.Connected)
            {
                return null;
            }
            if(connectionTimeout.Elapsed.TotalSeconds > 300)
            {
                Disconnect();
                await Connect();
            }
            Request request = new Request() { Username = SettingsManager.Username, Content = subrequest, SessionId = SettingsManager.AuthKey, Type = type };
            string message = JsonConvert.SerializeObject(request);
            try
            {
                NetworkStream serverStream = client.GetStream();

                byte[] buffer = Encoding.UTF8.GetBytes(message);
                serverStream.Write(buffer, 0, buffer.Length);
            }
            catch (Exception)
            {
                AppNotificationManager.PushMessage(new AppNotification() { Message = "Connection Error. Reconnect." });
            }
            
            return await GetResponse(resptype);
        }

        private static async Task<Object> GetResponse(ResponseType resptype, bool useDefault = true)
        {
            TcpClient server = client;
            if (!useDefault)
                server = _backupClient;
            string message = "";
            
            try
            {
                var serverStream = server.GetStream();
                serverStream.ReadTimeout = 10000;
                byte[] receiveBuffer = new byte[65536];
                await serverStream.ReadAsync(receiveBuffer, 0, 65535);
                message = Encoding.UTF8.GetString(receiveBuffer).Trim('\0');
                Response resp = JsonConvert.DeserializeObject<Response>(message);
                if (resp.Type == resptype)
                {
                    connectionTimeout.Restart();
                    return resp.Content;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                if(useDefault) //sends message in case default connection is not responding <- exception to upload connection.
                    AppNotificationManager.PushMessage(new AppNotification() { Message = "No Response From Server." });
            }
            return null;
        }

        public async static Task<Object> SendFileAsync(string filePath, TcpClient server)
        {
            bool useDefaultClient = false;
            if (server == null)
            {
                server = client;
                useDefaultClient = true;
            }

            NetworkStream ns = server.GetStream();
            if(File.Exists(filePath))
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    fs.CopyTo(ns);
                }

                return await GetResponse(ResponseType.Acknowledge, useDefaultClient);

            }

            return null;
        }

        public async static Task<Object> SendVideoFileAsync(string videoPath, string thumbnailPath, string port)
        {
            TcpClient secondaryClient = new TcpClient();
            secondaryClient.Connect(serverAddress, int.Parse(port));
            _backupClient = secondaryClient;
            await SendFileAsync(thumbnailPath, secondaryClient);
            return await SendFileAsync(videoPath, secondaryClient);
        }
    }
}
