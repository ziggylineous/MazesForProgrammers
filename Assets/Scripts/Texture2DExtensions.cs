using UnityEngine;
using System.Collections;
using System.IO;

public static class Texture2DExtensions
{
    public static void Fill(this Texture2D self, Color fillColor)
    {
        for (int y = 0; y != self.height; ++y)
        {
            for (int x = 0; x != self.width; ++x)
            {
                self.SetPixel(x, y, fillColor);
            }
        }

        // self.Apply();
    }

    public static void Fill(this Texture2D self, RectInt rect, Color fillColor)
    {
        int xMax = rect.xMax + 1;
        int yMax = rect.yMax + 1;

        for (int y = rect.yMin; y != yMax; ++y)
        {
            for (int x = rect.xMin; x != xMax; ++x)
            {
                self.SetPixel(x, y, fillColor);
            }
        }

        // self.Apply();
    }

    public static void HorizontalLine(this Texture2D self, int x1, int x2, int y, Color color)
    {
        int min = Mathf.Min(x1, x2);
        int max = Mathf.Max(x1, x2) + 1;

        min = Mathf.Clamp(min, 0, self.width - 1);
        max = Mathf.Clamp(max, 0, self.width - 1);

        for (int x = min; x != max; ++x)
        {
            self.SetPixel(x, y, color);
        }

        // self.Apply();
    }

    public static void VerticalLine(this Texture2D self, int x, int y1, int y2, Color color)
    {
        int min = Mathf.Min(y1, y2);
        int max = Mathf.Max(y1, y2) + 1;

        min = Mathf.Clamp(min, 0, self.height - 1);
        max = Mathf.Clamp(max, 0, self.height - 1);

        for (int y = min; y != max; ++y)
        {
            self.SetPixel(x, y, color);
        }

        // self.Apply();
    }

    private static void LineLow(this Texture2D self, Vector2Int a, Vector2Int b, Color color)
    {
        float dx = b.x - a.x;
        float dy = b.y - a.y;
        int yi = 1;

        if (dy < 0)
        {
            yi = -1;
            dy = -dy;
        }

        float D = 2.0f * dy - dx;
        int y = a.y;

        for (int x = a.x; x != b.x; ++x)
        {
            self.SetPixel(x, y, color);

            if (D > 0.0f)
            {
                y = y + yi;
                D -= 2.0f * dx;
            }

            D += 2.0f * dy;
        }
    }

    private static void LineHigh(this Texture2D self, Vector2Int a, Vector2Int b, Color color)
    {
        float dx = b.x - a.x;
        float dy = b.y - a.y;
        int xi = 1;

        if (dx < 0)
        {
            xi = -1;
            dx = -dx;
        }

        float D = 2.0f * dx - dy;
        int x = a.x;

        for (int y = a.y; y != b.y; ++y)
        {
            self.SetPixel(x, y, color);

            if (D > 0.0f)
            {
                x += xi;
                D -= 2.0f * dy;
            }

            D += 2.0f * dx;
        }
    }

    public static void Line(this Texture2D self, Vector2Int a, Vector2Int b, Color color)
    {
        if (Mathf.Abs(b.y - a.y) < Mathf.Abs(b.x - a.x))
        {
            if (a.x > b.x) LineLow(self, b, a, color);
            else LineLow(self, a, b, color);
        }
        else
        {
            if (a.y > b.y) LineHigh(self, b, a, color);
            else LineHigh(self, a, b, color);
        }
    }
}
