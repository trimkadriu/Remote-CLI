using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace RemoteCLI
{
    class CincDW
    {
        WebClient Client;

        public CincDW()
        {
            Client = new WebClient();
        }

        public String downloadFile(String url, String path)
        {
            String result = "";
            try
            {
                String fileName = url.Remove(0, url.LastIndexOf('/') + 1);//Path.GetFileName(url);
                string fullPath = path + "\\" + fileName;
                Client.DownloadFile(new Uri(url), fullPath);
                result = ">>>Cinci: File Downloaded Successfully\r\n";
                result += ">>>" + fullPath;
            }
            catch (Exception ex)
            {
                result = ">>>Cinci: File Download ERROR!\r\n";
                result += ">>>" + ex.Message;
            }
            return result;
        }
    }
}
