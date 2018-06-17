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

namespace Server2
{
    public partial class frmServer2 : Form, ILog, IView
    {
        public frmServer2()
        {
            InitializeComponent();
        }

        private void frmServer2_Load(object sender, EventArgs e)
        {
            ip2.Text = "127.000.000.002";
            myClient = new ClientAndServer(this, this);
        }


        private void button3_Click(object sender, EventArgs e)
        {
            if (myClient.IsConnect)
            {
                myClient.Stop();
                btnConnect.Text = "Kết nối";
            }
            else
            {
                myClient.Listening((client) =>
                {
                    var reader = new StreamReader(client.GetStream());
                    while (myClient.IsConnect)
                    {
                        var data = reader.ReadLine();
                        Log(data);
                        var array = data.Split('|');
                        var aip = array[0].Split('_');
                        var ip = new IPEndPoint(address: IPAddress.Parse(aip[0]), port: int.Parse(aip[1]));
                        myClient.SendTo(ip, DemSoHoanHao(array[1].Split(';')));
                    }
                });
                btnConnect.Text = "Dừng";
            }
        }

        private string DemSoHoanHao(string[] v)
        {
            List<Task<int>> lst = new List<Task<int>>();
            for (int i = 0; i < 4; i++)
            {
                lst.Add(DemSoHoanHao(v, v.Length / 5 * i, v.Length / 5));
            }
            lst.Add(DemSoHoanHao(v, v.Length / 5 * 4, v.Length - v.Length / 5 * 4));
            Task.WaitAll(lst.ToArray());
            return lst.Sum(q => q.Result).ToString();
        }

        private Task<int> DemSoHoanHao(string[] source, int indexStart, int Lenth)
        {
            return Task<int>.Run(() =>
            {
                int count = 0;
                for (int i = 0; i < Lenth; i++)
                {
                    var res = CheckSoHoanHao(source[indexStart + i]);
                    if (res)
                    {
                        count++;
                    }
                }
                return count;
            });
        }

        private bool CheckSoHoanHao(string v)
        {
            int input = int.Parse(v);
            decimal sum = 0;
            bool res = false;
            for (int i = 1; i < input; i++)
            {
                if (input % i == 0)
                {
                    sum += i;
                    if (sum > input)
                    {
                        break;
                    }
                }
            }
            if (sum == input) res = true;
            Log(string.Format("{0} : {1}", input, res));
            return res;
        }

        private ClientAndServer myClient;

        public IPAddress iPAddress_MyServer => IPAddress.Parse(ip2.Text);

        public IPAddress iPAddress_Listen => IPAddress.Parse(ip2.Text);

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
