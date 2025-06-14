using System.Drawing;
using System.Windows.Forms;
using System;

public class GameField : Panel
{
    public int[,] FieldData { get; set; }
    public bool ShowShips { get; set; }
    public bool IsInteractive { get; set; }

    // Для подсветки размещаемого корабля
    private int highlightX = -1;
    private int highlightY = -1;
    private int highlightSize = 0;
    private bool highlightHorizontal = true;
    private bool highlightValid = false;

    public event EventHandler<CellClickEventArgs> OnCellClicked;
    public event EventHandler<CellMouseEventArgs> OnCellMouseMove;

    public GameField()
    {
        FieldData = new int[10, 10];
        this.Size = new Size(300, 300);
        this.BorderStyle = BorderStyle.FixedSingle;
        this.DoubleBuffered = true;

        this.MouseClick += GameField_MouseClick;
        this.MouseMove += GameField_MouseMove;
        this.Paint += GameField_Paint;
    }

    public void HighlightShip(int x, int y, int size, bool isHorizontal, bool valid)
    {
        highlightX = x;
        highlightY = y;
        highlightSize = size;
        highlightHorizontal = isHorizontal;
        highlightValid = valid;
        this.Invalidate();
    }

    private void GameField_MouseClick(object sender, MouseEventArgs e)
    {
        if (IsInteractive)
        {
            int cellSize = this.Width / 10;
            int x = e.X / cellSize;
            int y = e.Y / cellSize;

            if (x >= 0 && x < 10 && y >= 0 && y < 10)
            {
                OnCellClicked?.Invoke(this, new CellClickEventArgs(x, y));
            }
        }
    }

    private void GameField_MouseMove(object sender, MouseEventArgs e)
    {
        if (IsInteractive)
        {
            int cellSize = this.Width / 10;
            int x = e.X / cellSize;
            int y = e.Y / cellSize;

            if (x >= 0 && x < 10 && y >= 0 && y < 10)
            {
                OnCellMouseMove?.Invoke(this, new CellMouseEventArgs(x, y));
            }
        }
    }

    private void GameField_Paint(object sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        int cellSize = Math.Min(this.Width / 10, this.Height / 10);

        // Рисуем сетку
        for (int i = 0; i <= 10; i++)
        {
            g.DrawLine(Pens.Black, 0, i * cellSize, 10 * cellSize, i * cellSize);
            g.DrawLine(Pens.Black, i * cellSize, 0, i * cellSize, 10 * cellSize);
        }

        // Рисуем содержимое клеток
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                Rectangle cell = new Rectangle(x * cellSize, y * cellSize, cellSize, cellSize);

                if (FieldData[x, y] == 2) // Промах
                {
                    g.FillEllipse(Brushes.Blue, cell);
                }
                else if (FieldData[x, y] == 3) // Попадание
                {
                    g.FillRectangle(Brushes.Red, cell);
                }

                // Корабли показываем только если разрешено
                if (FieldData[x, y] == 1 && ShowShips)
                {
                    g.FillRectangle(Brushes.DarkGray, cell);
                }
            }
        }

        // Подсветка недоступных для выстрела клеток
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                Rectangle cell = new Rectangle(x * cellSize, y * cellSize, cellSize, cellSize);

                if (FieldData[x, y] == 2 || FieldData[x, y] == 3)
                {
                    using (var brush = new SolidBrush(Color.FromArgb(30, Color.Black)))
                    {
                        g.FillRectangle(brush, cell);
                    }
                }
            }
        }
        // Рисуем подсветку для размещаемого корабля
        if (highlightSize > 0)
        {
            using (var brush = new SolidBrush(highlightValid ?
                  Color.FromArgb(100, Color.Green) : Color.FromArgb(100, Color.Red)))
            {
                for (int i = 0; i < highlightSize; i++)
                {
                    int drawX = highlightHorizontal ? highlightX + i : highlightX;
                    int drawY = highlightHorizontal ? highlightY : highlightY + i;

                    if (drawX < 10 && drawY < 10)
                    {
                        var rect = new Rectangle(
                            drawX * cellSize,
                            drawY * cellSize,
                            cellSize,
                            cellSize);
                        g.FillRectangle(brush, rect);
                    }
                }
            }
        }
    }
}

public class CellClickEventArgs : EventArgs
{
    public int X { get; }
    public int Y { get; }

    public CellClickEventArgs(int x, int y)
    {
        X = x;
        Y = y;
    }
}

public class CellMouseEventArgs : EventArgs
{
    public int X { get; }
    public int Y { get; }

    public CellMouseEventArgs(int x, int y)
    {
        X = x;
        Y = y;
    }
}