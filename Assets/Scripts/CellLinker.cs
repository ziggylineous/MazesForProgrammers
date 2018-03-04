using System;
using System.Collections.Generic;
using UnityEngine;


public abstract class CellLinker
{
    public static System.Random randomAccess = new System.Random();

    public abstract void Build(GridGraphShape grid);
    
    public int Sample(List<int> cells)
    {
        return cells[randomAccess.Next(cells.Count)];
    }
}

public class BinaryCellLinker : CellLinker
{
    public override void Build(GridGraphShape grid)
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

public class SidewinderCellLinker : CellLinker
{
    public override void Build(GridGraphShape grid)
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


public class RandomWalkCellLinker : CellLinker
{
    public override void Build(GridGraphShape grid)
    {
        Graph buildingGraph = grid.graph;
		Graph adjacentGraph = grid.AdjacentGraph;
        int vertex = buildingGraph.RandomVertex;
        int unvisitedCount = grid.graph.Size - 1;

        while (unvisitedCount > 0)
        {
            int neighbor = adjacentGraph.RandomLinkOf(vertex);

            if (!buildingGraph.HasAnyLink(neighbor))
            {
                buildingGraph.LinkVertices(vertex, neighbor);
                --unvisitedCount;
            }

            vertex = neighbor;
        }
    }
}


public class HuntAndKillCellLinker : CellLinker
{
    public override void Build(GridGraphShape grid)
    {
		Graph buildingGraph = grid.graph;
        Graph adjacentGraph = grid.AdjacentGraph;
        int cell = buildingGraph.RandomVertex;

        while (cell != -1)
        {
            List<int> unvisitedAdjacent = adjacentGraph.LinksOf(cell).FindAll(adj => !buildingGraph.HasAnyLink(adj));

            if (unvisitedAdjacent.Count != 0)
            {
                int neighbor = Sample(unvisitedAdjacent);
                buildingGraph.LinkVertices(cell, neighbor);
                cell = neighbor;
            }
            else
            {
				cell = -1;

                for (int i = 0; cell == -1 && i != buildingGraph.Size; ++i)
                {
                    if (!buildingGraph.HasAnyLink(i) &&
                        adjacentGraph.LinksOf(i).Exists(adj => buildingGraph.HasAnyLink(adj)))
                    {
                        cell = i;
                        List<int> visitedAdjacent = adjacentGraph.LinksOf(i).FindAll(adj => buildingGraph.HasAnyLink(adj));
                        buildingGraph.LinkVertices(cell, Sample(visitedAdjacent));
                    }
                }
            }
        }
    }
}


public class RecursiveBacktrackerCellLinker : CellLinker
{
    public override void Build(GridGraphShape grid)
    {
		Graph adjacentGraph = grid.AdjacentGraph;
        Graph buildingGraph = grid.graph;
        Stack<int> stack = new Stack<int>();
        stack.Push(buildingGraph.RandomVertex);

        while (stack.Count != 0)
        {
            int current = stack.Peek();
            List<int> unvisitedAdjacent = adjacentGraph.LinksOf(current).FindAll(adj => !buildingGraph.HasAnyLink(adj));

            if (unvisitedAdjacent.Count == 0)
            {
                stack.Pop();
            }
            else
            {
                int neighbor = Sample(unvisitedAdjacent);
                buildingGraph.LinkVertices(current, neighbor);
                stack.Push(neighbor);
            }
        }
    }
}