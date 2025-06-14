using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using ProbaMessendger;

namespace SeaBattle.ClientServerForm {

    public partial class ClientServer : Form
    {
        private Server server;
        private Client client;
        private char[,] playerField = new char[10, 10];
        private char[,] enemyField = new char[10, 10];

        private int shipsPlaced = 0;
        private const int maxShips = 5;
        private bool gameStarted = false;

        private int playerShipsRemaining = 5;
        private int enemyShipsRemaining = 5;
        private bool isGameOver = false;

        public ClientServer()
        {
            InitializeComponent();
            playerField = new char[10, 10];
            enemyField = new char[10, 10];
            InitializeGameFields();
            server = new Server(chatBox);
            server.Start();
            client = new Client();
        }
        private void InitializeGameFields()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    playerField[i, j] = ' '; // Пустое поле вместо воды
                    enemyField[i, j] = ' ';  // Пустое поле вместо воды
                }
            }

            // Инициализация панелей
            playerFieldPanel.Invalidate();
            enemyFieldPanel.Invalidate();
        }

        private bool IsCellValidForShip(int x, int y)
        {
            // Проверяем, что клетка свободна и вокруг нет других кораблей
            if (playerField[y, x] != ' ') return false;

            // Проверяем все соседние клетки (включая диагонали)
            for (int i = Math.Max(0, y - 1); i <= Math.Min(9, y + 1); i++)
            {
                for (int j = Math.Max(0, x - 1); j <= Math.Min(9, x + 1); j++)
                {
                    if (playerField[i, j] == 'S') return false;
                }
            }
            return true;
        }
        private void PlaceShip(int x, int y)
        {
            if (gameStarted || shipsPlaced >= maxShips || isGameOver) return;

            if (x >= 0 && x < 10 && y >= 0 && y < 10 && IsCellValidForShip(x, y))
            {
                playerField[y, x] = 'S';
                shipsPlaced++;
                playerFieldPanel.Invalidate();
                chatBox.Items.Add($"Корабль размещен на {Convert.ToChar('A' + x)}{y + 1}. Осталось {maxShips - shipsPlaced} кораблей.");

                if (shipsPlaced == maxShips)
                {
                    gameStarted = true;
                    chatBox.Items.Add("Все корабли размещены! Игра началась.");
                }
            }
            else
            {
                chatBox.Items.Add("Нельзя разместить корабль здесь! Корабли должны быть на расстоянии минимум одной клетки.");
            }
        }

        private void CheckGameOver()
        {
            if (playerShipsRemaining <= 0)
            {
                isGameOver = true;
                chatBox.Items.Add("Игра окончена! Вы проиграли. Все ваши корабли уничтожены.");
                MessageBox.Show("Игра окончена! Вы проиграли. Все ваши корабли уничтожены.", "Конец игры");
            }
            else if (enemyShipsRemaining <= 0)
            {
                isGameOver = true;
                chatBox.Items.Add("Игра окончена! Вы победили! Все корабли противника уничтожены.");
                MessageBox.Show("Игра окончена! Вы победили! Все корабли противника уничтожены.", "Конец игры");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (isGameOver) return;

            string attackCoordinates = messageTextBox.Text.ToUpper();

            if (!ValidateCoordinates(attackCoordinates))
            {
                chatBox.Items.Add("Неверные координаты! Используйте формат A1-J10");
                return;
            }

            client.Connect(ipTextBox.Text);
            client.SendMessage(messageTextBox.Text);
            chatBox.Items.Add("You: " + messageTextBox.Text);
            messageTextBox.Clear();
        }
        private void attackButton_Click(object sender, EventArgs e)
        {
            if (isGameOver || !gameStarted) return;

            string coordinates = messageTextBox.Text.ToUpper();

            if (!ValidateCoordinates(coordinates))
            {
                chatBox.Items.Add("Неверные координаты! Используйте формат A1-J10");
                return;
            }

            client.Connect(ipTextBox.Text);
            client.SendMessage(coordinates);
            chatBox.Items.Add($"Вы атаковали: {coordinates}");
            UpdateEnemyFieldAfterAttack(coordinates);
            messageTextBox.Clear();
        }

        // Новый метод для обработки атак противника
        public void ProcessIncomingAttack(string coordinates)
        {
            if (isGameOver) return;

            char xChar = coordinates[0];
            int x = xChar - 'A';
            int y = int.Parse(coordinates.Substring(1)) - 1;

            if (playerField[y, x] == 'S') // Попадание в корабль
            {
                playerField[y, x] = 'X';
                playerShipsRemaining--;
                chatBox.Items.Add($"Противник попал в ваш корабль на {coordinates}! Осталось кораблей: {playerShipsRemaining}");

                if (IsShipDestroyed(x, y, playerField))
                {
                    MarkAroundDestroyedShip(x, y, playerField);
                    chatBox.Items.Add($"Ваш корабль уничтожен!");
                }
            }
            else if (playerField[y, x] == ' ') // Промах
            {
                playerField[y, x] = 'O';
                chatBox.Items.Add($"Противник промахнулся на {coordinates}");
            }

            playerFieldPanel.Invalidate();
            CheckGameOver();
        }

        // Метод для проверки уничтожения корабля
        private bool IsShipDestroyed(int x, int y, char[,] field)
        {
            // Проверяем все клетки вокруг
            for (int i = Math.Max(0, y - 1); i <= Math.Min(9, y + 1); i++)
            {
                for (int j = Math.Max(0, x - 1); j <= Math.Min(9, x + 1); j++)
                {
                    if (field[i, j] == 'S')
                        return false;
                }
            }
            return true;
        }

        // Метод для пометки клеток вокруг уничтоженного корабля
        private void MarkAroundDestroyedShip(int x, int y, char[,] field)
        {
            for (int i = Math.Max(0, y - 1); i <= Math.Min(9, y + 1); i++)
            {
                for (int j = Math.Max(0, x - 1); j <= Math.Min(9, x + 1); j++)
                {
                    if (field[i, j] == ' ')
                    {
                        field[i, j] = 'O';
                    }
                }
            }
        }

        // Метод для обновления поля противника
        private void UpdateEnemyFieldAfterAttack(string coordinates)
        {
            char xChar = coordinates[0];
            int x = xChar - 'A';
            int y = int.Parse(coordinates.Substring(1)) - 1;

            // В реальной игре этот результат будет приходить от сервера
            // Здесь оставим имитацию для демонстрации
            Random rnd = new Random();
            bool isHit = rnd.Next(0, 2) == 1;

            if (isHit)
            {
                enemyField[y, x] = 'X';
                enemyShipsRemaining--;
                chatBox.Items.Add($"Вы попали в корабль противника! Осталось кораблей: {enemyShipsRemaining}");
            }
            else
            {
                enemyField[y, x] = 'O';
                chatBox.Items.Add("Вы промахнулись!");
            }

            enemyFieldPanel.Invalidate();
            CheckGameOver();
        }

        private void chatBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (chatBox.SelectedItem != null)
            {
                string message = chatBox.SelectedItem.ToString();

                if (message.StartsWith("Client Attack!!!:"))
                {
                    string coordinates = message.Substring("Client Attack!!!:".Length).Trim();
                    ProcessIncomingAttack(coordinates);
                }
                else if (message.Contains("Попадание") || message.Contains("Промах"))
                {
                    int start = message.IndexOf("в ") + 2;
                    int end = message.IndexOf("!");
                    if (start >= 0 && end > start)
                    {
                        string coordinates = message.Substring(start, end - start).Trim();
                        UpdateEnemyFieldAfterAttack(coordinates, message.Contains("Попадание"));
                    }
                }
            }
        }
        private void UpdateEnemyFieldAfterAttack(string coordinates, bool isHit)
        {
            char xChar = coordinates[0];
            int x = xChar - 'A';
            int y = int.Parse(coordinates.Substring(1)) - 1;

            if (x >= 0 && x < 10 && y >= 0 && y < 10)
            {
                if (isHit)
                {
                    enemyField[y, x] = 'X';
                    enemyShipsRemaining--;
                    chatBox.Items.Add($"Вы попали в корабль противника на {coordinates}! Осталось кораблей: {enemyShipsRemaining}");
                }
                else
                {
                    enemyField[y, x] = 'O';
                    chatBox.Items.Add($"Вы промахнулись на {coordinates}!");
                }

                enemyFieldPanel.Invalidate();
                CheckGameOver();
            }
        }

        private bool ValidateCoordinates(string coord)
        {
            if (coord.Length < 2 || coord.Length > 3) return false;
            if (coord[0] < 'A' || coord[0] > 'J') return false;
            if (!int.TryParse(coord.Substring(1), out int y)) return false;
            return y >= 1 && y <= 10;
        }
        private void UpdateFields()
        {
            // Здесь можно добавить код для обновления отображения полей
        }

        // DrawField для улучшенного отображения
        private void DrawField(Graphics g, char[,] field, bool showShips, string title)
        {
            if (field == null) return;

            int cellSize = 30;
            int offsetX = 25;
            int offsetY = 60;
            Pen pen = new Pen(Color.Black);
            Font font = new Font("Arial", 10);
            Font titleFont = new Font("Arial", 12, FontStyle.Bold);

            // Рисуем заголовок поля
            g.DrawString(title, titleFont, Brushes.Black,
                field.GetLength(1) * cellSize / 2 - titleFont.Size * title.Length / 4, 5);

            // Рисуем буквы (A-J) по горизонтали
            for (int x = 0; x < field.GetLength(1); x++)
            {
                g.DrawString(((char)('A' + x)).ToString(), font, Brushes.Black,
                    x * cellSize + offsetX + cellSize / 2 - 5, offsetY - 20);
            }

            // Рисуем цифры (1-10) по вертикали
            for (int y = 0; y < field.GetLength(0); y++)
            {
                g.DrawString((y + 1).ToString(), font, Brushes.Black,
                    offsetX - 20, y * cellSize + offsetY + cellSize / 2 - 5);
            }

            // Рисуем сетку
            for (int y = 0; y <= field.GetLength(0); y++)
            {
                g.DrawLine(pen, offsetX, y * cellSize + offsetY,
                    field.GetLength(1) * cellSize + offsetX, y * cellSize + offsetY);
            }

            for (int x = 0; x <= field.GetLength(1); x++)
            {
                g.DrawLine(pen, x * cellSize + offsetX, offsetY,
                    x * cellSize + offsetX, field.GetLength(0) * cellSize + offsetY);
            }

            // Рисуем состояние клеток
            for (int y = 0; y < field.GetLength(0); y++)
            {
                for (int x = 0; x < field.GetLength(1); x++)
                {
                    char cell = field[y, x];
                    int drawX = x * cellSize + offsetX;
                    int drawY = y * cellSize + offsetY;

                    // Вода
                    //g.FillRectangle(Brushes.LightBlue, drawX, drawY, cellSize, cellSize);

                    if (cell == 'X') // Попадание
                    {
                        g.FillEllipse(Brushes.Red, drawX + 5, drawY + 5, 20, 20);
                    }
                    else if (cell == 'O') // Промах
                    {
                        g.DrawEllipse(pen, drawX + 5, drawY + 5, 20, 20);
                    }
                    else if (cell == 'S' && showShips) // Корабль
                    {
                        g.FillRectangle(Brushes.Gray, drawX + 2, drawY + 2, 26, 26);
                    }
                }
            }
        }

        // Добавляем обработчик клика по полю противника для атаки
        private void enemyFieldPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (!gameStarted) return;

            int cellSize = 30;
            int offsetX = 25;
            int offsetY = 60;

            int x = (e.X - offsetX) / cellSize;
            int y = (e.Y - offsetY) / cellSize;

            if (x >= 0 && x < 10 && y >= 0 && y < 10)
            {
                string coordinates = $"{(char)('A' + x)}{y + 1}";
                messageTextBox.Text = coordinates;
                attackButton_Click(null, null);
            }
        }

        // Добавляем обработчик клика по своему полю для размещения кораблей
        private void playerFieldPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (gameStarted) return;

            int cellSize = 30;
            int offsetX = 25;
            int offsetY = 60;

            int x = (e.X - offsetX) / cellSize;
            int y = (e.Y - offsetY) / cellSize;

            if (x >= 0 && x < 10 && y >= 0 && y < 10)
            {
                PlaceShip(x, y);
            }
        }
    }

}