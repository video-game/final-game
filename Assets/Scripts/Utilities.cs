using UnityEngine;
using UnityEngine.AI;
public class Utilities
{
    public enum Direction { Left, Right, Up, Down }

    private bool cameraShaking = false;
    //This is probably inefficient as heck, works though.
    public static float PathDistance(NavMeshPath path)
    {
        float distance = 0;
        Vector3 lastCorner = Vector3.zero;
        foreach (Vector3 corner in path.corners)
        {
            if (lastCorner != Vector3.zero)
            {
                distance += Vector3.Distance(corner, lastCorner);
            }
            lastCorner = corner;
        }
        return distance;
    }

    public static Direction VectorToDirection(float x, float y)
    {
        Direction direction;

        if (x < 0 && Mathf.Abs(x) > Mathf.Abs(y))
            direction = Utilities.Direction.Left;
        else if (x > 0 && x > Mathf.Abs(y))
            direction = Utilities.Direction.Right;
        else if (y > 0 && y > Mathf.Abs(x))
            direction = Utilities.Direction.Up;
        else
            direction = Utilities.Direction.Down;

        return direction;
    }
}
