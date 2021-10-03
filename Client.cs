using System;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;

static class Constants
{
    public const int StatusOK = 200;
    public const int StatusBadRequest = 400;
    public const int StatusForbidden = 403;
    public const int StatusMethodNotAllowed = 405;
    public const int StatusNotFound = 404;
}


namespace web_server {

    class Client {
        private TcpClient client      { get; set; }
        private int       status      { get; set; }
        private string    method      { get; set; }
        private string    path        { get; set; }
        private string    contentType { get; set; }
        
        public Client(TcpClient client) {
            this.client = client;
            this.HandleRequest();
        }

        public void HandleRequest() {
            string req = this.ReadRequest();

            status = this.ParseRequest(req);

            this.SendResponse();

            client.Close();
        }

        public string ReadRequest() {
            byte[] buffer = new byte[1024];
            string request = String.Empty;
            int countData;

            while ((countData = this.client.GetStream().Read(buffer, 0, buffer.Length)) > 0)
            {
                request += Encoding.ASCII.GetString(buffer, 0, countData);
                if (request.IndexOf("\r\n\r\n") >= 0 || request.Length > 4096)
                {
                    break;
                }
            }
            return request;
        }

        public int ParseRequest(string req) {
            int space = req.IndexOf(' ');
            try {
            method = req.Substring(0, space);
            } catch (Exception) {
                method = "GET";
            }

            if (method != "GET" && method != "HEAD") {
                return Constants.StatusMethodNotAllowed;
            }

            try {
            path = req.Substring(space + 1, req.IndexOf(" HTTP/") - space - 1);
            } catch (Exception) {
                path = "/";
            }
            if (path.IndexOf("?") > 0) {
                path = path.Substring(0, path.IndexOf("?"));
            }
            path = Uri.UnescapeDataString(path);

            if (path.IndexOf("../") >= 0) {
                return Constants.StatusForbidden;
            }

            if (path.EndsWith("/")) {
                if (path.IndexOf(".") > 0) {
                    return Constants.StatusNotFound;
                } else {
                    path += "index.html";
                    if (!File.Exists(path.Substring(1)))
                    {
                        return Constants.StatusForbidden;
                    }
                }
            }
            if (!File.Exists(path.Substring(1)))
            {
                return Constants.StatusNotFound;
            }   

            this.SetContentType();

            return Constants.StatusOK;
        }

        private void SendResponse() {
            string resp = "";
            byte[] respBody;
            if (status != Constants.StatusOK) {
                var html = $"<html><body><h1>{status}</h1></body></html>";
                contentType = "text/html";
                resp = $"HTTP/1.1 {status} " + ((HttpStatusCode)status).ToString() + "\r\nServer: web_server 1.0.0\r\nDate:" + DateTime.Now.ToString() + "\r\nConnection: keep-alive"+ $"\r\nContent-Type:{contentType}" + $"\r\nContent-length:{html.Length}" + "\r\n\r\n";

                if (method != "HEAD") {
                    resp += html;
                }
                respBody =  Encoding.ASCII.GetBytes(resp);
                client.GetStream().Write(respBody, 0, respBody.Length);
                return;

            }

            FileStream file = new FileStream(path.Substring(1), FileMode.Open, FileAccess.Read, FileShare.Read);
            resp = $"HTTP/1.1 {status} " + ((HttpStatusCode)status).ToString() + "\r\nServer: web_server 1.0.0\r\nDate:" + DateTime.Now.ToString() + "\r\nConnection: keep-alive"+ $"\r\nContent-Type:{contentType}" + $"\r\nContent-length:{file.Length}" + "\r\n\r\n";
            respBody =  Encoding.ASCII.GetBytes(resp);
            client.GetStream().Write(respBody, 0, respBody.Length);

             if (method != "HEAD") {
                int lengthData = 0;
                byte[] buffer = new byte[1024];

                while (file.Position < file.Length)
                {
                    lengthData = file.Read(buffer, 0, buffer.Length);
                    client.GetStream().Write(buffer, 0, lengthData);
                }
            }

            file.Close();
        }

        private void SetContentType() {
            var ext = path.Substring(path.LastIndexOf('.'));
            switch(ext){
                case ".html":
                    contentType = "text/html";
                    break;
                case ".css":
                    contentType = "text/css";
                    break;
                case ".js":
                    contentType = "text/javascript";
                    break;
                case ".jpg":
                    contentType = "image/jpeg";
                    break;
                case ".swf":
                    contentType = "application/x-shockwave-flash";
                    break;
                case ".jpeg":
                case ".png":
                case ".gif":
                    contentType = "image/" + ext.Substring(1);
                    break;
                default:
                    if (ext.Length > 1)
                    {
                        contentType = "application/" + ext.Substring(1);
                    }
                    else
                    {
                        contentType = "application/unknown";
                    }
                    break;
            }
        }
    }
}