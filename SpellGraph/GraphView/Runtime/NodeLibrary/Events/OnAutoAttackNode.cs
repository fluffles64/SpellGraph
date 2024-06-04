using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
[NodeInfo("On AutoAttack", "Event", "Listens for autoattacks")]
public class OnAutoAttackNode : ExtendedNode
{
    [OutputPort("Out")]
    public Trigger Output;
}
#endif

public class RuntimeOnAutoAttackNode : EventNode
{
    public override void Subscribe(List<object> portValues, NodeData nodeData)
    {
        //// WARNING: Had to put the Hero script in a higher execution order. Edit > Project Settings > Script Execution order => +1 seems to do the trick.
        //// This might cause errors in the future, careful.
        //try
        //{
        //    Combat combatScript = TargetManager.Instance.PlayerCombat;
        //    combatScript.OnAutoAttack += Event;
        //}
        //catch (System.Exception ex)
        //{
        //    Debug.LogError("Couldn't subscribe to the event properly. " + ex);
        //}
    }

    protected override void Event()
    {
        Debug.Log("Autoattack detected!");
    }
}