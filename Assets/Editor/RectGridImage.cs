using UnityEngine;

public class RectGridImage
{
    private int cellSize;
    private Color backgroundColor;
    private Color wallColor;
    private Texture2D tex;

    public RectGridImage(int cellSize, Color backgroundColor, Color wallColor)
    {
        this.cellSize = cellSize;
        this.backgroundColor = backgroundColor;
        this.wallColor = wallColor;
        tex = new Texture2D(100, 100);
    }

    public void Draw(RectGrid maze, Color[] cellColors = null)
    {
        int imageWidth = cellSize * maze.width;
        int imageHeight = cellSize * maze.height;

        tex.Resize(imageWidth, imageHeight);

        if (cellColors == null) tex.Fill(backgroundColor);
        else PaintCells(maze, cellColors);

        DrawOutsideWalls(imageWidth, imageHeight);
        DrawWalls(maze);
        tex.Apply();
    }

    private void DrawOutsideWalls(int imageWidth, int imageHeight)
    {
        /*Vector2Int[] verts = new Vector2Int[]
        {
            new Vector2Int(0, 0),
            new Vector2Int(imageWidth, 0),
            new Vector2Int(imageWidth, imageHeight),
            new Vector2Int(0, imageHeight)
        };

        for (int i = 0; i != 4; ++i)
        {
            int next = (i + 1) % 4;
            tex.Line(verts[i], verts[next], Color.blue, 2);
        }*/
    }

    private void PaintCells(RectGrid maze, Color[] cellColors)
    {
        RectInt rect = new RectInt(0, 0, cellSize, cellSize);

        for (int i = 0; i != maze.height; ++i)
        {
            rect.y = tex.height - cellSize - (i * cellSize);

            for (int j = 0; j != maze.width; ++j)
            {
                Color cellColor = cellColors[maze.RowColIndex(i, j)];
                rect.x = j * cellSize;
                tex.Fill(rect, cellColor);
            }
        }

    }

    private void DrawWalls(RectGrid rectGrid)
    {
        int bottomRowIndex = rectGrid.height - 1;
        int rightmostColIndex = rectGrid.width - 1;

        for (int i = 0; i < bottomRowIndex; ++i)
        {
            int north = tex.height - (i * cellSize);
            int south = north - cellSize;

            for (int j = 0; j < rightmostColIndex; ++j)
            {
                int west = j * cellSize;
                int east = west + cellSize;

                int currentVertex = rectGrid.RowColIndex(i, j);

                if (!rectGrid.graph.AreLinked(currentVertex, rectGrid.EastOf(currentVertex)))
                    tex.VerticalLine(east, north, south, wallColor);

                if (!rectGrid.graph.AreLinked(currentVertex, rectGrid.SouthOf(currentVertex)))
                    tex.HorizontalLine(west, east, south, wallColor);
            }
        }

        // right column: only check south
        int rightmostWest = cellSize * rightmostColIndex;
        int rightmostEast = rightmostWest + cellSize;

        for (int i = 0; i < bottomRowIndex; ++i)
        {
            int currentVertex = rectGrid.RowColIndex(i, rightmostColIndex);

            if (!rectGrid.graph.AreLinked(currentVertex, rectGrid.SouthOf(currentVertex)))
            {
                int south = tex.height - ((i + 1) * cellSize);

                tex.HorizontalLine(
                    rightmostWest, rightmostEast, south, wallColor
                );
            }
        }

        // bottom row: only check east
        int bottomNorth = tex.height - (cellSize * bottomRowIndex);
        int bottomSouth = bottomNorth - cellSize;

        for (int j = 0; j < rightmostColIndex; ++j)
        {
            int currentCell = rectGrid.RowColIndex(bottomRowIndex, j);

            if (!rectGrid.graph.AreLinked(currentCell, rectGrid.EastOf(currentCell)))
            {
                int east = cellSize * (j + 1);

                tex.VerticalLine(
                    east, bottomNorth, bottomSouth, wallColor
                );
            }
        }
    }

    public int CellSize
    {
        get { return cellSize; }
        set { cellSize = value; }
    }

    public Texture2D Tex { get { return tex; } }
}
