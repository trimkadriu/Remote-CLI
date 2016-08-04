using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Net;

namespace Helpers
{
    /// <summary>
    /// Allow the transfer of data files using the W3C's specification
    /// for HTTP multipart form data. Microsoft's version has a bug
    /// where it does not format the ending boundary correctly.
    /// Original version written by Gregory Prentice : gregoryp@norvanco.com
    /// See: http://www.c-sharpcorner.com/UploadFile/gregoryprentice/DotNetBugs12062005230632PM/DotNetBugs.aspx
    /// </summary>
    public class MultipartForm
    {
        /// <summary>
        /// Holds any form fields and values that you
        /// wish to transfer with your data.
        /// </summary>
        private Hashtable coFormFields;

        /// <summary>
        /// Used mainly to avoid passing parameters to other routines.
        /// Could have been local to sendFile().
        /// </summary>
        protected HttpWebRequest coRequest;

        /// <summary>
        /// Used if we are testing and want to output the raw
        /// request, minus http headers, out to a file.
        /// </summary>
        private Stream coFileStream;

        /// <summary>
        /// Difined to build the form field data that is being
        /// passed along with the request.
        /// </summary>
        private static string CONTENT_DISP = "Content-Disposition: form-data; name=";

        /// <summary>
        /// Allows you to specify the specific version of HTTP to use for uploads.
        /// The dot NET stuff currently does not allow you to remove the continue-100 header
        /// from 1.1 and 1.0 currently has a bug in it where it adds the continue-100. MS
        /// has sent a patch to remove the continue-100 in HTTP 1.0.
        /// </summary>
        public Version TransferHttpVersion { get; set; }

        /// <summary>
        /// Used to change the content type of the file being sent.
        /// Currently defaults to: text/xml. Other options are
        /// text/plain or binary
        /// </summary>
        public string FileContentType { get; set; }

        /// <summary>
        /// Initialize our class for use to send data files.
        /// </summary>
        /// <param name="url">The web address of the recipient of the data transfer.</param>
        public MultipartForm(string url)
        {
            URL = url;
            coFormFields = new Hashtable();
            ResponseText = new StringBuilder();
            BufferSize = 1024 * 10;
            BeginBoundary = "ou812--------------8c405ee4e38917c";
            TransferHttpVersion = HttpVersion.Version11;
            FileContentType = "text/xml";
        }
        //---------- BEGIN PROPERTIES SECTION ----------
        private string _BeginBoundary;
        /// <summary>
        /// The string that defines the begining boundary of
        /// our multipart transfer as defined in the w3c specs.
        /// This method also sets the Content and Ending
        /// boundaries as defined by the w3c specs.
        /// </summary>
        public string BeginBoundary
        {
            get { return _BeginBoundary; }
            set
            {
                _BeginBoundary = value;
                ContentBoundary = "--" + BeginBoundary;
                EndingBoundary = ContentBoundary + "--";
            }
        }

        /// <summary>
        /// The string that defines the content boundary of
        /// our multipart transfer as defined in the w3c specs.
        /// </summary>
        protected string ContentBoundary { get; set; }

        /// <summary>
        /// The string that defines the ending boundary of
        /// our multipart transfer as defined in the w3c specs.
        /// </summary>
        protected string EndingBoundary { get; set; }

        /// <summary>
        /// The data returned to us after the transfer is completed.
        /// </summary>
        public StringBuilder ResponseText { get; set; }

        /// <summary>
        /// The web address of the recipient of the transfer.
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// Allows us to determine the size of the buffer used
        /// to send a piece of the file at a time out the IO
        /// stream. Defaults to 1024 * 10.
        /// </summary>
        public int BufferSize { get; set; }

        //---------- END PROPERTIES SECTION ----------

        /// <summary>
        /// Used to signal we want the output to go to a
        /// text file verses being transfered to a URL.
        /// </summary>
        /// <param name="path"></param>
        public void SetFilename(string path)
        {
            coFileStream = new System.IO.FileStream(path, FileMode.Create, FileAccess.Write);
        }
        /// <summary>
        /// Allows you to add some additional field data to be
        /// sent along with the transfer. This is usually used
        /// for things like userid and password to validate the
        /// transfer.
        /// </summary>
        /// <param name="key">The form field name</param>
        /// <param name="str">The form field value</param>
        public void SetField(string key, string str)
        {
            coFormFields[key] = str;
        }
        /// <summary>
        /// Determines if we have a file stream set, and returns either
        /// the HttpWebRequest stream of the file.
        /// </summary>
        /// <returns></returns>
        public virtual Stream GetStream()
        {
            Stream stream;
            if (null == coFileStream)
                stream = coRequest.GetRequestStream();
            else
                stream = coFileStream;
            return stream;
        }
        /// <summary>
        /// Here we actually make the request to the web server and
        /// retrieve it's response into a text buffer.
        /// </summary>
        public virtual void GetResponse()
        {
            if (null == coFileStream)
            {
                Stream stream;
                WebResponse response;
                try
                {
                    response = coRequest.GetResponse();
                }
                catch (WebException web)
                {
                    response = web.Response;
                }
                if (null != response)
                {
                    stream = response.GetResponseStream();
                    StreamReader sr = new StreamReader(stream);
                    string str;
                    ResponseText.Length = 0;
                    while ((str = sr.ReadLine()) != null)
                        ResponseText.Append(str);
                    response.Close();
                }
                else
                    throw new Exception("MultipartForm: Error retrieving server response");
            }
        }
        /// <summary>
        /// Transmits a file to the web server stated in the
        /// URL property. You may call this several times and it
        /// will use the values previously set for fields and URL.
        /// </summary>
        /// <param name="filename">The full path of file being transfered.</param>
        public void SendFile(string filename)
        {
            // The live of this object is only good during
            // this function. Used mainly to avoid passing
            // around parameters to other functions.
            coRequest = (HttpWebRequest)WebRequest.Create(URL);
            // Set use HTTP 1.0 or 1.1.
            coRequest.ProtocolVersion = TransferHttpVersion;
            coRequest.Method = "POST";
            coRequest.ContentType = "multipart/form-data; boundary=" + BeginBoundary;
            coRequest.Headers.Add("Cache-Control", "no-cache");
            coRequest.KeepAlive = true;
            string strFields = GetFormfields();
            string strFileHdr = GetFileheader(filename);
            string strFileTlr = GetFiletrailer();
            FileInfo info = new FileInfo(filename);
            coRequest.ContentLength = strFields.Length +
              strFileHdr.Length +
              strFileTlr.Length +
              info.Length;
            System.IO.Stream io;
            io = GetStream();
            WriteString(io, strFields);
            WriteString(io, strFileHdr);
            this.WriteFile(io, filename);
            WriteString(io, strFileTlr);
            GetResponse();
            io.Close();
            // End the life time of this request object.
            coRequest = null;
        }
        /// <summary>
        /// Mainly used to turn the string into a byte buffer and then
        /// write it to our IO stream.
        /// </summary>
        /// <param name="stream">The io stream for output.</param>
        /// <param name="str">The data to write.</param>
        public void WriteString(Stream stream, string str)
        {
            byte[] postData = System.Text.Encoding.ASCII.GetBytes(str);
            stream.Write(postData, 0, postData.Length);
        }
        /// <summary>
        /// Builds the proper format of the multipart data that
        /// contains the form fields and their respective values.
        /// </summary>
        /// <returns>The data to send in the multipart upload.</returns>
        public string GetFormfields()
        {
            string str = "";
            IDictionaryEnumerator myEnumerator = coFormFields.GetEnumerator();
            while (myEnumerator.MoveNext())
            {
                str += ContentBoundary + "\r\n" +
                  CONTENT_DISP + '"' + myEnumerator.Key + "\"\r\n\r\n" +
                  myEnumerator.Value + "\r\n";
            }
            return str;
        }
        /// <summary>
        /// Returns the proper content information for the
        /// file we are sending.
        /// </summary>
        /// <remarks>
        /// Hits Patel reported a bug when used with ActiveFile.
        /// Added semicolon after sendfile to resolve that issue.
        /// Tested for compatibility with IIS 5.0 and Java.
        /// </remarks>
        /// <param name="filename"></param>
        /// <returns></returns>
        public string GetFileheader(string filename)
        {
            return ContentBoundary + "\r\n" +
              CONTENT_DISP +
              "\"sendfile\"; filename=\"" +
              Path.GetFileName(filename) + "\"\r\n" +
              "Content-type: " + FileContentType + "\r\n\r\n";
        }
        /// <summary>
        /// Creates the proper ending boundary for the multipart upload.
        /// </summary>
        /// <returns>The ending boundary.</returns>
        public string GetFiletrailer()
        {
            return "\r\n" + EndingBoundary;
        }
        /// <summary>
        /// Reads in the file a chunck at a time then sends it to the
        /// output stream.
        /// </summary>
        /// <param name="stream">The io stream to write the file to.</param>
        /// <param name="filename">The name of the file to transfer.</param>
        public void WriteFile(Stream stream, string filename)
        {
            using (FileStream readIn = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                readIn.Seek(0, SeekOrigin.Begin); // move to the start of the file
                byte[] fileData = new byte[BufferSize];
                int bytes;
                while ((bytes = readIn.Read(fileData, 0, BufferSize)) > 0)
                {
                    // read the file data and send a chunk at a time
                    stream.Write(fileData, 0, bytes);
                }
            }
        }
    }
}
