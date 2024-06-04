using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[NodeInfo("Power", "Math/Basic", "Returns the result of input A to the power of input B.")]
public class MathBasicPowerNode : ExtendedNode
{
    [InputPort("A")]
    public float A;

    [InputPort("B")]
    public float B;

    [OutputPort("Output")]
    public float Output;
}
#endif

public class RuntimeMathBasicPowerNode : ActionNode
{
    public override object Action(List<object> portValues, NodeData nodeData)
    {
        float a = GetPortValue<float>(portValues[0]);
        float b = GetPortValue<float>(portValues[1]);

        return Mathf.Pow(a, b);
    }
}