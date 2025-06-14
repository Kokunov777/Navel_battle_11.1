using NetworkGame;
using NetworkGameForm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SeaBattle.NetworkGame
{
    public partial class NetworkGame : Form
    {
        // Кнопки главного меню
        private Button btnCreateGame;
        private Button btnJoinGame;
        private Button btnRotateShips;
        private Button btnExit;
        private bool isServer;

        public NetworkGame()
        {
            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            // Настройки формы
            this.Text = "Морской Бой";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            
            this.Text = "Сетевой Морской бой";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;

                this.Text = "Сетевой Морской бой";
                this.Size = new Size(400, 300);
                this.StartPosition = FormStartPosition.CenterScreen;

                // Кнопка "Создать игру"
                btnCreateGame = new Button
                {
                    Text = "Создать игру (Сервер)",
                    Location = new Point(100, 50),
                    Size = new Size(200, 40)
                };
                btnCreateGame.Click += (s, e) => {
                    isServer = true;
                    new OnlineGameForm(true).Show();
                };

                // Кнопка "Присоединиться"
                btnJoinGame = new Button
                {
                    Text = "Присоединиться (Клиент)",
                    Location = new Point(100, 100),
                    Size = new Size(200, 40)
                };
                btnJoinGame.Click += (s, e) => {
                    isServer = false;
                    new OnlineGameForm(false).Show();
                };

                // Кнопка "Выход"
                btnExit = new Button
                {
                    Text = "Выход",
                    Location = new Point(100, 150),
                    Size = new Size(200, 40)
                };
                btnExit.Click += (s, e) => this.Close();

                // Добавляем кнопки на форму
                this.Controls.Add(btnCreateGame);
            this.Controls.Add(btnJoinGame);
            this.Controls.Add(btnRotateShips);
            this.Controls.Add(btnExit);
        }

        private void BtnCreateGame_Click(object sender, EventArgs e)
        {
            // Запуск в режиме сервера
            var gameForm = new OnlineGameForm(true);
            gameForm.Show();
            this.Hide();
        }

        private void BtnJoinGame_Click(object sender, EventArgs e)
        {
            // Запуск в режиме клиента
            var gameForm = new OnlineGameForm(false);
            gameForm.Show();
            this.Hide();
        }

        private void BtnRotateShips_Click(object sender, EventArgs e)
        {
            // Логика поворота будет в игровой форме
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            // Возврат в главное меню
            this.Close();
        }
    }
}
