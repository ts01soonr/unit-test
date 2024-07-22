// Asynchronous Server Socket Example
// http://msdn.microsoft.com/en-us/library/fx6588te.aspx
// remark by @fang - 2024

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
//using AppiumTest;
// State object for reading client data asynchronously
namespace SocketDemo
{
    public class AsyncServer
    {
        // Client  socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }

    public class AsynchronousSocketListener
    {
        // Thread signal.
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public AsynchronousSocketListener()
        {
        }

        public static void StartListening()
        {
            // Data buffer for incoming data.
            byte[] bytes = new Byte[1024];

            // Establish the local endpoint for the socket.
            // The DNS name of the computer
            // running the listener is "host.contoso.com".
            //IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            //IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPAddress ipAddress = IPAddress.Parse("0.0.0.0");
            int PORT = 11111;
            Console.WriteLine("Socker Service start from IP=" + ipAddress + ":" + PORT);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, PORT);

            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    // Set the event to nonsignaled state.
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.
                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                    // Wait until a connection is made before continuing.
                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            allDone.Set();
            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            //Output log
            Console.WriteLine("Connection received from local=" + handler.LocalEndPoint + ",remote=" + handler.RemoteEndPoint);

            // Create the state object.
            AsyncServer state = new AsyncServer();
            state.workSocket = handler;
            String welcomeMsg = "Connection Sucessfull from " + handler.RemoteEndPoint + "\r\n";
            Send(handler, welcomeMsg);
            handler.BeginReceive(state.buffer, 0, AsyncServer.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            AsyncServer state = (AsyncServer)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket. 
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read 
                // more data.
                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1 || content.EndsWith("\r\n"))
                {
                    // All the data has been read from the 
                    // client. Display it on the console.
                    content = content.Trim();
                    Console.WriteLine("<< receive message: {0}", content);

                    // Echo the data back to the client.
                    //Send(handler, content);
                    //Send(handler, content+" "+PerformAction(content));
                    SendNPerform(handler, content);
                    if (!content.Equals("bye"))
                    {
                        state.sb.Clear();
                        handler.BeginReceive(state.buffer, 0, AsyncServer.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                    }
                }
                else
                {
                    // Not all data received. Get more.
                    handler.BeginReceive(state.buffer, 0, AsyncServer.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                }
            }
        }

        private static void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
        }
        private static void SendNPerform(Socket handler, String data)
        {
            String result = ConnectionHandler.PerformAction(data);
            Send(handler, data + " " + result + "\r\n");
            if (data.Equals("bye"))
            {
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }

        }
        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                if (ar.IsCompleted) return;
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine(">> Sent {0} bytes to client.", bytesSent);

                //Move into SendNPerform();
                //handler.Shutdown(SocketShutdown.Both);
                //handler.Close();

            }
            catch (Exception e)
            {
                //Console.WriteLine("Socket-Disconnect or Error" + ar.IsCompleted);
                if (!ar.IsCompleted) Console.WriteLine(e.ToString());
            }
        }

        public static String PerformActionx(String command)
        {
            if (command == null || command.Length == 0)
            {
                //sendMessage("no command given");
                return "No command given";
            }
            String[] tokens = command.Split(' ');
            String cmd = tokens[0];
            String path = tokens.Length > 1 ? tokens[1].Replace("\\*", " ") : "";
            String parm3 = tokens.Length > 2 ? tokens[2] : "";
            String parm4 = tokens.Length > 3 ? tokens[3] : "";
            String parm5 = tokens.Length > 4 ? tokens[4] : "";
            String parm6 = tokens.Length > 5 ? tokens[5] : "";
            String parm7 = tokens.Length > 6 ? tokens[6] : "";
            String parm8 = tokens.Length > 7 ? tokens[7] : "";
            String parm9 = tokens.Length > 8 ? tokens[8] : "";
            String parm10 = tokens.Length > 9 ? tokens[9] : "";
            String path2 = command.Substring(cmd.Length).Trim();
            String path3 = command.Substring((cmd + " " + path).Trim().Length).Trim();
            //String path4 = command.substring((cmd+" "+path+" "+path3).trim().length()).trim();
            String path4 = (parm4 + " " + parm5 + " " + parm6 + " " + parm7 + " " + parm8 + " " + parm9 + " " + parm10).Trim();
            if (cmd.Equals("bye") || cmd.Equals("quit"))
            {
                return "OK";
            }
            else if (cmd.Equals("aos"))
            {

                return "OK";
            }
            else if (cmd.Equals("aos2"))
            {

                return "OK";
            }
            else if (cmd.Equals("ios"))
            {

                return "OK";
            }
            else if (cmd.Equals("ios2"))
            {

                return "OK";
            }
            else if (cmd.Equals("run"))
            {
                //run system.shell command
                return "pending... OK";
            }
            else if (cmd.Equals("who"))
            {
                return "fang OK";
            }
            else if (cmd.Equals("help"))
            {
                return "aos|ios|help|who OK";
            }
            return "FAIL";
        }
        public static int Mainx(String[] args)
        {
            StartListening();
            return 0;
        }
    }
}