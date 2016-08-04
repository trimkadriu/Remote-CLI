using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteCLI.Helpers
{
    class Utils
    {
        public static string getRemoteConnectionString()
        {
            return "{\"command\":\"subscribe\",\"identifier\":\"{\\\"channel\\\":\\\"MessagesChannel\\\",\\\"authid\\\":\\\"" 
                + Settings.IDENTIFIER + "\\\"}\"}";
        }

        public static string getRemoteResultString(string message)
        {
            return "{\"command\":\"message\",\"identifier\":\"{\\\"channel\\\":\\\"MessagesChannel\\\",\\\"authid\\\":\\\"" 
                + Settings.IDENTIFIER + "\\\"}\",\"data\":\"{\\\"action\\\":\\\"stream_result\\\",\\\"authid\\\":\\\"" 
                + Settings.IDENTIFIER + "\\\",\\\"message\\\":\\\"" + encodeBase64(message) + "\\\", \\\"type\\\":\\\"result\\\"}\"}";
        }

        public static string encodeBase64(string text)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(text);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string decodeBase64(string text)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(text);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
