using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkGame
{
    public class GameClient
    {
        private TcpClient client;      // Подключение к серверу
        private NetworkStream stream;  // Поток для обмена данными
        private bool isConnected;      // Флаг подключения

        public void Connect(string ipAddress, int port = 8888)
        {
            client = new TcpClient();
            client.Connect(ipAddress, port);
            stream = client.GetStream();
            isConnected = true;

            // Запускаем поток для получения сообщений
            Thread receiveThread = new Thread(ReceiveMessages);
            receiveThread.Start();
        }

        private void ReceiveMessages()
        {
            try
            {
                while (isConnected)
                {
                    // Получаем и обрабатываем сообщения от сервера...
                }
            }
            catch (Exception)
            {
                Disconnect();
            }
        }

        public void Disconnect()
        {
            isConnected = false;
            client.Close();
        }

        public void SendMessage(NetworkMessage message)
        {
            // Отправка сообщения на сервер
            // (нужна сериализация сообщения)
        }
    }
}
