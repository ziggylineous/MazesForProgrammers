using UnityEngine;
using System.Collections.Generic;

public class WeaveRectGridImage : GridImage<WeaveRectGrid>
{
    private int cellSize;
    private int inset;

    public WeaveRectGridImage(int cellSize, int inset, Color backgroundColor, Color wallColor)
        : base(backgroundColor, wallColor)
    {
        this.cellSize = cellSize;
        this.inset = inset;
    }

    protected override void DrawOutsideWalls(WeaveRectGrid grid)
    {
        /*Vector2Int imageSize = CalculateImageSize(grid);

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
        }*/
    }

    protected override void PaintCells(WeaveRectGrid grid, Color[] cellColors)
    {
        /*RectInt rect = new RectInt(0, 0, cellSize, cellSize);

        for (int i = 0; i != grid.height; ++i)
        {
            rect.y = tex.height - cellSize - (i * cellSize);

            for (int j = 0; j != grid.width; ++j)
            {
                Color cellColor = cellColors[grid.RowColIndex(i, j)];
                rect.x = j * cellSize;
                tex.Fill(rect, cellColor);
            }
        }*/
    }

    protected override void DrawWalls(WeaveRectGrid grid)
    {
        for (int i = 0; i != grid.height; ++i)
        {
            int north = tex.height - (i * cellSize);
            int south = north - cellSize;

            for (int j = 0; j < grid.width; ++j)
            {
                int west = j * cellSize;
                int east = west + cellSize;
                
                int x1 = west;
                int x2 = x1 + inset;
				int x4 = east;
                int x3 = x4 - inset;

                int y1 = north;
                int y2 = y1 - inset;
                int y4 = south;
                int y3 = y4 + inset;

                int vertex = grid.PositionToVertex(i, j);

                List<KeyValuePair<int, Line[]>> links = new List<KeyValuePair<int, Line[]>>(4);
                links.Add(
                    new KeyValuePair<int, Line[]>(
                        grid.NorthOf(vertex),
                        new Line[3] {
                            new Line(x2, y1, x2, y2), new Line(x3, y1, x3, y2), new Line(x2, y2, x3, y2)
                        }
                    )
                );

                links.Add(
                    new KeyValuePair<int, Line[]>(
                        grid.SouthOf(vertex),
                        new Line[3] {
                            new Line(x2, y3, x2, y4), new Line(x3, y3, x3, y4), new Line(x2, y3, x3, y3)
                        }
                    )
                );

                links.Add(
                    new KeyValuePair<int, Line[]>(
                        grid.WestOf(vertex),
                        new Line[3] {
                            new Line(x1, y3, x2, y3), new Line(x1, y2, x2, y2), new Line(x2, y2, x2, y3)
                        }
                    )
                );

                links.Add(
                    new KeyValuePair<int, Line[]>(
                        grid.EastOf(vertex),
                        new Line[3] {
                            new Line(x3, y3, x4, y3), new Line(x3, y2, x4, y2), new Line(x3, y2, x3, y3)
                        }
                    )
                );

                foreach (var dir_lines in links)
                {
                    int neighbor = dir_lines.Key;
                    int linkedVertex = -1;
                    Line[] lines = dir_lines.Value;

                    if (grid.Graph.AreLinked(vertex, neighbor))
                    {
                        linkedVertex = neighbor;
                    }
                    else if (neighbor != -1)
                    {
                        List<int> vLinks = grid.Graph.LinksOf(vertex);
                        System.Predicate<int> isTunnelLink = linkd =>
                            linkd != neighbor &&
                            grid.IsTunnel(linkd) &&
                            grid.Graph[neighbor] == grid.Graph[linkd];
                        
                        int tunnelIndex = vLinks.FindIndex(isTunnelLink);

                        if (tunnelIndex != -1)
                            linkedVertex = vLinks[tunnelIndex];

                    }

                    if (linkedVertex != -1)
                    {
                        tex.Line(lines[0].a, lines[0].b, wallColor);
                        tex.Line(lines[1].a, lines[1].b, wallColor);
                    }
                    else
                    {
                        tex.Line(lines[2].a, lines[2].b, wallColor);
                    }
                }
            }
        }

        PositionGraph graph = grid.Graph;

        for (int tunnelVertex = grid.width * grid.height; tunnelVertex != grid.Graph.Size; ++tunnelVertex)
        {
            Position position = grid.Graph[tunnelVertex];

            int north = tex.height - (position.row * cellSize);
            int south = north - cellSize;
            int west = position.col * cellSize;
            int east = west + cellSize;

            int x1 = west;
            int x2 = x1 + inset;
            int x4 = east;
            int x3 = x4 - inset;

            int y1 = north;
            int y2 = y1 - inset;
            int y4 = south;
            int y3 = y4 + inset;

            bool isVerticalPassage =    graph.AreLinked(tunnelVertex, grid.NorthOf(tunnelVertex)) ||
                                        graph.AreLinked(tunnelVertex, grid.SouthOf(tunnelVertex));

            if (isVerticalPassage)
            {
				// north
				tex.Line(new Vector2Int(x2, y3), new Vector2Int(x2, y4), wallColor);
				tex.Line(new Vector2Int(x3, y3), new Vector2Int(x3, y4), wallColor);

                // south
                tex.Line(new Vector2Int(x2, y1), new Vector2Int(x2, y2), wallColor);
                tex.Line(new Vector2Int(x3, y1), new Vector2Int(x3, y2), wallColor);
            }
            else // horizontal passage
            {
                // w
                tex.Line(new Vector2Int(x1, y3), new Vector2Int(x2, y3), wallColor);
                tex.Line(new Vector2Int(x1, y2), new Vector2Int(x2, y2), wallColor);

                // e
                tex.Line(new Vector2Int(x3, y3), new Vector2Int(x4, y3), wallColor);
                tex.Line(new Vector2Int(x3, y2), new Vector2Int(x4, y2), wallColor);
            }
        }
    }

    protected override Vector2Int CalculateImageSize(WeaveRectGrid grid)
    {
        return new Vector2Int(grid.width * cellSize, grid.height * cellSize);
    }

    public int CellSize
    {
        get { return cellSize; }
        set { cellSize = value; }
    }

    public int Inset
    {
        get { return inset; }
        set { inset = value; }
    }
}
