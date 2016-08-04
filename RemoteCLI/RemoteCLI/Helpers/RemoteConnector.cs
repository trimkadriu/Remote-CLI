using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using WebSocketSharp;

namespace RemoteCLI.Helpers
{
    class RemoteConnector
    {
        private WebSocket webSocket;
        private MainForm gui;
        public Boolean connected;
        public RemoteConnector(MainForm form)
        {
            gui = form;
            webSocket = new WebSocket(Settings.WEBSOCKETURL);
            webSocket.OnMessage += onMessage;
        }
        public Boolean connect()
        {
            try
            {
                webSocket.Connect();
                webSocket.Send(Utils.getRemoteConnectionString());
                connected = true;
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
                connected = false;
                return false;
            }
        }

        private void onMessage(object s, MessageEventArgs e)
        {
            JObject result = JObject.Parse(e.Data);
            Debug.WriteLine(e.Data);
            if ((string)result["type"] == null && (string)result["message"]["type"] == "command") // Have a command
            {
                string command = Utils.decodeBase64((string)result["message"]["message"]);
                try
                {
                    gui.BeginInvoke((Action)delegate
                    {
                        gui.execute_remote_command(command);
                    }
                );
                }
                catch (Exception) { }
            }
        }

        public void sendMessage(string s)
        {
            webSocket.Send(Utils.getRemoteResultString(s));
        }
    }
}
