using Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace RemoteCLI
{
    class CincUP
    {
        WebClient Client;

        public CincUP()
        {
            Client = new WebClient();
        }

        public String uploadFile(String url, String path)
        {
            String result = "";
            try
            {
                String fileName = Path.GetFileName(url);
                MultipartForm form = new MultipartForm(url);
                form.SendFile(@path);
                result = ">>>Cinci: File Uploaded Successfully\r\n";
                result += ">>>" + path;
            }
            catch (Exception ex)
            {
                result = ">>>Cinci: File Upload ERROR!\r\n";
                result += ">>>" + ex.Message;
            }
            return result;
        }
    }
}
