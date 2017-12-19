using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tutor_Server.Model.Data;
using Tutor_Server.Pages;

namespace Tutor_Server.Model.Manager
{
    class Client
    {
        private TcpClient _client = new TcpClient();
        private TcpListener _backupClient = new TcpListener(IPAddress.Any, 0);
        private string serverroot = SettingsManager.ServerRoot;

        private bool ReceiveFile(string fileName, string extWithDot, TcpClient localClient)
        {
            string filepath = serverroot + "/Temp/" + fileName + extWithDot;
            int offset = 0;
            NetworkStream ns = localClient.GetStream();
            int timeout = 100;
            while(!ns.DataAvailable && timeout > 0)
            {
                Thread.Sleep(100);
                timeout--;
            }
            ns.ReadTimeout = 2000;
            using (FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate))
            {
                try
                {
                    ns.CopyTo(fs);
                }
                catch
                {

                }
                //byte[] buffer = new byte[65536];
                //while (ns.DataAvailable)
                //{
                //    int readBytes = ns.Read(buffer, offset, 65535);
                //    fs.Write(buffer, offset, readBytes);
                //    offset += readBytes;
                //}
            }
            //ns.Close();
            FileInfo fileinfo = new FileInfo(filepath);
            if (fileinfo.Length > 0)
                return true;
            else
                return false;
        }

        private void ReceiveImage(string fileName, string extWithDot,bool IsProfileImage = true)
        {
            string folder = "/User/";
            if (!IsProfileImage)
                folder = "/Thumbnails/";

            Acknowledge ack = new Acknowledge() { Status = "FAIL", Reason = "Photo failed to update." };
            Response resp = new Response() { Content = ack, Status = "OK", Type = ResponseType.Acknowledge };
            if(ReceiveFile(fileName, extWithDot,_client))
            {
                if (File.Exists(serverroot + folder + fileName + extWithDot))
                    File.Delete(serverroot + folder + fileName + extWithDot);
                File.Move(serverroot+"/Temp/" + fileName + extWithDot, serverroot + folder + fileName + extWithDot);
                ack.Status = "OK";
                ack.Reason = String.Format("{0} Updated Successfully.", IsProfileImage ? "Profile" : "Video");
            }
            NetworkStream ns = _client.GetStream();
            var response = JsonConvert.SerializeObject(resp);

            byte[] buffer = Encoding.UTF8.GetBytes(response);
            ns.Write(buffer, 0, response.Length);

        }

        public Client(TcpClient client)
        {
            _client = client;

            IPAddress address = IPAddress.Any;
            if(IPAddress.TryParse(SettingsManager.SpecificServerIP,out address))
            _backupClient = new TcpListener(SettingsManager.AnyIpUsed ? IPAddress.Any : address, 0);
        }

        private void _handleClient()
        {
            NetworkStream clientStream = _client.GetStream();
            int ConnectionTimeout = 1500; //30 Seconds
            //send acnowledgement
            Home_Page.ConnectedClient++;
            StatusManager.PushMessage("Client " + _client.Client.RemoteEndPoint.ToString() + " Connected. ", StatusType.Success);
            while (_client.Connected && MainServer.IsRunning && ConnectionTimeout > 0)
            {
                
                try
                {
                    while(!clientStream.DataAvailable && _client.Connected && ConnectionTimeout >0)
                    {
                        Thread.Sleep(200);
                        ConnectionTimeout--;
                        if (!MainServer.IsRunning)
                            return;
                    }
                    byte[] buffer = new byte[65536];

                    if (clientStream.DataAvailable)
                    {
                        int readBytes = clientStream.Read(buffer, 0, 65535);
                        string message = Encoding.UTF8.GetString(buffer).Trim('\0');
                        //Process Message 
                        Request request = JsonConvert.DeserializeObject<Request>(message);
                        Response result = RequestHandlers.GetResponse(request.Type, (JObject)request.Content);
                        if(result.Type == ResponseType.CreateVideo)
                        {
                            try
                            {
                                _backupClient.Start();
                                ((CreateVideoResponse)result.Content).Port =  _backupClient.LocalEndpoint.ToString().Split(':')[1];
                            }
                            catch (Exception e)
                            {
                                StatusManager.PushMessage(e.Message, StatusType.Error);
                            }
                            
                        }
                        var response = JsonConvert.SerializeObject(result);
                        //simulating heavy server load
                        //Thread.Sleep(2000);
                        buffer = Encoding.UTF8.GetBytes(response);
                        clientStream.Write(buffer, 0, response.Length);

                        if(result.Type == ResponseType.UpdateProfile)
                        {
                            if (((ProfileUpdateResponse)result.Content).RequiresImageUpdate)
                                ReceiveImage(request.Username, ".jpg");
                        }
                        else if(result.Type == ResponseType.CreateVideo)
                        {
                            Thread thread = new Thread(FileHandlingService)
                            {
                                IsBackground = true,
                                Name = "Backup server",
                            };
                            thread.Start(((CreateVideoResponse)result.Content).VideoID);
                        }
                        else if(result.Type == ResponseType.VideoUpdate)
                        {
                            VideoUpdateResponse resp = ((VideoUpdateResponse)result.Content);
                            if (resp.IsThumbnailUpdateRequired)
                                ReceiveImage(resp.ThumbnailImage, ".jpg", false);
                        }
                        ConnectionTimeout = 1500; //timeout restored.
                    }
                    
                }
                catch (Exception e)
                {
                    StatusManager.PushMessage(e.Message, StatusType.Error);
                    //throw;
                }
                
            }
            //might send a client disconnected message.
            Home_Page.ConnectedClient--;
            StatusManager.PushMessage("Client " + _client.Client.RemoteEndPoint.ToString() + " Disconnected. ", StatusType.Error);
            clientStream.Close();
        }

        public void Handle()
        {
            Thread thread = new Thread(new ThreadStart(_handleClient));
            thread.IsBackground = true;
            //thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public void FileHandlingService(object FileName)
        {
            //Acknowledge Packet to acknowledge successful reception.
            Acknowledge ack = new Acknowledge() { Status = "OK", Reason = "Video Uploaded Received Successfully" };
            Response rsp = new Response() { Type = ResponseType.Acknowledge, Content = ack, Status = "OK" };
            string message = JsonConvert.SerializeObject(rsp);
            byte[] buffer = Encoding.UTF8.GetBytes(message);


            TcpClient fileSender = _backupClient.AcceptTcpClient();
            ReceiveFile((string)FileName, ".jpg", fileSender);
            StatusManager.PushMessage(FileName + " Received", StatusType.Success);

            //Getting stream and writing acknowledgement.
            var stream = fileSender.GetStream();
            stream.Write(buffer, 0, buffer.Length);

            //Receiving Video File
            ReceiveFile((string)FileName, ".mp4", fileSender);
            StatusManager.PushMessage(FileName + " Received", StatusType.Success);
            stream.Write(buffer, 0, buffer.Length);

            //Move files from temporary storage to permanant 
            if (File.Exists($"{serverroot}/Temp/{FileName}.jpg"))
                File.Move($"{serverroot}/Temp/{FileName}.jpg", $"{serverroot}/Thumbnails/{FileName}.jpg");
            if (File.Exists($"{serverroot}/Temp/{FileName}.mp4"))
                File.Move($"{serverroot}/Temp/{FileName}.mp4", $"{serverroot}/Videos/{FileName}.mp4");
            _backupClient.Stop();
        }

    }
}
