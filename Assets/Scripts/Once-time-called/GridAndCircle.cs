using UnityEngine;

public class GridAndCircleGenerator : MonoBehaviour
{
    public LineRenderer linePrefab; // Префаб для линий сетки
    public LineRenderer circleRenderer; // Префаб для круга

    public float gridSize = 10f; // Размер ячейки сетки
    public int gridWidth = 10; // Количество линий по ширине
    public int gridHeight = 10; // Количество линий по высоте
    public float thickerLineInterval = 10f; // Интервал для толстых линий

    public float circleRadius = 50f; // Радиус круга
    public int circleSegments = 100; // Количество сегментов для круга

    private void Start()
    {
        GenerateGrid();
        GenerateCircle();
    }

    // Генерация сетки
    void GenerateGrid()
    {
        for (int i = 0; i <= gridWidth; i++)
        {
            CreateLine(new Vector3(i * gridSize - gridWidth * gridSize / 2, -gridHeight * gridSize / 2, 1f),
                       new Vector3(i * gridSize - gridWidth * gridSize / 2, gridHeight * gridSize / 2, 1f),
                       i % thickerLineInterval == 0);
        }

        for (int j = 0; j <= gridHeight; j++)
        {
            CreateLine(new Vector3(-gridWidth * gridSize / 2, j * gridSize - gridHeight * gridSize / 2, 1f),
                       new Vector3(gridWidth * gridSize / 2, j * gridSize - gridHeight * gridSize / 2, 1f),
                       j % thickerLineInterval == 0);
        }
    }
    void CreateLine(Vector3 start, Vector3 end, bool isThicker)
    {
        LineRenderer line = Instantiate(linePrefab, transform);
        line.SetPosition(0, start);
        line.SetPosition(1, end);
        line.startWidth = isThicker ? 0.1f : 0.05f;
        line.endWidth = isThicker ? 0.1f : 0.05f;
    }

    void GenerateCircle()
    {
        LineRenderer circle = Instantiate(circleRenderer, transform);
        circle.positionCount = circleSegments + 1;
        circle.useWorldSpace = false;

        float angleStep = 360f / circleSegments;

        for (int i = 0; i <= circleSegments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * circleRadius;
            float y = Mathf.Sin(angle) * circleRadius;
            circle.SetPosition(i, new Vector3(x, y, 1f));
        }

        circle.startWidth = 0.5f;
        circle.endWidth = 0.5f;
    }
}
