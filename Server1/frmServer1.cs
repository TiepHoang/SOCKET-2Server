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

namespace Server1
{
    public partial class frmServer1 : Form, ILog, IView
    {
        public frmServer1()
        {
            InitializeComponent();
        }

        private void frmServer1_Load(object sender, EventArgs e)
        {
            myIP.Text = "127.000.000.001";
            ip2.Text = "127.000.000.002";
            myClient = new ClientAndServer(this, this);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (myClient.IsConnect)
            {
                myClient.Stop();
                btnStart.Text = "Kết nối";
            }
            else
            {
                myClient.Connect();
                myClient.Listening((socketClient) =>
                {
                    var reader = new StreamReader(socketClient.GetStream());
                    while (myClient.IsConnect)
                    {
                        var data = reader.ReadLine();
                        Log("Received " + data);
                        var array = data.Split('|');
                        var mydata = string.Format("{0}|{1}", array[0], GetNSoNguyen(array[1]));
                        myClient.Send(mydata);
                    }
                });
                btnStart.Text = "Dừng";
            }
        }

        private string GetNSoNguyen(string data)
        {
            int input = int.Parse(data);
            var rd = new Random();
            int[] array = new int[input];
            for (int i = 0; i < input; i++)
            {
                array[i] = rd.Next(0, 100);
            }
            return string.Join(";", array);
        }

        private ClientAndServer myClient;

        public IPAddress iPAddress_MyServer => IPAddress.Parse(ip2.Text);

        public IPAddress iPAddress_Listen => IPAddress.Parse(myIP.Text);

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
