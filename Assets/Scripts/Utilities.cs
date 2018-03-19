using UnityEngine;
using UnityEngine.AI;
public class Utilities
{
    public enum Direction { Left, Right, Up, Down }
    
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
}
