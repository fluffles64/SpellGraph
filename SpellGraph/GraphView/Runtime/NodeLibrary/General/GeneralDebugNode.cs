using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
[NodeInfo("Debug", "General", "Debugs a string")]
public class GeneralDebugNode : ExtendedNode
{
    [InputPort("Debug")]
    public Generic Value;

    [InputPort("In")]
    public Trigger Input;

    [OutputPort("Out")]
    public Trigger Output;
}
#endif

public class RuntimeGeneralDebugNode : ActionNode
{
    public override object Action(List<object> portValues, NodeData nodeData)
    {
        string type = portValues[0]?.GetType().ToString();
        Debug.Log("Debug Node: " + portValues[0] + " [" + type + "]");

        return null;
    }
}