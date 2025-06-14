using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;


namespace SeaBattle
{
    public partial class MainForm : Form
    {
        public MainForm()
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

            // Относительный путь - изображение будет искаться в папке с exe-файлом
            string imagePath = Path.Combine(Application.StartupPath, "images", "karabal.jpg");

            

            try
            {
                if (File.Exists(imagePath))
                {
                    this.BackgroundImage = Image.FromFile(imagePath);
                    this.BackgroundImageLayout = ImageLayout.Stretch;
                }
                else
                {
                    // Обработка случая, когда изображение отсутствует
                    this.BackColor = Color.FromArgb(0, 0, 64); // Темно-синий фон
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибок загрузки изображения
                this.BackColor = Color.FromArgb(0, 0, 64);
                Debug.WriteLine($"Ошибка загрузки фонового изображения: {ex.Message}");
            }

            // Создание панели для кнопок (для лучшего визуального отображения)
            Panel buttonPanel = new Panel();
            buttonPanel.BackColor = Color.FromArgb(150, 0, 0, 50); // Полупрозрачный темно-синий
            buttonPanel.Size = new Size(300, 400);
            buttonPanel.Location = new Point(
                (this.ClientSize.Width - buttonPanel.Width) / 2,
                (this.ClientSize.Height - buttonPanel.Height) / 2);

            // Создание заголовка 
            Label title = new Label();
            title.Text = "МОРСКОЙ БОЙ";
            title.Font = new Font("Arial", 24, FontStyle.Bold);
            title.ForeColor = Color.White;
            title.AutoSize = false;
            title.Size = new Size(buttonPanel.Width, 50);
            title.TextAlign = ContentAlignment.MiddleCenter;
            title.Location = new Point(0, 20);

            // Создание кнопок
            Button[] buttons = new Button[5];
            string[] buttonTexts = {
                "Одиночная игра",
                "Игра по сети",
                "Авторы",
                "Инструкция",
                "Выход"
            };

            int yPos = 80;
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i] = new Button();
                buttons[i].Text = buttonTexts[i];
                buttons[i].Size = new Size(250, 40);
                buttons[i].Location = new Point(
                    (buttonPanel.Width - buttons[i].Width) / 2,
                    yPos);
                buttons[i].Font = new Font("Arial", 12);
                buttons[i].BackColor = Color.Navy;
                buttons[i].ForeColor = Color.White;
                buttons[i].FlatStyle = FlatStyle.Flat;
                buttons[i].FlatAppearance.BorderSize = 1;
                buttons[i].FlatAppearance.BorderColor = Color.White;

                // Подписка на события
                if (i == 5) // Кнопка "Выход"
                    buttons[i].Click += ExitButton_Click;
                else
                    buttons[i].Click += MenuButton_Click;

                yPos += 50;
            }

            // Добавление элементов на панель
            buttonPanel.Controls.Add(title);
            foreach (Button btn in buttons)
            {
                buttonPanel.Controls.Add(btn);
            }

            // Добавление панели на форму
            this.Controls.Add(buttonPanel);
        }

       

        private void MenuButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;

            switch (clickedButton.Text)
            {

                case "Одиночная игра":

                    var singleGameForm = new SeaBattle.SingleGame.SingleGame();
                    singleGameForm.ShowDialog();
                    break;

                case "Игра по сети":
                    
                    var clientServer = new SeaBattle.ClientServerForm.ClientServer();
                    clientServer.ShowDialog();
                    break;


                case "Авторы":
                    
                    MessageBox.Show("Разработчики:\nЗоркольцев Илья\nКокунов Андрей", "Авторы");
                    break;

                case "Инструкция":
                   
                    var instructionForm = new SeaBattle.Instruction.InstructionForm();
                    instructionForm.ShowDialog();
                    break;
                case "Выход":
                    Application.Exit();
                    break;
            }
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
           
            Application.Exit();
            
        }
    }
}