#if UNITY_EDITOR
using System;
using UnityEditor.Experimental.GraphView;

/// <summary>
/// Custom attributes to streamline the node creation and management process.
/// The input and output port attributes create custom ports in either direction.
/// The node info attribute adds information to the node so that it automatically
/// appears in the node library search window, and also adds a description on hover.
/// </summary>

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public class InputPortAttribute : Attribute
{
    public string displayName;
    public Port.Capacity capacity = Port.Capacity.Single;

    public InputPortAttribute(string displayName = "")
    {
        this.displayName = displayName;
    }

    public InputPortAttribute(Port.Capacity capacity, string displayName = "")
    {
        this.capacity = capacity;
        this.displayName = displayName;
    }
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public class OutputPortAttribute : Attribute
{
    public string displayName;
    public Port.Capacity capacity = Port.Capacity.Multi;

    public OutputPortAttribute(string displayName = "")
    {
        this.displayName = displayName;
    }

    public OutputPortAttribute(Port.Capacity capacity, string displayName = "")
    {
        this.capacity = capacity;
        this.displayName = displayName;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class NodeInfoAttribute : Attribute
{
    public string title;
    public string libraryPath;
    public string tooltip;

    public NodeInfoAttribute(string title)
    {
        this.title = title;
        this.tooltip = "";
    }

    public NodeInfoAttribute(string title, string libraryPath)
    {
        this.title = title;
        this.libraryPath = libraryPath;
    }

    public NodeInfoAttribute(string title, string libraryPath, string tooltip)
    {
        this.title = title;
        this.libraryPath = libraryPath;
        this.tooltip = tooltip;
    }
}
#endif