using SingleGameForm;
using System;
using System.Collections.Generic;
using System.Linq;

public class Bot
{
    public int[,] Field { get; }
    public List<Ship> Ships { get; }
    private Random random;

    private List<(int x, int y)> potentialTargets;
    private List<(int x, int y)> hitCells;
    private (int x, int y)? lastHit;
    private bool isHuntingMode;
    private int[,] probabilityMap;

    public Bot()
    {
        Field = new int[10, 10];
        Ships = new List<Ship>();
        random = new Random();
        potentialTargets = new List<(int x, int y)>();
        hitCells = new List<(int x, int y)>();
        probabilityMap = new int[10, 10];
        PlaceShipsRandom();
    }

    private void PlaceShipsRandom()
    {
        int[] shipSizes = { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };

        foreach (int size in shipSizes)
        {
            bool placed = false;
            int attempts = 0;

            while (!placed && attempts < 100)
            {
                int x = random.Next(0, 10);
                int y = random.Next(0, 10);
                bool isHorizontal = random.Next(0, 2) == 0;

                if (CanPlaceShip(x, y, size, isHorizontal))
                {
                    PlaceShip(x, y, size, isHorizontal);
                    placed = true;
                }
                attempts++;
            }
        }
    }

    private bool CanPlaceShip(int x, int y, int size, bool isHorizontal)
    {
        if (x < 0 || y < 0) return false;
        if (isHorizontal && x + size > 10) return false;
        if (!isHorizontal && y + size > 10) return false;

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

    private void PlaceShip(int x, int y, int size, bool isHorizontal)
    {
        for (int i = 0; i < size; i++)
        {
            int posX = isHorizontal ? x + i : x;
            int posY = isHorizontal ? y : y + i;
            Field[posX, posY] = 1;
        }
        Ships.Add(new Ship(x, y, size, isHorizontal));
    }

    public (int x, int y) MakeMove(int[,] playerField)
    {
        UpdateProbabilityMap(playerField);

        // Если есть недобитые попадания - сначала добиваем их
        if (hitCells.Count > 0)
        {
            return GetNextTarget();
        }

        // Если в предыдущем ходе было попадание - переходим в режим охоты
        if (lastHit.HasValue)
        {
            var target = Hunt(playerField);
            if (target.x != -1) return target;
        }

        // Режим поиска - выбираем клетку с максимальной вероятностью
        return Search(playerField);
    }

    private (int x, int y) GetNextTarget()
    {
        // Берем первую клетку из списка недобитых попаданий
        var target = hitCells[0];
        hitCells.RemoveAt(0);
        return target;
    }

    private (int x, int y) Hunt(int[,] playerField)
    {
        var (x, y) = lastHit.Value;

        // Проверяем 4 направления вокруг последнего попадания
        var directions = new (int dx, int dy)[] { (0, 1), (1, 0), (0, -1), (-1, 0) };

        // Перемешиваем направления для более естественного поведения
        directions = directions.OrderBy(d => random.Next()).ToArray();

        foreach (var dir in directions)
        {
            int nx = x + dir.dx;
            int ny = y + dir.dy;

            if (nx >= 0 && nx < 10 && ny >= 0 && ny < 10 && playerField[nx, ny] == 0)
            {
                return (nx, ny);
            }
        }

        // Если все направления проверены - сбрасываем режим охоты
        lastHit = null;
        isHuntingMode = false;
        return (-1, -1);
    }

    private (int x, int y) Search(int[,] playerField)
    {
        List<(int x, int y)> bestTargets = new List<(int x, int y)>();
        int maxProbability = 0;

        // Находим клетки с максимальной вероятностью
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (playerField[x, y] == 0)
                {
                    if (probabilityMap[x, y] > maxProbability)
                    {
                        maxProbability = probabilityMap[x, y];
                        bestTargets.Clear();
                        bestTargets.Add((x, y));
                    }
                    else if (probabilityMap[x, y] == maxProbability)
                    {
                        bestTargets.Add((x, y));
                    }
                }
            }
        }

        // Если нашли подходящие клетки - выбираем случайную
        if (bestTargets.Count > 0)
        {
            return bestTargets[random.Next(bestTargets.Count)];
        }

        // Если все клетки уже обстреляны (теоретически невозможно)
        return (-1, -1);
    }

    private void UpdateProbabilityMap(int[,] playerField)
    {
        Array.Clear(probabilityMap, 0, probabilityMap.Length);

        // Перебираем все возможные размещения кораблей
        foreach (var ship in Ships)
        {
            if (ship.IsSunk) continue;

            // Проверяем горизонтальные размещения
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    if (CanPlaceShipOnProbabilityMap(x, y, ship.Size, true, playerField))
                    {
                        for (int i = 0; i < ship.Size; i++)
                        {
                            if (x + i < 10)
                                probabilityMap[x + i, y]++;
                        }
                    }
                }
            }

            // Проверяем вертикальные размещения
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    if (CanPlaceShipOnProbabilityMap(x, y, ship.Size, false, playerField))
                    {
                        for (int i = 0; i < ship.Size; i++)
                        {
                            if (y + i < 10)
                                probabilityMap[x, y + i]++;
                        }
                    }
                }
            }
        }
    }

    private bool CanPlaceShipOnProbabilityMap(int x, int y, int size, bool isHorizontal, int[,] playerField)
    {
        if (x < 0 || y < 0) return false;
        if (isHorizontal && x + size > 10) return false;
        if (!isHorizontal && y + size > 10) return false;

        for (int i = 0; i < size; i++)
        {
            int checkX = isHorizontal ? x + i : x;
            int checkY = isHorizontal ? y : y + i;

            if (playerField[checkX, checkY] != 0) // Уже стреляли сюда
                return false;
        }

        return true;
    }

    public void RecordHit(int x, int y, bool isHit, bool isShipDestroyed)
    {
        if (isHit)
        {
            lastHit = (x, y);
            isHuntingMode = true;

            // Добавляем соседние клетки для добивания
            var directions = new (int dx, int dy)[] { (0, 1), (1, 0), (0, -1), (-1, 0) };
            foreach (var dir in directions)
            {
                int nx = x + dir.dx;
                int ny = y + dir.dy;
                if (nx >= 0 && nx < 10 && ny >= 0 && ny < 10)
                {
                    hitCells.Add((nx, ny));
                }
            }

            if (isShipDestroyed)
            {
                hitCells.Clear();
                lastHit = null;
                isHuntingMode = false;
            }
        }
    }
}