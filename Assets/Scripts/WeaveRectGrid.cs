using UnityEngine;
using System;
using System.Collections.Generic;


[CreateAssetMenu(menuName = "Mazes/Weaving Maze", fileName = "weavingMaze", order = 0)]
public class WeaveRectGrid : RectGrid
{
    public bool IsTunnel(int vertex)
    {
        return vertex > (width * height - 1);
    }

    public bool IsHorizontalPassage(int v)
    {
        return graph.AreLinked(v, WestOf(v)) &&
                    graph.AreLinked(v, EastOf(v)) &&
                    !graph.AreLinked(v, NorthOf(v)) &&
                    !graph.AreLinked(v, SouthOf(v));
    }

    public bool IsVerticalPassage(int v)
    {
        return !graph.AreLinked(v, WestOf(v)) &&
                    !graph.AreLinked(v, EastOf(v)) &&
                    graph.AreLinked(v, NorthOf(v)) &&
                    graph.AreLinked(v, SouthOf(v));
    }

    public List<int> TunnelLinks(int vertex)
    {
        return graph.LinksOf(vertex).FindAll(IsTunnel);
    }

    public void ClearTunnels()
    {
        int sizeWithoutTunnels = width * height - 1;

        for (int i = Graph.Size - 1; i != sizeWithoutTunnels; --i)
        {
            graph.RemoveVertex(i);
        }
    }
}




public class WeaveRectGridLinkerHelper : VertexLinkerHelper
{
	private WeaveRectGrid grid;

    public WeaveRectGridLinkerHelper(WeaveRectGrid grid) : base(grid.Graph, grid.AdjacentGraph)
    {
        this.grid = grid;
    }

    public override void Link(int v, int w)
    {
        int tunnelOver = TunnelForLink(v, w);

        if (tunnelOver != -1)
        {
            LinkWithTunnel(v, tunnelOver, w);
        }
        else
        {
			buildingGraph.LinkVertices(v, w);
        }
    }

    public override List<int> NeighborsOf(int v)
    {
        List<int> neighbors;

        if (grid.IsTunnel(v))
        {
            Position p = grid.Graph[v];
            int overCell = grid.PositionToVertex(p.row, p.col);

            neighbors = base.NeighborsOf(overCell);
		}
        else
        {
            neighbors = base.NeighborsOf(v);

            var tunnelNorth =
            new KeyValuePair<Func<int, int>, Func<int, bool>>(grid.NorthOf, grid.IsHorizontalPassage);

            var tunnelSouth =
                new KeyValuePair<Func<int, int>, Func<int, bool>>(grid.SouthOf, grid.IsHorizontalPassage);

            var tunnelWest =
                new KeyValuePair<Func<int, int>, Func<int, bool>>(grid.WestOf, grid.IsVerticalPassage);

            var tunnelEast =
                new KeyValuePair<Func<int, int>, Func<int, bool>>(grid.EastOf, grid.IsVerticalPassage);
            
            var tunnelTests = new KeyValuePair<Func<int, int>, Func<int, bool>>[] {
                tunnelNorth, tunnelSouth, tunnelWest, tunnelEast
            };

            foreach (var tunnelTest in tunnelTests)
            {
                var neighborGetter = tunnelTest.Key;
                var passageTest = tunnelTest.Value;
                var tunnelableNeighbor = neighborGetter(v);

                if (tunnelableNeighbor != -1 &&
                    neighborGetter(tunnelableNeighbor) != -1 &&
                    passageTest(tunnelableNeighbor))
                {
                    int throughTunnel = neighborGetter(tunnelableNeighbor);

                    if (!neighbors.Exists(vx => {
                        Position a = grid.Graph[vx];
                        Position b = grid.Graph[throughTunnel];
                        return a.row == b.row && a.col == b.col;
                    }))
                        neighbors.Add(throughTunnel);
                }
            }

			neighbors.AddRange(grid.TunnelLinks(v));
        }

        return neighbors;
    }

    private int TunnelForLink(int v, int w)
    {
        Func<int, int>[] oppositeDirs = new Func<int, int>[] {
            grid.NorthOf, grid.SouthOf,
            grid.SouthOf, grid.NorthOf,
            grid.WestOf, grid.EastOf,
            grid.EastOf, grid.WestOf
        };

        for (int i = 0; i != 4; ++i)
        {
            int inVDir = oppositeDirs[i * 2](v);
            int inWOppositeDir = oppositeDirs[i * 2 + 1](w);

            if (inVDir != -1 && inVDir == inWOppositeDir)
                return inVDir;
        }

        return -1;
    }

    private void LinkWithTunnel(int from, int tunnelOver, int to)
    {
        int tunnelVertex = grid.Graph.NewVertex(grid.Graph[tunnelOver]);

        buildingGraph.LinkVertices(from, tunnelVertex);
        buildingGraph.LinkVertices(tunnelVertex, to);
    }
}
