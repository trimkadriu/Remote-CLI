using RemoteCLI.Helpers;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using WebSocketSharp;

namespace RemoteCLI
{
    public partial class MainForm : Form
    {
        private CommandPrompt commandPrompt;
        private RemoteConnector remoteConnector;
        public MainForm()
        {
            InitializeComponent();
            commandPrompt = new CommandPrompt(this);
            remoteConnector = new RemoteConnector(this);
        }
        
        public void return_remote_result(string text)
        {
            txtOutput.AppendText(text);
            remoteConnector.sendMessage(text);
        }

        public void execute_remote_command(string command)
        {
            commandPrompt.exec_command(command);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            new Thread(delegate () {
                remoteConnector.connect();
            }).Start();
            new Thread(delegate () {
                commandPrompt.run_CMD();
            }).Start();
        }

    }
}
