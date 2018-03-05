using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Mazes/Triangular Maze", fileName = "triMaze", order = 0)]
public class TriGrid : RectGrid
{
    public override Graph AdjacentGraph
    {
        get
        {
            Graph adjacent = new Graph(graph.Size);
            int[] possibleAdjacent = new int[3];

            for (int v = 0; v < graph.Size; ++v)
            {
                possibleAdjacent[0] = EastOf(v);
                possibleAdjacent[1] = WestOf(v);
                possibleAdjacent[2] = BaseOf(v);

                foreach (int adj in possibleAdjacent)
                    if (adj != -1)
                        adjacent.LinkVertices(v, adj);
            }

            return adjacent;
        }
    }

    public bool IsUpright(int vertex)
    {
        Position position = graph[vertex];
        return ((position.row + position.col) % 2) == 0;
    }

    public int BaseOf(int vertex)
    {
        return  IsUpright(vertex) ?
                SouthOf(vertex) :
                NorthOf(vertex);
    }

    public override string ToString()
    {
        return string.Format("tri grid {0}, {1} : {2}. ok? {3}", width, height, graph.Size, graph.Size == width * height);
    }
}