using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NoNote.config;

namespace NoNote.util
{
    public class NoteWebServer
    {
        private TcpListener myListener;
        private int port = 5050;
        private IPAddress localAddr = IPAddress.Parse("127.0.0.1");
        public string WebServerPath = @"WebServer";
        private string serverEtag = Guid.NewGuid().ToString("N");
        private Thread mainThread;
        private bool listen;

        public void startListen()
        {
            if (listen)
            {
                Console.WriteLine("already TCP Running");
                return;
            }


            myListener = new TcpListener(localAddr, port);
            myListener.Start();
            Console.WriteLine(
                $"Web Server Running on {localAddr.ToString()} on port {port}... Press ^C to Stop...");
            mainThread = new Thread(StartListen);
            mainThread.Start();
            listen = true;
        }

        public void stopListen()
        {
            myListener.Stop();
            mainThread.Abort();
            listen = false;
        }

        private void StartListen()
        {
            while (true)
            {
                TcpClient client = myListener.AcceptTcpClient();
                NetworkStream stream = client.GetStream();
                MemoryStream memoryStream = new MemoryStream();

                //read request 
                byte[] requestBytes = new byte[1024];
                int bytesRead = 0;

                bytesRead = stream.Read(requestBytes, 0, requestBytes.Length);

                string request = Encoding.UTF8.GetString(requestBytes, 0, bytesRead);
                var requestHeaders = ParseHeaders(request);

                string[] requestFirstLine = requestHeaders.requestType.Split(' ');
                string httpVersion = requestFirstLine.LastOrDefault();
                string contentType;
                requestHeaders.headers.TryGetValue("Accept", out contentType);
                string contentEncoding;
                requestHeaders.headers.TryGetValue("Acept-Encoding", out contentEncoding);
                string contentLength;
                requestHeaders.headers.TryGetValue("Content-Length", out contentLength);
                if (request.StartsWith("POST"))
                {
                    SendHeaders(httpVersion, 200, "OK", contentType, contentEncoding, 0, ref stream);
                }
                else if (request.StartsWith("GET"))
                {
                    var requestedPath = requestFirstLine[1];
                    var fileContent = GetContent(requestedPath);
                    if (fileContent != null)
                    {
                        SendHeaders(httpVersion, 200, "OK", contentType, contentEncoding, 0, ref stream);
                        stream.Write(fileContent, 0, fileContent.Length);
                    }
                    else
                    {
                        SendHeaders(httpVersion, 404, "Page Not Found", contentType, contentEncoding, 0, ref stream);
                    }
                }

                client.Close();
            }
        }


        private byte[] GetContent(string requestedPath)
        {
            if (requestedPath == "/") requestedPath = "index.html";
            string filePath;
            if (requestedPath.Contains("note"))
            {
                filePath = ConfigUtil.configArray.workFolder + requestedPath;
            }
            else
            {
                filePath = WebServerPath + '/' + requestedPath;
            }

            if (!File.Exists(filePath)) return null;
            else
            {
                byte[] file = System.IO.File.ReadAllBytes(filePath);
                return file;
            }
        }

        private void SendHeaders(string? httpVersion, int statusCode, string statusMsg, string? contentType,
            string? contentEncoding,
            int byteLength, ref NetworkStream networkStream)
        {
            string responseHeaderBuffer = "";

            responseHeaderBuffer = $"HTTP/1.1 {statusCode} {statusMsg}\r\n" +
                                   $"Connection: Keep-Alive\r\n" +
                                   $"Date: {DateTime.UtcNow.ToString()}\r\n" +
                                   $"Server: MacOs PC \r\n" +
                                   $"Etag: \"{serverEtag}\"\r\n" +
                                   $"Content-Encoding: {contentEncoding}\r\n" +
                                   "X-Content-Type-Options: nosniff" +
                                   $"Content-Type: application/signed-exchange;v=b3\r\n\r\n";

            byte[] responseBytes = Encoding.UTF8.GetBytes(responseHeaderBuffer);
            networkStream.Write(responseBytes, 0, responseBytes.Length);
        }

        private (Dictionary<string, string> headers, string requestType) ParseHeaders(string headerString)
        {
            var headerLines = headerString.Split('\r', '\n');
            string firstLine = headerLines[0];
            var headerValues = new Dictionary<string, string>();
            foreach (var headerLine in headerLines)
            {
                var headerDetail = headerLine.Trim();
                var delimiterIndex = headerLine.IndexOf(':');
                if (delimiterIndex >= 0)
                {
                    var headerName = headerLine.Substring(0, delimiterIndex).Trim();
                    var headerValue = headerLine.Substring(delimiterIndex + 1).Trim();
                    headerValues.Add(headerName, headerValue);
                }
            }

            return (headerValues, firstLine);
        }
    }
}