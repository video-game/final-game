using UnityEngine;

public static class Utilities
{
    public enum Direction { Left, Right, Up, Down }

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
