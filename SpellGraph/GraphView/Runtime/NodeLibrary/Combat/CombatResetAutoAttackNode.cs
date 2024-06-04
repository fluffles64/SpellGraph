using System.Collections.Generic;

#if UNITY_EDITOR
[NodeInfo("Reset AA", "Combat", "Resets autoattack")]
public class CombatResetAutoAttackNode : ExtendedNode
{
    [InputPort("In")]
    public Trigger Input;

    [OutputPort("Out")]
    public Trigger Output;
}
#endif

public class RuntimeCombatResetAutoAttackNode : ActionNode
{
    public override object Action(List<object> portValues, NodeData nodeData)
    {
        //if (TargetManager.Instance.Target != null)
        //    TargetManager.Instance.PlayerCombat?.ResetAutoattack();

        return null;
    }
}