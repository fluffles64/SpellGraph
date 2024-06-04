using UnityEngine;

/// <summary>
/// Util to assign colors to the different categories in the node library.
/// </summary>

public static class NodeColorUtil
{
    public static Color GetColor(string nodeType)
    {
        string lowerCaseNodeType = nodeType.ToLower();

        if (lowerCaseNodeType.Contains("math"))
            return new Color(0.976f, 0.259f, 0.259f, 1f);
        else if (lowerCaseNodeType.Contains("stat"))
            return new Color(0.427f, 0.831f, 0.996f, 1f);
        else if (lowerCaseNodeType.Contains("time"))
            return new Color(0.463f, 0.961f, 0.455f, 1f);
        else if (lowerCaseNodeType.Contains("conditional"))
            return new Color(0.576f, 0.109f, 0.737f, 1f);
        else if (lowerCaseNodeType.Contains("vfx"))
            return new Color(0.678f, 0.529f, 0.435f, 1f);
        else if (lowerCaseNodeType.Contains("sfx"))
            return new Color(0.909f, 0.576f, 0.937f, 1f);
        else if (lowerCaseNodeType.Contains("general"))
            return new Color(0.988f, 0.976f, 0.545f, 1f);
        else if (lowerCaseNodeType.Contains("event"))
            return new Color(0.176f, 0.380f, 0.729f, 1f);
        else if (lowerCaseNodeType.Contains("combat"))
            return new Color(0.988f, 0.612f, 0.254f, 1f);
        else
            return new Color(0.505f, 0.498f, 0.498f, 1f);
    }
}