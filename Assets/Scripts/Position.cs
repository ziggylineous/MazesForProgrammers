using UnityEngine;
using System;

[Serializable]
public struct Position
{
    public int row, col;

    public Position(int i, int j)
    {
        this.row = i;
        this.col = j;
    }
}

[Serializable]
public class PositionGraph : Graph
{
    [SerializeField]
    private Position[] vertexData;

    public PositionGraph() : base(0) { }

    public PositionGraph(int nodeCount) : base(nodeCount) { }

    public Position this[int v]
    {
        get { return vertexData[v]; }
        set { vertexData[v] = value; }
    }

    public override int Size
    {
        get { return base.Size; }
        set
        {
            base.Size = value;

            vertexData = new Position[value];
        }
    }
}