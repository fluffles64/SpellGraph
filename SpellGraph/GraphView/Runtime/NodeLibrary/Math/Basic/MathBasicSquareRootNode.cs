using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[NodeInfo("Square Root", "Math/Basic", "Returns the square root of input In.")]
public class MathBasicSquareRootNode : ExtendedNode
{
    [InputPort("A")]
    public float A;

    [OutputPort("Output")]
    public float Output;
}
#endif

public class RuntimeMathBasicSquareRootNode : ActionNode
{
    public override object Action(List<object> portValues, NodeData nodeData)
    {
        float a = GetPortValue<float>(portValues[0]);

        return Mathf.Sqrt(a);
    }
}