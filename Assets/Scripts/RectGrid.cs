using UnityEngine;


[CreateAssetMenu(menuName = "Mazes/Rect Maze", fileName = "rectMaze", order = 0)]
public class RectGrid : ScriptableObject
{
    public int width;
    public int height;

    [SerializeField]
    protected PositionGraph graph;

    public void SetSize(int w, int h)
    {
        width = w;
        height = h;

        graph.Size = width * height;

        for (int i = 0; i != graph.Size; ++i)
            graph[i] = new Position(i / width, i % width);

        Debug.LogFormat("Initialized maze {0} {1}", width, height);
    }

    public int RowColIndex(int row, int col)
    {
        return row * width + col;
    }

    public virtual Graph AdjacentGraph
    {
        get
        {
            Graph adjacent = new Graph(graph.Size);
            int[] possibleAdjacent = new int[4];

            for (int v = 0; v != graph.Size; ++v)
            {
                possibleAdjacent[0] = EastOf(v);
                possibleAdjacent[1] = WestOf(v);
                possibleAdjacent[2] = SouthOf(v);
                possibleAdjacent[3] = NorthOf(v);

                foreach (int adj in possibleAdjacent)
                    if (adj != -1)
                        adjacent.LinkVertices(v, adj);
            }

            return adjacent;
        }
    }

    public int BottomLeftVertex { get { return RowColIndex(height - 1, 0); } }
    public int TopRightVertex { get { return RowColIndex(0, width - 1); } }

    public int EastOf(int westVertex)
    {
        Position position = graph[westVertex];
        return position.col == (width - 1) ? -1 : westVertex + 1;
    }

    public int WestOf(int eastVertex)
    {
        Position position = graph[eastVertex];
        return position.col == 0 ? -1 : eastVertex - 1;
    }

    public int NorthOf(int southVertex)
    {
        Position position = graph[southVertex];
        return position.row == 0 ? -1 : southVertex - width;
    }

    public int SouthOf(int northVertex)
    {
        Position position = graph[northVertex];
        return position.row == (height - 1) ? -1 : northVertex + width;
    }

    public Graph Graph { get { return graph; }}

    public override string ToString()
    {
        return string.Format("maze {0}, {1} : {2}. ok? {3}", width, height, graph.Size, graph.Size == width * height);
    }
}