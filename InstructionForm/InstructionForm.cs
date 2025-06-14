using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SeaBattle.Instruction
{
    public partial class InstructionForm : Form
    {
        public InstructionForm()
        {
            InitializeComponent();
            SetupInstructionForm();
        }

        private void SetupInstructionForm()
        {
            // Настройка формы
            this.Text = "Инструкция к игре Морской Бой";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Создание элементов управления
            Label titleLabel = new Label
            {
                Text = "Как играть в Морской Бой",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.Navy,
                AutoSize = true,
                Location = new Point(20, 20)
            };

            TextBox instructionText = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new Point(20, 60),
                Size = new Size(540, 350),
                Text = GetInstructionText(),
                Font = new Font("Arial", 11),
                BackColor = SystemColors.Window
            };

            Button closeButton = new Button
            {
                Text = "Закрыть",
                Size = new Size(100, 40),
                Location = new Point(240, 420),
                DialogResult = DialogResult.OK
            };

            // Добавление элементов на форму
            this.Controls.Add(titleLabel);
            this.Controls.Add(instructionText);
            this.Controls.Add(closeButton);

            // Назначение кнопки закрытия как AcceptButton
            this.AcceptButton = closeButton;
        }

        private string GetInstructionText()
        {
            return @"1. Цель игры:
   - Первым потопить все корабли противника

2. Подготовка к игре:
   - Каждый игрок расставляет свои корабли:
     • 1 корабль — 4 клетки
     • 2 корабля — 3 клетки
     • 3 корабля — 2 клетки
     • 4 корабля — 1 клетка
   - Корабли не могут соприкасаться
   - Используйте кнопку 'Повернуть' для изменения ориентации

3. Ход игры:
   - Игроки ходят по очередиы
   - Выбирайте клетку на поле противника для атаки
   - Попадание отмечается красным, промах - белым
   - Потопленный корабль отмечается полностью

4. Сетевая игра:
   - Создатель игры становится сервером
   - Второй игрок подключается как клиент
   - Укажите правильный IP-адрес и порт

5. Управление:
   - ЛКМ - выбор клетки/расстановка кораблей
   - Повернуть - изменить ориентацию корабля
   - Начать игру - после расстановки всех кораблей";
        }
    }
}

