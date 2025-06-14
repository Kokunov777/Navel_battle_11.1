using System;
using System.Net.Sockets;
using System.Text;

namespace ProbaMessendger
{
    public class Client
    {
        private TcpClient client;
        private NetworkStream stream;

        public bool Connect(string ipAddress)
        {
            try
            {
                client = new TcpClient();
                client.Connect(ipAddress, 5000);
                stream = client.GetStream();
                Console.WriteLine("Connected to server!");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Connection error: {e.Message}");
                return false;
            }
        }

        public void SendMessage(string message)
        {
            try
            {
                if (stream != null && stream.CanWrite)
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(message + "\n");
                    stream.Write(buffer, 0, buffer.Length);
                    Console.WriteLine($"Sent message: {message}");
                }
                else
                {
                    Console.WriteLine("Cannot send message: Stream is null or not writable.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error sending message: {e.Message}");
            }
        }

        public void ReceiveMessage()
        {
            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Received: " + message);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error receiving message: {e.Message}");
            }
        }

        public void Disconnect()
        {
            stream?.Close();
            client?.Close();
            Console.WriteLine("Disconnected from server.");
        }
    }
}