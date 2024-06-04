using System.Collections.Generic;

#if UNITY_EDITOR
[NodeInfo("Multiply", "Math/Basic", "Returns the result of input A multiplied by input B.")]
public class MathBasicMultiplyNode : ExtendedNode
{
    [InputPort("A")]
    public float A;

    [InputPort("B")]
    public float B;

    [OutputPort("Output")]
    public float Output;
}
#endif

public class RuntimeMathBasicMultiplyNode : ActionNode
{
    public override object Action(List<object> portValues, NodeData nodeData)
    {
        float a = GetPortValue<float>(portValues[0]);
        float b = GetPortValue<float>(portValues[1]);

        return a * b;
    }
}