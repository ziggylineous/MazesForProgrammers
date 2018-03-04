using UnityEngine;

public class TriImage
{
    private int sideSize;
    private Color backgroundColor;
    private Color wallColor;
    private Texture2D tex;

    public TriImage(int sideSize, Color backgroundColor, Color wallColor)
    {
        this.sideSize = sideSize;
        this.backgroundColor = backgroundColor;
        this.wallColor = wallColor;
        tex = new Texture2D(100, 100);
    }

    public void Draw(TriGrid maze, Color[] cellColors = null)
    {
        int imageWidth = CalcWidth(maze.width);
        int imageHeight = CalcHeight(maze.height);

        tex.Resize(imageWidth, imageHeight);

        if (cellColors == null) tex.Fill(backgroundColor);
        else PaintDistances(maze, cellColors);

        DrawOutsideWalls(maze);
        DrawWalls(maze);
        tex.Apply();
    }

    private void DrawOutsideWalls(TriGrid grid)
    {
        float halfSideSize = sideSize / 2;
        int halfSideSizeInt = (int)halfSideSize;
        float height = TriangleHeight;
        float halfHeight = height / 2.0f;
        int centerX = halfSideSizeInt;
        int west = Mathf.Max(0, centerX - halfSideSizeInt);

        // Upright (0 col, so first even)
        int row = 0;

        while (row < grid.height)
        {
            int centerY = tex.height - ((int)(halfHeight + row * height));
            int north = (int)(centerY + halfHeight);
            int south = north - (int)height;

            tex.Line(new Vector2Int(west, south), new Vector2Int(centerX, north), wallColor);
            row += 2;
        }

        row = 1;

        while (row < grid.height)
        {
            int centerY = tex.height - ((int)(halfHeight + row * height));
            int north = (int)(centerY + halfHeight);
            int south = north - (int)height;

            tex.Line(new Vector2Int(west, north), new Vector2Int(centerX, south), wallColor);
            row += 2;
        }
    }

    private void PaintDistances(TriGrid triGrid, Color[] vertexDistanceColors)
    {
        float halfSideSize = sideSize / 2;
        int halfSideSizeInt = (int)halfSideSize;
        float height = TriangleHeight;
        float halfHeight = height / 2.0f;
        Vector2 texPoint = new Vector2();


        for (int y = 0; y != tex.height; ++y)
        {
            int row = (int) (((float)(tex.height - y)) / height); // y texture to grid row
            texPoint.y = y + 0.5f;

            int centerY = tex.height - ((int)(halfHeight + row * height));

            float north = centerY + halfHeight + 0.5f;
            float south = north - height - 0.5f;


            for (int x = 0; x != tex.width; ++x)
            {
                Color color = backgroundColor;
                texPoint.x = x + 0.5f;

                int halfSizeCount = (int)(x / halfSideSize);
                int fromCol = Mathf.Max(0, halfSizeCount - 1);
                int maxCol = Mathf.Min(halfSizeCount + 2, triGrid.width);

                for (int col = fromCol; col != maxCol; ++col)
                {
                    int vertex = triGrid.RowColIndex(row, col);

					int centerX = halfSideSizeInt + col * halfSideSizeInt;
					int west = Mathf.Max(0, centerX - halfSideSizeInt);
					int east = centerX + halfSideSizeInt;
                    Vector2 a = new Vector2();
                    Vector2 b = new Vector2();
                    Vector2 c = new Vector2();

                    if (triGrid.IsUpright(vertex))
                    {
                        a.Set(centerX + 0.5f, north);
                        b.Set(east + 0.5f, south);
                        c.Set(west + 0.5f, south);
                    }
                    else
                    {
                        a.Set(centerX + 0.5f, south);
                        b.Set(west + 0.5f, north);
                        c.Set(east + 0.5f, north);
                    }

                    if (PointTest2D.IsInsideTri(texPoint, a, b, c))
                    {
                        color = vertexDistanceColors[vertex];
                        break;
                    }
                }

                tex.SetPixel(x, y, color);
            }
        }
    }

    private void DrawWalls(TriGrid grid)
    {
        float halfSideSize = sideSize / 2;
        int halfSideSizeInt = (int)halfSideSize;
        float height = TriangleHeight;
        float halfHeight = height / 2.0f;

        for (int i = 0; i < grid.height; ++i)
        {
            int centerY = tex.height - ((int) (halfHeight + i * height));

            int north = (int) (centerY + halfHeight);
            int south = north - (int) height;

            // upright
            for (int j = (i % 2); j < grid.width; j += 2)
            {
                int centerX = halfSideSizeInt + j * halfSideSizeInt;
                int west = Mathf.Max(0, centerX - halfSideSizeInt);
                int east = centerX + halfSideSizeInt;

                int currentVertex = grid.RowColIndex(i, j);

                if (!grid.graph.AreLinked(currentVertex, grid.EastOf(currentVertex)))
                    tex.Line(new Vector2Int(east, south), new Vector2Int(centerX, north), wallColor);

                if (!grid.graph.AreLinked(currentVertex, grid.SouthOf(currentVertex)))
                    tex.HorizontalLine(west, east, south, wallColor);
            }

            // !upright
            for (int j = ((i + 1) % 2); j < grid.width; j += 2)
            {
                int centerX = (int) (halfSideSize + j * halfSideSize);
                int west = centerX - halfSideSizeInt;
                int east = centerX + halfSideSizeInt;

                int currentVertex = grid.RowColIndex(i, j);

                if (!grid.graph.AreLinked(currentVertex, grid.EastOf(currentVertex)))
                    tex.Line(new Vector2Int(east, north), new Vector2Int(centerX, south), wallColor);

                if (!grid.graph.AreLinked(currentVertex, grid.NorthOf(currentVertex)))
                    tex.HorizontalLine(west, east, north, wallColor);
            }
        }
    }

    public int SideSize
    {
        get { return sideSize; }
        set { sideSize = value; }
    }
    
    public float TriangleHeight { get { return sideSize * Mathf.Sqrt(3) / 2.0f; } }

    public int CalcWidth(int gridWidth)
    {
        float widthf = Mathf.Ceil(
            (float) sideSize * (((float)gridWidth + 1.0f) / 2.0f)
        );

        return (int)widthf;
    }

    public int CalcHeight(int gridHeight)
    {
        return (int) (gridHeight * TriangleHeight);
    }

    public Texture2D Tex { get { return tex; } }
}

