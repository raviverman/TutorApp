using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tutor_Server.Pages;

namespace Tutor_Server.Model.Manager
{
    public class MainServer
    {
        private static TcpListener server;

        private static void Initialize()
        {
            IPAddress address = IPAddress.Any;
            if (IPAddress.TryParse(SettingsManager.SpecificServerIP, out address))
                server = new TcpListener(SettingsManager.AnyIpUsed ? IPAddress.Any : address, SettingsManager.ServerPort > 0 ? SettingsManager.ServerPort : 4020);
            else
                server = new TcpListener(IPAddress.Any, 4020);
        }

        public static bool IsRunning { get; private set; }
        private static void _startServer()
        {
            string serb = server.LocalEndpoint.ToString();
            IsRunning = true;
            try
            {
                server.Start();
                StatusManager.PushMessage("Tutor Server Started.", StatusType.Success);
            }
            catch (Exception e)
            {
                StatusManager.PushMessage(e.Message, StatusType.Error);
            }
            int _crashCounter = 5;
            while(IsRunning)
            {
                try
                {
                    TcpClient clientHandle = server.AcceptTcpClient();
                    Client client = new Client(clientHandle);
                    client.Handle();
                    //Handle Clients.
                }
                catch (Exception e)
                {
                    _crashCounter--;
                    if (_crashCounter == 0)
                    {
                        //Notify that server stopped.
                        Stop();
                    }
                }
            }
        }
        /// <summary>
        /// Start main Tutor Server.
        /// </summary>
        public static bool Start()
        {
            Initialize();
            Thread thread = new Thread(new ThreadStart(_startServer));
            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            return true;
        }

        /// <summary>
        /// Stop the main Tutor Server.
        /// </summary>
        public static void Stop()
        {
            IsRunning = false;
            server.Stop();
            Home_Page.ConnectedClient = 0;
            StatusManager.PushMessage("Tutor Server Stopped.", StatusType.Error);
        }

    }
}
