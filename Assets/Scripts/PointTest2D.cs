using UnityEngine;
using System.Collections;

public static class PointTest2D
{
    private static float V2CrossZ(Vector2 a, Vector2 b)
    {
        return (a.x * b.y) - (b.x * a.y);
    }

    private static bool SameSide(Vector2 p1, Vector2 p2, Vector2 a, Vector2 b)
	{
        float p1Sign = V2CrossZ(b - a, p1 - a);
        float p2Sign = V2CrossZ(b - a, p2 - a);

        return (p1Sign * p2Sign) >= 0;
		/*
        function SameSide(p1, p2, a, b)
            cp1 = CrossProduct(b - a, p1 - a)
            cp2 = CrossProduct(b - a, p2 - a)
            if DotProduct(cp1, cp2) >= 0 then return true
            else return false
        */
    }
	
    public static bool IsInsideTri(Vector2 pointToTest, Vector2 a, Vector2 b, Vector2 c)
    {
        return  SameSide(pointToTest, c, a, b) &&
                SameSide(pointToTest, b, c, a) &&
                SameSide(pointToTest, a, b, c);

        //SameSide(p, a, b, c) and SameSide(p, b, a, c)
        //and SameSide(p, c, a, b) then return true
    }
}
