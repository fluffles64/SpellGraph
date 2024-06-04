using System.Collections.Generic;

#if UNITY_EDITOR
[NodeInfo("Subtract", "Math/Basic", "Returns the result of input A minus input B.")]
public class MathBasicSubtractNode : ExtendedNode
{
    [InputPort("A")]
    public float A;

    [InputPort("B")]
    public float B;

    [OutputPort("Output")]
    public float Output;
}
#endif

public class RuntimeMathBasicSubtractNode : ActionNode
{
    public override object Action(List<object> portValues, NodeData nodeData)
    {
        float a = GetPortValue<float>(portValues[0]);
        float b = GetPortValue<float>(portValues[1]);

        return a - b;
    }
}