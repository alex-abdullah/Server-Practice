using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RawHttpServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            int port = 8080; // Choose a port
            IPAddress ipAddress = IPAddress.Loopback; // Listen on localhost

            TcpListener listener = new TcpListener(ipAddress, port);

            try
            {
                listener.Start();
                Console.WriteLine($"Server started. Listening on http://{ipAddress}:{port}/");

                while (true)
                {
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    Console.WriteLine("Client connected.");
                    _ = HandleClientAsync(client); // Handle each client connection asynchronously
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine($"SocketException: {e.Message}");
            }
            finally
            {
                listener.Stop();
            }
        }

        static async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                using (NetworkStream stream = client.GetStream())
                {
                    // Read the incoming request
                    byte[] buffer = new byte[1024];
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Received request:\n{request}");

                    // Basic response
                    string responseContent = "<h1>Hello from raw .NET server!</h1>";
                    string response = $"HTTP/1.1 200 OK\r\n" +
                                      $"Content-Type: text/html\r\n" +
                                      $"Content-Length: {Encoding.UTF8.GetBytes(responseContent).Length}\r\n" +
                                      $"Connection: close\r\n" + // For simplicity, close the connection after each request
                                      "\r\n" +
                                      responseContent;
                    byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                    await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
                    Console.WriteLine("Response sent.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error handling client: {e.Message}");
            }
            finally
            {
                client.Close();
                Console.WriteLine("Client disconnected.");
            }
        }
    }
}