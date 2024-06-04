using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
[NodeInfo("Set Stat", "Stats", "Modifies stats of a target")]
public class SetStatNode : ExtendedNode
{
    private string selectedTarget = "Player";
    private string selectedStat = "Health";
    private string[] targets = Enum.GetNames(typeof(CurrentTargetType));
    private string[] stats = Enum.GetNames(typeof(StatType));

    [InputPort("Value")]
    public float Value;

    [InputPort("In")]
    public Trigger Input;

    [OutputPort("New Value")]
    public float NewValue;

    [OutputPort("Old Value")]
    public float OldValue;

    [OutputPort("Out")]
    public Trigger Output;

    public SetStatNode()
    {
        VisualElement targetDropdown = DropdownContainer("Target", selectedTarget, targets, newValue => selectedTarget = newValue);
        titleContainer.Add(targetDropdown);

        VisualElement statDropdown = DropdownContainer("Stat", selectedStat, stats, newValue => selectedStat = newValue);
        extensionContainer.Add(statDropdown);
    }

    #region INodeData
    public override void SetValues(NodeData data)
    {
        if (data is StatNodeData nodeData)
        {
            selectedTarget = nodeData.Target;
            selectedStat = nodeData.Stat;
        }

        DropdownField targetDropdown = titleContainer.Q<DropdownField>();
        targetDropdown?.SetValueWithoutNotify(selectedTarget);

        DropdownField statDropdown = extensionContainer.Q<DropdownField>();
        statDropdown?.SetValueWithoutNotify(selectedStat);
    }

    public override object GetNodeData()
    {
        return new StatNodeData
        {
            Id = Id,
            Position = GetPosition().position,
            EditorType = new SerializableSystemType(GetType()),
            RuntimeType = new SerializableSystemType(GetRuntimeType()),
            InputPortNames = GetInputPortNames(),
            OutputPortNames = GetOutputPortNames(),
            Target = selectedTarget,
            Stat = selectedStat
        };
    }
    #endregion
}
#endif

public class RuntimeSetStatNode : ActionNode
{
    public override object Action(List<object> portValues, NodeData nodeData)
    {
        //if (portValues[0] != null && nodeData is StatNodeData statNodeData)
        //{
        //    float newValue = GetPortValue<float>(portValues[0]);
        //    Hero hero = TargetManager.Instance.Player;

        //    float oldValue = StatsUtil.GetStatValue((StatType)Enum.Parse(typeof(StatType), statNodeData.Stat), hero);
        //    StatsUtil.SetStatValue((StatType)Enum.Parse(typeof(StatType), statNodeData.Stat), newValue, hero);

        //    return new List<object> { newValue, oldValue };
        //}
        return null;
    }
}