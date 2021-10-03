using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace web_server {
    class Server {
        private TcpListener listener { get; set; }

        public Server() : this(80) {}

        public Server(int port) {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            while(true)
            {
                var client = listener.AcceptTcpClient();
                var thread = new Thread(new ParameterizedThreadStart(HandleClient));
                thread.Start(client);
            }
        }

        private static void HandleClient(object tcpClient) {
            new Client((TcpClient)tcpClient);
        }

        ~Server() {
            if (listener != null)
            {
                listener.Stop();
            }
        }
    }
}