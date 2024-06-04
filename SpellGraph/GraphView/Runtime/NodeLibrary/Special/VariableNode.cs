using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
#endif

public class VarNodeData : NodeData
{
    public string VarName;
    public SerializableSystemType VarType;
}

#if UNITY_EDITOR
public class VariableNode : ExtendedNode
{
    public string VarName;
    public Type VarType;

    public VariableNode()
    {
        Port port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, VarType);
        titleContainer.Add(port);
        titleContainer.AddToClassList("var-node__title-container");
        capabilities &= ~Capabilities.Collapsible;
    }

    #region INodeData
    public override void SetValues(NodeData data)
    {
        if (data is VarNodeData nodeData)
        {
            var port = titleContainer.Q<Port>();
            if (port != null)
            {
                port.portName = nodeData.VarName;
                port.portType = nodeData.VarType.SystemType;
                VarName = nodeData.VarName;
                VarType = nodeData.VarType.SystemType;
            }
        }
    }

    public override object GetNodeData()
    {
        return new VarNodeData
        {
            Id = Id,
            Position = GetPosition().position,
            DataType = new SerializableSystemType(typeof(VarNodeData)),
            EditorType = new SerializableSystemType(GetType()),
            RuntimeType = new SerializableSystemType(GetRuntimeType()),
            InputPortNames = GetInputPortNames(),
            OutputPortNames = GetOutputPortNames(),
            VarName = VarName,
            VarType = new SerializableSystemType(VarType)
        };
    }

    // Handle this one manually, seems like SerializableSystemType gives issues when deserializing
    public override object DeserializeJson(string data)
    {
        return JsonUtility.FromJson<VarNodeData>(data);
    }
    #endregion
}
#endif

public class RuntimeVariableNode : ActionNode
{
    public string VarName;

    public override object Action(List<object> portValues, NodeData nodeData)
    {
        if(nodeData is VarNodeData varNodeData)
            VarName = varNodeData.VarName;
        return null;
    }
}