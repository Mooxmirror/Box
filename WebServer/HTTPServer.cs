using System;
using System.Net;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace WebServer
{
    public class HTTPServer
    {
        public bool Running { get; protected set; }
        public int Port { get; protected set; }
        public IPAddress Address { get; protected set; }
        
        private TcpListener Listener { set; get; }
        private List<Thread> RunningThreads { get; } = new List<Thread>();
        private int MaximumClientThreads { get; } = 256;
        private int MaximumIOThreads { get; } = 128;

        /// <summary>
        /// Creates a new HTTP server running on the given port and bound to the given address.
        /// </summary>
        /// <param name="port"></param>
        /// <param name="address"></param>
        public HTTPServer(int port = 3000, IPAddress address = null)
        {
            if (address == null)
                address = IPAddress.Any;

            Port = port;
            Address = address;
        }

        static void Main(string[] args)
        {
            HTTPServer webServer = new HTTPServer();
            webServer.Start();
        }

        /// <summary>
        /// Starts the server runtime.
        /// </summary>
        public void Start()
        {
            Initialize();
            Loop();
        }

        /// <summary>
        /// Initializes the server.
        /// </summary>
        private void Initialize()
        {
            Listener = new TcpListener(Address, Port);
            Listener.Start();
            ThreadPool.SetMaxThreads(MaximumClientThreads, MaximumIOThreads);
            
            Running = true;
            Console.WriteLine($"Server running on {Address}:{Port}");
        }

        /// <summary>
        /// Starts the main loop (blocking).
        /// </summary>
        private void Loop()
        {
            while (Running)
            {
                Socket client = Listener.AcceptSocket();
                HandleSocket(client);
                
                Console.WriteLine($"New client connected with IP {client.LocalEndPoint}");
            }
        }

        /// <summary>
        /// Handles a new incoming connection.
        /// </summary>
        /// <param name="socket"></param>
        private void HandleSocket(Socket socket)
        {
            ClientHandler handler = new ClientHandler(Router.Default, socket);
            ThreadPool.QueueUserWorkItem(handler.Handle);
        }
    }
}
