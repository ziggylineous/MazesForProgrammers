using UnityEngine;

[CreateAssetMenu(menuName = "Mazes/Polar Maze", fileName = "polaMaze", order = 0)]
public class PolarGrid : ScriptableObject
{
    [SerializeField]
    private int rowCount;

    [SerializeField]
    private int[] rowLengths;

    [SerializeField]
    private int[] rowLengthsAccumulated;

    [SerializeField]
    private PositionGraph graph;

    public Graph Graph { get { return graph; } }

    public void Awake()
    {
        if (rowLengths == null) rowLengths = new int[0];
        if (rowLengthsAccumulated == null) rowLengthsAccumulated = new int[0];
    }

    public int RowCount
    {
        set
        {
            rowCount = value;
            InitRowLengths();
            InitGraph();
            InitPositions();
        }

        get { return rowCount; }
    }

    private void InitRowLengths()
    {
        rowLengths = new int[rowCount];
        rowLengthsAccumulated = new int[rowCount];

        // first row
        rowLengths[0] = 1;
        rowLengthsAccumulated[0] = 0;

        // rest
        float rowHeight = 1.0f / (float)rowCount;

        for (int r = 1; r != rowCount; ++r)
        {
            float radius = (float)r / (float)rowCount;
            float circumference = Mathf.PI * 2.0f * radius;

            int prevLength = rowLengths[r - 1];
            float estimatedCellWidth = circumference / prevLength;
            float ratio = Mathf.Round(estimatedCellWidth / rowHeight);
            int cellCount = (int) (ratio * prevLength);

            rowLengths[r] = cellCount;
            rowLengthsAccumulated[r] = rowLengthsAccumulated[r - 1] + rowLengths[r - 1];
        }
    }

    private void InitGraph()
    {
        int cellCount = rowLengthsAccumulated[rowCount - 1] + rowLengths[rowCount - 1];

        if (graph == null)
        {
            graph = new PositionGraph(cellCount);
            Debug.Log("polarGrid: creating graph (unity doesnt initialize it)");         
        }
        else
        {
            graph.Size = cellCount;
        }
    }

    private void InitPositions()
    {
        for (int i = 0; i != rowCount; ++i)
        {
            for (int j = 0; j != rowLengths[i]; ++j)
            {
                int vertex = RowColIndex(i, j);
                graph[vertex] = new Position(i, j);
            }
        }
    }

    public int RowColIndex(int row, int col)
    {
        return rowLengthsAccumulated[row] + col;
    }

    public Graph AdjacentGraph
    {
        get
        {
            Graph adjacent = new Graph(graph.Size);

            for (int vertex = 1; vertex != graph.Size; ++vertex)
            {
                Position position = graph[vertex];

                // sides
                int rowLen = rowLengths[position.row];
                int cwCol = ((position.col - 1) + rowLen) % rowLen;
                int ccwCol = (position.col + 1) % rowLen;

                adjacent.LinkVertices(vertex, RowColIndex(position.row, cwCol));
                adjacent.LinkVertices(vertex, RowColIndex(position.row, ccwCol));

                int parentRow = position.row - 1;
                int colCountRatio = rowLen / rowLengths[parentRow];
                int parent = RowColIndex(parentRow, position.col / colCountRatio);

                adjacent.LinkVertices(vertex, parent);
            }

            return adjacent;
        }
    }

    public int Inward(int vertex)
    {
        return Inward(graph[vertex]);
    }

    public int Inward(Position vertexPosition)
    {
        if (vertexPosition.row == 0) return -1;

        int ratio = rowLengths[vertexPosition.row] / rowLengths[vertexPosition.row - 1];
        return RowColIndex(vertexPosition.row - 1, vertexPosition.col / ratio);
    }

    public int CCW(Position vertexPosition)
    {
        return RowColIndex(vertexPosition.row, (vertexPosition.col + 1) % rowLengths[vertexPosition.row]);
    }

    public int RowLength(int row) { return rowLengths[row]; }

    public override string ToString()
    {
        return string.Format("polar grid. rowCount = {0}, cellCount = {1}", rowCount, graph.Size);
    }
}