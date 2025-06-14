using System.Windows.Forms;

namespace SeaBattle.ClientServerForm
{
    partial class ClientServer
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.messageTextBox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.chatBox = new System.Windows.Forms.ListBox();
            this.ipTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.playerFieldPanel = new System.Windows.Forms.Panel();
            this.enemyFieldPanel = new System.Windows.Forms.Panel();
            this.playerFieldPanel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.playerFieldPanel_MouseClick);
            this.enemyFieldPanel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.enemyFieldPanel_MouseClick);
            this.chatBox.SelectedIndexChanged += new System.EventHandler(this.chatBox_SelectedIndexChanged);
            this.SuspendLayout();
            // 
            // messageTextBox
            // 
            this.messageTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.messageTextBox.Location = new System.Drawing.Point(52, 52);
            this.messageTextBox.Name = "messageTextBox";
            this.messageTextBox.Size = new System.Drawing.Size(150, 44);
            this.messageTextBox.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(208, 52);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(163, 44);
            this.button1.TabIndex = 4;
            this.button1.Text = "Атаковать";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // chatBox
            // 
            this.chatBox.ItemHeight = 20;
            this.chatBox.Location = new System.Drawing.Point(52, 150);
            this.chatBox.Name = "chatBox";
            this.chatBox.Size = new System.Drawing.Size(319, 184);
            this.chatBox.TabIndex = 3;
            // 
            // ipTextBox
            // 
            this.ipTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ipTextBox.Location = new System.Drawing.Point(52, 400);
            this.ipTextBox.Name = "ipTextBox";
            this.ipTextBox.Size = new System.Drawing.Size(225, 53);
            this.ipTextBox.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(52, 377);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "IP противника:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(52, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(221, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "Координаты атаки (A1-J10):";
            // 
            // playerFieldPanel
            // 
            this.playerFieldPanel.BackColor = System.Drawing.Color.White;
            this.playerFieldPanel.Location = new System.Drawing.Point(400, 50);
            this.playerFieldPanel.Name = "playerFieldPanel";
            this.playerFieldPanel.Size = new System.Drawing.Size(360, 376);
            this.playerFieldPanel.TabIndex = 6;
            this.playerFieldPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.DrawPlayerField);
            // 
            // enemyFieldPanel
            // 
            this.enemyFieldPanel.BackColor = System.Drawing.Color.White;
            this.enemyFieldPanel.Location = new System.Drawing.Point(754, 50);
            this.enemyFieldPanel.Name = "enemyFieldPanel";
            this.enemyFieldPanel.Size = new System.Drawing.Size(360, 376);
            this.enemyFieldPanel.TabIndex = 5;
            this.enemyFieldPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.DrawEnemyField);
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(1153, 500);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ipTextBox);
            this.Controls.Add(this.chatBox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.messageTextBox);
            this.Controls.Add(this.enemyFieldPanel);
            this.Controls.Add(this.playerFieldPanel);
            this.Name = "Form1";
            this.Text = "Морской бой";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private void DrawPlayerField(object sender, PaintEventArgs e)
        {
            DrawField(e.Graphics, playerField, true, "Ваше поле");
            /*// Отрисовка поля игрока
            Graphics g = e.Graphics;
            DrawField(g, playerField, true);*/
        }

        private void DrawEnemyField(object sender, PaintEventArgs e)
        {
            DrawField(e.Graphics, enemyField, false, "Поле противника");
            /*// Отрисовка поля противника
            Graphics g = e.Graphics;
            DrawField(g, enemyField, false);*/
        }



        #endregion

        private System.Windows.Forms.TextBox messageTextBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox chatBox;
        private System.Windows.Forms.TextBox ipTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private Panel playerFieldPanel;
        private Panel enemyFieldPanel;
        private string title;
    }
}