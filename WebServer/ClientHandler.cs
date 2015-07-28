using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WebServer
{
    public class ClientHandler
    {
        public Router Router { get; protected set; }
        public Socket ClientSocket { get; protected set; }
        public StreamReader Reader { get; protected set; }
        public StreamWriter Writer { get; protected set; }

        /// <summary>
        /// The connection status between the server and the client.
        /// </summary>
        public bool Connected { get; set; }

        /// <summary>
        /// Creates a new client handler for the given socket and the given router.
        /// </summary>
        /// <param name="router"></param>
        /// <param name="socket"></param>
        public ClientHandler(Router router, Socket socket)
        {
            Router = router;
            ClientSocket = socket;
        }

        /// <summary>
        /// Initializes the handler and the I/O streams.
        /// </summary>
        private void Init()
        {
            Stream socketStream = new NetworkStream(ClientSocket);
            Reader = new StreamReader(socketStream);
            Writer = new StreamWriter(socketStream);
            Connected = true;
        }

        public void Handle(object threadContext)
        {
            Init();
            
            while (Connected)
            {
                // read request
                string line, requestText = "";
                while ((line = Reader.ReadLine()) != "")
                    requestText += line + "\n";

                Request req = new Request();
                req.Parse(requestText);
                
                Response res = Router.Handle(req);

                // keep connection alive if requested
                if (req.Header.ContainsKey("Connection") && req.Header["Connection"] == "keep-alive")
                {
                    res.Header.Connection = "keep-alive";
                    Connected = true;
                }
                else
                {
                    res.Header.Connection = "close";
                    Connected = false;
                }
                
                // put the stuff in the stream
                Writer.Write(res.GetText());
                Writer.Flush();
            }
        }
    }
}
