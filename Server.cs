using SimpleTCP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TCP_IP_Connction
{
    public partial class client : Form
    {
        private TcpClient cl;
        public StreamWriter _writer;
        public StreamReader _reader;
        public string recive;
        public string TextToSend;
        public IPAddress[] localIP = new IPAddress[0];
        public client()
        {
            InitializeComponent();

            localIP = Dns.GetHostAddresses(Dns.GetHostName());

            foreach (IPAddress address in localIP)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    txtHost.Text = address.ToString();
                }
            }
        }

        private void Server_Load(object sender, EventArgs e)
        {

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                TcpListener listner = new TcpListener(IPAddress.Any, int.Parse(txtPort.Text));
                listner.Start();

                cl = listner.AcceptTcpClient();
                _reader = new StreamReader(cl.GetStream());
                _writer = new StreamWriter(cl.GetStream());
                _writer.AutoFlush = true;
                backgroundWorker1.RunWorkerAsync();
                backgroundWorker2.WorkerSupportsCancellation = true;
            }
            catch (Exception)
            {
                throw;
            }

        }

        private void btnStop_Click(object sender, EventArgs e)
        {

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            cl = new TcpClient();
            IPEndPoint IpEnd = new IPEndPoint(IPAddress.Parse(txtClient.Text), int.Parse(txtPortCliente.Text));

            try
            {
                txtMessage.AppendText("Connect to Sever" + "\n");
                _reader = new StreamReader(cl.GetStream());
                _writer = new StreamWriter(cl.GetStream());
                _writer.AutoFlush = true;
                backgroundWorker1.RunWorkerAsync();
                backgroundWorker2.WorkerSupportsCancellation = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (cl.Connected)
            {
                try
                {
                    recive = _reader.ReadLine();
                    this.txtChatStatus.Invoke(new MethodInvoker(delegate ()
                    {
                        txtChatStatus.AppendText("Você: " + recive + "\n");
                    }));
                    recive = "";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            if (cl.Connected)
            {
                _writer.WriteLine(TextToSend);
                this.txtChatStatus.Invoke(new MethodInvoker(delegate ()
                {
                    txtChatStatus.AppendText("Eu: " + TextToSend + "\n");
                }));
            }
            else
            {
                MessageBox.Show("Sending Failed");
            }

            backgroundWorker2.CancelAsync();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (txtMessage.Text != "" || txtMessage.Text != string.Empty)
            {
                TextToSend = txtMessage.Text;
                backgroundWorker2.RunWorkerAsync();
            }
            txtMessage.Text = "";
        }
    }
}
