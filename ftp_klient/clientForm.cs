using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ftp_klient
{
    public partial class clientForm : Form
    {
        Client client;
        public clientForm()
        {
            InitializeComponent();
            
            
        }
        public void connectBtn_Click(object source, EventArgs args)
        {
            string ip = ipBox.Text;
            client = new Client(ip);
            client.MessageRecieved += this.OnMessageRecieved;
            try
            {
                client.startClient();
                connectBtn.Enabled = false;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                client.receiveMessages();
            }
            
        }

        private void sendBtn_Click(object sender, EventArgs e)
        {
            if (client.IsConnected())
            {
                if (messageBox.Text != "")
                {
                    client.sendMessage(messageBox.Text);
                    messagesBox.Text += $"You: {messageBox.Text} {Environment.NewLine}";
                    messageBox.Text = "";
                }
            }
            else
            {
                messagesBox.AppendText("You are not connected" + Environment.NewLine);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            client.shutDownClient();
        }
        public void OnMessageRecieved(object source, MessageEventArgs args)
        {
            messagesBox.AppendText($"{client.getSeverIp()} : {args.message} {Environment.NewLine}");
            messageBox.Text = "";
        }
    }
}
