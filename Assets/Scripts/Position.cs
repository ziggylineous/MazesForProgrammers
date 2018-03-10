using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public struct Position
{
    public int row, col;

    public Position(int i, int j)
    {
        this.row = i;
        this.col = j;
    }

    public static bool operator ==(Position a, Position b)
    {
        return a.row == b.row && a.col == b.col;
    }

    public static bool operator !=(Position a, Position b)
    {
        return  a.row != b.row ||
                a.col != b.col;
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;

        return (Position) obj == this;
    }
}

[Serializable]
public class PositionGraph : Graph
{
    [SerializeField]
    private List<Position> positions;

    public PositionGraph() : base(0) { }

    public PositionGraph(int nodeCount) : base(nodeCount) { }

    public Position this[int v]
    {
        get { return positions[v]; }
        set { positions[v] = value; }
    }

    public int NewVertex(Position position)
    {
        int newVertex = base.NewVertex();

        positions.Add(position);

        return newVertex;
    }

    public override void RemoveVertex(int v)
    {
        base.RemoveVertex(v);
        positions.RemoveAt(v);
    }

    public override int Size
    {
        get { return base.Size; }
        set
        {
            base.Size = value;

            positions = new List<Position>(value);
            for (int i = 0; i != value; ++i)
                positions.Add(new Position(-1, -1));
        }
    }
}