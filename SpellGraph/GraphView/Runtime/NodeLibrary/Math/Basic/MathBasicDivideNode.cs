using System.Collections.Generic;

#if UNITY_EDITOR
[NodeInfo("Divide", "Math/Basic", "Returns the result of input A divided by input B.")]
public class MathBasicDivideNode : ExtendedNode
{
    [InputPort("A")]
    public float A;

    [InputPort("B")]
    public float B;

    [OutputPort("Output")]
    public float Output;
}
#endif

public class RuntimeMathBasicDivideNode : ActionNode
{
    public override object Action(List<object> portValues, NodeData nodeData)
    {
        float a = GetPortValue<float>(portValues[0], 0f);
        float b = GetPortValue<float>(portValues[1], 1f);

        return a / b;
    }
}