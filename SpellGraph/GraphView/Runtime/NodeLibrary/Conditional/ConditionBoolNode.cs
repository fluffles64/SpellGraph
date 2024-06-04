using System.Collections.Generic;

#if UNITY_EDITOR
[NodeInfo("Bool", "Conditional", "Splits the node execution flow")]
public class ConditionBoolNode : ExtendedNode
{
    [InputPort("Bool")]
    public bool Value;

    [InputPort("In")]
    public Trigger Input;

    [OutputPort("True")]
    public Trigger OutputA;

    [OutputPort("False")]
    public Trigger OutputB;
}
#endif

public class RuntimeConditionBoolNode : ConditionNode
{
    public override Trigger Condition(List<object> portValues, NodeData nodeData)
    {
        bool value = (bool)(portValues[0] ?? false);

        if (value)
            return new Trigger("True");

        return new Trigger("False");
    }
}