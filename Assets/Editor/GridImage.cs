using UnityEngine;
using UnityEditor;

public abstract class GridImage<T>
{
    protected Color backgroundColor;
    protected Color wallColor;
    protected Texture2D tex;

    protected GridImage(Color backgroundColor, Color wallColor)
    {
        this.backgroundColor = backgroundColor;
        this.wallColor = wallColor;

        tex = new Texture2D(100, 100);
    }

    public void Draw(T grid, Color[] cellColors = null)
    {
        Resize(grid);

        if (cellColors == null) tex.Fill(backgroundColor);
        else PaintCells(grid, cellColors);

        DrawOutsideWalls(grid);
        DrawWalls(grid);
        tex.Apply();
    }

    protected abstract void DrawOutsideWalls(T grid);
    protected abstract void PaintCells(T grid, Color[] cellColors);
    protected abstract void DrawWalls(T grid);

    public void Resize(T grid)
    {
        Vector2Int imageSize = CalculateImageSize(grid);
        tex.Resize(imageSize.x, imageSize.y);
    }

    protected abstract Vector2Int CalculateImageSize(T grid);

    public Texture2D Tex { get { return tex; } }
}
