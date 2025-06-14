using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

public class Server
{
    private TcpListener listener;
    private ListBox chatBox;
    private Thread serverThread;
    private char[,] serverField = new char[10, 10]; // Добавляем поле сервера

    public Server(ListBox chatBox)
    {
        this.chatBox = chatBox;
        InitializeServerField();
    }
    private void InitializeServerField()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                serverField[i, j] = ' '; // Инициализация пустого поля
            }
        }
        // Здесь можно добавить расстановку кораблей для сервера
        // Например: serverField[0,0] = 'S'; - корабль в A1
    }
    public void Start()
    {
        serverThread = new Thread(() =>
        {
            listener = new TcpListener(IPAddress.Any, 5000);
            listener.Start();
            chatBox.Invoke((MethodInvoker)(() => chatBox.Items.Add("Server started...")));

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Thread clientThread = new Thread(HandleClient);
                clientThread.Start(client);
            }
        });
        serverThread.IsBackground = true;
        serverThread.Start();
    }

    private void HandleClient(object obj)
    {
        TcpClient client = (TcpClient)obj;
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];
        int bytesRead;

        try
        {
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                chatBox.Invoke((MethodInvoker)delegate { chatBox.Items.Add("Client Attack!!!: " + message); });

                // Проверяем попадание
                string response = CheckAttack(message);

                // Отправляем ответ
                byte[] responseData = Encoding.UTF8.GetBytes(response + "\n");
                stream.Write(responseData, 0, responseData.Length);
            }
        }
        catch (IOException)
        {
            chatBox.Invoke((MethodInvoker)delegate { chatBox.Items.Add("Client disconnected."); });
        }
        finally
        {
            client.Close();
        }
    }
    private string CheckAttack(string coordinates)
    {
        try
        {
            char xChar = coordinates[0];
            int x = xChar - 'A';
            int y = int.Parse(coordinates.Substring(1)) - 1;

            if (x >= 0 && x < 10 && y >= 0 && y < 10)
            {
                if (serverField[y, x] == 'S')
                {
                    serverField[y, x] = 'X';
                    return $"Попадание в {coordinates}!";
                }
                else
                {
                    return $"Промах на {coordinates}!";
                }
            }
            return "Неверные координаты!";
        }
        catch
        {
            return "Неверный формат координат!";
        }
    }
}
