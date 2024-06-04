using UnityEngine;

/// <summary>
/// Util to help display the custom animation curve in the inspector.
/// </summary>

public class AnimationCurveAttribute : PropertyAttribute
{
    public float x, y, width, height;

    public AnimationCurveAttribute(float x, float y, float width, float height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }
}