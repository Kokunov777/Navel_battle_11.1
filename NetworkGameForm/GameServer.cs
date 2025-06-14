using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkGame
{
    public class GameServer
    {
        private TcpListener listener; // Слушатель подключений
        private TcpClient client;     // Подключенный клиент
        private NetworkStream stream; // Поток для обмена данными
        private bool isRunning;      // Флаг работы сервера

        public void Start(int port = 8888)
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            isRunning = true;

            // Запускаем поток для принятия подключений
            Thread acceptThread = new Thread(AcceptClients);
            acceptThread.Start();
        }

        private void AcceptClients()
        {
            try
            {
                while (isRunning)
                {
                    // Ожидаем подключения клиента
                    client = listener.AcceptTcpClient();
                    stream = client.GetStream();

                    // Здесь обработка сообщений от клиента...
                }
            }
            catch (SocketException)
            {
                // Сервер был остановлен
            }
        }

        public void Stop()
        {
            isRunning = false;
            listener.Stop();

            if (client != null)
                client.Close();
        }

        public void SendMessage(NetworkMessage message)
        {
            // Отправка сообщения клиенту
            // (нужна сериализация сообщения)
        }
    }
}
