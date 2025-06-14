using NetworkGameForm;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace NetworkGame { 
public partial class OnlineGameForm : Form
{
    private bool isServer; // Сервер или клиент
    private GameServer server; // Объект сервера
    private GameClient client; // Объект клиента

    // Игровые поля (10x10)
    private Button[,] playerGrid; // Ваши корабли
    private Button[,] enemyGrid;  // Корабли противника
    private Button btnRotate;     // Кнопка поворота


        public OnlineGameForm(bool isServer)
        {
            this.isServer = isServer;
            InitializeComponent();
            SetupGame();
        }

        private void SetupGame()
        {
            this.Text = isServer ? "Морской бой - Сервер" : "Морской бой - Клиент";
            this.Size = new Size(800, 600);

            // Инициализация сеток
            playerGrid = new Button[10, 10];
            enemyGrid = new Button[10, 10];

            // Создание поля игрока
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    playerGrid[i, j] = new Button
                    {
                        Size = new Size(30, 30),
                        Location = new Point(50 + j * 32, 50 + i * 32),
                        Tag = new Point(i, j)
                    };
                    this.Controls.Add(playerGrid[i, j]);
                }
            }

            // Создание поля противника
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    enemyGrid[i, j] = new Button
                    {
                        Size = new Size(30, 30),
                        Location = new Point(400 + j * 32, 50 + i * 32),
                        Tag = new Point(i, j)
                    };
                    this.Controls.Add(enemyGrid[i, j]);
                }
            }

            // Инициализация сервера/клиента
            if (isServer)
            {
                server = new GameServer();
                server.Start(8888);
            }
            else
            {
                client = new GameClient();
                client.Connect("127.0.0.1", 8888);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (isServer && server != null)
                server.Stop();

            if (!isServer && client != null)
                client.Disconnect();

            base.OnFormClosing(e);
        }
    }
}