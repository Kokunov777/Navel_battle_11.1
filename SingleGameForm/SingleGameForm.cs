using SingleGameForm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SeaBattle.SingleGame
{
    public partial class SingleGame : Form
    {
        private Player player;
        private Bot bot;
        private GameField playerField;
        private GameField botField;
        private bool isGameStarted = false;
        private Ship currentShip = null;
        private int[] shipsToPlace = { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };
        private int currentShipIndex = 0;
        private Button rotateButton;
        private Label statusLabel;
        private Button stopButton;
        private Button restartButton;
        private Button mainMenuButton;

        public SingleGame()
        {
            InitializeComponent();
            InitializeGame();
            SetupUI();
            StartManualPlacement();
        }

        private void InitializeGame()
        {
            player = new Player("Игрок");
            bot = new Bot();
        }

        private void SetupUI()
        {

            // Кнопка с иконкой
            mainMenuButton = new Button()
            {
                Text = " Меню", // Пробел для отступа от иконки
                Size = new Size(100, 30),
                Location = new Point(10, 10),
                Font = new Font("Arial", 9),
                //  Image = Properties.Resources.home_icon, // Добавьте иконку в ресурсы
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent
            };

            this.Text = "Морской бой - Расстановка кораблей";
            this.Size = new Size(800, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            // Создание статусной строки
            statusLabel = new Label()
            {
                Location = new Point(50, 520),
                Size = new Size(700, 30),
                Font = new Font("Arial", 12),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Кнопка поворота
            rotateButton = new Button()
            {
                Text = "Повернуть корабль (R)",
                Location = new Point(300, 560),
                Size = new Size(200, 30),
                Font = new Font("Arial", 10)
            };
            rotateButton.Click += RotateButton_Click;

            // Создание игровых полей
            playerField = new GameField()
            {
                Location = new Point(50, 50),
                ShowShips = true,
                IsInteractive = true,
                BackColor = Color.Azure
            };

            botField = new GameField()
            {
                Location = new Point(450, 50),
                ShowShips = false,
                IsInteractive = false,
                BackColor = Color.Azure
            };

            // Добавление элементов на форму
            this.Controls.Add(playerField);
            this.Controls.Add(botField);
            this.Controls.Add(statusLabel);
            this.Controls.Add(rotateButton);

            // Обработчики событий
            playerField.OnCellClicked += PlayerFieldCell_Click;
            playerField.OnCellMouseMove += PlayerFieldCell_MouseMove;
            this.KeyPreview = true;
            this.KeyDown += SingleGameForm_KeyDown;

            // Кнопка "Главное меню" в левом верхнем углу
            mainMenuButton = new Button()
            {
                Text = "Главное меню",
                Size = new Size(120, 30),
                Location = new Point(10, 10), // Левый верхний угол (отступ 10px)
                Font = new Font("Arial", 9),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };
            mainMenuButton.Click += MainMenuButton_Click;
            this.Controls.Add(mainMenuButton);

            // Размеры и стиль кнопок
            Size buttonSize = new Size(150, 40);
            Font buttonFont = new Font("Arial", 10);



            // Кнопка "Перезагрузка игры" 
            restartButton = new Button()
            {
                Text = "Перезагрузка игры",
                Size = buttonSize,
                Location = new Point(this.ClientSize.Width - 200, 560),  // Правая позиция
                Font = buttonFont,
                BackColor = Color.LightGreen
            };
            restartButton.Click += RestartButton_Click;

            // Добавляем кнопки на форму
            this.Controls.Add(stopButton);
            this.Controls.Add(restartButton);

            // Центральная кнопка (если есть)
            if (rotateButton != null)
            {
                rotateButton.Location = new Point(
                    (this.ClientSize.Width - rotateButton.Width) / 2,
                    560);
            }
        }

        private void MainMenuButton_Click(object sender, EventArgs e)
        {
            if (isGameStarted)
            {
                var result = MessageBox.Show("Выйти в главное меню? Текущая игра будет завершена.",
                                           "Подтверждение",
                                           MessageBoxButtons.YesNo,
                                           MessageBoxIcon.Question);

                if (result == DialogResult.No)
                    return;
            }

            // Закрываем текущую форму
            this.DialogResult = DialogResult.Abort;
            this.Close();
        }

        private void ResetGame()
        {
            // Полный сброс состояния игры
            isGameStarted = false;
            currentShipIndex = 0;
            currentShip = null;

            // Переинициализируем игроков
            player = new Player("Игрок");
            bot = new Bot();

            // Очищаем поля
            playerField.FieldData = new int[10, 10];
            botField.FieldData = new int[10, 10];

            // Сбрасываем подсветку
            playerField.HighlightShip(-1, -1, 0, true, false);

            // Обновляем интерактивность
            playerField.IsInteractive = true;
            botField.IsInteractive = false;

            // Обновляем UI
            this.Text = "Морской бой - Расстановка кораблей";
            statusLabel.Text = "Разместите ваши корабли";

            rotateButton.Visible = true;

            // Перерисовываем поля
            playerField.Invalidate();
            botField.Invalidate();

            // Удаляем старые обработчики событий
            botField.OnCellClicked -= BotFieldCell_Click;

            // Добавляем обработчики для новой игры
            playerField.OnCellClicked += PlayerFieldCell_Click;
            playerField.OnCellMouseMove += PlayerFieldCell_MouseMove;

        }

        private void RestartButton_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Начать новую игру? Текущий прогресс будет потерян.",
                                       "Новая игра",
                                       MessageBoxButtons.YesNo,
                                       MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                ResetGame();
                StartManualPlacement();

                // Явно обновляем данные полей
                playerField.FieldData = player.Field;
                botField.FieldData = bot.Field;

                playerField.Invalidate();
                botField.Invalidate();
            }
        }

        private void StartManualPlacement()
        {
            // Сбрасываем все предыдущие состояния
            currentShipIndex = 0;
            UpdateShipToPlace();
            playerField.IsInteractive = true;
            botField.IsInteractive = false;
            UpdateStatusText();

            // Очищаем поле игрока
            player.ClearField();
            playerField.FieldData = new int[10, 10];
            playerField.Invalidate();
        }

        private void UpdateShipToPlace()
        {
            if (currentShipIndex < shipsToPlace.Length)
            {
                currentShip = new Ship(0, 0, shipsToPlace[currentShipIndex], true);
            }
            else
            {
                currentShip = null;
                StartGame();
            }
        }

        private void UpdateStatusText()
        {
            if (currentShip != null)
            {
                statusLabel.Text = $"Разместите {shipsToPlace[currentShipIndex]}-палубный корабль. " +
                                 $"Осталось: {shipsToPlace.Length - currentShipIndex - 1}";
            }
        }


        private void PlayerFieldCell_MouseMove(object sender, CellMouseEventArgs e)
        {
            if (!isGameStarted && currentShip != null)
            {
                playerField.HighlightShip(e.X, e.Y, currentShip.Size, currentShip.IsHorizontal,
                    player.CanPlaceShip(e.X, e.Y, currentShip.Size, currentShip.IsHorizontal));
            }
        }

        private void PlayerFieldCell_Click(object sender, CellClickEventArgs e)
        {
            if (!isGameStarted && currentShip != null)
            {
                if (player.PlaceShipManual(e.X, e.Y, currentShip.Size, currentShip.IsHorizontal))
                {
                    playerField.FieldData = player.Field;
                    playerField.Invalidate();
                    currentShipIndex++;
                    UpdateShipToPlace();
                    UpdateStatusText();
                }
                else
                {
                    MessageBox.Show("Здесь нельзя разместить корабль!\nКорабли не могут соприкасаться.",
                        "Ошибка размещения");
                }
            }
        }

        private void RotateButton_Click(object sender, EventArgs e)
        {
            if (currentShip != null)
            {
                currentShip.IsHorizontal = !currentShip.IsHorizontal;
                playerField.Invalidate();
            }
        }

        private void SingleGameForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.R)
            {
                RotateButton_Click(null, null);
            }
        }

        private void StartGame()
        {
            // Проверяем, что все корабли расставлены
            if (currentShipIndex < shipsToPlace.Length)
            {
                MessageBox.Show("Расставьте все корабли перед началом игры!", "Не готово");
                return;
            }

            isGameStarted = true;
            playerField.IsInteractive = false;
            botField.IsInteractive = true;
            this.Text = "Морской бой - Игра";
            statusLabel.Text = "Игра началась! Ваш ход.";
            rotateButton.Visible = false;


            // Удаляем обработчики расстановки
            playerField.OnCellClicked -= PlayerFieldCell_Click;
            playerField.OnCellMouseMove -= PlayerFieldCell_MouseMove;

            // Добавляем обработчик выстрелов
            botField.OnCellClicked += BotFieldCell_Click;

            // Обновляем данные полей
            playerField.FieldData = player.Field;
            botField.FieldData = bot.Field;

            playerField.Invalidate();
            botField.Invalidate();

        }

        private void BotFieldCell_Click(object sender, CellClickEventArgs e)
        {

            // Проверяем, что игра начата и не в режиме расстановки
            if (!isGameStarted || currentShip != null)
            {
                MessageBox.Show("Сначала завершите расстановку кораблей!", "Не готово");
                return;
            }

            // Проверяем, не стреляли ли уже в эту клетку
            if (bot.Field[e.X, e.Y] == 2 || bot.Field[e.X, e.Y] == 3)
            {
                MessageBox.Show("Вы уже стреляли в эту клетку!", "Ошибка");
                return;
            }

            if (isGameStarted)
            {
                // Проверяем, не стреляли ли уже в эту клетку
                if (bot.Field[e.X, e.Y] == 2 || bot.Field[e.X, e.Y] == 3)
                {
                    MessageBox.Show("Вы уже стреляли в эту клетку!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Обработка хода игрока
                if (bot.Field[e.X, e.Y] == 1) // Попадание
                {
                    bot.Field[e.X, e.Y] = 3;
                    botField.FieldData[e.X, e.Y] = 3;
                    statusLabel.Text = "Попадание! Стреляйте ещё.";

                    // Проверка потопления
                    if (CheckShipSunk(bot, e.X, e.Y))
                    {
                        statusLabel.Text = "Корабль потоплен! Ваш ход.";
                    }

                    // Проверка победы
                    if (CheckWin(bot))
                    {
                        MessageBox.Show("Поздравляем! Вы победили!", "Победа");
                        this.Close();
                        return;
                    }
                }
                else // Промах
                {
                    bot.Field[e.X, e.Y] = 2;
                    botField.FieldData[e.X, e.Y] = 2;
                    statusLabel.Text = "Промах! Ход противника.";
                    botField.Invalidate();
                    BotMove();
                }
            }
        }

        private void BotMove()
        {
            var (x, y) = bot.MakeMove(player.Field);

            if (player.Field[x, y] == 1) // Попадание бота
            {
                player.Field[x, y] = 3;
                playerField.FieldData[x, y] = 3;

                if (CheckShipSunk(player, x, y))
                {
                    statusLabel.Text = "Противник потопил ваш корабль! Его ход.";
                }
                else
                {
                    statusLabel.Text = "Противник попал! Его ход продолжается.";
                }

                if (CheckWin(player))
                {
                    MessageBox.Show("К сожалению, вы проиграли...", "Поражение");
                    this.Close();
                    return;
                }

                BotMove(); // Бот ходит ещё раз после попадания
            }
            else // Промах бота
            {
                player.Field[x, y] = 2;
                playerField.FieldData[x, y] = 2;
                statusLabel.Text = "Противник промахнулся! Ваш ход.";
                playerField.Invalidate();
            }
        }

        private bool CheckWin(Player target)
        {
            foreach (var ship in target.Ships)
            {
                if (!ship.IsSunk) return false;
            }
            return true;
        }

        private bool CheckWin(Bot target)
        {
            foreach (var ship in target.Ships)
            {
                if (!ship.IsSunk) return false;
            }
            return true;
        }

        private bool CheckShipSunk(Player target, int x, int y)
        {
            foreach (var ship in target.Ships)
            {
                if ((ship.IsHorizontal && y == ship.Y && x >= ship.X && x < ship.X + ship.Size) ||
                    (!ship.IsHorizontal && x == ship.X && y >= ship.Y && y < ship.Y + ship.Size))
                {
                    ship.Hits++;
                    return ship.IsSunk;
                }
            }
            return false;
        }

        private bool CheckShipSunk(Bot target, int x, int y)
        {
            foreach (var ship in target.Ships)
            {
                if ((ship.IsHorizontal && y == ship.Y && x >= ship.X && x < ship.X + ship.Size) ||
                    (!ship.IsHorizontal && x == ship.X && y >= ship.Y && y < ship.Y + ship.Size))
                {
                    ship.Hits++;
                    return ship.IsSunk;
                }
            }
            return false;
        }
    }
}
