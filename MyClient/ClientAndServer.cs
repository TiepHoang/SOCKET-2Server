using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MyClient
{
    public class ClientAndServer
    {
        public int PORT { get; set; } = 9999;
        public bool IsConnect { get; private set; }

        public TcpClient _tcpClient { get; private set; }
        public TcpListener _tcpListener { get; private set; }
        private ILog log;
        private IView view;
        private StreamWriter _writer;

        public ClientAndServer(ILog log, IView view)
        {
            this.log = log;
            this.view = view;
        }

        public void Listening(Action<TcpClient> AcceptTcpClient)
        {
            log.Log("INIT LISTEN...");

            _tcpListener = new TcpListener(view.iPAddress_Listen, PORT);
            _tcpListener.Start();
            log.Log(string.Format("{0} LISTENING...", _tcpListener.LocalEndpoint));

            Task.Run(() =>
            {
                while (IsConnect)
                {
                    try
                    {
                        var socket = _tcpListener.AcceptTcpClient();
                        log.Log(string.Format("{0} LISTENING... {1}", _tcpListener.LocalEndpoint, socket.Client.RemoteEndPoint));
                        AcceptTcpClient(socket);
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }
            });

            IsConnect = true;
        }

        public void Connect()
        {
            log.Log("INIT CONNECTING...");
            _tcpClient = new TcpClient();
            _tcpClient.Connect(view.iPAddress_MyServer, PORT);
            log.Log(string.Format("{0} CONNECTED TO {1}.", _tcpClient.Client.LocalEndPoint, _tcpClient.Client.RemoteEndPoint));
            IsConnect = true;
            _writer = new StreamWriter(_tcpClient.GetStream());
            _writer.AutoFlush = true;
        }

        public void Stop()
        {
            if (IsConnect)
            {
                IsConnect = false;

                Send("CLOSE");
                _tcpClient?.Close();
                _tcpListener?.Stop();
            }
        }

        public void Send(string data)
        {
            if (IsConnect && _tcpClient != null)
            {
                _writer.WriteLine(data);
                log.Log(string.Format("SENDED {0}: {1}", _tcpClient.Client.RemoteEndPoint, data));
            }
        }

        public void SendTo(IPEndPoint remoteEP, string data)
        {
            var client = new TcpClient();
            client.Connect(remoteEP);
            Task.Delay(1000);
            var writer = new StreamWriter(client.GetStream());
            writer.AutoFlush = true;
            if (client.Connected)
            {
                writer.WriteLine(data);
                log.Log(string.Format("SENDED TO {0}: {1}", remoteEP, data));
            }
            client.Close();
        }

    }

    public interface ILog
    {
        void Log(object obj);
    }

    public interface IView
    {
        IPAddress iPAddress_MyServer { get; }
        IPAddress iPAddress_Listen { get; }
    }
}
