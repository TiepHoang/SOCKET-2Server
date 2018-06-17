using MyClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class frmClient : Form, ILog, IView
    {
        public frmClient()
        {
            InitializeComponent();
        }

        private void frmClient_Load(object sender, EventArgs e)
        {
            ip1.Text = "127.000.000.001";
            myIP.Text = "127.000.000.003";
            myClient = new ClientAndServer(this, this);
        }

        private ClientAndServer myClient;

        public IPAddress iPAddress_MyServer => IPAddress.Parse(ip1.Text);

        public IPAddress iPAddress_Listen => IPAddress.Parse(myIP.Text);

        private void button1_Click(object sender, EventArgs e)
        {
            if (myClient.IsConnect)
            {
                myClient.Stop();
                btnConnect.Text = "Kết nối";
            }
            else
            {
                myClient.Connect();
                myClient.Listening((client) =>
                {
                    var reader = new StreamReader(client.GetStream());
                    var data = reader.ReadLine();
                    Log(data);
                    _setResult(data);
                });
                btnConnect.Text = "Dừng";
            }
            grData.Enabled = myClient.IsConnect;
        }

        private void _setResult(string data)
        {
            if (txtResult.InvokeRequired)
            {
                txtResult.Invoke((MethodInvoker)(() =>
                {
                    _setResult(data);
                }));
            }
            else
            {
                txtResult.Text = data;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (myClient.IsConnect)
            {
                var iPEndPoint = myClient._tcpListener.LocalEndpoint as IPEndPoint;
                myClient.Send(string.Format("{0}_{1}|{2}", iPEndPoint.Address, iPEndPoint.Port, num.Value.ToString()));
                _setResult("waiting...");
            }
        }

        public void Log(object obj)
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke((MethodInvoker)(() =>
                {
                    Log(obj);
                }));
            }
            else
            {
                richTextBox1.AppendText(string.Format("\n[{0:dd/MM/yyyy-HH:mm:ss}]>> {1}", DateTime.Now, obj));
            }
        }
    }
}
