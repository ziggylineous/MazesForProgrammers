using UnityEngine;
using System.Collections;

public class PolarGridImage
{
    private int cellHeight;
    private Color backgroundColor;
    private Color wallColor;

    private Texture2D tex;

    public PolarGridImage(int cellHeight, Color backgroundColor, Color wallColor)
    {
        this.cellHeight = cellHeight;
        this.wallColor = wallColor;
        this.backgroundColor = backgroundColor;

        tex = new Texture2D(100, 100);
    }

    public void Draw(PolarGrid maze, Color[] distances = null)
    {
        int size = cellHeight * maze.RowCount * 2;
        tex.Resize(size, size);


        if (distances != null)
            PaintDistances(maze, distances);
        else
            tex.Fill(backgroundColor);

        DrawOutsideWalls(maze);
        DrawWalls(maze);
        tex.Apply();
    }

    private void DrawOutsideWalls(PolarGrid polarGrid)
    {
        Vector2Int center = Center;

        int rowLength = polarGrid.RowLength(polarGrid.RowCount - 1) * 2;

        float theta = Mathf.PI * 2.0f / (float)rowLength;
        float inRadius = cellHeight * polarGrid.RowCount;

        Vector2 cwDir = Vector2.right;
        Vector2 ccwDir = Vector2.right;
        float accumAngle = 0.0f;

        for (int j = 0; j != rowLength; ++j)
        {
            cwDir = ccwDir;
            accumAngle += theta;
            ccwDir = new Vector2(Mathf.Cos(accumAngle), Mathf.Sin(accumAngle));

            // ccw
            Vector2 ccwInRadius = ccwDir * inRadius;
            Vector2Int a = center + new Vector2Int((int)ccwInRadius.x, (int)ccwInRadius.y);

            Vector2 cwInRadius = cwDir * inRadius;
            Vector2Int c = center + new Vector2Int((int)cwInRadius.x, (int)cwInRadius.y);

            tex.Line(a, c, wallColor);
        }
    }


    private void DrawWalls(PolarGrid polarGrid)
    {
        Vector2Int center = Center;

        for (int i = 1; i < polarGrid.RowCount; ++i)
        {
            int rowLength = polarGrid.RowLength(i);

            float theta = Mathf.PI * 2.0f / (float) rowLength;
            float inRadius = cellHeight * (float)i;
            float outRadius = inRadius + cellHeight;

            Vector2 cwDir = Vector2.right;
            Vector2 ccwDir = Vector2.right;
            float accumAngle = 0.0f;

            for (int j = 0; j != rowLength; ++j)
            {
                cwDir = ccwDir;
                accumAngle += theta;
                ccwDir = new Vector2(Mathf.Cos(accumAngle), Mathf.Sin(accumAngle));

                // ccw
                Vector2 ccwInRadius = ccwDir * inRadius;
                Vector2Int a = center + new Vector2Int((int) ccwInRadius.x, (int)ccwInRadius.y);

                Vector2 ccwOutRadius = ccwDir * outRadius;
                Vector2Int b = center + new Vector2Int((int)ccwOutRadius.x, (int)ccwOutRadius.y);

                Vector2 cwInRadius = cwDir * inRadius;
                Vector2Int c = center + new Vector2Int((int)cwInRadius.x, (int)cwInRadius.y);
				
                // Vector2 cwOutRadius = cwDir * outRadius;
                // Vector2Int d = center + new Vector2Int((int)cwOutRadius.x, (int)cwOutRadius.y);

                int vertex = polarGrid.RowColIndex(i, j);
                int inward = polarGrid.Inward(new Position(i, j));
				int ccwVertex = polarGrid.CCW(new Position(i, j));

                if (!polarGrid.graph.AreLinked(vertex, inward))
                    tex.Line(a, c, wallColor);

                if (!polarGrid.graph.AreLinked(vertex, ccwVertex))
                    tex.Line(a, b, wallColor);
            }
        }
    }

    private void PaintDistances(PolarGrid polarGrid, Color[] vertexDistanceColors)
    {
        Vector2 center = Center;
        int maxRadius = cellHeight * polarGrid.RowCount;
        float TwoPi = Mathf.PI * 2.0f;

        for (int y = 0; y != tex.height; ++y)
        {
            for (int x = 0; x != tex.width; ++x)
            {
                Color color;

                // inside circle?
                Vector2 fromCenter = new Vector2(x, y) - center;
                float magnitude = fromCenter.magnitude;

                if (magnitude < maxRadius)
                {
                    int row = (int) (magnitude / (float) cellHeight);
                    float theta = TwoPi / polarGrid.RowLength(row);
                    
                    float angle = Vector2.SignedAngle(Vector2.right, fromCenter.normalized) * Mathf.Deg2Rad;

                    if (angle < 0.0f)
                        angle += TwoPi;
                    
                    int col = (int) (angle / theta);

                    int vertex = polarGrid.RowColIndex(row, col);
                    color = vertexDistanceColors[vertex];
                }
                else
                {
                    color = backgroundColor;
                }

                tex.SetPixel(x, y, color);
            }
        }
    }

    private Vector2Int Center { get { return new Vector2Int(tex.width / 2, tex.height / 2); } }

    public int CellHeight
    {
        get { return cellHeight; }
        set { cellHeight = value; }
    }

    public Texture2D Tex { get { return tex; } }
}
