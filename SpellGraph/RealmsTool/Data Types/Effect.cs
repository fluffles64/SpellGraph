using System.Collections.Generic;
using UnityEngine;
using System;

#region Instance Variables
[Serializable]
public abstract class InstanceVariable
{
    public string Name;
}

public class InstanceVariableInt : InstanceVariable
{
    public int Value;
}

public class InstanceVariableFloat : InstanceVariable
{
    public float Value;
}

public class InstanceVariableString : InstanceVariable
{
    public string Value;
}

public class InstanceVariableBool : InstanceVariable
{
    public bool Value;
}

public class InstanceVariableVector2 : InstanceVariable
{
    public Vector2 Value;
}

public class InstanceVariableVector3 : InstanceVariable
{
    public Vector3 Value;
}

public class InstanceVariableVector4 : InstanceVariable
{
    public Vector4 Value;
}

public class InstanceVariableGameObject : InstanceVariable
{
    public GameObject Value;
}
#endregion

#region JSON
[Serializable]
public class NodeDataJson
{
    public List<string> serializedNodes = new List<string>();
}

[Serializable]
public class NodeLinkDataJson
{
    public List<string> serializedEdges = new List<string>();
}

[Serializable]
public class GraphDataJson
{
    public NodeDataJson nodeDataJson = new NodeDataJson();
    public NodeLinkDataJson nodeLinkDataJson = new NodeLinkDataJson();
}
#endregion

#region NodeData
[Serializable]
public class NodeData
{
    public string Id;
    public Vector2 Position;
    public SerializableSystemType DataType;
    public SerializableSystemType EditorType;
    public SerializableSystemType RuntimeType;
    public string[] InputPortNames;
    public string[] OutputPortNames;
}
#endregion

#region NodeLinkData
[Serializable]
public class NodeLinkData
{
    public string BaseId;
    public string TargetId;
    public string BasePortName;
    public string TargetPortName;
}
#endregion

#region Groups
[Serializable]
public class GroupData
{
    public List<string> Nodes = new List<string>();
    public Vector2 Position;
    public string Title = "New Group";
}
#endregion

[Serializable]
public class Effect : ScriptableObject
{
    [SerializeReference]
    public List<InstanceVariable> InstanceVariables = new List<InstanceVariable>();

    [SerializeReference]
    public List<NodeData> Nodes = new List<NodeData>();

    public List<NodeLinkData> Links = new List<NodeLinkData>();

    public List<GroupData> Groups = new List<GroupData>();
}