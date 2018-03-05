using System;
using System.Collections.Generic;
using UnityEngine;

public class VertexLinkerHelper
{
    public Graph buildingGraph;
    public Graph adjacent;

    public VertexLinkerHelper(Graph buildingGraph, Graph adjacent)
    {
        this.buildingGraph = buildingGraph;
        this.adjacent = adjacent;
    }

    public virtual void Link(int v, int w)
    {
        buildingGraph.LinkVertices(v, w);
    }

    public virtual List<int> NeighborsOf(int v)
    {
        return adjacent.LinksOf(v);
    }
}


public abstract class VertexLinker
{
    public static System.Random randomAccess = new System.Random();

    public abstract void Build(VertexLinkerHelper linkerHelper);
    
    public int Sample(List<int> cells)
    {
        return cells[randomAccess.Next(cells.Count)];
    }
}

public class BinaryVertexLinker : VertexLinker
{
    public override void Build(VertexLinkerHelper linkerHelper)
    {
        System.Random random = new System.Random();

        /*int lastColIndex = grid.width - 1;

        for (int i = 1; i != grid.height; ++i)
        {
            for (int j = 0; j != lastColIndex; ++j)
            {
                RectCell currentCell = grid[i, j];

                currentCell.Link(
                    random.NextDouble() < 0.5f ?
                    currentCell.E :
                    currentCell.N
                );
            }
        }

        for (int j = 0; j != lastColIndex; ++j)
        {
            RectCell currentCell = grid[0, j];
            currentCell.Link(currentCell.E);
        }

        for (int i = 1; i != grid.height; ++i)
        {
            RectCell currentCell = grid[i, lastColIndex];
            currentCell.Link(currentCell.N);
        }*/
    }
}

public class SidewinderVertexLinker : VertexLinker
{
    public override void Build(VertexLinkerHelper linkerHelper)
    {
        /*System.Random random = new System.Random();
        System.Random runRandom = new System.Random();

        int lastColIndex = grid.width - 1;

        // first row: cant link north => link east
        for (int j = 0; j != lastColIndex; ++j)
        {
            RectCell currentCell = grid[0, j];
            currentCell.Link(currentCell.E);
        }


        for (int i = 1; i != grid.height; ++i)
        {
            int runStart = 0;

            for (int j = 0; j != lastColIndex; ++j)
            {
                if (random.NextDouble() < 0.5f)
                {
                    // end run
                    int randCol = runRandom.Next(runStart, j);
                    RectCell randCell = grid[i, randCol];
                    randCell.Link(randCell.N);
                    runStart = j + 1;
                }
                else
                {
                    // keep run
                    RectCell currentCell = grid[i, j];
                    currentCell.Link(currentCell.E);
                }
            }

            int lastRandCol = runRandom.Next(runStart, lastColIndex);
            RectCell lastRandCell = grid[i, lastRandCol];
            lastRandCell.Link(lastRandCell.N);
        }*/
    }
}


public class RandomWalkVertexLinker : VertexLinker
{
    public override void Build(VertexLinkerHelper linkerHelper)
    {
        Graph buildingGraph = linkerHelper.buildingGraph;
        int vertex = buildingGraph.RandomVertex;
        int unvisitedCount = buildingGraph.Size - 1;

        while (unvisitedCount > 0)
        {
            int neighbor = Sample(linkerHelper.NeighborsOf(vertex));

            if (!buildingGraph.HasAnyLink(neighbor))
            {
                linkerHelper.Link(vertex, neighbor);
                --unvisitedCount;
            }

            vertex = neighbor;
        }
    }
}


public class HuntAndKillVertexLinker : VertexLinker
{
    public override void Build(VertexLinkerHelper linkerHelper)
    {
        Graph buildingGraph = linkerHelper.buildingGraph;
        int cell = buildingGraph.RandomVertex;

        while (cell != -1)
        {
            List<int> unvisitedAdjacent = linkerHelper.NeighborsOf(cell).FindAll(adj => !buildingGraph.HasAnyLink(adj));

            if (unvisitedAdjacent.Count != 0)
            {
                int neighbor = Sample(unvisitedAdjacent);
                linkerHelper.Link(cell, neighbor);
                cell = neighbor;
            }
            else
            {
				cell = -1;

                for (int i = 0; cell == -1 && i != buildingGraph.Size; ++i)
                {
                    if (!buildingGraph.HasAnyLink(i) &&
                        linkerHelper.NeighborsOf(i).Exists(adj => buildingGraph.HasAnyLink(adj)))
                    {
                        cell = i;
                        List<int> visitedAdjacent = linkerHelper.NeighborsOf(i).FindAll(adj => buildingGraph.HasAnyLink(adj));
                        linkerHelper.Link(cell, Sample(visitedAdjacent));
                    }
                }
            }
        }
    }
}


public class RecursiveBacktrackerVertexLinker : VertexLinker
{
    public override void Build(VertexLinkerHelper linkerHelper)
    {
        Stack<int> stack = new Stack<int>();
        stack.Push(linkerHelper.buildingGraph.RandomVertex);

        while (stack.Count != 0)
        {
			int current = stack.Peek();
            List<int> unvisitedAdjacent = linkerHelper.NeighborsOf(current).FindAll(adj => !linkerHelper.buildingGraph.HasAnyLink(adj));

            if (unvisitedAdjacent.Count == 0)
            {
                stack.Pop();
            }
            else
            {
                int neighbor = Sample(unvisitedAdjacent);
                linkerHelper.Link(current, neighbor);
                stack.Push(neighbor);
            }
        }
    }
}