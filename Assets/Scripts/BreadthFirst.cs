using UnityEngine;
using System.Collections.Generic;
using System;

public class BreadthFirst
{
    private Graph graph;
    private int source;
    private int[] parents;
    private int[] distances;

    public BreadthFirst(Graph graph, int source)
    {
        this.graph = graph;
        this.source = source;
        parents = new int[graph.Size];
        distances = new int[graph.Size];

        for (int i = 0; i != graph.Size; ++i)
            parents[i] = -1;
    }

    public void Run()
    {
        Queue<int> frontier = new Queue<int>();
        frontier.Enqueue(source);

        while (frontier.Count > 0)
        {
            int current = frontier.Dequeue();
            int distance = distances[current] + 1;

            foreach (int neighbor in graph.LinksOf(current))
            {
                if (CanVisit(neighbor))
                {
                    parents[neighbor] = current;
                    distances[neighbor] = distance;
                    frontier.Enqueue(neighbor);
                }
            }
        }
    }

    public int MaxDistance
    {
        get
        {
            int maxDistance = -1;

            for (int i = 0; i != distances.Length; ++i)
                if (distances[i] > maxDistance)
                    maxDistance = distances[i];

            return maxDistance;
        }
    }

    /*public List<Cell> PathTo(Cell dest)
    {
        
    }*/
    public int[] Distances
    {
        get { return distances; }
    }

    private bool CanVisit(int vertex)
    {
        return vertex != source && parents[vertex] == -1;
    }
}
