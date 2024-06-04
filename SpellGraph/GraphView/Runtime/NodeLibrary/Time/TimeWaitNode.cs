using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

#if UNITY_EDITOR
[NodeInfo("Wait", "Time", "Waits for x seconds")]
public class TimeWaitNode : ExtendedNode
{
    [InputPort("Time")]
    public float Value;

    [InputPort("In")]
    public Trigger Input;

    [OutputPort("Out")]
    public Trigger Output;
}
#endif

public class RuntimeTimeWaitNode : StateNode
{
    public override async Task<object> State(List<object> portValues, NodeData nodeData)
    {
        if (portValues[0] != null)
        {
            float value = (float)(portValues[0] ?? 0f);
            await Task.Delay(Mathf.RoundToInt(value * 1000));
        }
        return null;
    }
}