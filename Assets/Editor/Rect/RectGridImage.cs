using UnityEngine;

public class RectGridImage : GridImage<RectGrid>
{
    private int cellSize;

    public RectGridImage(int cellSize, Color backgroundColor, Color wallColor)
        : base(backgroundColor, wallColor)
    {
        this.cellSize = cellSize;
    }

    protected override void DrawOutsideWalls(RectGrid grid)
    {
        Vector2Int imageSize = CalculateImageSize(grid);

        Vector2Int[] verts = new Vector2Int[]
        {
            new Vector2Int(0, 0),
            new Vector2Int(imageSize.x, 0),
            new Vector2Int(imageSize.x, imageSize.y),
            new Vector2Int(0, imageSize.y)
        };

        for (int i = 0; i != 4; ++i)
        {
            int next = (i + 1) % 4;
            tex.Line(verts[i], verts[next], Color.blue);
        }
    }

    protected override void PaintCells(RectGrid grid, Color[] cellColors)
    {
        RectInt rect = new RectInt(0, 0, cellSize, cellSize);

        for (int i = 0; i != grid.height; ++i)
        {
            rect.y = tex.height - cellSize - (i * cellSize);

            for (int j = 0; j != grid.width; ++j)
            {
                Color cellColor = cellColors[grid.PositionToVertex(i, j)];
                rect.x = j * cellSize;
                tex.Fill(rect, cellColor);
            }
        }
    }

    protected override void DrawWalls(RectGrid grid)
    {
        int bottomRowIndex = grid.height - 1;
        int rightmostColIndex = grid.width - 1;

        for (int i = 0; i < bottomRowIndex; ++i)
        {
            int north = tex.height - (i * cellSize);
            int south = north - cellSize;

            for (int j = 0; j < rightmostColIndex; ++j)
            {
                int west = j * cellSize;
                int east = west + cellSize;

                int currentVertex = grid.PositionToVertex(i, j);

                if (!grid.Graph.AreLinked(currentVertex, grid.EastOf(currentVertex)))
                    tex.VerticalLine(east, north, south, wallColor);

                if (!grid.Graph.AreLinked(currentVertex, grid.SouthOf(currentVertex)))
                    tex.HorizontalLine(west, east, south, wallColor);
            }
        }

        // right column: only check south
        int rightmostWest = cellSize * rightmostColIndex;
        int rightmostEast = rightmostWest + cellSize;

        for (int i = 0; i < bottomRowIndex; ++i)
        {
            int currentVertex = grid.PositionToVertex(i, rightmostColIndex);

            if (!grid.Graph.AreLinked(currentVertex, grid.SouthOf(currentVertex)))
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
            int currentCell = grid.PositionToVertex(bottomRowIndex, j);

            if (!grid.Graph.AreLinked(currentCell, grid.EastOf(currentCell)))
            {
                int east = cellSize * (j + 1);

                tex.VerticalLine(
                    east, bottomNorth, bottomSouth, wallColor
                );
            }
        }
    }

	protected override Vector2Int CalculateImageSize(RectGrid grid)
	{
        return new Vector2Int(grid.width * cellSize, grid.height * cellSize);
	}

	public int CellSize
    {
        get { return cellSize; }
        set { cellSize = value; }
    }
}
