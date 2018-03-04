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
}


public abstract class GridGraphShape : ScriptableObject
{
    public Graph graph;
    public Position[] vertexPositions;

    public int this[int i, int j]
    {
        get
        {
            return RowColIndex(i, j);
        }
    }

    public abstract int RowColIndex(int row, int col);
    public int RowColIndex(Position position) { return RowColIndex(position.row, position.col); }

    public Position VertexPosition(int vertex) { return vertexPositions[vertex]; }

    public abstract Graph AdjacentGraph { get; }
}