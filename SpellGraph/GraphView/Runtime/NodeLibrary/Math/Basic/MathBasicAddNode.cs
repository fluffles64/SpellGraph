using System.Collections.Generic;

#if UNITY_EDITOR
[NodeInfo("Add", "Math/Basic", "Returns the sum of the two input values.")]
public class MathBasicAddNode : ExtendedNode
{
    [InputPort("A")]
    public float A;

    [InputPort("B")]
    public float B;

    [OutputPort("Output")]
    public float Output;
}
#endif

public class RuntimeMathBasicAddNode : ActionNode
{
    public override object Action(List<object> portValues, NodeData nodeData)
    {
        float a = GetPortValue<float>(portValues[0]);
        float b = GetPortValue<float>(portValues[1]);

        return a + b;
    }
}