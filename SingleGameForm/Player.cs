using SeaBattle.SingleGame;
using System.Collections.Generic;
using System;
using SingleGameForm;

public class Player
{
    public string Name { get; }
    public int[,] Field { get; }
    public List<Ship> Ships { get; }

    public Player(string name)
    {
        Name = name;
        Field = new int[10, 10];
        Ships = new List<Ship>();
    }

    public bool PlaceShipManual(int x, int y, int size, bool isHorizontal)
    {
        if (!CanPlaceShip(x, y, size, isHorizontal))
            return false;

        // Размещаем корабль
        for (int i = 0; i < size; i++)
        {
            int posX = isHorizontal ? x + i : x;
            int posY = isHorizontal ? y : y + i;
            Field[posX, posY] = 1; // 1 - клетка с кораблем
        }

        Ships.Add(new Ship(x, y, size, isHorizontal));
        return true;
    }

    public bool CanPlaceShip(int x, int y, int size, bool isHorizontal)
    {
        // Проверка выхода за границы
        if (x < 0 || y < 0) return false;
        if (isHorizontal && x + size > 10) return false;
        if (!isHorizontal && y + size > 10) return false;

        // Проверка соседних клеток
        for (int i = -1; i <= size; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int checkX = isHorizontal ? x + i : x + j;
                int checkY = isHorizontal ? y + j : y + i;

                if (checkX >= 0 && checkX < 10 && checkY >= 0 && checkY < 10)
                {
                    if (Field[checkX, checkY] != 0)
                        return false;
                }
            }
        }
        return true;
    }

    public void ClearField()
    {
        Array.Clear(Field, 0, Field.Length);
        Ships.Clear();
    }
}